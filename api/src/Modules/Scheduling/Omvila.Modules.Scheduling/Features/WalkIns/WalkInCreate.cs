namespace Omvila.Modules.Scheduling.Features.WalkIns;

/// <summary>Sözleşme WalkInCreate. Biçim denetimi uçta; alanlar eksik gelebilir.</summary>
internal sealed record WalkInCreate(
    Guid? BranchId,
    Guid? BrandCustomerId,
    string? FullName,
    string? Phone,
    Guid? ServiceId,
    Guid? PreferredStaffId);
