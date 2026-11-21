using Microsoft.Extensions.Hosting;

namespace Omvila.Infrastructure.Hosting;

/// <summary>
/// Ortama bağlı izinlerin tek yeri (T-73). Ortamlar: Development (yerel), Testing (otomatik testler),
/// Staging (test sunucusu, gerçek kişi verisi yok), Production. Ortam adı koşulu başka yerde yazılmaz.
/// </summary>
public static class HostEnvironmentExtensions
{
    /// <summary>Otomatik testlerin ortam adı (WebApplicationFactory, TestApiHost).</summary>
    public const string Testing = "Testing";

    /// <summary>Yerel geliştirme veya otomatik test: geçici anahtar ve http köken serbest.</summary>
    public static bool IsDevelopmentOrTesting(this IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);
        return environment.IsDevelopment() || environment.IsEnvironment(Testing);
    }

    /// <summary>
    /// Sahte SMS ve push sağlayıcısına izin: Development ve Testing'de her zaman; Staging'de yalnız
    /// <c>Staging:AllowFakeProviders=true</c> iken; Production'da ve diğer ortamlarda hiçbir ayarla.
    /// </summary>
    public static bool AllowsFakeProviders(this IHostEnvironment environment, StagingOptions staging)
    {
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(staging);
        return environment.IsDevelopmentOrTesting() || (environment.IsStaging() && staging.AllowFakeProviders);
    }

    /// <summary>
    /// SMS metni (giriş kodu dahil) günlüğe yazılabilir mi: yalnız Development (kodlama-dotnet.md § Günlük). Test sunucusunda
    /// metin günlüğe değil sahte SMS günlüğü sayfasına (/dev/sms, T-76) bakılarak okunur.
    /// </summary>
    public static bool AllowsSmsContentInLogs(this IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);
        return environment.IsDevelopment();
    }
}
