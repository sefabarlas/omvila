namespace Omvila.Modules.Sales.Contracts;

/// <summary>
/// Müşteri uygulaması "Salonlarım" rozeti ("Fön paketiniz var"; sözleşme MeBrandPackageBadge). Müşteri bağlamından çağrılır;
/// okuma platform bağlamında yapılır, oturumun önceki bağlamı geri yüklenir. Yalnız verilen kartların paketleri okunur.
/// </summary>
public interface ICustomerPackageBadges
{
    /// <summary>Kart başına Active ve kalan seansı olan paketler (son kullanım gününe göre artan); paketi olmayan kart yer almaz.</summary>
    /// <param name="brandCustomerIds">Müşterinin markalardaki kartları.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, IReadOnlyList<PackageBadge>>> ListActiveAsync(IReadOnlyCollection<Guid> brandCustomerIds, CancellationToken ct);
}
