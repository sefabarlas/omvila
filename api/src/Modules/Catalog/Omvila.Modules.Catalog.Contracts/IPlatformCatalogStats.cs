namespace Omvila.Modules.Catalog.Contracts;

/// <summary>
/// Platform paneli aktivasyon hunisi ("Hizmet ekledi"). Platform okuma servisidir (ADR-0006): yalnız admin yüzeyinin platform
/// bağlamında çağrılır, başka bağlamda istisna atar.
/// </summary>
public interface IPlatformCatalogStats
{
    /// <summary>Verilen markalardan en az bir etkin hizmeti olanlar.</summary>
    /// <param name="brandIds">Markalar.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlySet<Guid>> GetBrandsWithServicesAsync(IReadOnlyCollection<Guid> brandIds, CancellationToken ct);
}
