using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>staff.appointment_changed</c> iptal çeşidi (bildirimler.md § İşletme olayları: "{Müşteri} · {saat} iptal etti. Neden: {neden}."):
/// müşteri randevuyu SMS bağlantısından, müşteri uygulamasından veya hesabını silerek iptal etti (A6; T-104). Alıcılar ilgili personel
/// (kalemlerin personel kayıtlarına bağlı kullanıcılar) ve şubenin yöneticileridir (appointment.approve, şube kapsamı). Panel kaydı
/// müşterinin tam adıyla, push "Ad S." ile; push kişisel tercihle (cancellationAndReschedule) ve sessiz saatle outbox'ta süzülür.
/// Müşteriye mesaj gitmez (hatırlatmaları <see cref="CancelRemindersOnCustomerCancel"/> düşürür).
/// </summary>
internal sealed class NotifyAppointmentCancelledByCustomer(AppointmentSms sms, IBrandUserDirectory users, InAppNotifier inApp, INotificationOutbox outbox)
    : IIntegrationEventHandler<AppointmentCancelledByCustomer>
{
    // Neden çipleri (sözleşme meCancelAppointment; "Hastayım" yerine "Rahatsızlandım") ve hesap silme.
    private static readonly Dictionary<string, string> ReasonLabels = new(StringComparer.Ordinal)
    {
        ["PlanChanged"] = "Planım değişti",
        ["Sick"] = "Rahatsızlandım",
        ["WantsDifferentTime"] = "Başka saat istiyorum",
        ["Other"] = "Diğer",
        ["AccountDeleted"] = "Hesabını sildi",
    };

    // Kural: bildirimler.md staff.appointment_changed (İlgili personel; T-104: + şube yöneticileri)
    public async Task HandleAsync(AppointmentCancelledByCustomer integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context is null)
        {
            return;
        }

        var managers = await users.GetUserIdsWithPermissionAsync(
            integrationEvent.BrandId, Permissions.Appointment.Approve, context.Appointment.BranchId, ct);
        var staff = await users.GetUserIdsForStaffAsync(integrationEvent.BrandId, context.Appointment.StaffIds ?? [], ct);
        var recipients = staff.Concat(managers).Distinct().ToList();
        if (recipients.Count == 0)
        {
            return;
        }

        var fullName = context.Customer?.FullName ?? string.Empty;
        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["müşteri"] = fullName,
            ["saat"] = AppointmentSms.Time(context, integrationEvent.StartUtc),
            ["neden"] = ReasonLabels.GetValueOrDefault(integrationEvent.Reason, ReasonLabels["Other"]),
        };
        await inApp.AddAsync(
            new InAppEntry(integrationEvent.BrandId, NotificationEvents.StaffAppointmentChanged, values, PushTemplates.CancelledVariant,
                context.Appointment.BranchId, InAppNotifier.AppointmentTarget, integrationEvent.AppointmentId),
            recipients,
            ct);

        values["müşteri"] = StaffText.ShortName(fullName);
        await outbox.EnqueueStaffPushAsync(
            new StaffPushNotification(
                NotificationEvents.StaffAppointmentChanged, integrationEvent.BrandId, recipients, values, context.Zone.Id,
                NotificationReference.ForAppointment(integrationEvent.AppointmentId), PushTemplates.CancelledVariant),
            ct);
    }
}
