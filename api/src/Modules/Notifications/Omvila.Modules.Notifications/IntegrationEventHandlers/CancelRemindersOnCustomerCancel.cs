using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// Müşteri SMS bağlantısından iptal etti (A6): müşteriye mesaj gitmez, bekleyen hatırlatmalar düşer. Personele
/// <c>staff.appointment_changed</c> push ve panel bildirimidir; push kanalı ve bildirim paneli D3'te (T-53).
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
