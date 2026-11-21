using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain.Waitlist;

/// <summary>
/// Bekleme tercihi (sözleşme WaitlistPreferences; alan-modeli.md § WaitlistEntry). <see cref="Dates"/> boşsa her gün,
/// <see cref="TimeWindows"/> boşsa her saat uyar (T-86 (5)). <see cref="PreferredStaffId"/> null = fark etmez.
/// </summary>
internal sealed record WaitlistPreferences(
    IReadOnlyList<Guid> ServiceIds,
    Guid? PreferredStaffId,
    bool AcceptOtherStaff,
    IReadOnlyList<LocalDate> Dates,
    IReadOnlyList<WaitlistTimeWindow> TimeWindows)
{
    /// <summary>Sözleşme WaitlistPreferences.dates maxItems.</summary>
    public const int MaxDates = 14;

    /// <summary>
    /// Kural: T-90. Yerel başlangıç saatinin dilimi: Sabah &lt; 12:00 ≤ Öğleden sonra &lt; 17:00 ≤ Akşam
    /// (<see cref="PlatformDefaults.WaitlistMorningEndHour"/>, <see cref="PlatformDefaults.WaitlistAfternoonEndHour"/>).
    /// </summary>
    public static WaitlistTimeWindow WindowOf(LocalTime start) =>
        start.Hour < PlatformDefaults.WaitlistMorningEndHour ? WaitlistTimeWindow.Morning
        : start.Hour < PlatformDefaults.WaitlistAfternoonEndHour ? WaitlistTimeWindow.Afternoon
        : WaitlistTimeWindow.Evening;

    /// <summary>
    /// Kural: alan-modeli.md § WaitlistEntry, T-90. Boşalan saat (personel, yerel başlangıç) bu tercihe uyar mı: personel
    /// tercih edilen, fark etmez veya "Başka personel de olur"; gün listede veya liste boş; başlangıç seçilen saat
    /// diliminde veya liste boş. Hizmetin sığması motorun işidir.
    /// </summary>
    public bool Accepts(Guid staffId, LocalDateTime localStart) =>
        (PreferredStaffId is null || PreferredStaffId == staffId || AcceptOtherStaff)
        && (Dates.Count == 0 || Dates.Contains(localStart.Date))
        && (TimeWindows.Count == 0 || TimeWindows.Contains(WindowOf(localStart.TimeOfDay)));

    /// <summary>Aynı hizmetler mi (sıra ve tekrar önemsiz; "aynı şube + aynı hizmetler" için yeniden katılma).</summary>
    public bool SameServices(IEnumerable<Guid> serviceIds) => ServiceIds.ToHashSet().SetEquals(serviceIds);

    /// <summary>Kural: T-90. İstenen günlerin hepsi geçtiyse (bugünden önce) kayıt beklemeyi bırakır; gün listesi boşsa hiç.</summary>
    public bool IsPastAllDates(LocalDate today) => Dates.Count > 0 && Dates.All(d => d < today);
}
