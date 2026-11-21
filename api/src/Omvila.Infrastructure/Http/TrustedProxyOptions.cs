using System.Net;

namespace Omvila.Infrastructure.Http;

/// <summary>
/// appsettings "ForwardedHeaders" bölümü: X-Forwarded-For ve X-Forwarded-Proto'ya güvenilen ters vekiller (T-73 (7), T-76).
/// İkisi de boşsa (Development, Testing varsayılanı) başlıklar hiç işlenmez. Test sunucusunda Caddy'nin kapsayıcı ağı
/// ortam değişkeniyle verilir: <c>ForwardedHeaders__KnownNetworks__0=172.18.0.0/16</c> veya
/// <c>ForwardedHeaders__KnownProxies__0=172.18.0.2</c>.
/// </summary>
public sealed class TrustedProxyOptions
{
    public const string Section = "ForwardedHeaders";

    /// <summary>Güvenilen vekil IP adresleri (ör. "10.0.0.5").</summary>
    public IList<string> KnownProxies { get; } = [];

    /// <summary>Güvenilen ağlar, CIDR (ör. "172.18.0.0/16").</summary>
    public IList<string> KnownNetworks { get; } = [];

    public bool IsEnabled => KnownProxies.Count > 0 || KnownNetworks.Count > 0;

    /// <summary>Okunamayan girdiler; boşsa yapılandırma geçerlidir.</summary>
    public IReadOnlyList<string> InvalidEntries() =>
    [
        .. KnownProxies.Where(p => !IPAddress.TryParse(p, out _)).Select(p => $"{Section}:{nameof(KnownProxies)} '{p}'"),
        .. KnownNetworks.Where(n => !n.Contains('/', StringComparison.Ordinal) || !IPNetwork.TryParse(n, out _))
            .Select(n => $"{Section}:{nameof(KnownNetworks)} '{n}'"),
    ];
}
