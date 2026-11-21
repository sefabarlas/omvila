using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// Talep yanıtlandı (A2 onay, A3 "Kapora geldi", A4 red, A5 süre dolumu): randevunun henüz gönderilmemiş <c>staff.approval_needed</c> push'u
/// düşer (sessiz saatte sabaha ertelenmiş satır yanıtlanmış talebi hatırlatmasın; T-66 (8)) ve panel kayıtlarının kalan süresi
/// boşalır (sözleşme NotificationTarget.holdExpiresUtc: "Talep yanıtlandıysa null"). Panel kaydı silinmez.
/// </summary>
internal sealed class ResolveApprovalNotifications(AppointmentSms sms, InAppNotifier inApp, INotificationOutbox outbox)
    : IIntegrationEventHandler<AppointmentApproved>, IIntegrationEventHandler<AppointmentDeclined>, IIntegrationEventHandler<AppointmentApprovalExpired>,
      IIntegrationEventHandler<AppointmentDepositReceived>
{
    public Task HandleAsync(AppointmentApproved integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        return ResolveAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
    }

    public Task HandleAsync(AppointmentDeclined integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        return ResolveAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
    }

    public Task HandleAsync(AppointmentApprovalExpired integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        return ResolveAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
    }

    public Task HandleAsync(AppointmentDepositReceived integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        return ResolveAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
    }

    private async Task ResolveAsync(Guid brandId, Guid appointmentId, CancellationToken ct)
    {
        await sms.EnterBrandAsync(brandId, ct);
        await outbox.CancelPendingAsync(NotificationEvents.StaffApprovalNeeded, NotificationReference.ForAppointment(appointmentId), ct);
        await inApp.ClearHoldsAsync(brandId, appointmentId, ct);
    }
}
