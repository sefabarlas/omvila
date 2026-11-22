using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications;

/// <summary>
/// İşletme push'unun kişisel tercih süzgeci (sözleşme MyNotificationPreferences: anahtar → olay eşlemesi). Anahtara bağlı
/// olmayan olaylar (staff.deposit_reported, owner.credits_low, owner.subscription_payment_failed) kapatılamaz.
/// </summary>
internal static class StaffPushPreferences
{
    // Kural: sözleşme manageGetMyNotificationPreferences ("Sahip ve yönetici olayları … kapatılamaz")
    public static bool AllowsPush(NotificationPreference preference, string eventCode)
    {
        ArgumentNullException.ThrowIfNull(preference);
        return eventCode switch
        {
            NotificationEvents.StaffAppointmentCreated or NotificationEvents.StaffApprovalNeeded => preference.NewOnlineAppointment,
            NotificationEvents.StaffAppointmentChanged => preference.CancellationAndReschedule,
            NotificationEvents.StaffCustomerCheckedIn => preference.CustomerCheckedIn,
            NotificationEvents.StaffWalkInQueue => preference.WalkInQueue,
            NotificationEvents.StaffReviewReceived => preference.NewReview,
            NotificationEvents.StaffDailySummary => preference.DailySummary,
            _ => true,
        };
    }
}
