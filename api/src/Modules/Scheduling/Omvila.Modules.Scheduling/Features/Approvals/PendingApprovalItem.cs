using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>Sözleşme PendingApprovalItem. <c>reference</c> Dilim 4'e kadar yazılmaz (T-50 (8)).</summary>
internal sealed record PendingApprovalItem(
    Guid AppointmentId,
    Guid BranchId,
    string BranchName,
    string TimeZone,
    string CustomerName,
    bool IsNewCustomer,
    string ServiceSummary,
    string? StaffName,
    Instant StartUtc,
    Instant EndUtc,
    Instant HoldExpiresUtc,
    Instant CreatedUtc,
    string Channel);
