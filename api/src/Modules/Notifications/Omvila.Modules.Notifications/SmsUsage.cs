using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications;

/// <summary>
/// Kontör tüketimi (sözleşme SmsOverview): "bu tempoyla ~18 gün yeter" son <see cref="PlatformDefaults.SmsUsageWindowDays"/>
/// günün net tüketimine (düşüm − iade) göre. Tüketim yoksa tahmin yoktur (null). Kiracı bağlamında çağrılır.
/// </summary>
internal sealed class SmsUsage(NotificationsDbContext db, IClock clock)
{
    public async Task<int?> EstimateDaysLeftAsync(Guid brandId, int balance, CancellationToken ct)
    {
        var from = clock.GetCurrentInstant() - Duration.FromDays(PlatformDefaults.SmsUsageWindowDays);
        var used = await NetConsumedAsync(brandId, from, null, ct);
        return used <= 0 ? null : (int)(Math.Max(0, balance) * (long)PlatformDefaults.SmsUsageWindowDays / used);
    }

    /// <summary>[<paramref name="from"/>, <paramref name="to"/>) aralığında düşülen kontör, iadeler çıkarılmış.</summary>
    public async Task<int> NetConsumedAsync(Guid brandId, Instant from, Instant? to, CancellationToken ct)
    {
        var rows = db.CreditTransactions.AsNoTracking()
            .Where(t => t.BrandId == brandId && t.CreatedUtc >= from
                && (t.Type == CreditTransactionType.Consume || t.Type == CreditTransactionType.Refund));
        if (to is { } end)
        {
            rows = rows.Where(t => t.CreatedUtc < end);
        }

        return Math.Max(0, -await rows.SumAsync(t => t.Amount, ct));
    }
}
