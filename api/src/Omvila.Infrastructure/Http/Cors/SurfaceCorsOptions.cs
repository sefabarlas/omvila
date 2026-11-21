namespace Omvila.Infrastructure.Http.Cors;

/// <summary>
/// appsettings "Cors" bölümü: yüzey başına izinli kökenler (guvenlik-kvkk.md § CORS; joker yok).
/// Örnek: <c>Cors:Surfaces:Manage:Origins:0 = https://panel.omvila.com</c>. Production'da ortamdan verilir.
/// </summary>
public sealed class SurfaceCorsOptions
{
    public const string Section = "Cors";

    /// <summary>Yüzey adı (<see cref="ApiSurface.Name"/>) → kökenler.</summary>
    public IDictionary<string, SurfaceCorsOrigins> Surfaces { get; init; } =
        new Dictionary<string, SurfaceCorsOrigins>(StringComparer.OrdinalIgnoreCase);

    /// <summary>Ön uçuş (preflight) yanıtının tarayıcıda önbellekte kalma süresi.</summary>
    public int PreflightMaxAgeSeconds { get; set; }

    public IReadOnlyList<string> OriginsFor(ApiSurface surface) =>
        Surfaces.FirstOrDefault(s => s.Key.Equals(surface.Name, StringComparison.OrdinalIgnoreCase)).Value?.Origins is { } origins
            ? [.. origins]
            : [];
}
