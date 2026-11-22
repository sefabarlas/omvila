namespace Omvila.Modules.Identity.Features.Users;

/// <summary>Sözleşme ManageUserUpdate.</summary>
internal sealed record ManageUserUpdate(string? Role, IReadOnlyList<Guid>? BranchIds);
