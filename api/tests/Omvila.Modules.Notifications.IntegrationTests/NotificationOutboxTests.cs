using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
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
        await using var command = new NpgsqlCommand("TRUNCATE notifications.notification_outbox, notifications.credit_transactions, infra.dev_sms_log, infra.job_runs", connection);
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
    public async Task ScheduledReminder_IsNotSentBeforeItsTime_ThenSentWithVariantText()
    {
        await using var services = Services();
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        var appointmentId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
        await GrantAsync(services, BrandA, 10);
        await EnqueueAsync(services, TenantScope.ForBrand(BrandA), Reminder(appointmentId, clock.GetCurrentInstant() + Duration.FromHours(2)), commit: true);

        await DispatchAsync(services);
        (await CountAsync("SELECT count(*) FROM infra.dev_sms_log")).ShouldBe(0);

        clock.Advance(Duration.FromHours(2));
        await DispatchAsync(services);

        (await ScalarAsync<string>("SELECT body FROM infra.dev_sms_log"))
            .ShouldBe("Bugün 14:30'da Kuaför Elif'te görüşmek üzere. Yol tarifi: omvila.com/r/AbCdEfGh12");
        (await ScalarAsync<string>("SELECT reference_type || ':' || reference_id || ':' || template_variant FROM notifications.notification_outbox"))
            .ShouldBe($"Appointment:{appointmentId}:same_day");
    }

    [Fact]
    public async Task CancelPending_CancelsOnlyPendingRowsOfThatEventAndReference_InOwnBrand()
    {
        await using var services = Services();
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        var appointment = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
        var other = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a2");
        var later = clock.GetCurrentInstant() + Duration.FromDays(1);
        await EnqueueAsync(services, TenantScope.ForBrand(BrandA), Reminder(appointment, later), commit: true);
        await EnqueueAsync(services, TenantScope.ForBrand(BrandA), Reminder(appointment, later) with { Variant = null }, commit: true);
        await EnqueueAsync(services, TenantScope.ForBrand(BrandA), Reminder(other, later), commit: true);
        await EnqueueAsync(services, TenantScope.ForBrand(BrandB), Reminder(appointment, later) with { BrandId = BrandB }, commit: true);
        await EnqueueAsync(services, TenantScope.ForBrand(BrandA), Otp() with { BrandId = BrandA, Reference = NotificationReference.ForAppointment(appointment) }, commit: true);

        (await CancelAsync(services, BrandA, appointment)).ShouldBe(2);

        (await ScalarAsync<string>("""
            SELECT string_agg(status, ',' ORDER BY status) FROM notifications.notification_outbox
            """)).ShouldBe("Cancelled,Cancelled,Pending,Pending,Pending");
        (await CancelAsync(services, BrandA, appointment)).ShouldBe(0, "zaten iptal; gönderilmiş/iptal satır değişmez");
    }

    private static SmsNotification Reminder(Guid appointmentId, Instant at) => new(
        NotificationEvents.AppointmentReminder,
        Phone,
        new Dictionary<string, string>(StringComparer.Ordinal) { ["saat"] = "14:30", ["salon"] = "Kuaför Elif", ["bağlantı"] = "omvila.com/r/AbCdEfGh12" },
        BrandA,
        NotificationReference.ForAppointment(appointmentId),
        at,
        "same_day");

    private static async Task<int> CancelAsync(IServiceProvider services, Guid brandId, Guid appointmentId)
    {
        await using var serviceScope = services.CreateAsyncScope();
        var session = serviceScope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), Ct);
        await session.BeginAsync(Ct);
        var count = await serviceScope.ServiceProvider.GetRequiredService<INotificationOutbox>()
            .CancelPendingAsync(NotificationEvents.AppointmentReminder, NotificationReference.ForAppointment(appointmentId), Ct);
        await session.CommitAsync(Ct);
        return count;
    }

    private ServiceProvider Services()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(db.Settings()).Build();
        return db.CreateServices(configure: services => services.AddNotificationsModule(configuration));
    }

    private static async Task GrantAsync(IServiceProvider services, Guid brandId, int credits)
    {
        await using var serviceScope = services.CreateAsyncScope();
        var session = serviceScope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), Ct);
        await session.BeginAsync(Ct);
        await serviceScope.ServiceProvider.GetRequiredService<ICreditLedger>().GrantAsync(brandId, credits, "Test", null, Ct);
        await session.CommitAsync(Ct);
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

}
