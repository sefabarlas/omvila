using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// A5 süre dolumu: bekleyen hatırlatmalar düşer ve müşteriye iptal metni gider. Neden <c>DepositNotReceived</c> (kapora
/// istenmişti) ise <c>deposit.not_received</c> ("… kapora {süre} içinde gelmediği için … iptal edildi. Havale yaptıysanız salonu
/// arayın."; yalnız SMS), değilse <c>appointment.approval_expired</c> ("…zamanında yanıtlayamadı, talep iptal edildi…").
/// </summary>
internal sealed class NotifyAppointmentApprovalExpired(AppointmentSms sms, DepositSms depositSms) : IIntegrationEventHandler<AppointmentApprovalExpired>
{
    // Scheduling CancellationReason metni (AppointmentApprovalExpired.Reason).
    private const string DepositNotReceivedReason = "DepositNotReceived";

    // Kural: durum-makinesi.md A5; bildirimler.md appointment.approval_expired, deposit.not_received
    public async Task HandleAsync(AppointmentApprovalExpired integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        if (!string.Equals(integrationEvent.Reason, DepositNotReceivedReason, StringComparison.Ordinal) || integrationEvent.DepositAmountMinor is not { } amount)
        {
            await sms.NotifyRequestCancelledAsync(
                integrationEvent.BrandId, integrationEvent.AppointmentId, integrationEvent.StartUtc, NotificationEvents.AppointmentApprovalExpired, ct);
            return;
        }

        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        await sms.CancelRemindersAsync(integrationEvent.AppointmentId, ct);
        if (context is not null && AppointmentSms.CanText(context))
        {
            await depositSms.EnqueueNotReceivedAsync(context, integrationEvent.StartUtc, amount, ct);
        }
    }
}
