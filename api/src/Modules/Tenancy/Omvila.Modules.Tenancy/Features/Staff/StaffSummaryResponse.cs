namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme StaffSummary.</summary>
internal sealed record StaffSummaryResponse(
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
    string? MembershipStatus);
