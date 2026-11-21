using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>appointment.cancelled_by_salon</c> (A7; "Müşteriye bildir" kapalıysa mesaj yok) ve bekleyen hatırlatmaların düşmesi.
/// Kapora cümlesi ("Kapora iadesi için salon sizinle iletişime geçecek.") kapora alınan randevuda gerekir; kapora D4'te.
/// </summary>
internal sealed class NotifyAppointmentCancelledBySalon(AppointmentSms sms) : IIntegrationEventHandler<AppointmentCancelledBySalon>
{
    public async Task HandleAsync(AppointmentCancelledBySalon integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        await sms.CancelRemindersAsync(integrationEvent.AppointmentId, ct);
        if (context is null || !integrationEvent.NotifyCustomer || !AppointmentSms.CanText(context))
        {
            return;
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["salon"] = context.Branch.BrandName,
            ["tarih"] = AppointmentSms.Date(context, integrationEvent.StartUtc),
            ["saat"] = AppointmentSms.Time(context, integrationEvent.StartUtc),
            ["rezervasyon bağlantısı"] = sms.BookingLink(context),
        };
        await sms.EnqueueAsync(context, NotificationEvents.AppointmentCancelledBySalon, values, ct);
    }
}
