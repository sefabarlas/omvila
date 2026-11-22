using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Doluluk (sözleşme ManageOverview.weeklyOccupancy; docs/acik-sorular.md T-120): randevulu dakika ÷ çalışma dakikası.
/// Çalışma = personelin bu şubedeki saatleri ∩ şubenin açık saatleri (özel günler ve "Yalnızca bu gün için aç" motordaki gibi)
/// − izinler. Randevulu = personelin meşgul dilimleri (bekleme dilimi hariç; taslak ve iptal dışı, Gelmedi dahil) ∩ çalışma;
/// böylece saat dışına taşan randevu yüzdeyi 100'ün üstüne çıkarmaz. Yüzde tam sayıya yarım yukarı yuvarlanır.
/// </summary>
internal static class Occupancy
{
    /// <summary>Personelin [<paramref name="from"/>, <paramref name="to"/>] günlerindeki doluluk dakikaları (saniye).</summary>
    // Kural: musaitlik.md § 1–3 (çalışma saati, özel gün, izin); sözleşme manageGetOverview ("izin ve kapalı gün düşülür; Draft ve Cancelled sayılmaz, NoShow sayılır")
    public static (long BookedSeconds, long WorkingSeconds) Measure(
        AvailabilitySnapshot snapshot, StaffSchedule staff, LocalDate from, LocalDate to, IEnumerable<Interval> busy)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(staff);
        ArgumentNullException.ThrowIfNull(busy);
        var working = IntervalSet.Empty;
        for (var date = from; date <= to; date = date.PlusDays(1))
        {
            var open = ZoneTime.ToSet(date, snapshot.BranchHours.RangesOn(date), snapshot.Zone);
            working = working.Union(ZoneTime.ToSet(date, AvailabilityEngine.StaffHoursOn(snapshot, staff, date), snapshot.Zone).Intersect(open));
        }

        working = working.Subtract(IntervalSet.From(staff.TimeOffs));
        var booked = IntervalSet.From(busy).Intersect(working);
        return ((long)booked.TotalDuration.TotalSeconds, (long)working.TotalDuration.TotalSeconds);
    }

    /// <summary>Yüzde (0–100, yarım yukarı); çalışma süresi yoksa null.</summary>
    public static int? Percent(long bookedSeconds, long workingSeconds) =>
        workingSeconds <= 0 ? null : (int)Math.Min(100, Math.Round(bookedSeconds * 100m / workingSeconds, MidpointRounding.AwayFromZero));
}
