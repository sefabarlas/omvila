using NodaTime;

namespace Omvila.Modules.Scheduling.DemoData;

/// <summary>Tek kalemli demo randevusu.</summary>
public sealed record DemoAppointment(
    Guid BrandId,
    Guid BranchId,
    Guid CustomerId,
    Guid BrandCustomerId,
    Instant StartUtc,
    Guid ServiceId,
    Guid StaffId,
    DemoAppointmentOutcome Outcome);
