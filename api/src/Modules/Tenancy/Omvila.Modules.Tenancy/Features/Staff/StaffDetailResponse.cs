
namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme StaffDetail (StaffSummary + ayrıntılar).</summary>
internal sealed record StaffDetailResponse(
    Guid Id,
    string FullName,
    string? DisplayName,
    string? Title,
    string? PhotoUrl,
    string CalendarColor,
    bool IsBookableOnline,
    bool IsActive,
    IReadOnlyList<Guid> BranchIds,
    string? Role,
    string? MembershipStatus,
    string? PhoneMasked,
    string? Bio,
    decimal ServiceCommissionPercent,
    decimal SaleCommissionPercent,
    IReadOnlyList<StaffServiceDto> Services,
    IReadOnlyList<StaffBranchHoursDto> WorkingHours,
    string Version);
