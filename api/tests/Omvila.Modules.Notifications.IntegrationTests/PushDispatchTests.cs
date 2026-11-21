using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Push;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.Jobs;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Notifications.IntegrationTests;

// Kural: bildirimler.md § Kanal planı (aktif cihaz varsa [Push, Sms]; push başarısızsa SMS'e düşer; kod her zaman SMS),
// § Sessiz saatler (işletme push'u ertelenir), § İşletme olayları (staff.*, SMS'e düşmez); dilim-03 kriter 7 (kontör 0
// iken push'u olan müşteriye push gider) ve 8; alan-modeli.md § Device (geçersiz jeton → pasif).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class PushDispatchTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Phone = "+905550000001";
    private static readonly Guid BrandA = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid BrandB = Guid.Parse("0193a8f0-0000-7000-8000-00000000000b");
    private static readonly Guid UserId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1");
    private static readonly Guid CustomerId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d1");
    private static readonly Guid AppointmentId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("""
            TRUNCATE notifications.notification_outbox, notifications.credit_transactions, notifications.devices,
              infra.dev_sms_log, infra.job_runs, infra.outbox_events CASCADE
            """, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task StaffPush_GoesToTheUsersDevicesInThatBrand_WithoutCredits()
    {
        var push = new RecordingPushSender();
        await using var services = Services(push);
        await AddDeviceAsync(services, BrandA, Device.UserOwner, UserId, "token-a-0123456789abcdef");
        await AddDeviceAsync(services, BrandB, Device.UserOwner, UserId, "token-b-0123456789abcdef");
        await EnqueueStaffAsync(services, ApprovalNeeded());

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || channel_used || ':' || credits_consumed FROM notifications.notification_outbox"))
            .ShouldBe("Sent:Push:0");
        var sent = push.Sent.ShouldHaveSingleItem();
        sent.Token.ShouldBe("token-a-0123456789abcdef", "başka markada kayıtlı cihaza o markanın push'u gitmez");
        sent.Title.ShouldBe("Onay bekliyor · 1 sa 58 dk");
        sent.Body.ShouldBe("Selin Ö. · Çar 25 Kas 14:30 Saç kesimi. Yanıtlanmazsa otomatik iptal olur.");
        sent.Data["eventCode"].ShouldBe(NotificationEvents.StaffApprovalNeeded);
        sent.Data["referenceId"].ShouldBe(AppointmentId.ToString());
    }

    [Fact]
    public async Task StaffPush_WithoutDevice_IsSkipped_AndNeverFallsBackToSms()
    {
        await using var services = Services(new RecordingPushSender());
        await EnqueueStaffAsync(services, ApprovalNeeded());

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || failure_reason FROM notifications.notification_outbox")).ShouldBe("Skipped:no_device");
        (await ScalarAsync<long>("SELECT count(*) FROM infra.dev_sms_log")).ShouldBe(0);
    }

    [Fact]
    public async Task StaffPush_TransientFailure_IsRetriedWithBackoff()
    {
        var push = new RecordingPushSender { Result = PushSendStatus.Failed };
        await using var services = Services(push);
        await AddDeviceAsync(services, BrandA, Device.UserOwner, UserId, "token-a-0123456789abcdef");
        await EnqueueStaffAsync(services, ApprovalNeeded());

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || attempt_count FROM notifications.notification_outbox")).ShouldBe("Pending:1");
        (await ScalarAsync<DateTime>("SELECT scheduled_utc FROM notifications.notification_outbox"))
            .ShouldBe(TestClock.Start.Plus(Duration.FromSeconds(30)).ToDateTimeUtc());
    }

    [Fact]
    public async Task StaffPush_InQuietHours_IsDeferredToEightInTheBranchZone()
    {
        await using var services = Services(new RecordingPushSender());
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        clock.Reset(Instant.FromUtc(2026, 11, 25, 20, 30)); // İstanbul 23:30
        await AddDeviceAsync(services, BrandA, Device.UserOwner, UserId, "token-a-0123456789abcdef");
        await EnqueueStaffAsync(services, ApprovalNeeded());

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status FROM notifications.notification_outbox")).ShouldBe("Pending");
        (await ScalarAsync<DateTime>("SELECT scheduled_utc FROM notifications.notification_outbox"))
            .ShouldBe(new DateTime(2026, 11, 26, 5, 0, 0, DateTimeKind.Utc));

        clock.Reset(Instant.FromUtc(2026, 11, 26, 5, 0));
        await DispatchAsync(services);
        (await ScalarAsync<string>("SELECT status || ':' || channel_used FROM notifications.notification_outbox")).ShouldBe("Sent:Push");
    }

    [Fact]
    public async Task CustomerWithDevice_GetsPush_EvenWithZeroCredits()
    {
        var push = new RecordingPushSender();
        await using var services = Services(push);
        await AddDeviceAsync(services, BrandA, Device.CustomerOwner, CustomerId, "token-c-0123456789abcdef");
        await EnqueueSmsAsync(services, Reminder(CustomerId));

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || channel_used || ':' || array_to_string(channel_plan, '>') FROM notifications.notification_outbox"))
            .ShouldBe("Sent:Push:Push>Sms");
        push.Sent.ShouldHaveSingleItem().Title.ShouldBe("Bugün 14:30 Kuaför Elif");
        (await ScalarAsync<long>("SELECT count(*) FROM infra.dev_sms_log")).ShouldBe(0);
    }

    [Fact]
    public async Task CustomerWithoutDevice_FallsBackToSms()
    {
        await using var services = Services(new RecordingPushSender());
        await GrantAsync(services, 10);
        await EnqueueSmsAsync(services, Reminder(CustomerId));

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || channel_used || ':' || credits_consumed FROM notifications.notification_outbox")).ShouldBe("Sent:Sms:1");
        (await ScalarAsync<long>("SELECT count(*) FROM infra.dev_sms_log")).ShouldBe(1);
    }

    [Fact]
    public async Task InvalidToken_DeactivatesDevice_AndFallsBackToSms()
    {
        await using var services = Services(new RecordingPushSender { Result = PushSendStatus.InvalidToken });
        await GrantAsync(services, 10);
        await AddDeviceAsync(services, BrandA, Device.CustomerOwner, CustomerId, "token-c-0123456789abcdef");
        await EnqueueSmsAsync(services, Reminder(CustomerId));

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || channel_used FROM notifications.notification_outbox")).ShouldBe("Sent:Sms");
        (await ScalarAsync<bool>("SELECT is_active FROM notifications.devices")).ShouldBeFalse();
    }

    [Fact]
    public async Task PushFailure_FallsBackToSms_InTheSameRun()
    {
        await using var services = Services(new RecordingPushSender { Result = PushSendStatus.Failed });
        await GrantAsync(services, 10);
        await AddDeviceAsync(services, BrandA, Device.CustomerOwner, CustomerId, "token-c-0123456789abcdef");
        await EnqueueSmsAsync(services, Reminder(CustomerId));

        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT status || ':' || channel_used FROM notifications.notification_outbox")).ShouldBe("Sent:Sms");
        (await ScalarAsync<bool>("SELECT is_active FROM notifications.devices")).ShouldBeTrue();
    }

    [Fact]
    public async Task OtpCode_IsAlwaysSmsOnly()
    {
        await using var services = Services(new RecordingPushSender());
        await EnqueueSmsAsync(services, new SmsNotification(
            NotificationEvents.OtpCode, Phone, new Dictionary<string, string>(StringComparer.Ordinal) { ["kod"] = "482193" }, CustomerId: CustomerId));

        (await ScalarAsync<string>("SELECT recipient_type || ':' || array_to_string(channel_plan, '>') FROM notifications.notification_outbox"))
            .ShouldBe("Phone:Sms");
    }

    private static StaffPushNotification ApprovalNeeded() => new(
        NotificationEvents.StaffApprovalNeeded,
        BrandA,
        [UserId],
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["kalan süre"] = "1 sa 58 dk",
            ["müşteri"] = "Selin Ö.",
            ["tarih saat"] = "Çar 25 Kas 14:30",
            ["hizmet"] = "Saç kesimi",
        },
        PlatformDefaults.DefaultTimeZoneId,
        NotificationReference.ForAppointment(AppointmentId));

    private static SmsNotification Reminder(Guid? customerId) => new(
        NotificationEvents.AppointmentReminder,
        Phone,
        new Dictionary<string, string>(StringComparer.Ordinal) { ["saat"] = "14:30", ["salon"] = "Kuaför Elif", ["bağlantı"] = "omvila.com/r/AbCdEfGh12" },
        BrandA,
        NotificationReference.ForAppointment(AppointmentId),
        Variant: "same_day",
        CustomerId: customerId);

    private ServiceProvider Services(RecordingPushSender push) =>
        db.CreateServices(configure: services =>
        {
            services.AddNotificationsModule(db.Configuration());
            services.AddScoped<IPushSender>(_ => push);
        });

    private static async Task InBrandAsync(IServiceProvider services, Guid brandId, Func<IServiceProvider, Task> action)
    {
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), Ct);
        await session.BeginAsync(Ct);
        await action(scope.ServiceProvider);
        await session.CommitAsync(Ct);
    }

    private static Task AddDeviceAsync(IServiceProvider services, Guid brandId, string ownerType, Guid ownerId, string token) =>
        InBrandAsync(services, brandId, async sp =>
        {
            var context = sp.GetRequiredService<NotificationsDbContext>();
            var device = Device.RegisterForUser(sp.GetRequiredService<IIdGenerator>().New(), brandId, ownerId, DevicePlatform.Android, token, "1.0.0", TestClock.Start);
            context.Devices.Add(device);
            context.Entry(device).Property(d => d.OwnerType).CurrentValue = ownerType;
            await context.SaveChangesAsync(Ct);
        });

    private static Task GrantAsync(IServiceProvider services, int credits) =>
        InBrandAsync(services, BrandA, sp => sp.GetRequiredService<ICreditLedger>().GrantAsync(BrandA, credits, "Test", null, Ct));

    private static Task EnqueueSmsAsync(IServiceProvider services, SmsNotification notification) =>
        InBrandAsync(services, BrandA, sp => sp.GetRequiredService<INotificationOutbox>().EnqueueSmsAsync(notification, Ct));

    private static Task EnqueueStaffAsync(IServiceProvider services, StaffPushNotification notification) =>
        InBrandAsync(services, BrandA, sp => sp.GetRequiredService<INotificationOutbox>().EnqueueStaffPushAsync(notification, Ct));

    private static async Task DispatchAsync(IServiceProvider services) =>
        (await services.GetRequiredService<JobRunner>().RunOnceAsync(typeof(DispatchNotifications), Ct)).ShouldBe(JobOutcome.Succeeded);

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }

    private sealed class RecordingPushSender : IPushSender
    {
        public PushSendStatus Result { get; init; } = PushSendStatus.Sent;

        public ConcurrentQueue<PushMessage> Sent { get; } = new();

        public Task<PushSendResult> SendAsync(PushMessage message, CancellationToken ct)
        {
            if (Result == PushSendStatus.Sent)
            {
                Sent.Enqueue(message);
            }

            return Task.FromResult(new PushSendResult(Result, Result == PushSendStatus.Sent ? $"push-{Sent.Count}" : null, Result == PushSendStatus.Sent ? null : "test"));
        }
    }
}
