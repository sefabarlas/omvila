using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary>
/// Müşteri birleştirme ve KVKK silmesinde paketlerin payı (T-92, T-95): birleştirmede müşteri paketleri (ve devir bağı) hedef
/// karta taşınır, seans defteri paket kalemine bağlı olduğundan kendiliğinden gelir. Silme önizlemesinde aktif paketler kalan
/// seanslarıyla listelenir ("Aktif paket varsa arayüz önce uyarır"); anonimleştirmede paket kayıtları anonim karta bağlı kalır.
/// </summary>
internal sealed class CustomerPackagesParticipant(SalesDbContext db) : ICustomerMergeParticipant, ICustomerErasureParticipant
{
    // Kural: alan-modeli.md § Birleştirme ("randevular, paketler, ödemeler ... hedefe taşınır")
    public async Task MergeAsync(CustomerMerge merge, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(merge);
        await db.CustomerPackages
            .Where(p => p.BrandId == merge.BrandId && p.BrandCustomerId == merge.SourceBrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.BrandCustomerId, merge.TargetBrandCustomerId), ct);
        await db.CustomerPackages
            .Where(p => p.BrandId == merge.BrandId && p.TransferredToBrandCustomerId == merge.SourceBrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.TransferredToBrandCustomerId, merge.TargetBrandCustomerId), ct);
    }

    // Kural: alan-modeli.md § Anonimleştirme ("Aktif paket varsa arayüz önce uyarır"); sözleşme CustomerDeletionPreview.activePackages
    public async Task<CustomerErasureFootprint> GetFootprintAsync(Guid brandId, Guid brandCustomerId, Instant now, CancellationToken ct)
    {
        var packages = await db.CustomerPackages.AsNoTracking().Include(p => p.Items)
            .Where(p => p.BrandId == brandId && p.BrandCustomerId == brandCustomerId && p.Status == CustomerPackageStatus.Active)
            .OrderBy(p => p.ExpiresOn).ThenBy(p => p.Id)
            .ToListAsync(ct);
        if (packages.Count == 0)
        {
            return CustomerErasureFootprint.None;
        }

        var itemIds = packages.SelectMany(p => p.Items).Select(i => i.Id).ToList();
        var used = await db.PackageSessionUsages.AsNoTracking()
            .Where(u => itemIds.Contains(u.CustomerPackageItemId) && u.RevertedUtc == null)
            .GroupBy(u => u.CustomerPackageItemId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);
        return new CustomerErasureFootprint(ActivePackages: [.. packages.Select(p => new ActivePackageInfo(
            p.Id, p.PackageNameSnapshot, p.Items.Sum(i => i.Remaining(used.GetValueOrDefault(i.Id)))))]);
    }

    // Paket ve seans kayıtları karta bağlıdır; kart anonimleşince müşteri bağı da anonim olur (işlem kaydı silinmez).
    public Task EraseAsync(CustomerErasure erasure, CancellationToken ct) => Task.CompletedTask;
}
