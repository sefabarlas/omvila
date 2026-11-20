namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Kayıtta yeni marka kurar (dilim-01 kabul kriteri 1): marka (Onboarding), varsayılan ayarlar, rezervasyon sayfası
/// ayarları, "Merkez" şubesi ve sahibin personel kaydı. İstek transaction'ı içinde çağrılır; dönüşte oturumun kiracı
/// bağlamı yeni markaya daraltılmıştır, aynı transaction'da o markanın diğer satırları eklenebilir.
/// </summary>
public interface ITenantProvisioning
{
    /// <param name="businessName">İşletme adı (bağlantı adı bundan türetilir; alınmışsa -2, -3…).</param>
    /// <param name="industryCode">Sözleşmedeki sektör kodu.</param>
    /// <param name="ownerUserId">Sahibin kullanıcı kimliği (Identity).</param>
    /// <param name="ownerFullName">Sahibin adı; yoksa personel adı işletme adı olur.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<ProvisionedTenant> ProvisionBrandAsync(
        string businessName, string industryCode, Guid ownerUserId, string? ownerFullName, CancellationToken ct);
}
