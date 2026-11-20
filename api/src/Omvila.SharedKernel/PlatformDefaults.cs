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
}
