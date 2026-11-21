using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Bir günün saat ızgarası (sözleşme SlotSearchResult).</summary>
/// <param name="Date">Yerel gün.</param>
/// <param name="IsClosed">Şube o gün kapalı.</param>
/// <param name="Slots">Aday saatler, saate göre sıralı.</param>
/// <param name="UnavailableTimes">Şube açıkken aday olmayan ızgara saatleri (üstü çizili gösterim).</param>
internal sealed record DaySlots(LocalDate Date, bool IsClosed, IReadOnlyList<SlotCandidate> Slots, IReadOnlyList<LocalTime> UnavailableTimes);
