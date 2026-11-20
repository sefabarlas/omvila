using System.Globalization;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Haftalık tekrar kuralı (alan-modeli.md § AppointmentSeries: RFC 5545 RRULE; v1 arayüzü yalnız
/// <c>FREQ=WEEKLY;INTERVAL=1|2|4</c> üretir; sözleşme RecurrenceRequest.intervalWeeks).
/// </summary>
internal readonly record struct WeeklyRecurrence
{
    private const string Prefix = "FREQ=WEEKLY;INTERVAL=";

    private WeeklyRecurrence(int intervalWeeks) => IntervalWeeks = intervalWeeks;

    public int IntervalWeeks { get; }

    public static Result<WeeklyRecurrence> Create(int intervalWeeks) => intervalWeeks is 1 or 2 or 4
        ? new WeeklyRecurrence(intervalWeeks)
        : Error.Validation("recurrence.intervalWeeks", "invalid_format");

    /// <summary>Saklanan RRULE metninden; bozuk metin programcı hatasıdır.</summary>
    public static WeeklyRecurrence Parse(string rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        if (rule.StartsWith(Prefix, StringComparison.Ordinal)
            && int.TryParse(rule.AsSpan(Prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out var weeks)
            && Create(weeks) is { IsSuccess: true } parsed)
        {
            return parsed.Value;
        }

        throw new DomainException("Geçersiz tekrar kuralı.");
    }

    public string ToRRule() => Prefix + IntervalWeeks.ToString(CultureInfo.InvariantCulture);

    public override string ToString() => ToRRule();
}
