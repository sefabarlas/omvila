namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>Sözleşme CustomerRef (kasada tam telefon dönmez, yalnız maskeli).</summary>
internal sealed record CashCustomerRef(Guid BrandCustomerId, string FullName, string? PhoneMasked);
