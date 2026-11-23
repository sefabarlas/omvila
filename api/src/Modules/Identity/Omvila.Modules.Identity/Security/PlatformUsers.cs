using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Platform kullanıcısı oluşturma (K2) ve platform okuma servisleri (ADR-0006: yalnız platform bağlamında; başka bağlamda
/// istisna). Aynı kullanıcı hem marka üyesi hem platform kullanıcısı olabilir; oturumlar ayrı audience'tır (T-128).
/// </summary>
internal sealed class PlatformUsers(IdentityDbContext db, ITenantContext tenant, IIdGenerator ids, IClock clock)
    : IPlatformUserProvisioning, IBrandOwnerDirectory, IUserNameDirectory
{
    private const int NameSearchLimit = 200;

    public async Task<Result<ProvisionedPlatformUser>> EnsureAsync(PhoneNumber phone, string fullName, string platformRole, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(phone);
        RequirePlatformScope();
        if (!Enum.TryParse<PlatformRole>(platformRole, ignoreCase: false, out var role) || !Enum.IsDefined(role))
        {
            return Error.Validation("platformRole", "invalid_format");
        }

        if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length > 120)
        {
            return Error.Validation("fullName", "invalid_format");
        }

        var user = await db.Users.SingleOrDefaultAsync(u => u.PhoneE164 == phone.E164, ct);
        var created = user is null;
        // Marka üyesinin salonda görünen adı komutla değişmez (T-128); ad hiç girilmemişse komuttaki ad yazılır.
        var keepName = user is not null && !string.IsNullOrWhiteSpace(user.FullName)
            && await db.Memberships.AnyAsync(m => m.UserId == user.Id, ct);
        if (user is null)
        {
            user = User.Create(ids.New(), phone.E164, fullName, clock.GetCurrentInstant());
            db.Users.Add(user);
        }

        user.AssignPlatformRole(role, keepName ? null : fullName);
        await db.SaveChangesAsync(ct);
        return new ProvisionedPlatformUser(user.Id, created);
    }

    public async Task<IReadOnlyDictionary<Guid, BrandOwnerInfo>> GetOwnersAsync(IReadOnlyCollection<Guid>? brandIds, CancellationToken ct)
    {
        RequirePlatformScope();
        var query = from m in db.Memberships.AsNoTracking()
                    join u in db.Users.AsNoTracking() on m.UserId equals u.Id
                    where m.Role == Roles.Owner && m.Status != MembershipStatus.Disabled
                    select new { m.BrandId, m.CreatedUtc, u.FullName, u.PhoneE164 };
        if (brandIds is not null)
        {
            query = query.Where(o => brandIds.Contains(o.BrandId));
        }

        var rows = await query.ToListAsync(ct);
        // Birden çok sahip olursa (v1'de olmaz) ilk eklenen gösterilir.
        return rows.GroupBy(r => r.BrandId)
            .ToDictionary(g => g.Key, g => g.OrderBy(r => r.CreatedUtc).Select(r => new BrandOwnerInfo(r.FullName ?? string.Empty, r.PhoneE164)).First());
    }

    public async Task<IReadOnlyDictionary<Guid, UserNameInfo>> GetAsync(IReadOnlyCollection<Guid> userIds, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(userIds);
        RequirePlatformScope();
        var rows = await db.Users.AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullName, u.PlatformRole })
            .ToListAsync(ct);
        return rows.ToDictionary(r => r.Id, r => new UserNameInfo(r.FullName ?? string.Empty, r.PlatformRole?.ToString()));
    }

    public async Task<IReadOnlyList<Guid>> FindByNameAsync(string query, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(query);
        RequirePlatformScope();
        var pattern = $"%{query.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("%", "\\%", StringComparison.Ordinal).Replace("_", "\\_", StringComparison.Ordinal)}%";
        return await db.Users.AsNoTracking()
            .Where(u => u.FullName != null && EF.Functions.ILike(u.FullName, pattern))
            .OrderBy(u => u.Id)
            .Select(u => u.Id)
            .Take(NameSearchLimit)
            .ToListAsync(ct);
    }

    /// <summary>Oturumdaki etkin platform kullanıcısı (yoksa veya rolü kaldırıldıysa null).</summary>
    public Task<User?> FindActiveAsync(Guid userId, CancellationToken ct) =>
        db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId && u.IsActive && u.PlatformRole != null, ct);

    private void RequirePlatformScope()
    {
        if (!tenant.Scope.IsPlatform)
        {
            throw new DomainException("Platform okuma servisi yalnız platform bağlamında çağrılır (ADR-0006).");
        }
    }
}
