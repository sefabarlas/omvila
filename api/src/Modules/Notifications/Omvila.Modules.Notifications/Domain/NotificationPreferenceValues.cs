namespace Omvila.Modules.Notifications.Domain;

/// <summary>Kullanıcının değiştirebildiği tercih anahtarları.</summary>
internal sealed record NotificationPreferenceValues(
    bool NewOnlineAppointment,
    bool CancellationAndReschedule,
    bool CustomerCheckedIn,
    bool WalkInQueue,
    bool NewReview,
    bool DailySummary,
    bool QuietHoursEnabled);
