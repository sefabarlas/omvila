using NodaTime;

namespace Omvila.Modules.Scheduling.Features.WalkIns;

/// <summary>Sözleşme WalkInTicketView.</summary>
internal sealed record WalkInTicketView(
    Guid Id,
    Guid BranchId,
    LocalDate QueueDate,
    int QueueNumber,
    string Status,
    string CustomerName,
    Guid? BrandCustomerId,
    string? PhoneMasked,
    Guid? ServiceId,
    string? ServiceName,
    Guid? PreferredStaffId,
    string? PreferredStaffName,
    Instant JoinedUtc,
    int? EstimatedWaitMinutes,
    Guid? AppointmentId,
    string Version);
