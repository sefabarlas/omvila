using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Security;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Features.ManageAuth;

/// <summary>Salon girişi uçları (sözleşme manage-auth; ADR-0007). Kayıt ucu (manageSignup) Tenancy ile birlikte gelir.</summary>
internal static partial class ManageAuthEndpoints
{
    public const string OtpRateLimitPolicy = "manage-otp-ip";

    public static void Map(IEndpointRouteBuilder app)
    {
        var auth = app.MapApiGroup("/api/v1/manage/auth");

        // Giriş öncesi: anonim (oturum henüz yok). Numaranın kayıtlı olup olmadığı açık edilmez.
        auth.MapPost("/otp/request", RequestOtpAsync).WithName("manageRequestOtp").AllowAnonymous().RequireRateLimiting(OtpRateLimitPolicy);

        // Giriş öncesi: anonim (kod doğrulaması oturumu açar).
        auth.MapPost("/otp/verify", VerifyOtpAsync).WithName("manageVerifyOtp").AllowAnonymous();

        // Yenileme jetonuyla çalışır (erişim jetonunun süresi dolmuştur); çerezde CSRF için X-Requested-With.
        auth.MapPost("/refresh", RefreshAsync).WithName("manageRefreshToken").AllowAnonymous().AddEndpointFilter<XRequestedWithFilter>();

        auth.MapPost("/logout", LogoutAsync).WithName("manageLogout").RequireAuthenticated(Audiences.Manage);
    }

    private static async Task<IResult> RequestOtpAsync(ManageOtpRequest body, HttpRequest request, OtpService otp, CancellationToken ct)
    {
        var client = ClientTypeHeader.Read(request);
        if (!client.IsSuccess)
        {
            return client.Error.ToProblem();
        }

        // Giriş SMS koduyla yapılır: yalnız cep telefonu.
        if (!PhoneNumber.TryParse(body.Phone, out var phone) || !phone.IsMobile)
        {
            return Error.Validation("phone", "invalid_format").ToProblem();
        }

        var purpose = body.Purpose switch
        {
            "Login" => OtpPurpose.ManageLogin,
            "Signup" => OtpPurpose.ManageSignup,
            _ => (OtpPurpose?)null,
        };
        if (purpose is null)
        {
            return Error.Validation("purpose", "invalid_format").ToProblem();
        }

        var issued = await otp.RequestAsync(phone, purpose.Value, ct);
        return issued.ToHttp(i => TypedResults.Json(
            new OtpChallengeResponse(i.OtpId, i.ExpiresInSeconds, i.ResendAfterSeconds, i.PhoneMasked), statusCode: StatusCodes.Status202Accepted));
    }

    private static async Task<IResult> VerifyOtpAsync(
        OtpVerifyRequest body, HttpContext http, OtpService otp, ManageSignInService signIn, CancellationToken ct)
    {
        var client = ClientTypeHeader.Read(http.Request);
        if (!client.IsSuccess)
        {
            return client.Error.ToProblem();
        }

        if (body.Code is null || !SixDigits().IsMatch(body.Code))
        {
            return Error.Validation("code", "invalid_format").ToProblem();
        }

        var verified = await otp.VerifyAsync(body.OtpId, body.Code, [OtpPurpose.ManageLogin, OtpPurpose.ManageSignup], ct);
        if (!verified.IsSuccess)
        {
            return verified.Error.ToProblem();
        }

        var outcome = await signIn.SignInAsync(verified.Value.PhoneE164, client.Value, body.RememberDevice, ct);
        if (outcome.Session is not { } session)
        {
            return TypedResults.Ok(new ManageAuthResult("SignupRequired", SignupToken: outcome.SignupToken));
        }

        return TypedResults.Ok(new ManageAuthResult(
            "SignedIn", session.AccessToken, session.ExpiresInSeconds, DeliverRefreshToken(http.Response, client.Value, session.RefreshToken)));
    }

    private static async Task<IResult> RefreshAsync(
        RefreshRequest? body, HttpContext http, ManageSignInService signIn, CancellationToken ct)
    {
        var client = ClientTypeHeader.Read(http.Request);
        if (!client.IsSuccess)
        {
            return client.Error.ToProblem();
        }

        var token = client.Value == RefreshClientType.Web ? RefreshTokenCookie.Read(http.Request) : body?.RefreshToken;
        if (string.IsNullOrWhiteSpace(token))
        {
            return Error.Unauthorized().ToProblem();
        }

        var session = await signIn.RefreshAsync(token, ct);
        if (!session.IsSuccess)
        {
            if (client.Value == RefreshClientType.Web)
            {
                RefreshTokenCookie.Delete(http.Response);
            }

            return session.Error.ToProblem();
        }

        var value = session.Value;
        return TypedResults.Ok(new TokenPair(value.AccessToken, value.ExpiresInSeconds, DeliverRefreshToken(http.Response, client.Value, value.RefreshToken)));
    }

    private static async Task<IResult> LogoutAsync(HttpContext http, RefreshTokenService refreshTokens, CancellationToken ct)
    {
        if (RefreshTokenCookie.Read(http.Request) is { } token)
        {
            await refreshTokens.RevokeFamilyAsync(token, ct);
        }
        else if (Guid.TryParse(http.User.FindFirst(OmvilaClaims.MembershipId)?.Value, out var membershipId))
        {
            // Mobil istemci çıkışta jeton göndermez (sözleşmede gövde yok): üyeliğin mobil oturumları kapanır.
            await refreshTokens.RevokeMembershipAsync(membershipId, RefreshClientType.Mobile, ct);
        }

        RefreshTokenCookie.Delete(http.Response);
        return TypedResults.NoContent();
    }

    // Web: yenileme jetonu HttpOnly çerezde, gövdede yok. Mobil: gövdede.
    private static string? DeliverRefreshToken(HttpResponse response, RefreshClientType client, IssuedRefreshToken token)
    {
        if (client == RefreshClientType.Mobile)
        {
            return token.Token;
        }

        RefreshTokenCookie.Append(response, token);
        return null;
    }

    [GeneratedRegex(@"^\d{6}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex SixDigits();
}
