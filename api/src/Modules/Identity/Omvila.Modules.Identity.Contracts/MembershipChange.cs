namespace Omvila.Modules.Identity.Contracts;

/// <summary>Üyelik değişikliği; boş alan değişmez.</summary>
/// <param name="Role">Yeni rol (BranchManager veya Staff).</param>
/// <param name="BranchIds">Yeni şube kapsamı.</param>
/// <param name="IsActive">false = üyelik devre dışı (personel pasif), true = yeniden etkin.</param>
public sealed record MembershipChange(string? Role, IReadOnlyCollection<Guid>? BranchIds, bool? IsActive);
