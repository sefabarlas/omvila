using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Sms;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Jobs;
using Omvila.TestKit;

namespace Omvila.Modules.Notifications.IntegrationTests;

// Kural: bildirimler.md (outbox iş transaction'ında; kod her zaman SMS, gönderen OMVILA),
// ADR-0006 (platform satırı: her bağlamdan eklenir, yalnız platform okur; markalar birbirini görmez)
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class NotificationOutboxTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Phone = "+905550000001";
    private static readonly Guid BrandA = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid BrandB = Guid.Parse("0193a8f0-0000-7000-8000-00000000000b");

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("TRUNCATE notifications.notification_outbox, infra.dev_sms_log, infra.job_runs", connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task Enqueue_InRolledBackTransaction_WritesNothing()
    {
        await using var services = Services();

        await EnqueueAsync(services, TenantScope.None, Otp(), commit: false);

        (await CountAsync("SELECT count(*) FROM notifications.notification_outbox")).ShouldBe(0);
    }

    [Fact]
    public async Task Dispatch_SendsOtpFromOmvila_AndRecordsRenderedBody()
    {
        await using var services = Services();
        await EnqueueAsync(services, TenantScope.None, Otp("482193"), commit: true);

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT sender || '|' || phone_e164 || '|' || body FROM infra.dev_sms_log"))
            .ShouldBe("OMVILA|+905550000001|OMVILA giriş kodunuz: 482193. Bu kodu kimseyle paylaşmayın.");
        (await ScalarAsync<string>("SELECT status || '|' || channel_used FROM notifications.notification_outbox")).ShouldBe("Sent|Sms");
    }

    [Fact]
    public async Task AnonymousContext_CanInsertPlatformRow_ButCannotReadIt()
    {
        await using var services = Services();
        await EnqueueAsync(services, TenantScope.None, Otp(), commit: true);

        (await VisibleCountAsync(services, TenantScope.None)).ShouldBe(0);
        (await VisibleCountAsync(services, TenantScope.ForBrand(BrandA))).ShouldBe(0);
        (await VisibleCountAsync(services, TenantScope.Platform)).ShouldBe(1);
    }

    [Fact]
    public async Task Brands_CannotSeeEachOthersNotifications_OrWriteForAnotherBrand()
    {
        await using var services = Services();
        await EnqueueAsync(services, TenantScope.ForBrand(BrandA), Otp() with { BrandId = BrandA }, commit: true);

        (await VisibleCountAsync(services, TenantScope.ForBrand(BrandA))).ShouldBe(1);
        (await VisibleCountAsync(services, TenantScope.ForBrand(BrandB))).ShouldBe(0);
        var error = await Should.ThrowAsync<Microsoft.EntityFrameworkCore.DbUpdateException>(() =>
            EnqueueAsync(services, TenantScope.ForBrand(BrandA), Otp() with { BrandId = BrandB }, commit: true));
        error.InnerException.ShouldBeOfType<PostgresException>().SqlState.ShouldBe(PostgresErrorCodes.InsufficientPrivilege);
    }

    [Fact]
    public async Task FailedSend_IsRetriedAfterDelay_ThenMarkedFailedAfterMaxAttempts()
    {
        await using var services = Services(failing: true);
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        await EnqueueAsync(services, TenantScope.None, Otp(), commit: true);

        await DispatchAsync(services);
        (await ScalarAsync<string>("SELECT status || ':' || attempt_count FROM notifications.notification_outbox")).ShouldBe("Pending:1");

        await DispatchAsync(services);
        (await ScalarAsync<string>("SELECT status || ':' || attempt_count FROM notifications.notification_outbox"))
            .ShouldBe("Pending:1", "tekrar deneme zamanı gelmeden gönderilmez");

        clock.Advance(Duration.FromSeconds(60));
        await DispatchAsync(services);
        clock.Advance(Duration.FromSeconds(60));
        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || attempt_count FROM notifications.notification_outbox")).ShouldBe("Failed:3");
    }

    private ServiceProvider Services(bool failing = false)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(db.Settings()).Build();
        return db.CreateServices(configure: services =>
        {
            services.AddNotificationsModule(configuration);
            if (failing)
            {
                services.AddScoped<ISmsSender, FailingSmsSender>();
            }
        });
    }

    private static SmsNotification Otp(string code = "123456") =>
        new(NotificationEvents.OtpCode, Phone, new Dictionary<string, string>(StringComparer.Ordinal) { ["kod"] = code });

    private static async Task EnqueueAsync(IServiceProvider services, TenantScope scope, SmsNotification notification, bool commit)
    {
        await using var serviceScope = services.CreateAsyncScope();
        var session = serviceScope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(scope, Ct);
        await session.BeginAsync(Ct);
        await serviceScope.ServiceProvider.GetRequiredService<INotificationOutbox>().EnqueueSmsAsync(notification, Ct);
        if (commit)
        {
            await session.CommitAsync(Ct);
        }
        else
        {
            await session.RollbackAsync(Ct);
        }
    }

    private static async Task<long> VisibleCountAsync(IServiceProvider services, TenantScope scope)
    {
        await using var serviceScope = services.CreateAsyncScope();
        var session = serviceScope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(scope, Ct);
        await session.BeginAsync(Ct);
        await using var command = new NpgsqlCommand("SELECT count(*) FROM notifications.notification_outbox", session.Connection, session.Transaction);
        return (long)(await command.ExecuteScalarAsync(Ct))!;
    }

    private static async Task DispatchAsync(IServiceProvider services) =>
        await services.GetRequiredService<JobRunner>().RunOnceAsync(typeof(DispatchNotifications), Ct);

    private Task<long> CountAsync(string sql) => ScalarAsync<long>(sql);

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }

    private sealed class FailingSmsSender : ISmsSender
    {
        public Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken ct) =>
            Task.FromResult(new SmsSendResult(Succeeded: false, ProviderMessageId: null, FailureReason: "provider_down"));
    }
}
