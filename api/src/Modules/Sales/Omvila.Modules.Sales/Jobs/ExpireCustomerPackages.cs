using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Features.Packages;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Sales.Jobs;

/// <summary>
/// ExpireCustomerPackages (durum-makinesi.md § 5: Active → Expired, ExpiresOn geçti; günlük iş). Son kullanım günü satıldığı
/// şubenin yerel takvimine göredir: yerel bugün &gt; ExpiresOn ise paket Expired olur. Adaylar platform kapsamında UTC gününe göre
/// geniş bulunur (en erken saat dilimi bile bugünden bir gün ileride değildir), her marka kendi kiracı bağlamında şube saatiyle
/// kesinleştirilir. Ayrılmış ama randevusu son günden sonra kalan seans (ör. randevu ertelendi) dokunulmaz: o randevu paketten
/// düşülmüş olarak yapılır; iptal olursa seans geri döner ama süresi geçmiş paket yeniden kullanılamaz (docs/acik-sorular.md T-95).
/// </summary>
internal sealed partial class ExpireCustomerPackages(
    SalesDbContext db,
    IDbSession session,
    ITenantQueries tenants,
    IClock clock,
    ILogger<ExpireCustomerPackages> logger) : IPeriodicJob
{
    public string Key => "ExpireCustomerPackages";

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var utcToday = now.InUtc().Date;
        var brandIds = await db.CustomerPackages.AsNoTracking()
            .Where(p => p.Status == CustomerPackageStatus.Active && p.ExpiresOn <= utcToday)
            .Select(p => p.BrandId)
            .Distinct()
            .ToListAsync(ct);

        foreach (var brandId in brandIds)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
            var expired = await ExpireBrandAsync(brandId, utcToday, now, ct);
            if (expired > 0)
            {
                LogExpired(logger, brandId, expired);
            }
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
    }

    // Kural: durum-makinesi.md § 5
    private async Task<int> ExpireBrandAsync(Guid brandId, LocalDate utcToday, Instant now, CancellationToken ct)
    {
        var due = await db.CustomerPackages
            .Where(p => p.BrandId == brandId && p.Status == CustomerPackageStatus.Active && p.ExpiresOn <= utcToday)
            .ToListAsync(ct);
        var todayByBranch = new Dictionary<Guid, LocalDate>();
        var expired = 0;
        foreach (var package in due)
        {
            if (!todayByBranch.TryGetValue(package.BranchId, out var today))
            {
                today = now.InZone(await BranchCalendar.ZoneAsync(tenants, brandId, package.BranchId, ct)).Date;
                todayByBranch[package.BranchId] = today;
            }

            if (package.Expire(today, now))
            {
                expired++;
            }
        }

        await db.SaveChangesAsync(ct);
        return expired;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Expired {Count} customer packages for brand {BrandId}")]
    private static partial void LogExpired(ILogger logger, Guid brandId, int count);
}
