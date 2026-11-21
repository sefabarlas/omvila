using NodaTime.Text;
using Omvila.Modules.Scheduling.Domain.Availability;

namespace Omvila.Modules.Scheduling.Features;

/// <summary>Sözleşme TimeInterval (yerel saat HH:mm).</summary>
internal sealed record TimeIntervalView(string Start, string End)
{
    private static readonly LocalTimePattern Pattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");

    public static string Format(NodaTime.LocalTime time) => Pattern.Format(time);

    public static TimeIntervalView From(LocalTimeRange range) => new(Format(range.Start), Format(range.End));
}
