namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Abonelik ve faturalama için marka bilgisi ve marka yaşam döngüsü (ADR-0006 K4: platformun başka modüle yazması o modülün
/// Contracts arayüzüyle). Markanın kiracı bağlamında veya platform bağlamında çağrılır.
/// </summary>
public interface IBrandBilling
{
    /// <summary>Fatura ve e-posta için marka bilgisi ile faturalanan sayılar; marka yoksa null.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<BrandBillingProfile?> GetProfileAsync(Guid brandId, CancellationToken ct);

    /// <summary>Markayı arşive alır (Brand.Status = Archived; veri silinmez). Zaten arşivdeyse veya marka yoksa false.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<bool> ArchiveAsync(Guid brandId, CancellationToken ct);
}
