using System.Globalization;
using NodaTime;
using Omvila.Modules.Notifications.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Features.Preferences;

/// <summary>
/// Sözleşme MyNotificationPreferences (istek ve yanıt). İstekte anahtarların hepsi zorunludur; salt okunur saat alanları ve
/// sürüm yok sayılır.
/// </summary>
internal sealed record MyNotificationPreferences(
    bool? NewOnlineAppointment,
    bool? CancellationAndReschedule,
    bool? CustomerCheckedIn,
    bool? WalkInQueue,
    bool? NewReview,
    bool? DailySummary,
    bool? QuietHoursEnabled,
    string? DailySummaryLocalTime = null,
    string? QuietHoursStartLocal = null,
    string? QuietHoursEndLocal = null,
    string? Version = null)
{
    public static MyNotificationPreferences From(NotificationPreference p)
    {
        ArgumentNullException.ThrowIfNull(p);
        return new(
            p.NewOnlineAppointment,
            p.CancellationAndReschedule,
            p.CustomerCheckedIn,
            p.WalkInQueue,
            p.NewReview,
            p.DailySummary,
            p.QuietHoursEnabled,
            Hour(PlatformDefaults.DailySummaryHour),
            Hour(PlatformDefaults.QuietHoursStartHour),
            Hour(PlatformDefaults.QuietHoursEndHour),
            p.Version.ToString(CultureInfo.InvariantCulture));
    }

    public Dictionary<string, string[]> Validate()
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        Need("newOnlineAppointment", NewOnlineAppointment);
        Need("cancellationAndReschedule", CancellationAndReschedule);
        Need("customerCheckedIn", CustomerCheckedIn);
        Need("walkInQueue", WalkInQueue);
        Need("newReview", NewReview);
        Need("dailySummary", DailySummary);
        Need("quietHoursEnabled", QuietHoursEnabled);
        return errors;

        void Need(string field, bool? value)
        {
            if (value is null)
            {
                errors[field] = ["required"];
            }
        }
    }

    /// <summary><see cref="Validate"/> hatasız döndükten sonra çağrılır.</summary>
    public NotificationPreferenceValues ToValues() => new(
        NewOnlineAppointment!.Value,
        CancellationAndReschedule!.Value,
        CustomerCheckedIn!.Value,
        WalkInQueue!.Value,
        NewReview!.Value,
        DailySummary!.Value,
        QuietHoursEnabled!.Value);

    private static string Hour(int hour) => new LocalTime(hour, 0).ToString("HH:mm", CultureInfo.InvariantCulture);
}
