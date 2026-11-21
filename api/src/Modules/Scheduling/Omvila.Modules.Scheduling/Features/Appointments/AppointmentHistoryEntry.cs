using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme AppointmentHistoryEntry.</summary>
internal sealed record AppointmentHistoryEntry(Instant OccurredUtc, string Kind, string? ActorName, string? Detail);
