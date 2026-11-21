namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme CustomerRef.</summary>
internal sealed record CustomerRefView(Guid BrandCustomerId, string FullName, string? PhoneMasked);
