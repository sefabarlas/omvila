using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Slots;

/// <summary>Sözleşme SlotSearchResult.</summary>
internal sealed record SlotSearchResult(string TimeZone, LocalDate Date, IReadOnlyList<Slot> Slots, IReadOnlyList<string> UnavailableTimes);
