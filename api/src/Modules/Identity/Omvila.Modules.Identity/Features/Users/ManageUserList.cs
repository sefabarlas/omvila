namespace Omvila.Modules.Identity.Features.Users;

/// <summary>Sözleşme ManageUserList.</summary>
internal sealed record ManageUserList(IReadOnlyList<ManageUserResponse> Items);
