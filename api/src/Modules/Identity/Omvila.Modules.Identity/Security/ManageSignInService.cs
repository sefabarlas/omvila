using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Salon kullanıcısının oturumu (ADR-0007). İstek transaction'ı içinde çağrılır.
/// Giriş ve yenileme kiracı bağlamı olmadan gelir; üyelik yalnız kullanıcı kimliğiyle süzülerek platform
/// bağlamında okunur (ADR-0006 § sistem süreçleri). Sonunda bağlam üyeliğin markasına daraltılır.
/// Rol ve şube kapsamı her yenilemede veritabanından okunur (T-19: rol değişikliği en geç bir yenilemede etkili).
/// </summary>
internal sealed class ManageSignInService(
    IdentityDbContext db,
    IDbSession session,
    AccessTokenIssuer accessTokens,
    RefreshTokenService refreshTokens,
    SignupTokenService signupTokens,
    IClock clock)
{
    public async Task<SignInOutcome> SignInAsync(string phoneE164, RefreshClientType clientType, bool rememberDevice, CancellationToken ct)
    {
        await session.SetTenantAsync(TenantScope.Platform, ct);
        // Platform rolü salon girişini etkilemez: salon oturumu yalnız marka üyeliğine bakar (T-128).
        var user = await db.Users.SingleOrDefaultAsync(u => u.PhoneE164 == phoneE164 && u.IsActive, ct);
        var membership = user is null
            ? null
            : await db.Memberships.SingleOrDefaultAsync(m => m.UserId == user.Id && m.Status != MembershipStatus.Disabled, ct);
        if (user is null || membership is null)
        {
            await session.SetTenantAsync(TenantScope.None, ct);
            return new SignInOutcome(null, signupTokens.Issue(phoneE164));
        }

        var now = clock.GetCurrentInstant();
        if (membership.Status == MembershipStatus.Invited)
        {
            membership.Activate(now);
        }

        user.RecordLogin(now);
        await db.SaveChangesAsync(ct);
        var refresh = await refreshTokens.IssueAsync(user.Id, membership.Id, clientType, rememberDevice, ct);
        await session.SetTenantAsync(TenantScope.ForBrand(membership.BrandId), ct);
        return new SignInOutcome(Session(user.Id, membership, refresh), null);
    }

    /// <summary>Yenileme: jeton döndürülür; üyelik etkin değilse 401.</summary>
    public async Task<Result<ManageSession>> RefreshAsync(string token, CancellationToken ct)
    {
        var rotated = await refreshTokens.RotateAsync(token, ct);
        if (!rotated.IsSuccess)
        {
            return rotated.Error;
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
        var membershipId = rotated.Value.MembershipId;
        var membership = membershipId is null
            ? null
            : await db.Memberships.AsNoTracking().SingleOrDefaultAsync(m => m.Id == membershipId && m.Status == MembershipStatus.Active, ct);
        if (membership is null)
        {
            return Error.Unauthorized();
        }

        await session.SetTenantAsync(TenantScope.ForBrand(membership.BrandId), ct);
        return Session(rotated.Value.UserId, membership, rotated.Value);
    }

    private ManageSession Session(Guid userId, UserMembership membership, IssuedRefreshToken refresh)
    {
        var branchIds = string.Equals(membership.Role, Roles.Owner, StringComparison.Ordinal) || membership.BranchIds.Length == 0
            ? null
            : membership.BranchIds;
        var (token, expiresIn) = accessTokens.IssueManage(
            new ManageTokenClaims(userId, membership.BrandId, membership.Id, membership.Role, branchIds, membership.StaffId));
        return new ManageSession(token, expiresIn, refresh);
    }
}
