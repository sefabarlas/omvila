using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme AppointmentCreated.</summary>
internal sealed record AppointmentCreated(AppointmentDetail Appointment, IReadOnlyList<LocalDate> SkippedOccurrences);
