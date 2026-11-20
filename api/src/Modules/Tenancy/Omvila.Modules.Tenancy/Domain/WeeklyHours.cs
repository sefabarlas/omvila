using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Şubenin (veya personelin) haftalık saatleri: listede olmayan gün kapalıdır, mola iki aralık arasındaki
/// boşluktur (sözleşme WeeklyHours). Günde birden çok aralık olabilir; aralıklar örtüşmez.
/// </summary>
internal sealed class WeeklyHours
{
    private WeeklyHours(IReadOnlyDictionary<IsoDayOfWeek, IReadOnlyList<OpeningInterval>> days) => Days = days;

    public IReadOnlyDictionary<IsoDayOfWeek, IReadOnlyList<OpeningInterval>> Days { get; }

    public static WeeklyHours Closed { get; } = new(new Dictionary<IsoDayOfWeek, IReadOnlyList<OpeningInterval>>());

    public bool HasAnyOpening => Days.Count > 0;

    public static Result<WeeklyHours> Create(IReadOnlyList<(IsoDayOfWeek Day, IReadOnlyList<(LocalTime Start, LocalTime End)> Intervals)> days)
    {
        ArgumentNullException.ThrowIfNull(days);
        var result = new Dictionary<IsoDayOfWeek, IReadOnlyList<OpeningInterval>>();
        for (var i = 0; i < days.Count; i++)
        {
            var (day, raw) = days[i];
            if (day == IsoDayOfWeek.None || !result.TryAdd(day, []))
            {
                return Error.Validation($"days[{i}].dayOfWeek", "invalid_format");
            }

            var intervals = HoursValidation.Intervals(raw, $"days[{i}].intervals");
            if (!intervals.IsSuccess)
            {
                return intervals.Error;
            }

            result[day] = intervals.Value;
        }

        return new WeeklyHours(result);
    }
}
