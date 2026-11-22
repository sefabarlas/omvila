using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure;

/// <summary>
/// <see cref="ICustomerBrandDirectory"/>: müşteri bağlamında marka tabloları görünmez (RLS); okuma platform bağlamında yapılır,
/// sonra oturumun önceki bağlamı geri yüklenir (SlugRegistry deseni). Yalnız müşteriye görünen kart alanları döner.
/// </summary>
internal sealed class CustomerBrandDirectory(TenancyDbContext db, IDbSession session, PublicBookingQueries booking, IClock clock)
    : ICustomerBrandDirectory
{
    public Task<Guid?> ResolveSlugAsync(string slug, CancellationToken ct) =>
        AsPlatformAsync(async () =>
        {
            var current = await db.Brands.AsNoTracking()
                .Where(b => b.Slug == slug && b.Status != BrandStatus.Archived)
                .Select(b => (Guid?)b.Id)
                .SingleOrDefaultAsync(ct);
            if (current is not null)
            {
                return current;
            }

            // Kural: sureler-ve-ayarlar.md SlugRedirectDays — yönlendirmesi süren eski ad güncel markaya çözülür.
            var now = clock.GetCurrentInstant();
            return await (from h in db.SlugHistory.AsNoTracking()
                          join b in db.Brands.AsNoTracking() on h.BrandId equals b.Id
                          where h.Slug == slug && h.RedirectExpiresUtc > now && b.Status != BrandStatus.Archived
                          orderby h.ChangedUtc descending
                          select (Guid?)b.Id).FirstOrDefaultAsync(ct);
        }, ct);

    public async Task<IReadOnlyDictionary<Guid, CustomerBrandCard>> GetCardsAsync(IReadOnlyCollection<Guid> brandIds, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(brandIds);
        if (brandIds.Count == 0)
        {
            return new Dictionary<Guid, CustomerBrandCard>();
        }

        return await AsPlatformAsync(async () =>
        {
            var brands = await (from b in db.Brands.AsNoTracking()
                                join p in db.BookingPageSettings.AsNoTracking() on b.Id equals p.BrandId
                                where brandIds.Contains(b.Id) && b.Status != BrandStatus.Archived
                                select new { b.Id, b.Name, b.Slug, b.LogoUrl, b.Status, p.CoverColorToken }).ToListAsync(ct);
            var ids = brands.Select(b => b.Id).ToList();
            var branches = (await db.Branches.AsNoTracking()
                    .Where(b => ids.Contains(b.BrandId) && b.IsActive)
                    .OrderBy(b => b.CreatedUtc)
                    .Select(b => new { b.BrandId, b.Id, b.Name, b.District })
                    .ToListAsync(ct))
                .ToLookup(b => b.BrandId);
            var cards = new Dictionary<Guid, CustomerBrandCard>();
            foreach (var brand in brands)
            {
                cards[brand.Id] = new CustomerBrandCard(
                    brand.Id,
                    brand.Name,
                    brand.Slug,
                    brand.LogoUrl,
                    brand.CoverColorToken,
                    await booking.IsBookingEnabledAsync(brand.Id, brand.Status, ct),
                    [.. branches[brand.Id].Select(b => new CustomerBranchCard(b.Id, b.Name, b.District))]);
            }

            return (IReadOnlyDictionary<Guid, CustomerBrandCard>)cards;
        }, ct);
    }

    private async Task<T> AsPlatformAsync<T>(Func<Task<T>> read, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            return await read();
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }
}
