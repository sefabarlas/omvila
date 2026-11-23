using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Features.ManageAuth;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.Modules.Identity.Security;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Features.AdminAuth;

/// <summary>
/// Platform paneli girişi ve oturumu (sözleşme admin-auth, adminGetSession; K2, ADR-0007 admin yüzeyi). Telefon + SMS kodu
/// (amaç AdminLogin; salon veya müşteri kodu burada geçmez). Numara etkin bir platform kullanıcısına ait değilse istek yine 202
/// döner ama SMS gitmez; doğrulama otp_invalid. Erişim jetonu aud=admin (sub + platform_role), yenileme jetonu gövdede
/// (yüzeyi admin, sekme ömrü; Auth:RefreshTokenDaysSession). Aynı numara marka üyesi de olabilir; jetonlar ayrı audience'tır (T-128).
/// </summary>
internal static partial class AdminAuthEndpoints
{
    public const string OtpRateLimitPolicy = "admin-otp-ip";
    private const string SessionEntity = "PlatformSession";

    public static void Map(IEndpointRouteBuilder app)
    {
        var auth = app.MapApiGroup("/api/v1/admin/auth");

        // Giriş öncesi: anonim (oturum henüz yok). Numaranın platform kullanıcısı olup olmadığı açık edilmez (her durumda 202). IP başına sınırlı.
        auth.MapPost("/otp/request", RequestOtpAsync).WithName("adminRequestOtp").AllowAnonymous().RequireRateLimiting(OtpRateLimitPolicy);

        // Giriş öncesi: anonim (kod doğrulaması oturumu açar).
        auth.MapPost("/otp/verify", VerifyOtpAsync).WithName("adminVerifyOtp").AllowAnonymous();

        // Yenileme jetonuyla çalışır (erişim jetonunun süresi dolmuştur); jeton gövdede, çerez yok (CSRF yüzeyi yok).
        auth.MapPost("/refresh", RefreshAsync).WithName("adminRefreshToken").AllowAnonymous();

        auth.MapPost("/logout", LogoutAsync).WithName("adminLogout").RequireAuthenticated(Audiences.Admin);

        // Oturum özeti bütün platform rollerine açıktır (izin listesini döner); rol veritabanından okunur.
        app.MapApiGroup("/api/v1/admin/session", Audiences.Admin)
            .MapGet(string.Empty, GetSessionAsync)
            .WithName("adminGetSession")
            .RequireAuthenticated(Audiences.Admin);
    }

    private static async Task<IResult> RequestOtpAsync(AdminOtpRequest body, OtpService otp, IdentityDbContext db, CancellationToken ct)
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

