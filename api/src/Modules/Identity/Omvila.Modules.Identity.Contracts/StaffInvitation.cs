namespace Omvila.Modules.Identity.Contracts;

/// <summary>Personel daveti.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="BrandName">SMS'teki salon adı.</param>
/// <param name="StaffId">Tenancy personel kaydı.</param>
/// <param name="PhoneE164">Cep telefonu (E.164).</param>
/// <param name="FullName">Personelin adı.</param>
/// <param name="Role">BranchManager veya Staff.</param>
/// <param name="BranchIds">Şube kapsamı.</param>
/// <param name="SendInvite">"Davet SMS'i gönder".</param>
public sealed record StaffInvitation(
    Guid BrandId, string BrandName, Guid StaffId, string PhoneE164, string FullName, string Role, IReadOnlyCollection<Guid> BranchIds, bool SendInvite);
