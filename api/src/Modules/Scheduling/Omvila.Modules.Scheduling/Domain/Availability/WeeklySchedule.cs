using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Haftalık saatler ve tarihe özel ezmeler (musaitlik.md § 1–2). Ezme varsa o gün haftalık saatin yerine geçer; boş
/// liste kapalı gündür (<c>IsClosed</c>). Günde birden çok aralık olabilir (öğle arası; M19).
/// </summary>
internal sealed record WeeklySchedule(
    IReadOnlyDictionary<IsoDayOfWeek, IReadOnlyList<LocalTimeRange>> Days,
    IReadOnlyDictionary<LocalDate, IReadOnlyList<LocalTimeRange>> Overrides)
{
    public static WeeklySchedule Closed { get; } = new(
        new Dictionary<IsoDayOfWeek, IReadOnlyList<LocalTimeRange>>(),
        new Dictionary<LocalDate, IReadOnlyList<LocalTimeRange>>());

    public IReadOnlyList<LocalTimeRange> RangesOn(LocalDate date) =>
        Overrides.TryGetValue(date, out var overridden) ? overridden
        : Days.TryGetValue(date.DayOfWeek, out var weekly) ? weekly
        : [];
}
