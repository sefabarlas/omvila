using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary>
/// <see cref="ICustomerPackageBadges"/>: müşterinin kartları birden çok markada olduğundan okuma platform bağlamında, yalnız
/// verilen kart kimlikleriyle süzülerek yapılır (ADR-0006 § sistem süreçleri). Kalan seans defterden.
/// </summary>
internal sealed class CustomerPackageBadges(SalesDbContext db, IDbSession session) : ICustomerPackageBadges
{
    public async Task<IReadOnlyDictionary<Guid, IReadOnlyList<PackageBadge>>> ListActiveAsync(
        IReadOnlyCollection<Guid> brandCustomerIds, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(brandCustomerIds);
        if (brandCustomerIds.Count == 0)
        {
            return new Dictionary<Guid, IReadOnlyList<PackageBadge>>();
        }

        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            var packages = await db.CustomerPackages.AsNoTracking()
                .Include(p => p.Items)
                .Where(p => brandCustomerIds.Contains(p.BrandCustomerId) && p.Status == CustomerPackageStatus.Active)
                .ToListAsync(ct);
            var itemIds = packages.SelectMany(p => p.Items).Select(i => i.Id).ToList();
            var used = itemIds.Count == 0
                ? []
                : await db.PackageSessionUsages.AsNoTracking()
                    .Where(u => itemIds.Contains(u.CustomerPackageItemId) && u.RevertedUtc == null)
                    .GroupBy(u => u.CustomerPackageItemId)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Key, x => x.Count, ct);

            // Kural: sözleşme meListBrands.activePackages (Active ve kalan seansı olan)
            return packages
                .Select(p => (Package: p, Remaining: p.Items.Sum(i => i.Remaining(used.GetValueOrDefault(i.Id)))))
                .Where(x => x.Remaining > 0)
                .GroupBy(x => x.Package.BrandCustomerId)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<PackageBadge>)[.. g.OrderBy(x => x.Package.ExpiresOn).ThenBy(x => x.Package.Id)
                        .Select(x => new PackageBadge(x.Package.Id, x.Package.PackageNameSnapshot, x.Remaining))]);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }
}
