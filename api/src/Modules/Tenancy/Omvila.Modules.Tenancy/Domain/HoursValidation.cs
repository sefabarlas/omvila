using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Bir günün aralıklarını doğrular: 1–4 aralık (sözleşme WeeklyHours), her biri geçerli, örtüşmez.</summary>
internal static class HoursValidation
{
    // Sözleşme: WeeklyHours.days[].intervals maxItems 4.
    public const int MaxIntervalsPerDay = 4;

    public static Result<IReadOnlyList<OpeningInterval>> Intervals(
        IReadOnlyList<(LocalTime Start, LocalTime End)> raw, string field)
    {
        ArgumentNullException.ThrowIfNull(raw);
        if (raw.Count is 0 or > MaxIntervalsPerDay)
        {
            return Error.Validation(field, "out_of_range");
        }

        var intervals = new List<OpeningInterval>(raw.Count);
        for (var i = 0; i < raw.Count; i++)
        {
            var interval = OpeningInterval.Create(raw[i].Start, raw[i].End, $"{field}[{i}]");
            if (!interval.IsSuccess)
            {
                return interval.Error;
            }

            intervals.Add(interval.Value);
        }

        intervals.Sort((a, b) => a.Start.CompareTo(b.Start));
        for (var i = 1; i < intervals.Count; i++)
        {
            if (intervals[i - 1].Overlaps(intervals[i]))
            {
                return Error.Validation(field, "overlapping");
            }
        }

        return intervals;
    }
}
