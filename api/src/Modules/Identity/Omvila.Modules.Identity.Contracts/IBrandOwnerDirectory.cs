namespace Omvila.Modules.Identity.Contracts;

/// <summary>
/// Markaların sahipleri (platform paneli; Owner üyeliği ve kullanıcısı). Platform okuma servisidir (ADR-0006): yalnız admin
/// yüzeyinin platform bağlamında (TenantScope.Platform) çağrılır, başka bağlamda istisna atar. Telefon yalnız arama ve
/// maskeleme içindir, yanıta açık yazılmaz (roller-yetkiler.md § Platform rolleri).
/// </summary>
public interface IBrandOwnerDirectory
{
    /// <param name="brandIds">Markalar; null = bütün markalar.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, BrandOwnerInfo>> GetOwnersAsync(IReadOnlyCollection<Guid>? brandIds, CancellationToken ct);
}