        // users markaya ait değildir (RLS yok): platform kullanıcısı mı, bağlam kurmadan okunur.
        var isPlatformUser = await db.Users.AsNoTracking()
            .AnyAsync(u => u.PhoneE164 == phone.E164 && u.IsActive && u.PlatformRole != null, ct);
        var issued = await otp.RequestAsync(phone, OtpPurpose.AdminLogin, deliver: isPlatformUser, ct);
        return issued.ToHttp(i => TypedResults.Json(
            new OtpChallengeResponse(i.OtpId, i.ExpiresInSeconds, i.ResendAfterSeconds, i.PhoneMasked), statusCode: StatusCodes.Status202Accepted));
    }

    private static async Task<IResult> VerifyOtpAsync(
        OtpVerifyRequest body,
        HttpContext http,
        OtpService otp,
        IdentityDbContext db,
        IDbSession session,
        RefreshTokenService refreshTokens,
        AccessTokenIssuer accessTokens,
        IAuditLog audit,
        IClock clock,
        CancellationToken ct)
    {
        if (body.Code is null || !SixDigits().IsMatch(body.Code))
        {
            return Error.Validation("code", "invalid_format").ToProblem();
        }

        // rememberDevice yok sayılır (sözleşme adminVerifyOtp): oturum sekme ömrüdür.
        var verified = await otp.VerifyAsync(body.OtpId, body.Code, OtpPurpose.AdminLogin, ct);
        if (!verified.IsSuccess)
        {
            return verified.Error.ToProblem();
        }

        var user = await db.Users.SingleOrDefaultAsync(u => u.PhoneE164 == verified.Value.PhoneE164 && u.IsActive && u.PlatformRole != null, ct);
        if (user?.PlatformRole is not { } role)
        {
            return Error.Unprocessable(ErrorCodes.OtpInvalid).ToProblem();
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
        user.RecordLogin(clock.GetCurrentInstant());
        await db.SaveChangesAsync(ct);
        var refresh = await refreshTokens.IssueAsync(
            user.Id, membershipId: null, RefreshClientType.Web, rememberDevice: false, ct, RefreshToken.AdminAudience);
        var (token, expiresIn) = accessTokens.IssueAdmin(user.Id, role.ToString());
        // Kural: sözleşme adminVerifyOtp — giriş denetime yazılır (kişisel veri yok; rol kodu).
        await audit.WriteAsync(
            new AuditEntry(
                null, AuditActorType.PlatformUser, user.Id, AuditAction.Create, SessionEntity, user.Id,
                new Dictionary<string, object?>(StringComparer.Ordinal) { ["platformRole"] = role.ToString() },
                http.Connection.RemoteIpAddress?.ToString(), http.Request.Headers.UserAgent.ToString()),
            ct);
        return TypedResults.Ok(new AdminAuthResult(token, expiresIn, refresh.Token));
    }

    private static async Task<IResult> RefreshAsync(
        AdminRefreshRequest? body, PlatformUsers users, IDbSession session, RefreshTokenService refreshTokens, AccessTokenIssuer accessTokens, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(body?.RefreshToken))
        {
            return Error.Unauthorized().ToProblem();
        }

        var rotated = await refreshTokens.RotateAsync(body.RefreshToken, ct, RefreshToken.AdminAudience);
        if (!rotated.IsSuccess)
        {
            return rotated.Error.ToProblem();
        }

        // Pasifleştirilen veya rolü kaldırılan kullanıcının oturumu yenilenmez; rol her yenilemede veritabanından.
        await session.SetTenantAsync(TenantScope.Platform, ct);
        var user = await users.FindActiveAsync(rotated.Value.UserId, ct);
        if (user?.PlatformRole is not { } role)
        {
            return Error.Unauthorized().ToProblem();
        }

        var (token, expiresIn) = accessTokens.IssueAdmin(user.Id, role.ToString());
        return TypedResults.Ok(new TokenPair(token, expiresIn, rotated.Value.Token));
    }

    private static async Task<IResult> LogoutAsync(AdminRefreshRequest? body, ClaimsPrincipal principal, RefreshTokenService refreshTokens, CancellationToken ct)
    {
        if (!Guid.TryParse(principal.FindFirst(OmvilaClaims.Subject)?.Value, out var userId))
        {
            return Error.Unauthorized().ToProblem();
        }

        await refreshTokens.RevokeAdminAsync(body?.RefreshToken, userId, ct);
        return TypedResults.NoContent();
    }

    private static async Task<IResult> GetSessionAsync(
        ClaimsPrincipal principal, PlatformUsers users, IPendingApprovalCounts approvals, CancellationToken ct)
    {
        if (!Guid.TryParse(principal.FindFirst(OmvilaClaims.Subject)?.Value, out var userId))
        {
            return Error.Unauthorized().ToProblem();
        }

        var user = await users.FindActiveAsync(userId, ct);
        if (user?.PlatformRole is not { } role)
        {
            return Error.Unauthorized().ToProblem();
        }

        var permissions = PlatformRolePermissions.For(role.ToString());
        // Kural: sözleşme AdminSession.pendingApprovalCount — kullanıcının görebildiği bekleyenler.
        var pending = await approvals.GetAsync(ct);
        var count = (permissions.Contains(AdminPermissions.PaymentsMatch, StringComparer.Ordinal) ? pending.PaymentNotices : 0)
            + (permissions.Contains(AdminPermissions.SmsSendersDecide, StringComparer.Ordinal) ? pending.SmsSenders : 0);
        var phone = PhoneNumber.TryParse(user.PhoneE164, out var parsed) ? parsed.Masked : string.Empty;
        return TypedResults.Ok(new AdminSession(new AdminSessionUser(user.Id, user.FullName ?? string.Empty, phone), role.ToString(), permissions, count));
    }

    [GeneratedRegex(@"^\d{6}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex SixDigits();
}
