using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.Modules.Identity.Security;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-74) için Identity yazma yolu. Kayıt akışıyla (ManageSignupService)
/// aynı adımlar: kullanıcı, koşul onayları, marka (ITenantProvisioning), Owner üyeliği ve deneme aboneliği
/// (ISubscriptionProvisioning). OTP ve jeton yoktur; çağıranın transaction'ında çalışır.
/// </summary>
public sealed class IdentityDemoData
{
    private readonly IdentityDbContext _db;
    private readonly IDbSession _session;
    private readonly ITenantProvisioning _tenants;
    private readonly ISubscriptionProvisioning _subscriptions;
    private readonly IIdGenerator _ids;
    private readonly IClock _clock;
    private readonly LegalOptions _legal;

    internal IdentityDemoData(
        IdentityDbContext db,
        IDbSession session,
        ITenantProvisioning tenants,
        ISubscriptionProvisioning subscriptions,
        IIdGenerator ids,
        IClock clock,
        IOptions<LegalOptions> legal)
    {
        _db = db;
        _session = session;
        _tenants = tenants;
        _subscriptions = subscriptions;
        _ids = ids;
        _clock = clock;
        _legal = legal.Value;
    }

    /// <summary>Numaranın sahibi (Owner) olduğu marka; yoksa null. Üyelik v1'de kullanıcı başına tektir (S-16).</summary>
    public async Task<Guid?> FindOwnedBrandAsync(PhoneNumber ownerPhone, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(ownerPhone);
        await _session.SetTenantAsync(TenantScope.Platform, ct);
        return await (from m in _db.Memberships.AsNoTracking()
                      join u in _db.Users.AsNoTracking() on m.UserId equals u.Id
                      where u.PhoneE164 == ownerPhone.E164 && m.Role == Roles.Owner
                      select (Guid?)m.BrandId).SingleOrDefaultAsync(ct);
    }

    /// <summary>
    /// Markayı sahibiyle kurar (kayıtla aynı). Numaranın başka bir üyeliği varsa <see cref="DomainException"/>: demo verisi
    /// başka bir hesaba dokunmaz. Dönüşte oturum yeni markanın kiracı bağlamındadır.
    /// </summary>
    public async Task<ProvisionedTenant> CreateBrandWithOwnerAsync(
        PhoneNumber ownerPhone, string ownerFullName, string businessName, string industryCode, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(ownerPhone);
        await _session.SetTenantAsync(TenantScope.Platform, ct);
        var user = await _db.Users.SingleOrDefaultAsync(u => u.PhoneE164 == ownerPhone.E164, ct);
        if (user is not null && await _db.Memberships.AnyAsync(m => m.UserId == user.Id, ct))
        {
            throw new DomainException("Demo sahibinin numarasının zaten bir üyeliği var.");
        }

        var now = _clock.GetCurrentInstant();
        if (user is null)
        {
            user = User.Create(_ids.New(), ownerPhone.E164, ownerFullName, now);
            _db.Users.Add(user);
        }

        // --reset'te kullanıcı kalır; aynı sürümün onayı yeniden yazılmaz.
        foreach (var (type, version) in new[] { (LegalDocumentType.TermsOfUse, _legal.TermsOfUseVersion), (LegalDocumentType.PrivacyNotice, _legal.PrivacyNoticeVersion) })
        {
            if (!await _db.TermsAcceptances.AnyAsync(t => t.UserId == user.Id && t.DocumentType == type && t.DocumentVersion == version, ct))
            {
                _db.TermsAcceptances.Add(TermsAcceptance.Record(_ids.New(), user.Id, type, version, null, now));
            }
        }

        await _db.SaveChangesAsync(ct);

        var tenant = await _tenants.ProvisionBrandAsync(businessName, industryCode, user.Id, ownerFullName, ct);
        _db.Memberships.Add(UserMembership.Create(
            _ids.New(), user.Id, tenant.BrandId, Roles.Owner, [], tenant.OwnerStaffId, MembershipStatus.Active, now));
        await _db.SaveChangesAsync(ct);
        await _subscriptions.StartTrialAsync(tenant.BrandId, PlatformDefaults.DefaultTimeZoneId, ct);
        return tenant;
    }
}
