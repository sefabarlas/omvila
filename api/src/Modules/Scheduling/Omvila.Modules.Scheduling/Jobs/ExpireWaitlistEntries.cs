using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Jobs;

/// <summary>
/// ExpireWaitlistEntries (T-90; durum-makinesi.md § 3 "Expired: istenen son tarih geçti"): istenen günlerinin hepsi
/// şubenin bugününden önce kalan bekleme kaydı Expired olur. "Herhangi bir gün" (gün listesi boş) kayıtlar beklemeye
/// devam eder; bekleyen teklifi olan kayıt teklif sonuçlanana kadar beklenir. Müşteriye bildirim gitmez.
/// </summary>
internal sealed partial class ExpireWaitlistEntries(
    SchedulingDbContext db,
    IDbSession session,
    ITenantQueries tenants,
    IClock clock,
    ILogger<ExpireWaitlistEntries> logger) : IPeriodicJob
{
    public string Key => "ExpireWaitlistEntries";

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var brandIds = await db.WaitlistEntries.AsNoTracking()
            .Where(e => e.Status == WaitlistEntryStatus.Waiting && e.Dates.Count > 0)
            .Select(e => e.BrandId)
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

    private async Task<int> ExpireBrandAsync(Guid brandId, Instant now, CancellationToken ct)
    {
        var entries = await db.WaitlistEntries
            .Include(e => e.Offers)
            .Where(e => e.Status == WaitlistEntryStatus.Waiting && e.Dates.Count > 0)
            .ToListAsync(ct);
        var expired = 0;
        foreach (var branch in entries.GroupBy(e => e.BranchId))
        {
            var zoneId = await tenants.GetBranchTimeZoneAsync(brandId, branch.Key, ct) ?? PlatformDefaults.DefaultTimeZoneId;
            var today = now.InZone(DateTimeZoneProviders.Tzdb[zoneId]).Date;
            expired += branch.Count(e => e.Expire(today, now).IsSuccess);
        }

        await db.SaveChangesAsync(ct);
        return expired;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Expired {Count} waitlist entries for brand {BrandId}")]
    private static partial void LogExpired(ILogger logger, Guid brandId, int count);
}
