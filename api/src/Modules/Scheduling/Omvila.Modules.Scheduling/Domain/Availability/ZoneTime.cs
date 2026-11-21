using NodaTime;
using NodaTime.TimeZones;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Yerel saatten <see cref="Instant"/>'a dönüşüm (musaitlik.md § Kavramlar: yerel → anlık). Şubenin saat dilimi kuralları
/// kullanılır (<see cref="Resolvers.LenientResolver"/>); sabit ofset yazılmaz. Yaz saati geçişinde atlanan yerel saat
/// ileri kayar, iki kez yaşanan yerel saatte ilki seçilir.
/// </summary>
internal static class ZoneTime
{
    public static Instant ToInstant(LocalDate date, LocalTime time, DateTimeZone zone)
    {
        ArgumentNullException.ThrowIfNull(zone);
        return zone.ResolveLocal(date.At(time), Resolvers.LenientResolver).ToInstant();
    }

    /// <summary>Yerel günün tamamı: o günün başından ertesi günün başına (yaz saati gününde 23 veya 25 saat).</summary>
    public static Interval Day(LocalDate date, DateTimeZone zone)
    {
        ArgumentNullException.ThrowIfNull(zone);
        return new Interval(zone.AtStartOfDay(date).ToInstant(), zone.AtStartOfDay(date.PlusDays(1)).ToInstant());
    }

    /// <summary>Bir günün yerel saat aralıklarını anlık aralık kümesine çevirir.</summary>
    public static IntervalSet ToSet(LocalDate date, IEnumerable<LocalTimeRange> ranges, DateTimeZone zone)
    {
        ArgumentNullException.ThrowIfNull(ranges);
        return IntervalSet.From(ranges.Select(r =>
        {
            var start = ToInstant(date, r.Start, zone);
            var end = ToInstant(date, r.End, zone);
            return end > start ? new Interval(start, end) : new Interval(start, start);
        }));
    }

    /// <summary>
    /// Izgara: yerel gece yarısından itibaren <paramref name="intervalMinutes"/> adımlı yerel saatler (musaitlik.md § 8;
    /// 15 dk ızgarada 10:00, 10:15…). Her yerel saat ayrı ayrı çevrilir; yaz saati gününde de ızgara yerel saatte kalır.
    /// Atlanan saatlerden doğan yinelenen anlar tekilleştirilir.
    /// </summary>
    public static IReadOnlyList<GridPoint> Grid(LocalDate date, DateTimeZone zone, int intervalMinutes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(intervalMinutes);
        var points = new List<GridPoint>(1440 / intervalMinutes);
        for (var minute = 0; minute < 1440; minute += intervalMinutes)
        {
            var local = LocalTime.Midnight.PlusMinutes(minute);
            var instant = ToInstant(date, local, zone);
            if (points.Count == 0 || instant > points[^1].Instant)
            {
                points.Add(new GridPoint(local, instant));
            }
        }

        return points;
    }
}
