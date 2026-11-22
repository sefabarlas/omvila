namespace Omvila.Modules.Identity.Features.Users;

/// <summary>Sözleşme ManageUser (Ayarlar → Roller "Kullanıcılar" satırı).</summary>
internal sealed record ManageUserResponse(
    Guid UserId,
    string FullName,
    string PhoneMasked,
    string Role,
    IReadOnlyList<Guid> BranchIds,
    string Status,
    Guid? StaffId,
    bool IsCurrentUser,
    string Version);
