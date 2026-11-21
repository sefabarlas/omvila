using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Sözleşme AppointmentItemView.</summary>
internal sealed record AppointmentItemView(
    Guid Id,
    Guid ServiceId,
    string ServiceName,
    Guid? StaffId,
    string? StaffName,
    Instant StartUtc,
    Instant EndUtc,
    Money Price,
    Money StaffSurcharge,
    Money Discount,
    bool PaidFromPackage,
    string Status,
    IReadOnlyList<SegmentView> Segments);
