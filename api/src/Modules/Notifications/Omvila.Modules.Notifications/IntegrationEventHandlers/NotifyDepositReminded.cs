using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// Salon "Hatırlat" dedi (V6): müşteriye <c>deposit.awaiting</c> SMS'i kalan süreyle yeniden yazılır (her zaman SMS, sessiz
/// saate kaydırılmaz, 1 kontör). Kontör uçta denetlenir; gönderime kadar biterse satır <c>Skipped</c> olur. İşleyici çalışana
/// kadar kapora artık beklenmiyorsa (geldi, süre doldu) veya müşteriye SMS yazılamıyorsa hiçbir şey yazılmaz.
/// </summary>
internal sealed class NotifyDepositReminded(AppointmentSms sms, DepositSms depositSms) : IIntegrationEventHandler<AppointmentDepositReminded>
{
    // Kural: V6; bildirimler.md deposit.awaiting
    public async Task HandleAsync(AppointmentDepositReminded integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context?.Appointment.Deposit is { } deposit && AppointmentSms.CanText(context))
        {
            await depositSms.EnqueueAwaitingAsync(context, deposit, ct);
        }
    }
}
