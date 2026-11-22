namespace Omvila.Modules.Customers.Contracts;

/// <summary>Kişinin bir markadaki kartı.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="BrandCustomerId">Kart.</param>
public sealed record CustomerBrandCardRef(Guid BrandId, Guid BrandCustomerId);
