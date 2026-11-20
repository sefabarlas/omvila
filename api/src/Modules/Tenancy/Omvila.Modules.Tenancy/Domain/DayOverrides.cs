using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Özel gün ekleme (sözleşme WorkingHoursOverrideCreate): tek gün veya çok günlü kapanış ("Ramazan Bayramı · 3 gün")
/// tarih başına satırlara açılır. Kapalı günde saat olmaz; açık günde geçerli aralık zorunlu.
/// Tür tutarlılığı: Closure kapalı, ExtraOpening ve ShortDay açık; Holiday ikisi de olabilir.
/// </summary>
internal static class DayOverrides
{
    public static Result<IReadOnlyList<DayOverride>> Create(
        LocalDate fromDate,
        LocalDate? toDate,
        OverrideKind kind,
        bool isClosed,
        IReadOnlyList<(LocalTime Start, LocalTime End)> intervals,
        string? note)
    {
        ArgumentNullException.ThrowIfNull(intervals);
        var lastDate = toDate ?? fromDate;
        if (lastDate < fromDate)
        {
            return Error.Validation("toDate", "invalid_range");
        }

        var kindAllowsState = kind switch
        {
            OverrideKind.Closure => isClosed,
            OverrideKind.ExtraOpening or OverrideKind.ShortDay => !isClosed,
            _ => true,
        };
        if (!kindAllowsState)
        {
            return Error.Validation("isClosed", "invalid_format");
        }

        IReadOnlyList<OpeningInterval> validated = [];
        if (isClosed)
        {
            if (intervals.Count > 0)
            {
                return Error.Validation("intervals", "invalid_format");
            }
        }
        else
        {
            var result = HoursValidation.Intervals(intervals, "intervals");
            if (!result.IsSuccess)
            {
                return result.Error;
            }

            validated = result.Value;
        }

        var days = new List<DayOverride>();
        for (var date = fromDate; date <= lastDate; date = date.PlusDays(1))
        {
            days.Add(new DayOverride(date, kind, isClosed, validated, string.IsNullOrWhiteSpace(note) ? null : note.Trim()));
        }

        return days;
    }
}
