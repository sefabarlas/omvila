using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme RecurrenceRequest.</summary>
internal sealed record RecurrenceRequest(string? Frequency, int? IntervalWeeks, LocalDate? EndsOn);
