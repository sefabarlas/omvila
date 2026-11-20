using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Üretilecek tekrarlar ve çakıştığı için atlananlar (sözleşme AppointmentCreated.skippedOccurrences).</summary>
internal sealed record RecurrencePlan(IReadOnlyList<Occurrence> Planned, IReadOnlyList<LocalDate> Skipped, LocalDate GeneratedUntil);
