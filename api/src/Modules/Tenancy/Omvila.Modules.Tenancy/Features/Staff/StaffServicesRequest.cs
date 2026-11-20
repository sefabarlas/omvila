namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme manageSetStaffServices gövdesi.</summary>
internal sealed record StaffServicesRequest(IReadOnlyList<StaffServiceInput>? Items);
