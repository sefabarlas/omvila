using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Sms;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.Jobs;
using Omvila.TestKit;

namespace Omvila.Modules.Notifications.IntegrationTests;

// Kural: dilim-03 kriter 7 (kontör 0 → Skipped + uyarı), 10 (üstel geri çekilme, 5. denemede Failed), 11 (başvuru
// beklerken OMVILA); bildirimler.md § Kontör (gönderim öncesi düşüm, OTP OMVILA'dan), § Gönderen;
// alan-modeli.md § CreditTransaction (bakiye negatif olamaz, eşik altına inince "Kontör azaldı").
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class DispatchNotificationsTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Phone = "+905550000001";
    private static readonly Guid BrandA = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("""
            TRUNCATE notifications.notification_outbox, notifications.credit_transactions, notifications.brand_sms_senders,
              infra.dev_sms_log, infra.job_runs, infra.outbox_events CASCADE
            """, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task SentMessage_ConsumesItsSegmentsFromTheBrandLedger()
    {
        await using var services = Services();
        await GrantAsync(services, 10);
        await EnqueueAsync(services, Reminder());

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || credits_consumed FROM notifications.notification_outbox")).ShouldBe("Sent:1");
        (await ScalarAsync<long>("SELECT sum(amount) FROM notifications.credit_transactions")).ShouldBe(9);
        (await ScalarAsync<string>("""
            SELECT t.type || ':' || t.amount || ':' || t.balance_after || ':' || (t.reference_id = o.id)
            FROM notifications.credit_transactions t, notifications.notification_outbox o WHERE t.type = 'Consume'
            """)).ShouldBe("Consume:-1:9:true");
    }

    [Fact]
    public async Task ZeroCredits_SkipsSms_WithReason_AndRaisesOneWarningPerBrand()
    {
        await using var services = Services();
        await EnqueueAsync(services, Reminder());
        await EnqueueAsync(services, Reminder());

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT string_agg(status || ':' || failure_reason, ',') FROM notifications.notification_outbox"))
            .ShouldBe("Skipped:insufficient_credits,Skipped:insufficient_credits");
        (await ScalarAsync<long>("SELECT count(*) FROM infra.dev_sms_log")).ShouldBe(0);
        (await ScalarAsync<long>("SELECT count(*) FROM notifications.credit_transactions")).ShouldBe(0);
        (await ScalarAsync<string>($"""
            SELECT payload->>'skippedCount' || ':' || (payload->>'balance') || ':' || (payload->>'brandId')
            FROM infra.outbox_events WHERE type = '{typeof(SmsCreditsInsufficient).FullName}'
            """)).ShouldBe($"2:0:{BrandA}");
    }

    [Fact]
    public async Task DroppingBelowLowThreshold_RaisesCreditsLowOnce()
    {
        await using var services = Services();
        await GrantAsync(services, 200);
        await EnqueueAsync(services, Reminder());
        await EnqueueAsync(services, Reminder());

        await DispatchAsync(services);

        (await ScalarAsync<long>("SELECT sum(amount) FROM notifications.credit_transactions")).ShouldBe(198);
        (await ScalarAsync<string>($"""
            SELECT string_agg(payload->>'balance' || ':' || (payload->>'threshold'), ',')
            FROM infra.outbox_events WHERE type = '{typeof(SmsCreditsLow).FullName}'
            """)).ShouldBe("198:200");
    }

    [Fact]
    public async Task OtpCode_IsNotChargedToTheBrand_AndAlwaysGoesFromOmvila()
    {
        await using var services = Services();
        await SubmitSenderAsync(services, approve: true);
        await EnqueueAsync(services, new SmsNotification(
            NotificationEvents.OtpCode, Phone, new Dictionary<string, string>(StringComparer.Ordinal) { ["kod"] = "482193" }, BrandA));

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status FROM notifications.notification_outbox")).ShouldBe("Sent");
        (await ScalarAsync<string>("SELECT sender FROM infra.dev_sms_log")).ShouldBe("OMVILA");
        (await ScalarAsync<long>("SELECT count(*) FROM notifications.credit_transactions")).ShouldBe(0);
    }

    [Fact]
    public async Task ParallelRuns_DoNotSpendTheSameCreditTwice()
    {
        // İki örnek iş kilidini (JobRunner) atlayarak aynı anda çalışır: satırlar SKIP LOCKED ile ayrılır, marka defteri
        // kilidi ikinciyi birincinin commit'ine kadar bekletir.
        await using var services = Services(batchSize: 1);
        await GrantAsync(services, 1);
        await EnqueueAsync(services, Reminder());
        await EnqueueAsync(services, Reminder());

        await Task.WhenAll(RunJobDirectlyAsync(services), RunJobDirectlyAsync(services));

        (await ScalarAsync<string>("SELECT string_agg(status, ',' ORDER BY status) FROM notifications.notification_outbox")).ShouldBe("Sent,Skipped");
        (await ScalarAsync<long>("SELECT sum(amount) FROM notifications.credit_transactions")).ShouldBe(0);
        (await ScalarAsync<long>("SELECT count(*) FROM notifications.credit_transactions WHERE type = 'Consume'")).ShouldBe(1);
    }

    [Fact]
    public async Task TransientFailures_AreRetriedWithExponentialBackoff_AndFailOnFifthAttempt()
    {
        var sender = new ControllableSmsSender(failures: int.MaxValue);
        await using var services = Services(sender: sender);
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        await GrantAsync(services, 10);
        await EnqueueAsync(services, Reminder());

        await DispatchAsync(services);
        (await StateAsync()).ShouldBe("Pending:1");
        await DispatchAsync(services);
        (await StateAsync()).ShouldBe("Pending:1", "geri çekilme süresi dolmadan denenmez");

        // 30 sn, 1 dk, 2 dk, 4 dk.
        foreach (var (seconds, expected) in new[] { (30, "Pending:2"), (60, "Pending:3"), (120, "Pending:4") })
        {
            clock.Advance(Duration.FromSeconds(seconds - 1));
            await DispatchAsync(services);
            clock.Advance(Duration.FromSeconds(1));
            await DispatchAsync(services);
            (await StateAsync()).ShouldBe(expected);
        }

        clock.Advance(Duration.FromSeconds(240));
        await DispatchAsync(services);

        (await StateAsync()).ShouldBe("Failed:5");
        sender.Calls.ShouldBe(5);
        (await ScalarAsync<long>("SELECT sum(amount) FROM notifications.credit_transactions")).ShouldBe(10, "başarısız gönderim kontör düşmez");
    }

    [Fact]
    public async Task FailureThenSuccess_ChargesOnce_AndStoresProviderMessageId()
    {
        var sender = new ControllableSmsSender(failures: 2);
        await using var services = Services(sender: sender);
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        await GrantAsync(services, 10);
        await EnqueueAsync(services, Reminder());

        await DispatchAsync(services);
        clock.Advance(Duration.FromSeconds(30));
        await DispatchAsync(services);
        clock.Advance(Duration.FromSeconds(60));
        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || attempt_count || ':' || provider_message_id FROM notifications.notification_outbox"))
            .ShouldBe("Sent:3:ctl-3");
        (await ScalarAsync<long>("SELECT sum(amount) FROM notifications.credit_transactions")).ShouldBe(9);
    }

    [Fact]
    public async Task PendingSenderName_SendsAsOmvila_UntilApproved()
    {
        await using var services = Services();
        await GrantAsync(services, 10);
        var senderId = await SubmitSenderAsync(services, approve: false);
        await EnqueueAsync(services, Reminder());

        await DispatchAsync(services);
        (await ScalarAsync<string>("SELECT sender FROM infra.dev_sms_log")).ShouldBe("OMVILA");

        await ApproveSenderAsync(services, senderId);
        await EnqueueAsync(services, Reminder());
        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT string_agg(sender, ',' ORDER BY sent_utc, sender) FROM infra.dev_sms_log")).ShouldBe("KUAFORELIF,OMVILA");
    }

    private static SmsNotification Reminder() => new(
        NotificationEvents.AppointmentReminder,
        Phone,
        new Dictionary<string, string>(StringComparer.Ordinal) { ["saat"] = "14:30", ["salon"] = "Kuaför Elif", ["bağlantı"] = "omvila.com/r/AbCdEfGh12" },
        BrandA,
        NotificationReference.ForAppointment(Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1")),
        null,
        "same_day");

    private ServiceProvider Services(int? batchSize = null, ControllableSmsSender? sender = null)
    {
        var settings = db.Settings();
        if (batchSize is { } size)
        {
            settings["Notifications:BatchSize"] = size.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        return db.CreateServices(configure: services =>
        {
            services.AddNotificationsModule(configuration);
            if (sender is not null)
            {
                services.AddScoped<ISmsSender>(_ => sender);
            }
        });
    }

    private static async Task InBrandAsync(IServiceProvider services, Func<IServiceProvider, Task> action)
    {
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(BrandA), Ct);
        await session.BeginAsync(Ct);
        await action(scope.ServiceProvider);
        await session.CommitAsync(Ct);
    }

    private static Task GrantAsync(IServiceProvider services, int credits) =>
        InBrandAsync(services, sp => sp.GetRequiredService<ICreditLedger>().GrantAsync(BrandA, credits, "Test", null, Ct));

    private static Task EnqueueAsync(IServiceProvider services, SmsNotification notification) =>
        InBrandAsync(services, sp => sp.GetRequiredService<INotificationOutbox>().EnqueueSmsAsync(notification, Ct));

    private static async Task<Guid> SubmitSenderAsync(IServiceProvider services, bool approve)
    {
        await InBrandAsync(services, async sp =>
        {
            var context = sp.GetRequiredService<NotificationsDbContext>();
            var sender = BrandSmsSender.Submit(BrandA, "KUAFORELIF", sp.GetRequiredService<IClock>().GetCurrentInstant());
            if (approve)
            {
                sender.Approve(sp.GetRequiredService<IClock>().GetCurrentInstant());
            }

            context.SmsSenders.Add(sender);
            await context.SaveChangesAsync(Ct);
        });
        return BrandA;
    }

    private static Task ApproveSenderAsync(IServiceProvider services, Guid brandId) =>
        InBrandAsync(services, async sp =>
        {
            var context = sp.GetRequiredService<NotificationsDbContext>();
            var sender = await context.SmsSenders.FindAsync([brandId], Ct);
            sender!.Approve(sp.GetRequiredService<IClock>().GetCurrentInstant());
            await context.SaveChangesAsync(Ct);
        });

    private static async Task RunJobDirectlyAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.Platform, Ct);
        await session.BeginAsync(Ct);
        await scope.ServiceProvider.GetRequiredService<DispatchNotifications>().RunAsync(Ct);
        await session.CommitAsync(Ct);
    }

    private static async Task DispatchAsync(IServiceProvider services) =>
        (await services.GetRequiredService<JobRunner>().RunOnceAsync(typeof(DispatchNotifications), Ct)).ShouldBe(JobOutcome.Succeeded);

    private Task<string> StateAsync() => ScalarAsync<string>("SELECT status || ':' || attempt_count FROM notifications.notification_outbox");

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }

    /// <summary>İlk <paramref name="failures"/> çağrıda geçici hata döner, sonra kabul eder ("ctl-{çağrı no}").</summary>
    private sealed class ControllableSmsSender(int failures) : ISmsSender
    {
        private int _calls;

        public ConcurrentQueue<SmsMessage> Sent { get; } = new();

        public int Calls => _calls;

        public Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct)
        {
            var call = Interlocked.Increment(ref _calls);
            if (call <= failures)
            {
                return Task.FromResult(new SmsSendResult(Succeeded: false, ProviderMessageId: null, FailureReason: "provider_down"));
            }

            Sent.Enqueue(message);
            return Task.FromResult(new SmsSendResult(Succeeded: true, ProviderMessageId: $"ctl-{call}", FailureReason: null));
        }
    }
}
