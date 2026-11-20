namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme StaffCreate.</summary>
internal sealed record StaffCreateRequest(
    string? FullName,
    string? Phone,
    string? Title,
    string? Role,
    string? CalendarColor,
    IReadOnlyList<Guid>? BranchIds,
    IReadOnlyList<StaffServiceInput>? Services,
    bool? CopyBranchWorkingHours,
    decimal? ServiceCommissionPercent,
    decimal? SaleCommissionPercent,
    bool? IsBookableOnline,
    bool? SendInvite);
