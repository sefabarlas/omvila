using NodaTime;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// <c>staff.approval_needed</c> (bildirimler.md § İşletme olayları; dilim-03 kriter 1): onay bekleyen talep (A1, A13) talebi
/// yanıtlayabilen kullanıcılara (appointment.approve, şube kapsamı: Owner ve şubenin yöneticileri) bildirim paneli kaydı
/// (müşteri "Ad Soyad", kalan süre <c>holdExpiresUtc</c>'den) ve işletme push'u ("Ad S."). Aynı randevunun önceki bekleyen
/// push'u düşer, eski panel kayıtlarının kalan süresi boşalır (A13'te yeni tutma başlar). Push sessiz saatte sabaha ertelenir;
/// tutma süresi sabaha kadar dolacaksa push yazılmaz, yalnız panel kaydı kalır (T-66 (8)). İşleyici çalışana kadar talep
/// yanıtlandıysa veya süresi dolduysa hiçbir şey yazılmaz.
/// </summary>
internal sealed class NotifyApprovalNeeded(
    AppointmentSms sms,
    IBrandUserDirectory users,
    InAppNotifier inApp,
    INotificationOutbox outbox,
    IClock clock) : IIntegrationEventHandler<AppointmentRequested>
{
    public async Task HandleAsync(AppointmentRequested integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var now = clock.GetCurrentInstant();
        var context = await sms.LoadAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);
        if (context is null || !context.IsPendingApproval || integrationEvent.HoldExpiresUtc <= now)
        {
            return;
        }

        var reference = NotificationReference.ForAppointment(integrationEvent.AppointmentId);
        await outbox.CancelPendingAsync(NotificationEvents.StaffApprovalNeeded, reference, ct);
        await inApp.ClearHoldsAsync(integrationEvent.BrandId, integrationEvent.AppointmentId, ct);

        var recipients = await users.GetUserIdsWithPermissionAsync(
            integrationEvent.BrandId, Permissions.Appointment.Approve, context.Appointment.BranchId, ct);
        if (recipients.Count == 0)
        {
            return;
        }

        var start = context.Appointment.StartUtc;
        var fullName = context.Customer?.FullName ?? string.Empty;
        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["kalan süre"] = StaffText.Remaining(integrationEvent.HoldExpiresUtc - now),
            ["müşteri"] = fullName,
            ["tarih saat"] = $"{AppointmentSms.Date(context, start)} {AppointmentSms.Time(context, start)}",
            ["hizmet"] = SmsFormat.FirstAndRest(context.Appointment.ServiceNames),
        };
        await inApp.AddAsync(
            new InAppEntry(integrationEvent.BrandId, NotificationEvents.StaffApprovalNeeded, values, null, context.Appointment.BranchId,
                InAppNotifier.AppointmentTarget, integrationEvent.AppointmentId, integrationEvent.HoldExpiresUtc),
            recipients,
            ct);

        values["müşteri"] = StaffText.ShortName(fullName);
        await outbox.EnqueueStaffPushAsync(
            // Kural: T-66 (8) — sessiz saatte ertelenen push tutma süresinden sonra gidecekse yazılmaz (yanıtlanamaz talep).
            new StaffPushNotification(
                NotificationEvents.StaffApprovalNeeded, integrationEvent.BrandId, recipients, values, context.Zone.Id, reference,
                NotAfterUtc: integrationEvent.HoldExpiresUtc),
            ct);
    }
}
