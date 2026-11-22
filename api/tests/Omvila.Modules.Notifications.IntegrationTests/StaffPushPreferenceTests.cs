using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.Jobs;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Notifications.IntegrationTests;

// Kural: sözleşme MyNotificationPreferences (anahtar → olay; kapalı anahtarın olayı push olarak gitmez; staff.deposit_reported,
// owner.credits_low kapatılamaz; quietHoursEnabled açıkken sessiz saatte ertelenir), manageUpdateMyNotificationPreferences
// (ertelenmiş push gönderim anındaki tercihe göre süzülür); bildirimler.md § Sessiz saatler; T-66 (8); T-71 (10); T-105.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class StaffPushPreferenceTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid BrandA = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid Elif = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1");
    private static readonly Guid Mert = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2");
    private static readonly Guid AppointmentId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("""
            TRUNCATE notifications.notification_outbox, notifications.notification_preferences, notifications.devices,
              infra.job_runs, infra.outbox_events CASCADE
            """, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task DisabledKey_WritesNoPush_ForThatUserOnly()
    {
        await using var services = Services();
        await SetPreferenceAsync(services, Elif, Values() with { NewReview = false });

        await EnqueueAsync(services, Review([Elif, Mert]));

        (await RecipientsAsync()).ShouldBe([Mert.ToString()]);
    }

    [Fact]
    public async Task NonClosableEvent_IsNotFiltered()
    {
        await using var services = Services();
        await SetPreferenceAsync(services, Elif, new NotificationPreferenceValues(false, false, false, false, false, false, false));

        await EnqueueAsync(services, DepositReported([Elif]));

        (await RecipientsAsync()).ShouldBe([Elif.ToString()]);
    }

    [Fact]
    public async Task QuietHours_DeferByDefault_ButNotWhenTheUserTurnedThemOff()
    {
        await using var services = Services();
        ((FakeClock)services.GetRequiredService<IClock>()).Reset(Instant.FromUtc(2026, 11, 25, 20, 30)); // İstanbul 23:30
        await SetPreferenceAsync(services, Mert, Values() with { QuietHoursEnabled = false });

        await EnqueueAsync(services, Review([Elif, Mert]));

        (await ScalarAsync<string>($"SELECT to_char(scheduled_utc AT TIME ZONE 'UTC', 'HH24:MI') FROM notifications.notification_outbox WHERE recipient_id = '{Elif}'"))
            .ShouldBe("05:00", "08:00 İstanbul");
        (await ScalarAsync<string>($"SELECT to_char(scheduled_utc AT TIME ZONE 'UTC', 'HH24:MI') FROM notifications.notification_outbox WHERE recipient_id = '{Mert}'"))
            .ShouldBe("20:30");
    }

    [Fact]
    public async Task DeferredPush_IsSkipped_WhenTheKeyIsTurnedOffBeforeSending()
    {
        await using var services = Services();
        var clock = (FakeClock)services.GetRequiredService<IClock>();
        clock.Reset(Instant.FromUtc(2026, 11, 25, 20, 30));
        await EnqueueAsync(services, Review([Elif]));
        await SetPreferenceAsync(services, Elif, Values() with { NewReview = false });

        clock.Reset(Instant.FromUtc(2026, 11, 26, 5, 0));
        (await services.GetRequiredService<JobRunner>().RunOnceAsync(typeof(DispatchNotifications), Ct)).ShouldBe(JobOutcome.Succeeded);

        (await ScalarAsync<string>("SELECT status || ':' || failure_reason FROM notifications.notification_outbox")).ShouldBe("Skipped:preference_off");
    }

    [Fact]
    public async Task NotAfter_SkipsPushThatWouldBeDeferredPastIt()
    {
        await using var services = Services();
        ((FakeClock)services.GetRequiredService<IClock>()).Reset(Instant.FromUtc(2026, 11, 25, 20, 30));
        await SetPreferenceAsync(services, Mert, Values() with { QuietHoursEnabled = false });

        await EnqueueAsync(services, Review([Elif, Mert]) with { NotAfterUtc = Instant.FromUtc(2026, 11, 25, 22, 30) });

        (await RecipientsAsync()).ShouldBe([Mert.ToString()], "Elif'in push'u sabaha ertelenecekti");
    }

    private static NotificationPreferenceValues Values() => new(true, true, true, true, true, false, true);

    private static StaffPushNotification Review(Guid[] users) => new(
        NotificationEvents.StaffReviewReceived,
        BrandA,
        users,
        new Dictionary<string, string>(StringComparer.Ordinal) { ["puan"] = "5", ["müşteri"] = "Selin Ö." },
        PlatformDefaults.DefaultTimeZoneId,
        NotificationReference.ForAppointment(AppointmentId));

    private static StaffPushNotification DepositReported(Guid[] users) => new(
        NotificationEvents.StaffDepositReported,
        BrandA,
        users,
        new Dictionary<string, string>(StringComparer.Ordinal) { ["müşteri"] = "Selin Ö.", ["tutar"] = "150 ₺" },
        PlatformDefaults.DefaultTimeZoneId,
        NotificationReference.ForAppointment(AppointmentId));

    private ServiceProvider Services() => db.CreateServices(configure: services => services.AddNotificationsModule(db.Configuration()));

    private static async Task InBrandAsync(IServiceProvider services, Func<IServiceProvider, Task> action)
    {
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(BrandA), Ct);
        await session.BeginAsync(Ct);
        await action(scope.ServiceProvider);
        await session.CommitAsync(Ct);
    }

    private static Task SetPreferenceAsync(IServiceProvider services, Guid userId, NotificationPreferenceValues values) =>
        InBrandAsync(services, async sp =>
        {
            var context = sp.GetRequiredService<NotificationsDbContext>();
            context.NotificationPreferences.Add(NotificationPreference.Create(BrandA, userId, values, TestClock.Start));
            await context.SaveChangesAsync(Ct);
        });

    private static Task EnqueueAsync(IServiceProvider services, StaffPushNotification notification) =>
        InBrandAsync(services, sp => sp.GetRequiredService<INotificationOutbox>().EnqueueStaffPushAsync(notification, Ct));

    private async Task<string[]> RecipientsAsync() =>
        (await ScalarAsync<string?>("SELECT string_agg(recipient_id::text, ',' ORDER BY recipient_id) FROM notifications.notification_outbox"))?.Split(',') ?? [];

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        var value = await command.ExecuteScalarAsync(Ct);
        return value is DBNull ? default! : (T)value!;
    }
}
