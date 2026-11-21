using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>deposit.received</c> (A3; dilim-04 kabul kriteri 5): "{salon}: {tutar} TL kaporanız alındı…" ve hatırlatmaların bu anda
/// planlanması ("Kapora geldi" onay sayılır; <see cref="AppointmentApproved"/> yayımlanmaz). Kapora mesajı her zaman SMS'tir
/// (V3; push denenmez) ve hemen gider (sessiz saatte kaydırılmaz). Randevu işleyici çalışana kadar artık onaylı değilse
/// hiçbir şey yazılmaz.
/// </summary>
internal sealed class NotifyDepositReceived(AppointmentSms sms) : IIntegrationEventHandler<AppointmentDepositReceived>
{
    // Kural: bildirimler.md deposit.received
    public async Task HandleAsync(AppointmentDepositReceived integrationEvent, CancellationToken ct)
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
                ["tutar"] = SmsFormat.Amount(integrationEvent.AmountMinor),
                ["tarih"] = AppointmentSms.Date(context, start),
                ["saat"] = AppointmentSms.Time(context, start),
                ["bağlantı"] = link,
            };
            await sms.EnqueueAsync(context, NotificationEvents.DepositReceived, values, ct);
        }

        await sms.RescheduleRemindersAsync(context, ct);
    }
}
