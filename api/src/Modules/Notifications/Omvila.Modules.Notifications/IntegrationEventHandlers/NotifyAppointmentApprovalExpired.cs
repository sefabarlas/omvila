using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>appointment.approval_expired</c> (A5, kapora yok): müşteriye "…zamanında yanıtlayamadı, talep iptal edildi…" ve bekleyen
/// hatırlatmaların düşmesi. Neden <c>DepositNotReceived</c> ise metin Dilim 4'teki <c>deposit.not_received</c>; şimdilik
/// yalnız hatırlatmalar düşer.
/// </summary>
internal sealed class NotifyAppointmentApprovalExpired(AppointmentSms sms) : IIntegrationEventHandler<AppointmentApprovalExpired>
{
    // Scheduling CancellationReason metni (AppointmentApprovalExpired.Reason).
    private const string ApprovalExpiredReason = "ApprovalExpired";

    public async Task HandleAsync(AppointmentApprovalExpired integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        if (!string.Equals(integrationEvent.Reason, ApprovalExpiredReason, StringComparison.Ordinal))
        {
            await sms.EnterBrandAsync(integrationEvent.BrandId, ct);
            await sms.CancelRemindersAsync(integrationEvent.AppointmentId, ct);
            return;
        }

        await sms.NotifyRequestCancelledAsync(
            integrationEvent.BrandId, integrationEvent.AppointmentId, integrationEvent.StartUtc, NotificationEvents.AppointmentApprovalExpired, ct);
    }
}
