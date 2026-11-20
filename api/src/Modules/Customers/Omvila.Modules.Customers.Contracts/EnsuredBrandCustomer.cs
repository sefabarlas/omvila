namespace Omvila.Modules.Customers.Contracts;

/// <summary><see cref="ICustomerDirectory.EnsureBrandCustomerAsync"/> sonucu.</summary>
/// <param name="BrandCustomerId">Markadaki müşteri kartı.</param>
/// <param name="CustomerId">Global kişi.</param>
/// <param name="Created">Markada yeni kart açıldı.</param>
/// <param name="IsBlocked">Engellenen müşteri online randevu alamaz (customer_blocked).</param>
/// <param name="NoShowCount">Gelmeme sayısı (kapora "yalnız riskli müşterilerde" kuralı için).</param>
public sealed record EnsuredBrandCustomer(Guid BrandCustomerId, Guid CustomerId, bool Created, bool IsBlocked, int NoShowCount);
