using Microsoft.EntityFrameworkCore;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary>Müşteri paketi kalemlerinin seans payı (ödenen ÷ paketin toplam seansı; W2), toplu iki sorguda.</summary>
internal static class PackageSessionShares
{
    public static async Task<Dictionary<Guid, long>> LoadAsync(SalesDbContext db, IReadOnlyCollection<Guid> customerPackageItemIds, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(customerPackageItemIds);
        if (customerPackageItemIds.Count == 0)
        {
            return [];
        }

        var ids = customerPackageItemIds.Distinct().ToList();
        var packageIds = await db.CustomerPackageItems.AsNoTracking()
            .Where(i => ids.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, i => i.CustomerPackageId, ct);
        var distinct = packageIds.Values.Distinct().ToList();
        var packages = await db.CustomerPackages.AsNoTracking().Include(p => p.Items)
            .Where(p => distinct.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, ct);
        return packageIds.ToDictionary(x => x.Key, x => packages[x.Value].SessionShareMinor);
    }
}
