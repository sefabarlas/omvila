using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary>
/// <see cref="IPackageSessions"/>: seans defteri isteğin transaction'ında yazılır. Ayırmada ilgili müşteri paketi satırları
/// kimlik sırasıyla <c>FOR UPDATE</c> kilitlenir: aynı paketin son seansına eşzamanlı iki randevu yazılamaz, iade ile ayırma
/// sıraya girer.
/// </summary>
internal sealed class PackageSessions(SalesDbContext db, IIdGenerator ids, IClock clock) : IPackageSessions
{
    // Kural: durum-makinesi.md A1 ("paket seansı ayrılır"); alan-modeli.md § CustomerPackage
    public async Task<Result> ReserveAsync(PackageSessionReservation reservation, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        if (reservation.Claims.Count == 0)
        {
            return Result.Success();
        }

        var itemIds = reservation.Claims.Select(c => c.CustomerPackageItemId).Distinct().ToList();
        var items = await db.CustomerPackageItems
            .Where(i => i.BrandId == reservation.BrandId && itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, ct);
        var packageIds = items.Values.Select(i => i.CustomerPackageId).Distinct().ToArray();
        var packages = await LockAsync(packageIds, ct);
        var used = await ActiveUsageCountsAsync([.. items.Keys], ct);

        foreach (var claim in reservation.Claims)
        {
            if (!items.TryGetValue(claim.CustomerPackageItemId, out var item)
                || !packages.TryGetValue(item.CustomerPackageId, out var package)
                || package.BrandCustomerId != reservation.BrandCustomerId
                || !package.IsUsable(reservation.AppointmentDate, reservation.BranchId)
                || item.ServiceId != claim.ServiceId
                || item.Remaining(used.GetValueOrDefault(item.Id)) <= 0)
            {
                return Unavailable(claim.CustomerPackageItemId);
            }

            used[item.Id] = used.GetValueOrDefault(item.Id) + 1;
        }

        var now = clock.GetCurrentInstant();
        foreach (var claim in reservation.Claims)
        {
            db.PackageSessionUsages.Add(PackageSessionUsage.Use(
                ids, reservation.BrandId, claim.CustomerPackageItemId, reservation.AppointmentId, claim.AppointmentItemId, now));
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }

    // Kural: durum-makinesi.md A4–A7 ("seans geri")
    public async Task RevertAsync(Guid brandId, IReadOnlyCollection<Guid> appointmentIds, CancellationToken ct)
    {
        if (appointmentIds.Count == 0)
        {
            return;
        }

        var usages = await db.PackageSessionUsages
            .Where(u => u.BrandId == brandId && appointmentIds.Contains(u.AppointmentId) && u.RevertedUtc == null)
            .ToListAsync(ct);
        if (usages.Count == 0)
        {
            return;
        }

        var now = clock.GetCurrentInstant();
        foreach (var usage in usages)
        {
            usage.Revert(now);
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<UsablePackageItem>> ListUsableAsync(
        Guid brandId, Guid branchId, Guid brandCustomerId, LocalDate today, CancellationToken ct)
    {
        var packages = await db.CustomerPackages.AsNoTracking().Include(p => p.Items)
            .Where(p => p.BrandId == brandId && p.BrandCustomerId == brandCustomerId && p.Status == CustomerPackageStatus.Active && p.ExpiresOn >= today)
            .ToListAsync(ct);
        var usable = packages.Where(p => p.IsUsable(today, branchId)).ToList();
        var used = await ActiveUsageCountsAsync([.. usable.SelectMany(p => p.Items).Select(i => i.Id)], ct);
        return [.. usable
            .OrderBy(p => p.ExpiresOn).ThenBy(p => p.Id)
            .SelectMany(p => p.Items.OrderBy(i => i.SeqNo).Select(i => new UsablePackageItem(
                i.Id, i.ServiceId, p.PackageNameSnapshot, i.Remaining(used.GetValueOrDefault(i.Id)), i.TotalSessions, p.ExpiresOn)))
            .Where(i => i.RemainingSessions > 0)];
    }

    private async Task<Dictionary<Guid, CustomerPackage>> LockAsync(Guid[] packageIds, CancellationToken ct)
    {
        if (packageIds.Length == 0)
        {
            return [];
        }

        Array.Sort(packageIds);
        return await db.CustomerPackages
            .FromSql($"SELECT *, xmin FROM sales.customer_packages WHERE id = ANY({packageIds}) ORDER BY id FOR UPDATE")
            .ToDictionaryAsync(p => p.Id, ct);
    }

    private async Task<Dictionary<Guid, int>> ActiveUsageCountsAsync(IReadOnlyCollection<Guid> itemIds, CancellationToken ct)
    {
        if (itemIds.Count == 0)
        {
            return [];
        }

        return await db.PackageSessionUsages.AsNoTracking()
            .Where(u => itemIds.Contains(u.CustomerPackageItemId) && u.RevertedUtc == null)
            .GroupBy(u => u.CustomerPackageItemId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);
    }

    private static Error Unavailable(Guid customerPackageItemId) =>
        Error.Conflict(
            ErrorCodes.PackageSessionUnavailable,
            new Dictionary<string, object?>(StringComparer.Ordinal) { [IPackageSessions.CustomerPackageItemMeta] = customerPackageItemId });
}
