using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary>
/// Net tahsilat hareketleri (<see cref="ICollectionQueries"/> ve Raporlar): tahsilat kaydedildiği anda artı, iade işaretlendiği
/// anda eksi (W3, kasa ile aynı kural). İptal edilen (Voided) kayıt ve 0 TL'lik paketten düşüm sayılmaz; paket satışı ve kapora
/// dahildir (sözleşme ReportOverview.collections).
/// </summary>
internal sealed class CollectionLedger(SalesDbContext db) : ICollectionQueries
{
    public async Task<long> GetNetCollectedAsync(Guid brandId, Guid branchId, Instant fromUtc, Instant toUtc, CancellationToken ct) =>
        (await LoadAsync(db, brandId, [branchId], fromUtc, toUtc, ct)).Sum(m => m.AmountMinor);

    // Kural: sözleşme ReportOverview.collections ("paket satışları ve kaporalar dahil, iadeler düşülmüş"); docs/acik-sorular.md W3
    public static async Task<List<(Instant At, long AmountMinor)>> LoadAsync(
        SalesDbContext db, Guid brandId, IReadOnlyCollection<Guid>? branchIds, Instant fromUtc, Instant toUtc, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(db);
        var payments = db.Payments.AsNoTracking()
            .Where(p => p.BrandId == brandId && p.Status != PaymentStatus.Voided && p.Method != PaymentMethod.PackageRedemption);
        if (branchIds is not null)
        {
            var ids = branchIds.ToList();
            payments = payments.Where(p => ids.Contains(p.BranchId));
        }

        var paid = await payments.Where(p => p.PaidUtc >= fromUtc && p.PaidUtc < toUtc)
            .Select(p => new { At = p.PaidUtc, p.AmountMinor })
            .ToListAsync(ct);
        var refunded = await payments.Where(p => p.RefundedUtc != null && p.RefundedUtc >= fromUtc && p.RefundedUtc < toUtc)
            .Select(p => new { At = p.RefundedUtc!.Value, p.RefundedAmountMinor })
            .ToListAsync(ct);
        return [.. paid.Select(p => (p.At, p.AmountMinor)), .. refunded.Select(r => (r.At, -r.RefundedAmountMinor))];
    }
}
