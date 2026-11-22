namespace Omvila.Modules.Customers.Contracts;

/// <summary>
/// Platform paneli marka detayı "Müşteri kaydı" sayısı. Platform okuma servisidir (ADR-0006): yalnız admin yüzeyinin platform
/// bağlamında çağrılır, başka bağlamda istisna atar. Müşteri verisi dönmez, yalnız sayı.
/// </summary>
public interface IPlatformCustomerStats
{
    /// <summary>Markanın etkin müşteri kartı sayısı (birleştirilmiş ve anonimleştirilmiş kartlar hariç).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<int> CountBrandCustomersAsync(Guid brandId, CancellationToken ct);
}
