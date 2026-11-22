namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Faturalama için marka bilgisi (AyarlarIsletme alanları) ve etkin sayılar.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="Name">İşletme adı.</param>
/// <param name="LegalName">Ticari unvan (varsa).</param>
/// <param name="TaxOffice">Vergi dairesi (varsa).</param>
/// <param name="TaxNumber">Vergi numarası (varsa).</param>
/// <param name="Email">İşletme e-postası; yoksa OMVILA e-postası gönderilmez.</param>
/// <param name="ActiveBranchCount">Etkin şube sayısı.</param>
/// <param name="ActiveStaffCount">Etkin personel sayısı.</param>
public sealed record BrandBillingProfile(
    Guid BrandId,
    string Name,
    string? LegalName,
    string? TaxOffice,
    string? TaxNumber,
    string? Email,
    int ActiveBranchCount,
    int ActiveStaffCount);
