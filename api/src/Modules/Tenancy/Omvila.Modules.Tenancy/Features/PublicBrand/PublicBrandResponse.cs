namespace Omvila.Modules.Tenancy.Features.PublicBrand;

/// <summary>Sözleşme PublicBrand. <see cref="Rating"/> değerlendirmeler (Reviews) gelene kadar yazılmaz.</summary>
internal sealed record PublicBrandResponse(
    Guid Id,
    string Name,
    string Slug,
    string? ShortDescription,
    string? LogoUrl,
    string? IndustryLabel,
    string CoverColor,
    bool ShowPrices,
    bool BookingEnabled,
    PublicRating? Rating,
    IReadOnlyList<PublicBranchResponse> Branches);
