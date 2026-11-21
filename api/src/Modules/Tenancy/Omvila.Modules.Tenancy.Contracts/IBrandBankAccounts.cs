namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Kapora hesabı okuması (alan-modeli.md § BrandBankAccount; sureler-ve-ayarlar.md § Kapora). Markanın kiracı bağlamında
/// çağrılır. Aktif hesap yoksa hiçbir hizmete kapora konamaz ve kapora politikası None gibi davranır.
/// </summary>
public interface IBrandBankAccounts
{
    /// <summary>Markanın aktif kapora hesabı var mı (BrandSettings.depositAvailable).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<bool> HasActiveAsync(Guid brandId, CancellationToken ct);

    /// <summary>Aktif kapora hesabı (havale talimatı, açıklama kodu öneki, havale süresi); yoksa null.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<ActiveBankAccount?> GetActiveAsync(Guid brandId, CancellationToken ct);
}
