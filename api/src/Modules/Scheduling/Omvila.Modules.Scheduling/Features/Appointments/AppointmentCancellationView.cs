using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>AppointmentDetail.cancellation.</summary>
internal sealed record AppointmentCancellationView(Instant CancelledUtc, string CancelledBy, string Reason, string? Note);
