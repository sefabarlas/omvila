using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Tenancy.Infrastructure;

internal sealed class TenantQueries(TenancyDbContext db) : ITenantQueries
{
    public async Task<TenantSummary?> GetSummaryAsync(Guid brandId, IReadOnlyCollection<Guid>? branchIds, CancellationToken ct)
    {
        var brand = await db.Brands.AsNoTracking().SingleOrDefaultAsync(b => b.Id == brandId, ct);
        if (brand is null)
        {
            return null;
        }

        var branches = db.Branches.AsNoTracking().Where(b => b.BrandId == brandId && b.IsActive);
        if (branchIds is not null)
        {
            branches = branches.Where(b => branchIds.Contains(b.Id));
        }

        var list = await branches.OrderBy(b => b.CreatedUtc).Select(b => new BranchSummary(b.Id, b.Name)).ToListAsync(ct);
        return new TenantSummary(brand.Id, brand.Name, brand.Slug, brand.LogoUrl, brand.Status.ToString(), list);
    }
}
