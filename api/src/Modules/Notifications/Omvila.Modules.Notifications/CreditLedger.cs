using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications;

internal sealed class CreditLedger(NotificationsDbContext db, IClock clock, IIdGenerator ids) : ICreditLedger
{
    public async Task GrantAsync(Guid brandId, int credits, string referenceType, Guid? referenceId, CancellationToken ct)
    {
        var balance = await LockBalanceAsync(db, brandId, ct);
        db.CreditTransactions.Add(CreditTransaction.Grant(ids.New(), brandId, credits, balance, referenceType, referenceId, clock.GetCurrentInstant()));
        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Markanın defterini transaction sonuna kadar kilitler ve bakiyeyi döner: aynı anda iki yazım (gönderim işinin iki
    /// örneği, teslim raporu, satın alma) aynı bakiyeden hesaplanmaz. Bakiye satırların toplamıdır: aynı anda yazılan
    /// satırların CreatedUtc'si eşit olabilir ve UUID v7 aynı milisaniyede sıralı değildir, "son satır" güvenilmez.
    /// </summary>
    public static async Task<int> LockBalanceAsync(NotificationsDbContext db, Guid brandId, CancellationToken ct)
    {
        var lockKey = $"credits:{brandId}";
        await db.Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        return await db.CreditTransactions.AsNoTracking()
            .Where(t => t.BrandId == brandId)
            .SumAsync(t => t.Amount, ct);
    }
}
