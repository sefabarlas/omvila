using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>appointment.requested</c> (A1 onay gerekli; A13 müşteri saati değiştirince yeniden onay): "{salon}: Talebiniz alındı…".
/// Onay bekleyen talebe hatırlatma planlanmaz; varsa düşer (A13'te onaylı randevunun hatırlatmaları). İşleyici çalışana
/// kadar talep yanıtlandıysa hiçbir şey yazılmaz. Personel bildirimi (<c>staff.approval_needed</c>) ayrı işleyicide.
/// </summary>
internal sealed class NotifyAppointmentRequested(AppointmentSms sms) : IIntegrationEventHandler<AppointmentRequested>
{
    public async Task HandleAsync(AppointmentRequested integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context is null || !context.IsPendingApproval)
        {
            return;
        }

        await sms.CancelRemindersAsync(integrationEvent.AppointmentId, ct);
        if (!AppointmentSms.CanText(context) || sms.ManageLink(context) is not { } link)
        {
            return;
        }

        var start = context.Appointment.StartUtc;
        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["salon"] = context.Branch.BrandName,
            ["tarih"] = AppointmentSms.Date(context, start),
            ["saat"] = AppointmentSms.Time(context, start),
            ["bağlantı"] = link,
        };
        await sms.EnqueueAsync(context, NotificationEvents.AppointmentRequested, values, ct);
    }
}
