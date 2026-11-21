using System.Globalization;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Reviews.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>staff.review_received</c> (bildirimler.md § İşletme olayları): yeni değerlendirme. Alıcılar değerlendirmeleri görebilenlerdir
/// (review.view, şube kapsamı: Owner ve şubenin yöneticileri); personel kendi aldığı değerlendirmenin bildirimini almaz (W8,
/// T-86 (4)). Panel kaydı (müşteri "Ad Soyad", hedef Değerlendirmeler) ve işletme push'u ("Ad S."; sessiz saatte sabaha ertelenir).
/// </summary>
internal sealed class NotifyReviewReceived(AppointmentSms sms, IBrandUserDirectory users, InAppNotifier inApp, INotificationOutbox outbox)
    : IIntegrationEventHandler<ReviewReceived>
{
    // Kural: bildirimler.md staff.review_received; roller-yetkiler.md review.view (Owner ✔, BranchManager Ş, personel —)
    public async Task HandleAsync(ReviewReceived integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context is null)
        {
            return;
        }

        var recipients = await users.GetUserIdsWithPermissionAsync(
            integrationEvent.BrandId, Permissions.Review.View, integrationEvent.BranchId, ct);
        if (recipients.Count == 0)
        {
            return;
        }

        var fullName = context.Customer?.FullName ?? string.Empty;
        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["puan"] = integrationEvent.Rating.ToString(CultureInfo.InvariantCulture),
            ["müşteri"] = fullName,
        };
        await inApp.AddAsync(
            new InAppEntry(integrationEvent.BrandId, NotificationEvents.StaffReviewReceived, values, null, integrationEvent.BranchId,
                InAppNotifier.ReviewsTarget, integrationEvent.ReviewId),
            recipients,
            ct);

        values["müşteri"] = StaffText.ShortName(fullName);
        await outbox.EnqueueStaffPushAsync(
            new StaffPushNotification(
                NotificationEvents.StaffReviewReceived, integrationEvent.BrandId, recipients, values, context.Zone.Id,
                NotificationReference.ForAppointment(integrationEvent.AppointmentId)),
            ct);
    }
}
