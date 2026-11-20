namespace Omvila.Modules.Identity.Features.ManageSession;

internal sealed record SessionMembership(string Role, IReadOnlyList<Guid> BranchIds, Guid? StaffId);
