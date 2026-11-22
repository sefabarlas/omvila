using System.Globalization;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>staff.walkin_queue</c> (bildirimler.md § İşletme olayları, "Tercihe göre"; T-91 (6), T-105): kapıda sıraya yeni bilet eklenince
/// sırayı yönetebilen kullanıcılara (walkin.manage, şube kapsamı) bildirim paneli kaydı ve işletme push'u. Bileti ekleyen kullanıcı
/// almaz. Push kişisel tercihle (walkInQueue) ve sessiz saatle outbox'ta süzülür. Müsait personel yoksa tahmini bekleme
/// yazılamaz: katalog dışı kısa çeşit ("Kapıda sıra.").
/// </summary>
internal sealed class NotifyWalkInQueue(
    IDbSession session, IBrandUserDirectory users, ITenantQueries tenants, InAppNotifier inApp, INotificationOutbox outbox)
    : IIntegrationEventHandler<WalkInTicketCreated>
{
    // Kural: bildirimler.md staff.walkin_queue; roller-yetkiler.md walkin.manage (Owner ✔, BranchManager Ş, personel ✔)
    public async Task HandleAsync(WalkInTicketCreated integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        await session.SetTenantAsync(TenantScope.ForBrand(integrationEvent.BrandId), ct);
        var zoneId = await tenants.GetBranchTimeZoneAsync(integrationEvent.BrandId, integrationEvent.BranchId, ct);
        if (zoneId is null)
        {
            return;
        }

        var recipients = (await users.GetUserIdsWithPermissionAsync(
                integrationEvent.BrandId, Permissions.WalkIn.Manage, integrationEvent.BranchId, ct))
            .Where(id => id != integrationEvent.CreatedByUserId)
            .ToList();
        if (recipients.Count == 0)
        {
            return;
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["n"] = integrationEvent.WaitingCount.ToString(CultureInfo.InvariantCulture),
        };
        string? variant = PushTemplates.WithoutEstimateVariant;
        if (integrationEvent.EstimatedWaitMinutes is { } minutes)
        {
            values["dk"] = minutes.ToString(CultureInfo.InvariantCulture);
            variant = null;
        }

        await inApp.AddAsync(
            new InAppEntry(integrationEvent.BrandId, NotificationEvents.StaffWalkInQueue, values, variant, integrationEvent.BranchId,
                InAppNotifier.WalkInQueueTarget, integrationEvent.TicketId),
            recipients,
            ct);
        await outbox.EnqueueStaffPushAsync(
            new StaffPushNotification(
                NotificationEvents.StaffWalkInQueue, integrationEvent.BrandId, recipients, values, zoneId,
                NotificationReference.ForWalkInTicket(integrationEvent.TicketId), variant),
            ct);
    }
}
