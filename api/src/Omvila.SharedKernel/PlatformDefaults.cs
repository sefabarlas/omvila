namespace Omvila.SharedKernel;

/// <summary>
/// Platform düzeyindeki adlandırılmış varsayılanlar. Her biri docs/alan/sureler-ve-ayarlar.md'deki satıra
/// karşılık gelir; değer değişirse önce o tablo, sonra bu sınıf değişir (AGENTS.md § 5: sabit değer yok).
/// Ortama göre değişen teknik ayarlar (süreler, anahtarlar) appsettings'tedir.
/// </summary>
public static class PlatformDefaults
{
    /// <summary>sureler-ve-ayarlar.md § Abonelik ve platform: SlugRedirectDays.</summary>
    public const int SlugRedirectDays = 30;

    /// <summary>Şube saat dilimi varsayılanı (AGENTS.md § 5, ADR-0003, dilim-01 § Alan kuralları).</summary>
    public const string DefaultTimeZoneId = "Europe/Istanbul";

    /// <summary>Kayıtta oluşan ilk şubenin adı (dilim-01 § Alan kuralları: ad "Merkez").</summary>
    public const string FirstBranchName = "Merkez";

    /// <summary>Para birimi varsayılanı (alan-modeli.md § Brand.DefaultCurrencyCode).</summary>
    public const Currency DefaultCurrency = Currency.TRY;
}
