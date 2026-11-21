using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Features.ManageAuth;
using Omvila.Modules.Identity.Security;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Features.PublicBookingAuth;

/// <summary>
/// Rezervasyon sayfasında telefon doğrulama (sözleşme publicRequestOtp, publicVerifyOtp; ADR-0007; dilim-02 kabul kriteri
/// 9). Kod amacı PublicBooking: giriş kodu burada, rezervasyon kodu panelde geçmez. Doğrulama 30 dk'lık public-booking
/// jetonu verir. "Bu cihazı hatırla" (rememberDevice) seçilirse omvila_td çerezi yazılır; aynı tarayıcıdan aynı telefonla
/// gelen kod isteği SMS göndermeden AlreadyVerified + jeton döner. Salonda kartı olan müşterinin adı (kartın adı) ilk
/// adıyla döner; başka markadaki kayıt açık edilmez.
/// </summary>
internal static partial class PublicOtpEndpoints
{
    public const string RateLimitPolicy = "public-otp-ip";

    public static void Map(IEndpointRouteBuilder app)
    {
        var otp = app.MapApiGroup("/api/v1/public/otp");

        // Rezervasyon sayfası üyeliksizdir (ürün kuralı 7): telefon doğrulaması oturum açılmadan başlar. IP başına sınırlı.
        otp.MapPost("/request", RequestAsync).WithName("publicRequestOtp").AllowAnonymous().RequireRateLimiting(RateLimitPolicy);

        // Üyeliksiz: kod doğrulaması rezervasyon jetonunu verir.
        otp.MapPost("/verify", VerifyAsync).WithName("publicVerifyOtp").AllowAnonymous();
    }

    private static async Task<IResult> RequestAsync(
        PublicOtpRequest body,
        HttpRequest request,
        OtpService otp,
        TrustedDeviceService devices,
        AccessTokenIssuer tokens,
        IPublicBookingQueries booking,
        ICustomerDirectory customers,
        CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        PhoneNumber? phone = null;
        if (string.IsNullOrWhiteSpace(body.Phone))
        {
            errors["phone"] = ["required"];
        }
        else if (!PhoneNumber.TryParse(body.Phone, out phone) || !phone.IsMobile)
        {
            // Kod SMS ile gider: yalnız cep telefonu.
            errors["phone"] = ["invalid_format"];
        }

        if (body.BranchId is null)
        {
            errors["branchId"] = ["required"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        if (await booking.EnterBranchAsync(body.BranchId!.Value, ct) is not { } scope)
        {
            return Error.Validation("branchId", "out_of_range").ToProblem();
        }

        if (TrustedDeviceCookie.Read(request) is { } cookie && await devices.UseAsync(cookie, phone!.E164, ct) is { } device)
        {
            var result = await BookingTokenAsync(phone, device.VerifiedOtpId, scope.BrandId, tokens, customers, ct);
            return TypedResults.Ok(new PublicOtpRequestResult("AlreadyVerified", Booking: result));
        }

        var issued = await otp.RequestAsync(phone!, OtpPurpose.PublicBooking, scope.BranchId, ct);
        return issued.ToHttp(i => TypedResults.Ok(new PublicOtpRequestResult(
            "CodeSent", Challenge: new OtpChallengeResponse(i.OtpId, i.ExpiresInSeconds, i.ResendAfterSeconds, i.PhoneMasked))));
    }

    private static async Task<IResult> VerifyAsync(
        OtpVerifyRequest body,
        HttpResponse response,
        OtpService otp,
        TrustedDeviceService devices,
        AccessTokenIssuer tokens,
        IPublicBookingQueries booking,
        ICustomerDirectory customers,
        CancellationToken ct)
    {
        if (body.Code is null || !SixDigits().IsMatch(body.Code))
        {
            return Error.Validation("code", "invalid_format").ToProblem();
        }

        var verified = await otp.VerifyAsync(body.OtpId, body.Code, OtpPurpose.PublicBooking, ct);
        if (!verified.IsSuccess)
        {
            return verified.Error.ToProblem();
        }

        var phone = PhoneNumber.Parse(verified.Value.PhoneE164);
        var scope = verified.Value.BranchId is { } branchId ? await booking.EnterBranchAsync(branchId, ct) : null;
        if (body.RememberDevice)
        {
            TrustedDeviceCookie.Append(response, await devices.IssueAsync(phone.E164, verified.Value.OtpId, ct));
        }

        return TypedResults.Ok(await BookingTokenAsync(phone, verified.Value.OtpId, scope?.BrandId, tokens, customers, ct));
    }

    private static async Task<BookingTokenResult> BookingTokenAsync(
        PhoneNumber phone, Guid otpId, Guid? brandId, AccessTokenIssuer tokens, ICustomerDirectory customers, CancellationToken ct)
    {
        var (token, expiresIn) = tokens.IssuePublicBooking(phone.E164, otpId);
        var name = brandId is { } brand ? await customers.FindBrandDisplayNameAsync(brand, phone, ct) : null;
        return new BookingTokenResult(token, expiresIn, phone.Masked, FirstName(name));
    }

    private static string? FirstName(string? fullName) =>
        string.IsNullOrWhiteSpace(fullName) ? null : fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

    [GeneratedRegex(@"^\d{6}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex SixDigits();
}
