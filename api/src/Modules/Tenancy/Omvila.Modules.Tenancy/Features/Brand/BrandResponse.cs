using System.Globalization;

namespace Omvila.Modules.Tenancy.Features.Brand;

/// <summary>Sözleşme Brand.</summary>
internal sealed record BrandResponse(
    Guid Id,
    string Name,
    string Slug,
    string IndustryCode,
    string Status,
    string? ShortDescription,
    string? LogoUrl,
    string? Phone,
    string? Email,
    string? Instagram,
    string? Website,
    string? LegalName,
    string? TaxOffice,
    string? TaxNumber,
    string Version)
{
    public static BrandResponse From(Domain.Brand b) => new(
        b.Id, b.Name, b.Slug, b.IndustryCode, b.Status.ToString(), b.ShortDescription, b.LogoUrl, b.Phone, b.Email, b.Instagram,
        b.Website, b.LegalName, b.TaxOffice, b.TaxNumber, b.Version.ToString(CultureInfo.InvariantCulture));
}
