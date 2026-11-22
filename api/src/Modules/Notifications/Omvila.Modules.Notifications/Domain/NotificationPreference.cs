using NodaTime;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>
/// İşletme kullanıcısının kendi bildirim tercihleri (alan-modeli.md § NotificationPreference; sözleşme MyNotificationPreferences).
/// Marka + kullanıcı başına tek satır; satır yoksa <see cref="Defaults"/> geçerlidir (gün sonu özeti kapalı, diğerleri açık).
/// Tercihler yalnız push'u etkiler; bildirim paneli kaydı her durumda yazılır.
/// </summary>
internal sealed class NotificationPreference
{
    private NotificationPreference()
    {
    }

    public Guid BrandId { get; private set; }

    public Guid UserId { get; private set; }

    public bool NewOnlineAppointment { get; private set; } = true;

    public bool CancellationAndReschedule { get; private set; } = true;

    public bool CustomerCheckedIn { get; private set; } = true;

    public bool WalkInQueue { get; private set; } = true;

    public bool NewReview { get; private set; } = true;

    public bool DailySummary { get; private set; }

    public bool QuietHoursEnabled { get; private set; } = true;

    public Instant CreatedUtc { get; private set; }

    public Instant UpdatedUtc { get; private set; }

    public uint Version { get; private set; }

    /// <summary>Kaydı olmayan kullanıcının tercihleri (bildirimler.md: gün sonu özeti kapalı varsayılan).</summary>
    public static NotificationPreference Defaults(Guid brandId, Guid userId) => new() { BrandId = brandId, UserId = userId };

    public static NotificationPreference Create(Guid brandId, Guid userId, NotificationPreferenceValues values, Instant now)
    {
        var preference = new NotificationPreference { BrandId = brandId, UserId = userId, CreatedUtc = now };
        preference.Apply(values, now);
        return preference;
    }

    public void Apply(NotificationPreferenceValues values, Instant now)
    {
        ArgumentNullException.ThrowIfNull(values);
        NewOnlineAppointment = values.NewOnlineAppointment;
        CancellationAndReschedule = values.CancellationAndReschedule;
        CustomerCheckedIn = values.CustomerCheckedIn;
        WalkInQueue = values.WalkInQueue;
        NewReview = values.NewReview;
        DailySummary = values.DailySummary;
        QuietHoursEnabled = values.QuietHoursEnabled;
        UpdatedUtc = now;
    }
}
