using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>owner.credits_low</c> (bildirimler.md § İşletme olayları; alan-modeli.md § CreditTransaction "salona uyarı üretilir"):
/// bakiye eşiğin altına indiğinde (<see cref="SmsCreditsLow"/>) veya kontör yetmediği için SMS atlandığında
/// (<see cref="SmsCreditsInsufficient"/>) ayarları yönetebilen kullanıcılara (settings.manage: Owner) bildirim paneli kaydı ve
/// işletme push'u. Okunmamış aynı uyarısı olan kullanıcıya yeniden yazılmaz (atlama her gönderim çalışmasında olabilir);
/// işlenene kadar bakiye eşiğin üstüne çıktıysa hiçbir şey yazılmaz.
/// </summary>
internal sealed class NotifyCreditsLow(
    IDbSession session,
    ITenantQueries tenants,
    IBrandUserDirectory users,
    ICreditLedger ledger,
    SmsUsage usage,
    InAppNotifier inApp,
    INotificationOutbox outbox) : IIntegrationEventHandler<SmsCreditsLow>, IIntegrationEventHandler<SmsCreditsInsufficient>
{
    public Task HandleAsync(SmsCreditsLow integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        return NotifyAsync(integrationEvent.BrandId, ct);
    }

    public Task HandleAsync(SmsCreditsInsufficient integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        return NotifyAsync(integrationEvent.BrandId, ct);
    }

    private async Task NotifyAsync(Guid brandId, CancellationToken ct)
    {
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var settings = await tenants.GetNotificationSettingsAsync(brandId, ct);
        var balance = Math.Max(0, await ledger.GetBalanceAsync(brandId, ct));
        if (settings is null || balance >= PlatformDefaults.LowCreditThreshold)
        {
            return;
        }

        var recipients = await users.GetUserIdsWithPermissionAsync(brandId, Permissions.Settings.Manage, null, ct);
        var alreadyWarned = await inApp.UsersWithUnreadAsync(brandId, NotificationEvents.OwnerCreditsLow, recipients, ct);
        var fresh = recipients.Where(u => !alreadyWarned.Contains(u)).ToList();
        if (fresh.Count == 0)
        {
            return;
        }

        var days = await usage.EstimateDaysLeftAsync(brandId, balance, ct);
        var values = new Dictionary<string, string>(StringComparer.Ordinal) { ["kalan"] = StaffText.Count(balance) };
        if (days is { } d)
        {
            values["gün"] = StaffText.Count(d);
        }

        var variant = days is null ? PushTemplates.WithoutEstimateVariant : null;
        await inApp.AddAsync(
            new InAppEntry(brandId, NotificationEvents.OwnerCreditsLow, values, variant, null, InAppNotifier.SmsCreditsTarget, null), fresh, ct);
        await outbox.EnqueueStaffPushAsync(
            new StaffPushNotification(NotificationEvents.OwnerCreditsLow, brandId, fresh, values, settings.TimeZoneId, Variant: variant), ct);
    }
}
