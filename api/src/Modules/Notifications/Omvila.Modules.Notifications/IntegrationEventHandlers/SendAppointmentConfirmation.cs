using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>appointment.confirmed</c> ve hatırlatmalar (dilim-02 kabul kriterleri 1 ve 11). Onay mesajı "Müşteriye SMS gönder"
/// açıksa ve tekrarsız randevuda ya da serinin ilk randevusunda yazılır; hatırlatmalar her tekrar için planlanır. Randevu
/// işleyici çalışana kadar iptal edildiyse veya artık onaylı değilse hiçbir şey yazılmaz.
/// </summary>
internal sealed class SendAppointmentConfirmation(AppointmentSms sms) : IIntegrationEventHandler<AppointmentConfirmed>
{
    public async Task HandleAsync(AppointmentConfirmed integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context is null || !context.IsConfirmed)
        {
            return;
        }

        var sendConfirmation = integrationEvent.NotifyCustomer && (integrationEvent.SeriesId is null || integrationEvent.IsSeriesStart);
        if (sendConfirmation && AppointmentSms.CanText(context) && sms.ManageLink(context) is { } link)
        {
            var start = context.Appointment.StartUtc;
            var staff = SmsFormat.FirstAndRest(context.Appointment.StaffNames);
            var values = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["salon"] = context.Branch.BrandName,
                ["tarih"] = AppointmentSms.Date(context, start),
                ["saat"] = AppointmentSms.Time(context, start),
                ["hizmet"] = SmsFormat.FirstAndRest(context.Appointment.ServiceNames),
                ["personel"] = staff,
                ["bağlantı"] = link,
            };
            await sms.EnqueueAsync(
                context, NotificationEvents.AppointmentConfirmed, values, ct, variant: staff.Length == 0 ? SmsTemplates.WithoutStaffVariant : null);
        }

        await sms.RescheduleRemindersAsync(context, ct);
    }
}
