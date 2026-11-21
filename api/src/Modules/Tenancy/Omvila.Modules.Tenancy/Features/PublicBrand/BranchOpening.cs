using NodaTime;

namespace Omvila.Modules.Tenancy.Features.PublicBrand;

/// <summary>
/// Şube kartındaki "Şu an açık · 19:00'a kadar" / "Bugün kapalı · Yarın 10:00'da açılır" (sözleşme PublicBrand.branches:
/// openNow, todayClosesAt, nextOpeningUtc). Gün saatleri: o güne özel gün varsa onun saatleri, yoksa haftalık saatler.
/// Hesap şubenin saat dilimindedir.
/// </summary>
internal static class BranchOpening
{
    /// <summary>Kapalıyken ileriye bakılan gün sayısı (iki hafta; daha uzun kapalı şubede null).</summary>
    private const int LookAheadDays = 14;

    public static (bool OpenNow, LocalTime? TodayClosesAt, Instant? NextOpening) Compute(
        Instant now,
        DateTimeZone zone,
        IReadOnlyList<(IsoDayOfWeek Day, LocalTime Start, LocalTime End)> weekly,
        IReadOnlyDictionary<LocalDate, IReadOnlyList<(LocalTime Start, LocalTime End)>> overrides)
    {
        var local = now.InZone(zone);
        var today = RangesOn(local.Date, weekly, overrides);
        var openNow = today.Any(r => r.Start <= local.TimeOfDay && local.TimeOfDay < r.End);
        LocalTime? closesAt = today.Count == 0 ? null : today.Max(r => r.End);
        if (openNow)
        {
            return (true, closesAt, null);
        }

        for (var offset = 0; offset <= LookAheadDays; offset++)
        {
            var date = local.Date.PlusDays(offset);
            foreach (var range in RangesOn(date, weekly, overrides).OrderBy(r => r.Start))
            {
                var start = zone.AtLeniently(date.At(range.Start)).ToInstant();
                if (start > now)
                {
                    return (false, closesAt, start);
                }
            }
        }

        return (false, closesAt, null);
    }

    private static List<(LocalTime Start, LocalTime End)> RangesOn(
        LocalDate date,
        IReadOnlyList<(IsoDayOfWeek Day, LocalTime Start, LocalTime End)> weekly,
        IReadOnlyDictionary<LocalDate, IReadOnlyList<(LocalTime Start, LocalTime End)>> overrides) =>
        overrides.TryGetValue(date, out var special)
            ? [.. special]
            : [.. weekly.Where(w => w.Day == date.DayOfWeek).Select(w => (w.Start, w.End))];
}
