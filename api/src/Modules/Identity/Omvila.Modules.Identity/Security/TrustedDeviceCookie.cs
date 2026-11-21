using Microsoft.AspNetCore.Http;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Rezervasyon sayfasının güvenilir cihaz çerezi (sözleşme publicRequestOtp/publicVerifyOtp: omvila_td, 30 gün).
/// HttpOnly; Secure; SameSite=Strict (rezervasyon sayfası ve API aynı kayıtlı alan altında); yalnız OTP uçlarına gider.
/// </summary>
internal static class TrustedDeviceCookie
{
    public const string Name = "omvila_td";
    public const string Path = "/api/v1/public/otp";

    public static void Append(HttpResponse response, IssuedTrustedDevice device) =>
        response.Cookies.Append(Name, device.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = Path,
            Expires = device.ExpiresUtc.ToDateTimeOffset(),
            IsEssential = true,
        });

    public static string? Read(HttpRequest request) =>
        request.Cookies.TryGetValue(Name, out var value) && !string.IsNullOrWhiteSpace(value) ? value : null;
}
