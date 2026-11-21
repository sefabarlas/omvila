using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Gün şeridindeki bir gün (sözleşme DaySearchResult.days): yalnız ilk aday aranır.</summary>
internal sealed record DayAvailability(LocalDate Date, bool IsClosed, SlotCandidate? FirstSlot)
{
    public bool HasAvailability => FirstSlot is not null;
}
