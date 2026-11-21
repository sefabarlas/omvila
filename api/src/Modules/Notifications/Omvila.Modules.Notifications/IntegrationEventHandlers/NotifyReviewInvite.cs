using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Reviews.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>review.invite</c> (bildirimler.md § Müşteri olayları; durum-makinesi.md A11): tamamlandıktan ReviewInviteDelayMinutes sonra
/// zamanlanmış "Nasıldı?" satırı. Kanal planı Push → SMS (müşterinin uygulama hesabı verilir). Sessiz saate denk gelirse sabaha
/// kayar (T-89). Müşterinin "Randevu bilgilendirme SMS'leri" kapalıysa veya randevu artık tamamlanmış değilse yazılmaz.
/// </summary>
internal sealed class NotifyReviewInvite(AppointmentSms sms) : IIntegrationEventHandler<ReviewInviteScheduled>
{
    // Kural: bildirimler.md review.invite (Completed + 2 saat, Push → SMS)
    public async Task HandleAsync(ReviewInviteScheduled integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context is null || !string.Equals(context.Appointment.Status, "Completed", StringComparison.Ordinal) || !AppointmentSms.CanText(context))
        {
            return;
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["salon"] = context.Branch.BrandName,
            ["bağlantı"] = sms.ReviewLink(integrationEvent.AccessCode),
        };
        await sms.EnqueueAsync(
            context,
            NotificationEvents.ReviewInvite,
            values,
            ct,
            QuietHours.Defer(integrationEvent.SendAtUtc, context.Zone),
            customerId: context.Customer!.CustomerId);
    }
}
