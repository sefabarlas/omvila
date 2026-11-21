using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>appointment.declined</c> (A4): müşteriye "…talebinizi onaylayamadı, talep iptal edildi…" ("reddedildi" yazılmaz;
/// "Müşteriye bildir" anahtarı yok, T-62 (1)) ve bekleyen hatırlatmaların düşmesi.
/// </summary>
internal sealed class NotifyAppointmentDeclined(AppointmentSms sms) : IIntegrationEventHandler<AppointmentDeclined>
{
    public Task HandleAsync(AppointmentDeclined integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        return sms.NotifyRequestCancelledAsync(
            integrationEvent.BrandId, integrationEvent.AppointmentId, integrationEvent.StartUtc, NotificationEvents.AppointmentDeclined, ct);
    }
}
