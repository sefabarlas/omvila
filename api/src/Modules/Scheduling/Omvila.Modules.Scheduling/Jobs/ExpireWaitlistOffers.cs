using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.Modules.Scheduling.Features.Waitlist;
using Omvila.Modules.Scheduling.Infrastructure;

namespace Omvila.Modules.Scheduling.Jobs;

/// <summary>
/// ExpireWaitlistOffers (durum-makinesi.md § 3, Pending → Expired; dilim-05 kabul kriteri 3): tutma süresi dolan bekleme
/// teklifi Expired olur, kişi listede kalır ve aynı saat sıradaki uygun kişiye teklif edilir. Saat zaten tutma bitince
/// müsaitlikte boşalır; iş durumu kapatır ve sıradakine geçirir. Adaylar platform kapsamında kısmi indeksle
/// (<c>ix_waitlist_offers_hold_candidates</c>) bulunur, her marka kendi kiracı bağlamında işlenir.
/// </summary>
internal sealed partial class ExpireWaitlistOffers(
    SchedulingDbContext db,
    IDbSession session,
    WaitlistOfferEngine engine,
    IClock clock,
    ILogger<ExpireWaitlistOffers> logger) : IPeriodicJob
{
    public string Key => "ExpireWaitlistOffers";

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var brandIds = await db.WaitlistOffers.AsNoTracking()
            .Where(o => o.Status == WaitlistOfferStatus.Pending && o.HoldExpiresUtc <= now)
            .Select(o => o.BrandId)
            .Distinct()
            .ToListAsync(ct);

        foreach (var brandId in brandIds)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
            var expired = await ExpireBrandAsync(brandId, now, ct);
            if (expired > 0)
            {
                LogExpired(logger, brandId, expired);
            }
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
    }

    // Kural: durum-makinesi.md § 3 (Pending → Expired: sıradakine teklif; kişi listede kalır)
    private async Task<int> ExpireBrandAsync(Guid brandId, Instant now, CancellationToken ct)
    {
        var entries = await db.WaitlistEntries
            .Include(e => e.Offers)
            .Where(e => e.Offers.Any(o => o.Status == WaitlistOfferStatus.Pending && o.HoldExpiresUtc <= now))
            .ToListAsync(ct);
        var expired = new List<WaitlistOffer>();
        foreach (var entry in entries)
        {
            if (entry.PendingOffer is { } offer && entry.ExpireOffer(offer, now).IsSuccess)
            {
                expired.Add(offer);
            }
        }

        await db.SaveChangesAsync(ct);
        foreach (var offer in expired.OrderBy(o => o.OfferedStartUtc))
        {
            await engine.OfferFreedSlotAsync(brandId, offer.BranchId, offer.OfferedStaffId, offer.OfferedStartUtc, ct);
        }

        return expired.Count;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Expired {Count} waitlist offers for brand {BrandId}")]
    private static partial void LogExpired(ILogger logger, Guid brandId, int count);
}
