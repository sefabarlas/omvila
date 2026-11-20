using Microsoft.AspNetCore.Http;
using NodaTime;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Web panelin yenileme çerezi (ADR-0007): HttpOnly; Secure; SameSite=Strict; Path=/api/v1/manage/auth.
/// "Bu bilgisayarda 30 gün hatırla" işaretliyse kalıcı, değilse oturum çerezi (tarayıcı kapanınca düşer).
/// </summary>
internal static class RefreshTokenCookie
{
    public const string Name = "omvila_rt";
    public const string Path = "/api/v1/manage/auth";

    public static void Append(HttpResponse response, IssuedRefreshToken token) =>
        response.Cookies.Append(Name, token.Token, Options(token.RememberDevice ? token.ExpiresUtc.ToDateTimeOffset() : null));

    public static string? Read(HttpRequest request) =>
        request.Cookies.TryGetValue(Name, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null;

    public static void Delete(HttpResponse response) => response.Cookies.Delete(Name, Options(expires: null));

    private static CookieOptions Options(DateTimeOffset? expires) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = Path,
        Expires = expires,
        IsEssential = true,
    };
}
