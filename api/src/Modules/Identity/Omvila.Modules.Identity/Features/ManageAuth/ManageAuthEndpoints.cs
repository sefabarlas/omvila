using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Security;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Features.ManageAuth;

/// <summary>Salon girişi ve kayıt uçları (sözleşme manage-auth; ADR-0007).</summary>
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

        // Kayıt jetonuyla çalışır (oturum henüz yok). Yanıt jeton taşıdığı için idempotency kaydı tutulmaz (jetonlar
        // veritabanına açık yazılmaz, ADR-0007); tekrar gönderim var olan işletmeye giriş olur (ManageSignupService).
        auth.MapPost("/signup", SignupAsync).WithName("manageSignup").AllowAnonymous();

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

    private static async Task<IResult> SignupAsync(SignupRequest body, HttpContext http, ManageSignupService signup, CancellationToken ct)
    {
        var client = ClientTypeHeader.Read(http.Request);
        if (!client.IsSuccess)
        {
            return client.Error.ToProblem();
        }

        // Sözleşmede zorunlu başlık; biçimi denetlenir, yanıt saklanmaz (yukarıdaki not).
        if (!Guid.TryParse(http.Request.Headers["Idempotency-Key"].ToString(), out _))
        {
            return Error.Validation("Idempotency-Key", "required").ToProblem();
        }

        var errors = ValidateSignup(body, http.RequestServices.GetRequiredService<IOptions<LegalOptions>>().Value);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var command = new SignupCommand(
            body.SignupToken!,
            body.BusinessName!.Trim(),
            body.IndustryCode!,
            string.IsNullOrWhiteSpace(body.OwnerFullName) ? null : body.OwnerFullName.Trim(),
            http.Connection.RemoteIpAddress?.ToString());
        var session = await signup.SignupAsync(command, client.Value, ct);
        return session.ToHttp(value => TypedResults.Json(
            new ManageAuthResult("SignedIn", value.AccessToken, value.ExpiresInSeconds, DeliverRefreshToken(http.Response, client.Value, value.RefreshToken)),
            statusCode: StatusCodes.Status201Created));
    }

    private static Dictionary<string, string[]> ValidateSignup(SignupRequest body, LegalOptions legal)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(body.SignupToken))
        {
            errors["signupToken"] = ["required"];
        }

        var name = body.BusinessName?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            errors["businessName"] = ["required"];
        }
        else if (name.Length < 2)
        {
            errors["businessName"] = ["too_short"];
        }
        else if (name.Length > 160)
        {
            errors["businessName"] = ["too_long"];
        }

        if (body.IndustryCode is null)
        {
            errors["industryCode"] = ["required"];
        }
        else if (!IndustryCodes.Contains(body.IndustryCode))
        {
            errors["industryCode"] = ["invalid_format"];
        }

        if (body.OwnerFullName?.Trim().Length > 120)
        {
            errors["ownerFullName"] = ["too_long"];
        }

        if (!body.AcceptedTerms)
        {
            errors["acceptedTerms"] = ["required"];
        }

        // İstemci yürürlükteki aydınlatma metnini göstermiş olmalı; eski sürümle onay kabul edilmez.
        if (string.IsNullOrWhiteSpace(body.PrivacyNoticeVersion))
        {
            errors["privacyNoticeVersion"] = ["required"];
        }
        else if (!string.Equals(body.PrivacyNoticeVersion, legal.PrivacyNoticeVersion, StringComparison.Ordinal))
        {
            errors["privacyNoticeVersion"] = ["out_of_range"];
        }

        return errors;
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

    // Sözleşme IndustryCode.
    private static readonly HashSet<string> IndustryCodes =
        new(["barber", "hair_salon", "beauty", "spa", "nail", "dietitian", "other"], StringComparer.Ordinal);

    [GeneratedRegex(@"^\d{6}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex SixDigits();
}
