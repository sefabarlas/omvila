using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Infrastructure;

/// <summary><see cref="IPlatformCatalogStats"/>: platform okuma servisi (ADR-0006); başka bağlamda çağrı programlama hatasıdır.</summary>
internal sealed class PlatformCatalogStats(CatalogDbContext db, ITenantContext tenant) : IPlatformCatalogStats
{
    public async Task<IReadOnlySet<Guid>> GetBrandsWithServicesAsync(IReadOnlyCollection<Guid> brandIds, CancellationToken ct)
    {
        if (!tenant.Scope.IsPlatform)
        {
            throw new DomainException("Platform okuma servisi yalnız platform bağlamında çağrılır (ADR-0006).");
        }

        return (await db.Services.AsNoTracking()
                .Where(s => brandIds.Contains(s.BrandId) && s.IsActive)
                .Select(s => s.BrandId)
                .Distinct()
                .ToListAsync(ct))
            .ToHashSet();
    }
}
