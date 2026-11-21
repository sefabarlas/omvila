using NodaTime;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>Sözleşme DaySearchResult. <see cref="NextAvailableUtc"/> yalnız aralıkta hiç boş gün yoksa doludur.</summary>
internal sealed record DaySearchResult(string TimeZone, LocalDate MaxDate, IReadOnlyList<DaySearchResult.Day> Days, Instant? NextAvailableUtc)
{
    /// <summary>DaySearchResult.days[] (gün şeridi: nokta = boş saat var).</summary>
    internal sealed record Day(LocalDate Date, bool HasAvailability, bool IsClosed, Instant? FirstSlotUtc);
}
