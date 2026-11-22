using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// Müşteri SMS bağlantısından, uygulamadan veya hesabını silerek iptal etti (A6): müşteriye mesaj gitmez, bekleyen hatırlatmalar düşer.
/// Personele <c>staff.appointment_changed</c> <see cref="NotifyAppointmentCancelledByCustomer"/> ile gider (T-104).
/// </summary>
internal sealed class CancelRemindersOnCustomerCancel(AppointmentSms sms) : IIntegrationEventHandler<AppointmentCancelledByCustomer>
{
    public async Task HandleAsync(AppointmentCancelledByCustomer integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        await sms.EnterBrandAsync(integrationEvent.BrandId, ct);
        await sms.CancelPendingOnCancellationAsync(integrationEvent.AppointmentId, ct);
    }
}
