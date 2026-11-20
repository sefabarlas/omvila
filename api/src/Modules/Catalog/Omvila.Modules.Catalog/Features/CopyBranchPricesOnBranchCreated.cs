using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Domain;
using Omvila.Modules.Catalog.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features;

/// <summary>"Hizmet ve fiyatlar: …'den kopyala" (SubeEkle): kaynak şubenin şube fiyatları yeni şubeye yazılır.</summary>
internal sealed class CopyBranchPricesOnBranchCreated(CatalogDbContext db, IDbSession session, IIdGenerator ids) : IIntegrationEventHandler<BranchCreated>
{
    public async Task HandleAsync(BranchCreated integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        if (integrationEvent.CopyServicePricesFromBranchId is not { } source)
        {
            return;
        }

        await session.SetTenantAsync(TenantScope.ForBrand(integrationEvent.BrandId), ct);
        var prices = await db.BranchPrices.AsNoTracking().Where(p => p.BranchId == source).ToListAsync(ct);
        var already = await db.BranchPrices.Where(p => p.BranchId == integrationEvent.BranchId).Select(p => p.ServiceId).ToListAsync(ct);
        db.BranchPrices.AddRange(prices.Where(p => !already.Contains(p.ServiceId)).Select(p => BranchServicePrice.Create(
            ids.New(), integrationEvent.BrandId, integrationEvent.BranchId, p.ServiceId,
            new BranchPrice(Money.Try(p.PriceMinor), p.DurationOverrideMinutes, p.IsAvailable))));
        await db.SaveChangesAsync(ct);
    }
}
