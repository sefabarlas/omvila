using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>appointment.approved</c> (A2): "{salon} talebinizi onayladı…" ve hatırlatmaların bu anda planlanması (onaylanan talep
/// için <see cref="AppointmentConfirmed"/> yayımlanmaz). Randevu işleyici çalışana kadar artık onaylı değilse hiçbir şey
/// yazılmaz.
/// </summary>
internal sealed class NotifyAppointmentApproved(AppointmentSms sms) : IIntegrationEventHandler<AppointmentApproved>
{
    public async Task HandleAsync(AppointmentApproved integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context is null || !context.IsConfirmed)
        {
            return;
        }

        if (AppointmentSms.CanText(context) && sms.ManageLink(context) is { } link)
        {
            var start = context.Appointment.StartUtc;
            var values = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["salon"] = context.Branch.BrandName,
                ["tarih"] = AppointmentSms.Date(context, start),
                ["saat"] = AppointmentSms.Time(context, start),
                ["hizmet"] = SmsFormat.FirstAndRest(context.Appointment.ServiceNames),
                ["bağlantı"] = link,
            };
            await sms.EnqueueAsync(context, NotificationEvents.AppointmentApproved, values, ct);
        }

        await sms.RescheduleRemindersAsync(context, ct);
    }
}
