using NodaTime;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>Sözleşme WeeklyHours gövdesinin çözümü (şube ve personel saatleri ortak): istekteki gün sırası ve saatler.</summary>
internal static class WeeklyHoursInput
{
    public static Result<(IReadOnlyList<IsoDayOfWeek> Days, WeeklyHours Hours)> Parse(WeeklyHoursDto body)
    {
        ArgumentNullException.ThrowIfNull(body);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var days = body.Days ?? [];
        if (body.Days is null)
        {
            errors["days"] = ["required"];
        }
        else if (days.Count > 7)
        {
            errors["days"] = ["too_long"];
        }

        var raw = new List<(IsoDayOfWeek, IReadOnlyList<(LocalTime, LocalTime)>)>();
        for (var i = 0; i < days.Count && errors.Count == 0; i++)
        {
            if (days[i].DayOfWeek is < 1 or > 7)
            {
                errors[$"days[{i}].dayOfWeek"] = ["out_of_range"];
                break;
            }

            if (TimeText.Parse(days[i].Intervals, $"days[{i}].intervals", errors) is { } intervals)
            {
                raw.Add(((IsoDayOfWeek)days[i].DayOfWeek, intervals));
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var hours = WeeklyHours.Create(raw);
        return hours.IsSuccess ? (raw.Select(r => r.Item1).ToList(), hours.Value) : hours.Error;
    }
}
