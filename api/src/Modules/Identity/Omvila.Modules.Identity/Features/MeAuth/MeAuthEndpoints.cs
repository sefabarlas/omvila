using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Features.ManageAuth;
using Omvila.Modules.Identity.Security;
using Omvila.Modules.Notifications.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Features.MeAuth;

/// <summary>
/// Müşteri uygulaması girişi (sözleşme me-auth; ADR-0007 me yüzeyi; T-103). Kod amacı CustomerLogin: salon veya rezervasyon kodu
/// burada geçmez. Doğrulama global Customer kaydını açar veya bulur (BrandCustomer açılmaz), aud=me erişim jetonu ve gövdede
/// yenileme jetonu (yüzeyi me, istemci Mobile) verir. Yenileme jetonu her kullanımda döner; yeniden kullanımda zincir iptal.
/// </summary>
internal static partial class MeAuthEndpoints
{
    public const string OtpRateLimitPolicy = "me-otp-ip";

    public static void Map(IEndpointRouteBuilder app)
    {
        var auth = app.MapApiGroup("/api/v1/me/auth");

        // Giriş öncesi: anonim (oturum henüz yok). Numaranın kayıtlı olup olmadığı açık edilmez (her durumda 202). IP başına sınırlı.
        auth.MapPost("/otp/request", RequestOtpAsync).WithName("meRequestOtp").AllowAnonymous().RequireRateLimiting(OtpRateLimitPolicy);

        // Giriş öncesi: anonim (kod doğrulaması oturumu açar).
        auth.MapPost("/otp/verify", VerifyOtpAsync).WithName("meVerifyOtp").AllowAnonymous();

        // Yenileme jetonuyla çalışır (erişim jetonunun süresi dolmuştur); jeton gövdede, çerez yok (CSRF yüzeyi yok).
        auth.MapPost("/refresh", RefreshAsync).WithName("meRefreshToken").AllowAnonymous();

        auth.MapPost("/logout", LogoutAsync).WithName("meLogout").RequireAuthenticated(Audiences.Me);
    }

    private static async Task<IResult> RequestOtpAsync(MeOtpRequest body, OtpService otp, CancellationToken ct)
    {
        // Giriş SMS koduyla yapılır: yalnız cep telefonu.
        if (string.IsNullOrWhiteSpace(body.Phone))
        {
            return Error.Validation("phone", "required").ToProblem();
        }

        if (!PhoneNumber.TryParse(body.Phone, out var phone) || !phone.IsMobile)
        {
            return Error.Validation("phone", "invalid_format").ToProblem();
        }

        var issued = await otp.RequestAsync(phone, OtpPurpose.CustomerLogin, ct);
        return issued.ToHttp(i => TypedResults.Json(
            new OtpChallengeResponse(i.OtpId, i.ExpiresInSeconds, i.ResendAfterSeconds, i.PhoneMasked), statusCode: StatusCodes.Status202Accepted));
    }

    private static async Task<IResult> VerifyOtpAsync(
        OtpVerifyRequest body,
        OtpService otp,
        ICustomerAccounts customers,
        RefreshTokenService refreshTokens,
        AccessTokenIssuer accessTokens,
        CancellationToken ct)
    {
        if (body.Code is null || !SixDigits().IsMatch(body.Code))
        {
            return Error.Validation("code", "invalid_format").ToProblem();
        }

        // rememberDevice bu uçta yok sayılır (sözleşme meVerifyOtp): mobil oturum her zaman uzun ömürlüdür.
        var verified = await otp.VerifyAsync(body.OtpId, body.Code, OtpPurpose.CustomerLogin, ct);
        if (!verified.IsSuccess)
        {
            return verified.Error.ToProblem();
        }

        var account = await customers.SignInAsync(PhoneNumber.Parse(verified.Value.PhoneE164), ct);
        var refresh = await refreshTokens.IssueAsync(
            account.CustomerId, membershipId: null, RefreshClientType.Mobile, rememberDevice: false, ct, RefreshToken.MeAudience);
        var (token, expiresIn) = accessTokens.IssueMe(account.CustomerId);
        return TypedResults.Ok(new MeAuthResult(token, expiresIn, refresh.Token, account.IsNewAccount));
    }

    private static async Task<IResult> RefreshAsync(
        MeRefreshRequest? body, ICustomerAccounts customers, RefreshTokenService refreshTokens, AccessTokenIssuer accessTokens, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body?.RefreshToken))
        {
            return Error.Unauthorized().ToProblem();
        }

        var rotated = await refreshTokens.RotateAsync(body.RefreshToken, ct, RefreshToken.MeAudience);
        if (!rotated.IsSuccess)
        {
            return rotated.Error.ToProblem();
        }

        // Hesabı silinmiş (anonimleştirilmiş) kişinin oturumu yenilenmez.
        if (!await customers.CanSignInAsync(rotated.Value.UserId, ct))
        {
            return Error.Unauthorized().ToProblem();
        }

        var (token, expiresIn) = accessTokens.IssueMe(rotated.Value.UserId);
        return TypedResults.Ok(new TokenPair(token, expiresIn, rotated.Value.Token));
    }

    private static async Task<IResult> LogoutAsync(
        MeLogoutRequest? body, HttpContext http, RefreshTokenService refreshTokens, ICustomerDevices devices, CancellationToken ct)
    {
        if (!Guid.TryParse(http.User.FindFirst(OmvilaClaims.Subject)?.Value, out var customerId))
        {
            return Error.Unauthorized().ToProblem();
        }

        if (!string.IsNullOrWhiteSpace(body?.RefreshToken))
        {
            await refreshTokens.RevokeCustomerFamilyAsync(body.RefreshToken, customerId, ct);
        }
        else
        {
            // Jeton gönderilmediyse hangi zincir olduğu bilinemez: müşterinin bütün uygulama oturumları kapanır.
            await refreshTokens.RevokeCustomerSessionsAsync(customerId, ct);
        }

        if (!string.IsNullOrWhiteSpace(body?.PushToken))
        {
            await devices.DeactivateAsync(customerId, body.PushToken, ct);
        }

        return TypedResults.NoContent();
    }

    [GeneratedRegex(@"^\d{6}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex SixDigits();
}
