using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>staff.daily_summary</c> (bildirimler.md § İşletme olayları: "Tercihe göre (kapalı varsayılan)"; T-120): şubenin gün sonu
/// özeti. Alıcılar şubenin sayılarını görebilenlerdir (report.view, şube kapsamı: Owner ve şubenin yöneticileri); kişisel tercih
/// (dailySummary) kapalıysa outbox satır yazmaz. Yalnız push; panel kaydı yazılmaz.
/// </summary>
internal sealed class NotifyDailySummary(IDbSession session, IBrandUserDirectory users, INotificationOutbox outbox)
    : IIntegrationEventHandler<BranchDailySummaryReady>
{
    // Kural: bildirimler.md staff.daily_summary ("{n} randevu tamamlandı, {tutar} tahsilat."); roller-yetkiler.md report.view
    public async Task HandleAsync(BranchDailySummaryReady integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        await session.SetTenantAsync(TenantScope.ForBrand(integrationEvent.BrandId), ct);
        var recipients = await users.GetUserIdsWithPermissionAsync(integrationEvent.BrandId, Permissions.Report.View, integrationEvent.BranchId, ct);
        if (recipients.Count == 0)
        {
            return;
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["n"] = StaffText.Count(integrationEvent.CompletedCount),
            ["tutar"] = StaffText.Amount(integrationEvent.CollectedMinor),
        };
        await outbox.EnqueueStaffPushAsync(
            new StaffPushNotification(NotificationEvents.StaffDailySummary, integrationEvent.BrandId, recipients, values, integrationEvent.TimeZoneId),
            ct);
    }
}
