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
/// "Hesabımı ve verilerimi sil" (sözleşme meRequestAccountDeletionOtp, meRequestAccountDeletion; HesapSil; dilim-05 KK9; W7 hemen
/// uygulanır). Kod oturumdaki müşterinin kendi numarasına gider (amaç AccountDeletion; giriş veya salon kodu burada geçmez). Doğru kodla
/// Customers bütün markalarda anonimleştirir (<see cref="ICustomerAccounts.DeleteAccountAsync"/>), Notifications cihazları ve bildirim
/// listesini siler, müşterinin bütün uygulama oturumları kapanır; yanıttan sonra jetonlar geçersizdir.
/// </summary>
internal static partial class MeAccountDeletionEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/me/account/deletion", Audiences.Me);

        // Sınırlar giriş koduyla aynıdır: telefon başına OtpService'te, IP başına giriş politikasıyla birlikte sayılır.
        group.MapPost("/otp", RequestOtpAsync).WithName("meRequestAccountDeletionOtp").RequireAuthenticated(Audiences.Me)
            .RequireRateLimiting(MeAuthEndpoints.OtpRateLimitPolicy);
        group.MapPost(string.Empty, DeleteAsync).WithName("meRequestAccountDeletion").RequireAuthenticated(Audiences.Me);
    }

    private static async Task<IResult> RequestOtpAsync(HttpContext http, ICustomerAccounts customers, OtpService otp, CancellationToken ct)
    {
        if (!TryCustomerId(http, out var customerId) || await customers.GetIdentityAsync(customerId, ct) is not { } identity)
        {
            return Error.Unauthorized().ToProblem();
        }

        var issued = await otp.RequestAsync(identity.Phone, OtpPurpose.AccountDeletion, ct);
        return issued.ToHttp(i => TypedResults.Json(
            new OtpChallengeResponse(i.OtpId, i.ExpiresInSeconds, i.ResendAfterSeconds, i.PhoneMasked), statusCode: StatusCodes.Status202Accepted));
    }

    // Kural: guvenlik-kvkk.md § Haklar ve işleyiş (Silme, müşteri: SMS koduyla); sözleşme meRequestAccountDeletion
    private static async Task<IResult> DeleteAsync(
        MeAccountDeletionConfirm body,
        HttpContext http,
        ICustomerAccounts customers,
        OtpService otp,
        ICustomerAppDataEraser appData,
        RefreshTokenService refreshTokens,
        CancellationToken ct)
    {
        if (body.OtpId is null)
        {
            return Error.Validation("otpId", "required").ToProblem();
        }

        if (body.Code is null || !SixDigits().IsMatch(body.Code))
        {
            return Error.Validation("code", "invalid_format").ToProblem();
        }

        if (!TryCustomerId(http, out var customerId) || await customers.GetIdentityAsync(customerId, ct) is not { } identity)
        {
            return Error.Unauthorized().ToProblem();
        }

        var verified = await otp.VerifyAsync(body.OtpId.Value, body.Code, OtpPurpose.AccountDeletion, ct);
        if (!verified.IsSuccess)
        {
            return verified.Error.ToProblem();
        }

        // Kod başka bir numaraya gönderilmişse (başka hesabın silme kodu) bu hesabı silmez.
        if (!string.Equals(verified.Value.PhoneE164, identity.Phone.E164, StringComparison.Ordinal))
        {
            return Error.Unprocessable(ErrorCodes.OtpInvalid).ToProblem();
        }

        var userAgent = http.Request.Headers.UserAgent.ToString();
        var deleted = await customers.DeleteAccountAsync(
            customerId, http.Connection.RemoteIpAddress?.ToString(), userAgent.Length > 0 ? userAgent : null, ct);
        if (deleted is null)
        {
            return Error.Unauthorized().ToProblem();
        }

        await appData.EraseAsync(customerId, ct);
        await refreshTokens.RevokeCustomerSessionsAsync(customerId, ct);
        return TypedResults.Ok(new MeAccountDeletionResult(deleted.DeletedUtc, deleted.CancelledAppointmentCount));
    }

    private static bool TryCustomerId(HttpContext http, out Guid customerId) =>
        Guid.TryParse(http.User.FindFirst(OmvilaClaims.Subject)?.Value, out customerId);

    [GeneratedRegex(@"^\d{6}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex SixDigits();
}
