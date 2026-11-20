namespace Omvila.Modules.Tenancy.Domain;

/// <summary>İşletme bilgileri (sözleşme BrandUpdate'in uygulanmış hâli).</summary>
internal sealed record BrandProfile(
    string Name,
    string IndustryCode,
    string? ShortDescription,
    string? Phone,
    string? Email,
    string? Instagram,
    string? Website,
    string? LegalName,
    string? TaxOffice,
    string? TaxNumber);
