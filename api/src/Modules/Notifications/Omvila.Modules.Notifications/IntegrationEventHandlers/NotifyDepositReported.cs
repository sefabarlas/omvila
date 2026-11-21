using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>staff.deposit_reported</c> (bildirimler.md § İşletme olayları; dilim-04 kabul kriteri 4): müşteri "Havaleyi yaptım" dedi.
/// Alıcılar kaporayı işaretleyebilenlerdir (appointment.deposit.mark, şube kapsamı: Owner ve şubenin yöneticileri; V5).
/// Panel kaydı (müşteri "Ad Soyad", hedef "Kapora geldi mi?") ve işletme push'u ("Ad S."; sessiz saatte sabaha ertelenir) yazılır.
/// Kullanıcı tercihiyle kapatılamaz (kapora işlemi). İşleyici çalışana kadar kapora artık beklenmiyorsa hiçbir şey yazılmaz.
/// </summary>
internal sealed class NotifyDepositReported(AppointmentSms sms, IBrandUserDirectory users, InAppNotifier inApp, INotificationOutbox outbox)
    : IIntegrationEventHandler<AppointmentDepositReported>
{
    // Kural: bildirimler.md staff.deposit_reported
    public async Task HandleAsync(AppointmentDepositReported integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context is null || context.Appointment.Deposit is null)
        {
            return;
        }

        var recipients = await users.GetUserIdsWithPermissionAsync(
            integrationEvent.BrandId, Permissions.Appointment.DepositMark, context.Appointment.BranchId, ct);
        if (recipients.Count == 0)
        {
            return;
        }

        var fullName = context.Customer?.FullName ?? string.Empty;
        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["müşteri"] = fullName,
            ["tutar"] = StaffText.Amount(integrationEvent.AmountMinor),
        };
        await inApp.AddAsync(
            new InAppEntry(integrationEvent.BrandId, NotificationEvents.StaffDepositReported, values, null, context.Appointment.BranchId,
                InAppNotifier.DepositTarget, integrationEvent.AppointmentId),
            recipients,
            ct);

        values["müşteri"] = StaffText.ShortName(fullName);
        await outbox.EnqueueStaffPushAsync(
            new StaffPushNotification(
                NotificationEvents.StaffDepositReported, integrationEvent.BrandId, recipients, values, context.Zone.Id,
                NotificationReference.ForAppointment(integrationEvent.AppointmentId)),
            ct);
    }
}
