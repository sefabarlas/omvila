namespace Omvila.Infrastructure.Http.Cors;

/// <summary>
/// CORS açısından API yüzeyi (docs/mimari/mimari.md § API yüzeyleri). Her yüzeyin kendi köken listesi ve politikası vardır.
/// Çerezle kimlik bilgisi (credentials) yalnız çerez kullanan yüzeylerde açıktır: manage (yenileme çerezi omvila_rt)
/// ve public (güvenilir cihaz çerezi omvila_td).
/// </summary>
public sealed record ApiSurface(string Name, string PathPrefix, bool AllowCredentials)
{
    public static readonly ApiSurface Manage = new("Manage", "/api/v1/manage", AllowCredentials: true);
    public static readonly ApiSurface Admin = new("Admin", "/api/v1/admin", AllowCredentials: false);
    public static readonly ApiSurface Me = new("Me", "/api/v1/me", AllowCredentials: false);
    public static readonly ApiSurface Public = new("Public", "/api/v1/public", AllowCredentials: true);

    public static IReadOnlyList<ApiSurface> All { get; } = [Manage, Admin, Me, Public];

    /// <summary>Uçlara bağlanan adlandırılmış CORS politikası.</summary>
    public string PolicyName => "cors:" + Name.ToLowerInvariant();

    /// <summary>Grup ön ekinin yüzeyi (ör. "/api/v1/manage/customers" → Manage); yüzey dışı ön ek için null.</summary>
    public static ApiSurface? FromPrefix(string prefix) =>
        All.FirstOrDefault(s => prefix.Equals(s.PathPrefix, StringComparison.OrdinalIgnoreCase)
            || prefix.StartsWith(s.PathPrefix + "/", StringComparison.OrdinalIgnoreCase));
}
