using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>appointment.rescheduled</c> (A14, salon veya müşteri; "Müşteriye bildir" kapalıysa mesaj yok) ve hatırlatmaların
/// yeniden planlanması (bildirimler.md § İptal: "Saat değişince hatırlatmalar yeniden planlanır"). Anahtar kapalı olsa da
/// hatırlatmalar yeni saate taşınır.
/// </summary>
internal sealed class NotifyAppointmentRescheduled(AppointmentSms sms) : IIntegrationEventHandler<AppointmentRescheduled>
{
    public async Task HandleAsync(AppointmentRescheduled integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context is null)
        {
            return;
        }

        if (integrationEvent.NotifyCustomer && context.IsConfirmed && AppointmentSms.CanText(context) && sms.ManageLink(context) is { } link)
        {
            var values = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["salon"] = context.Branch.BrandName,
                ["yeni tarih"] = AppointmentSms.Date(context, integrationEvent.StartUtc),
                ["saat"] = AppointmentSms.Time(context, integrationEvent.StartUtc),
                ["bağlantı"] = link,
            };
            await sms.EnqueueAsync(context, NotificationEvents.AppointmentRescheduled, values, ct);
        }

        await sms.RescheduleRemindersAsync(context, ct);
    }
}
