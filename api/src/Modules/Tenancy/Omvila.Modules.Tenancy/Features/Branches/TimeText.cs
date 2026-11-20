using NodaTime;
using NodaTime.Text;

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>Sözleşme LocalTime ("HH:mm", 00:00–23:59) ve TimeInterval dönüşümü.</summary>
internal static class TimeText
{
    private static readonly LocalTimePattern Pattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");

    public static string Format(LocalTime time) => Pattern.Format(time);

    /// <summary>Aralıkları çözer; biçim hatası "{field}[i].start|end" olarak yazılır.</summary>
    public static List<(LocalTime Start, LocalTime End)>? Parse(IReadOnlyList<TimeIntervalDto>? intervals, string field, Dictionary<string, string[]> errors)
    {
        var result = new List<(LocalTime, LocalTime)>();
        for (var i = 0; i < (intervals?.Count ?? 0); i++)
        {
            var start = One(intervals![i].Start, $"{field}[{i}].start", errors);
            var end = One(intervals[i].End, $"{field}[{i}].end", errors);
            if (start is { } s && end is { } e)
            {
                result.Add((s, e));
            }
        }

        return errors.Count > 0 ? null : result;
    }

    private static LocalTime? One(string? text, string field, Dictionary<string, string[]> errors)
    {
        if (text is { Length: 5 } && Pattern.Parse(text) is { Success: true } parsed)
        {
            return parsed.Value;
        }

        errors[field] = [text is null ? "required" : "invalid_format"];
        return null;
    }
}
