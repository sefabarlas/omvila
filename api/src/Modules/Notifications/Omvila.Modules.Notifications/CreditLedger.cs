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
        // Markanın defteri işlem sonuna kadar kilitlenir: aynı anda iki yazım aynı bakiyeden hesaplanmaz.
        var lockKey = $"credits:{brandId}";
        await db.Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", ct);
        var balance = await db.CreditTransactions.AsNoTracking()
            .Where(t => t.BrandId == brandId)
            .OrderByDescending(t => t.CreatedUtc).ThenByDescending(t => t.Id)
            .Select(t => (int?)t.BalanceAfter)
            .FirstOrDefaultAsync(ct) ?? 0;
        db.CreditTransactions.Add(CreditTransaction.Grant(ids.New(), brandId, credits, balance, referenceType, referenceId, clock.GetCurrentInstant()));
        await db.SaveChangesAsync(ct);
    }
}
