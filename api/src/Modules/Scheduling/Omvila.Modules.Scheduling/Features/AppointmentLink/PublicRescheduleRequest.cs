using NodaTime;

namespace Omvila.Modules.Scheduling.Features.AppointmentLink;

/// <summary>Sözleşme publicRescheduleAppointment gövdesi.</summary>
internal sealed record PublicRescheduleRequest(Instant? StartUtc);
