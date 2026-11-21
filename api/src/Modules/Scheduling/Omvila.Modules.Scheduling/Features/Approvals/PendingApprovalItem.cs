using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>Sözleşme PendingApprovalItem. <c>reference</c> yazılmaz (T-50 (8)); kapora alanları yalnız kapora istenmiş talepte.</summary>
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
    string Channel,
    string? DepositStatus,
    Money? DepositAmount);
