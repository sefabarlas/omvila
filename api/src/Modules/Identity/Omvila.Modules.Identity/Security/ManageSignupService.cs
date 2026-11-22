using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// İşletme kaydı (dilim-01 § Alan kuralları, kabul kriteri 1): tek transaction'da kullanıcı, koşul onayları, marka
/// (Tenancy), Owner üyeliği ve deneme aboneliği (Platform). Kayıt jetonu OTP ile doğrulanmış telefonu taşır.
/// Numaranın zaten bir üyeliği varsa (ör. yanıtı kaybolan isteğin tekrarı) yeni marka açılmaz: kayıt jetonu OTP
/// doğrulamasının kanıtı olduğundan sonuç girişle aynıdır. Böylece çift marka oluşmaz ve jetonlar saklanmaz.
/// </summary>
internal sealed class ManageSignupService(
    IdentityDbContext db,
    IDbSession session,
    ManageSignInService signIn,
    SignupTokenService signupTokens,
    ITenantProvisioning tenants,
    ISubscriptionProvisioning subscriptions,
    RefreshTokenService refreshTokens,
    AccessTokenIssuer accessTokens,
    IIdGenerator ids,
    IClock clock,
    IOptions<LegalOptions> legal)
{
    public async Task<Result<ManageSession>> SignupAsync(SignupCommand command, RefreshClientType clientType, CancellationToken ct)
    {
        var phone = await signupTokens.ValidateAsync(command.SignupToken);
        if (!phone.IsSuccess)
        {
            return phone.Error;
        }

        // Kullanıcı ve üyelik markalar üstüdür: telefonla süzülmüş okuma platform bağlamında (ADR-0006 § sistem süreçleri).
        await session.SetTenantAsync(TenantScope.Platform, ct);
        var user = await db.Users.SingleOrDefaultAsync(u => u.PhoneE164 == phone.Value, ct);
        if (user?.PlatformRole is not null)
        {
            // Platform kullanıcısının marka üyeliği olamaz (alan-modeli.md § User.PlatformRole).
            return Error.Unauthorized();
        }

        if (user is not null && await db.Memberships.AnyAsync(m => m.UserId == user.Id, ct))
        {
            var existing = await signIn.SignInAsync(phone.Value, clientType, rememberDevice: false, ct);
            return existing.Session is { } signedIn ? signedIn : Error.Unauthorized();
        }

        var now = clock.GetCurrentInstant();
        if (user is null)
        {
            user = User.Create(ids.New(), phone.Value, command.OwnerFullName, now);
            db.Users.Add(user);
        }

        user.RecordLogin(now);
        db.TermsAcceptances.Add(TermsAcceptance.Record(
            ids.New(), user.Id, LegalDocumentType.TermsOfUse, legal.Value.TermsOfUseVersion, command.IpAddress, now));
        db.TermsAcceptances.Add(TermsAcceptance.Record(
            ids.New(), user.Id, LegalDocumentType.PrivacyNotice, legal.Value.PrivacyNoticeVersion, command.IpAddress, now));
        await db.SaveChangesAsync(ct);

        // Buradan sonra oturum yeni markanın bağlamındadır (ITenantProvisioning).
        var tenant = await tenants.ProvisionBrandAsync(command.BusinessName, command.IndustryCode, user.Id, command.OwnerFullName, ct);
        var membership = UserMembership.Create(
            ids.New(), user.Id, tenant.BrandId, Roles.Owner, [], tenant.OwnerStaffId, MembershipStatus.Active, now);
        db.Memberships.Add(membership);
        await db.SaveChangesAsync(ct);
        await subscriptions.StartTrialAsync(tenant.BrandId, PlatformDefaults.DefaultTimeZoneId, ct);

        // Kayıtta "hatırla" seçeneği yok (sözleşme): web oturumu girişteki varsayılanla (hatırlama kapalı) açılır.
        var refresh = await refreshTokens.IssueAsync(user.Id, membership.Id, clientType, rememberDevice: false, ct);
        var (token, expiresIn) = accessTokens.IssueManage(
            new ManageTokenClaims(user.Id, tenant.BrandId, membership.Id, Roles.Owner, BranchIds: null, tenant.OwnerStaffId));
        return new ManageSession(token, expiresIn, refresh);
    }
}
