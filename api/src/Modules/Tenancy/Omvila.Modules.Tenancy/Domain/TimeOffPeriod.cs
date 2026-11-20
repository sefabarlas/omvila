using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// İzin dönemi (alan-modeli.md § TimeOff; sözleşme TimeOffCreate). Tüm gün: başlangıç günü 00:00'dan bitiş gününün
/// ertesi 00:00'ına (bitiş dahil), personelin şubesinin saat diliminde. Saatli: <c>startUtc &lt; endUtc</c>.
/// İzin bütün şubelerde geçerlidir ve randevuları iptal etmez.
/// </summary>
internal sealed record TimeOffPeriod(Instant StartUtc, Instant EndUtc, bool IsAllDay)
{
    public static Result<TimeOffPeriod> AllDay(LocalDate? fromDate, LocalDate? toDate, DateTimeZone zone)
    {
        ArgumentNullException.ThrowIfNull(zone);
        if (fromDate is not { } from)
        {
            return Error.Validation("fromDate", "required");
        }

        var to = toDate ?? from;
        if (to < from)
        {
            return Error.Validation("toDate", "invalid_range");
        }

        return new TimeOffPeriod(
            zone.AtStartOfDay(from).ToInstant(),
            zone.AtStartOfDay(to.PlusDays(1)).ToInstant(),
            IsAllDay: true);
    }

    public static Result<TimeOffPeriod> Timed(Instant? startUtc, Instant? endUtc)
    {
        if (startUtc is not { } start)
        {
            return Error.Validation("startUtc", "required");
        }

        if (endUtc is not { } end)
        {
            return Error.Validation("endUtc", "required");
        }

        return start < end ? new TimeOffPeriod(start, end, IsAllDay: false) : Error.Validation("endUtc", "invalid_range");
    }

    public bool Overlaps(Instant start, Instant end) => StartUtc < end && start < EndUtc;
}
