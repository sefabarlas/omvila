using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>
/// Paket okuma: tanımlar ve üst kartlar (Paketler), müşteri paketi satırları (Müşteri paketleri, müşteri kartı). Adlar diğer
/// modüllerin sözleşmelerinden toplu okunur (hizmet: Catalog, personel: Tenancy, müşteri: Customers, randevu saati: Scheduling).
/// </summary>
internal sealed class PackageReader(
    SalesDbContext db, IServiceDirectory services, ICustomerDirectory customers, ITenantQueries tenants, IAppointmentQueries appointments)
{
    // Kural: sözleşme manageListPackages (satışta olanlar önce; sayılar şube kapsamına göre)
    public async Task<PackageListView> ListDefinitionsAsync(Guid brandId, PackageScope scope, LocalDate today, DateTimeZone zone, Currency currency, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(scope);
        var packages = await db.Packages.AsNoTracking().Include(p => p.Items)
            .Where(p => p.BrandId == brandId)
            .OrderByDescending(p => p.IsActive).ThenBy(p => p.Name).ThenBy(p => p.Id)
            .ToListAsync(ct);
        var sold = await scope.Apply(db.CustomerPackages.AsNoTracking().Where(p => p.BrandId == brandId))
            .Include(p => p.Items)
            .ToListAsync(ct);
        var names = await ServiceNamesAsync(brandId, packages.SelectMany(p => p.Items).Select(i => i.ServiceId), ct);
        var items = packages.Select(p => ToView(p, names, sold.Count(s => s.PackageId == p.Id), sold.Count(s => s.PackageId == p.Id && s.Status == CustomerPackageStatus.Active))).ToList();
        return new PackageListView(items, await OverviewAsync(sold, today, zone, currency, ct));
    }

    public async Task<PackageDefinitionView> MapDefinitionAsync(Package package, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(package);
        var names = await ServiceNamesAsync(package.BrandId, package.Items.Select(i => i.ServiceId), ct);
        var counts = await db.CustomerPackages.AsNoTracking()
            .Where(p => p.BrandId == package.BrandId && p.PackageId == package.Id)
            .GroupBy(p => p.Status)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);
        return ToView(package, names, counts.Sum(c => c.Count), counts.Where(c => c.Key == CustomerPackageStatus.Active).Sum(c => c.Count));
    }

    /// <summary>Müşteri paketi satırları; <paramref name="withUsages"/> yalnız müşteri kartında (kullanımlar yeniden eskiye).</summary>
    public async Task<IReadOnlyList<CustomerPackageView>> MapAsync(Guid brandId, IReadOnlyList<CustomerPackage> packages, bool withUsages, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(packages);
        if (packages.Count == 0)
        {
            return [];
        }

        var itemIds = packages.SelectMany(p => p.Items).Select(i => i.Id).ToList();
        var usages = await db.PackageSessionUsages.AsNoTracking().Where(u => itemIds.Contains(u.CustomerPackageItemId)).ToListAsync(ct);
        var active = usages.Where(u => u.RevertedUtc == null).GroupBy(u => u.CustomerPackageItemId).ToDictionary(g => g.Key, g => g.Count());
        var names = await ServiceNamesAsync(brandId, packages.SelectMany(p => p.Items).Select(i => i.ServiceId), ct);
        var cards = await customers.GetDisplayInfoAsync(brandId, [.. packages.Select(p => p.BrandCustomerId).Distinct()], ct);
        var staff = await tenants.GetStaffCardsAsync(brandId, [.. packages.Where(p => p.SoldByStaffId is not null).Select(p => p.SoldByStaffId!.Value).Distinct()], ct);
        var starts = withUsages
            ? await appointments.GetStartTimesAsync(brandId, [.. usages.Select(u => u.AppointmentId).Distinct()], ct)
            : new Dictionary<Guid, Instant>();
        return [.. packages.Select(p => ToView(p, cards, staff, names, active, withUsages ? usages : null, starts))];
    }

    private static CustomerPackageView ToView(
        CustomerPackage package,
        IReadOnlyDictionary<Guid, CustomerDisplayInfo> cards,
        IReadOnlyDictionary<Guid, StaffCard> staff,
        IReadOnlyDictionary<Guid, string> names,
        Dictionary<Guid, int> active,
        List<PackageSessionUsage>? usages,
        IReadOnlyDictionary<Guid, Instant> starts)
    {
        var items = package.Items.OrderBy(i => i.SeqNo)
            .Select(i => new CustomerPackageItemView(i.Id, i.ServiceId, names.GetValueOrDefault(i.ServiceId, string.Empty), i.TotalSessions, i.Remaining(active.GetValueOrDefault(i.Id))))
            .ToList();
        var card = cards.GetValueOrDefault(package.BrandCustomerId);
        var serviceByItem = package.Items.ToDictionary(i => i.Id, i => i.ServiceId);
        var usageViews = usages?
            .Where(u => serviceByItem.ContainsKey(u.CustomerPackageItemId))
            .OrderByDescending(u => u.UsedUtc).ThenByDescending(u => u.Id)
            .Select(u => new PackageSessionUsageView(
                u.AppointmentId,
                names.GetValueOrDefault(serviceByItem[u.CustomerPackageItemId], string.Empty),
                starts.TryGetValue(u.AppointmentId, out var start) ? start : u.UsedUtc,
                u.UsedUtc,
                u.RevertedUtc))
            .ToList();
        var seller = package.SoldByStaffId is { } staffId ? staff.GetValueOrDefault(staffId) : null;
        var currency = Enum.Parse<Currency>(package.CurrencyCode);
        return new CustomerPackageView(
            package.Id,
            new CustomerRefView(package.BrandCustomerId, card?.FullName ?? string.Empty, card?.PhoneMasked),
            package.PackageId,
            package.PackageNameSnapshot,
            package.BranchId,
            package.IsAvailableAllBranches,
            package.PricePaid,
            package.PurchasedUtc,
            package.ExpiresOn,
            package.SoldByStaffId,
            seller?.FullName,
            package.Status.ToString(),
            items,
            package.TotalSessions,
            items.Sum(i => i.RemainingSessions),
            package.RefundedAmountMinor is { } refunded ? new Money(refunded, currency) : null,
            package.RefundedUtc,
            usageViews,
            ETagVersion.Format(package.Version));
    }

    // Kural: sözleşme PackageOverview
    private async Task<PackageOverviewView> OverviewAsync(List<CustomerPackage> sold, LocalDate today, DateTimeZone zone, Currency currency, CancellationToken ct)
    {
        var itemIds = sold.Where(p => p.Status is CustomerPackageStatus.Active or CustomerPackageStatus.Expired)
            .SelectMany(p => p.Items).Select(i => i.Id).ToList();
        var used = itemIds.Count == 0
            ? []
            : await db.PackageSessionUsages.AsNoTracking()
                .Where(u => itemIds.Contains(u.CustomerPackageItemId) && u.RevertedUtc == null)
                .GroupBy(u => u.CustomerPackageItemId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count, ct);
        var active = sold.Where(p => p.Status == CustomerPackageStatus.Active).ToList();
        var monthStart = zone.AtStartOfDay(new LocalDate(today.Year, today.Month, 1)).ToInstant();
        var monthEnd = zone.AtStartOfDay(new LocalDate(today.Year, today.Month, 1).PlusMonths(1)).ToInstant();
        var thisMonth = sold.Where(p => p.PurchasedUtc >= monthStart && p.PurchasedUtc < monthEnd).ToList();
        var expired = sold.Where(p => p.Status == CustomerPackageStatus.Expired).ToList();
        var expiredTotal = expired.Sum(p => p.TotalSessions);
        var expiredUsed = expired.SelectMany(p => p.Items).Sum(i => Math.Min(i.TotalSessions, used.GetValueOrDefault(i.Id)));
        var soonLimit = today.PlusDays(PlatformDefaults.PackageExpiringSoonDays);
        return new PackageOverviewView(
            active.Count,
            active.SelectMany(p => p.Items).Sum(i => i.Remaining(used.GetValueOrDefault(i.Id))),
            thisMonth.Count,
            new Money(thisMonth.Sum(p => p.PricePaidMinor), currency),
            active.Count(p => p.ExpiresOn >= today && p.ExpiresOn <= soonLimit),
            expiredTotal == 0 ? null : Math.Round(expiredUsed * 100m / expiredTotal, 1, MidpointRounding.AwayFromZero));
    }

    private static PackageDefinitionView ToView(Package package, IReadOnlyDictionary<Guid, string> names, int soldCount, int activeCount) =>
        new(
            package.Id,
            package.Name,
            package.Price,
            [.. package.Items.OrderBy(i => i.SeqNo).Select(i => new PackageDefinitionItemView(i.ServiceId, names.GetValueOrDefault(i.ServiceId, string.Empty), i.SessionCount))],
            package.TotalSessions,
            package.ValidityDays,
            package.SaleCommissionPercentOverride,
            package.IsAvailableAllBranches,
            package.IsActive,
            soldCount,
            activeCount,
            ETagVersion.Format(package.Version));

    private async Task<IReadOnlyDictionary<Guid, string>> ServiceNamesAsync(Guid brandId, IEnumerable<Guid> serviceIds, CancellationToken ct) =>
        (await services.GetSummariesAsync(brandId, [.. serviceIds.Distinct()], ct)).ToDictionary(x => x.Key, x => x.Value.Name);
}
