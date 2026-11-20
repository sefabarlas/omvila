namespace Omvila.Modules.Identity.Security;

/// <summary>manage jetonunun iddiaları (ADR-0007). İzinler jetona yazılmaz.</summary>
internal sealed record ManageTokenClaims(
    Guid UserId,
    Guid BrandId,
    Guid MembershipId,
    string Role,
    IReadOnlyCollection<Guid>? BranchIds,
    Guid? StaffId);
