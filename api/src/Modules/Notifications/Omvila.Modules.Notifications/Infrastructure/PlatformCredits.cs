using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Sms;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Infrastructure;

/// <summary>
/// <see cref="IPlatformCredits"/>: platform servisi (ADR-0006, K4). Admin isteği platform bağlamındadır; başka bağlamda çağrı
/// programlama hatasıdır.
/// </summary>
internal sealed class PlatformCredits(
    NotificationsDbContext db, ITenantContext tenant, IClock clock, IIdGenerator ids, IOptions<SmsOptions> sms) : IPlatformCredits
{
    public async Task<IReadOnlyDictionary<Guid, int>> GetBalancesAsync(CancellationToken ct)
    {
        RequirePlatformScope();
        return await db.CreditTransactions.AsNoTracking()
            .GroupBy(t => t.BrandId)
            .Select(g => new { BrandId = g.Key, Balance = g.Sum(t => t.Amount) })
            .ToDictionaryAsync(g => g.BrandId, g => g.Balance, ct);
    }

    public async Task<IReadOnlyList<PlatformCreditEntry>> ListAsync(
        Guid brandId, int limit, PlatformCreditCursor? before, Instant? since, CancellationToken ct)
    {
        RequirePlatformScope();
        var take = limit + 1;
        var timeZone = PlatformDefaults.DefaultTimeZoneId;
        var beforeUtc = before?.OccurredUtc ?? Instant.FromUtc(9999, 1, 1, 0, 0);
        var beforeId = before?.Id ?? Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var sinceUtc = since ?? Instant.FromUtc(2000, 1, 1, 0, 0);
        // Kural: sözleşme adminListBrandCreditTransactions — gönderim düşümleri (Consume) gün başına tek satır; diğerleri tek tek.
        // Sütun adları snake_case: EF adlandırma kuralı GroupedRow özelliklerini böyle eşler.
        var rows = await db.Database.SqlQuery<GroupedRow>($"""
            SELECT g.id, g.occurred_utc, g.type, g.reference_type, g.credits, g.balance_after
            FROM (
              SELECT (array_agg(t.id ORDER BY t.created_utc, t.id))[1] AS id,
                     max(t.created_utc) AS occurred_utc,
                     t.type AS type,
                     (array_agg(t.reference_type ORDER BY t.created_utc, t.id))[1] AS reference_type,
                     sum(t.amount)::int AS credits,
                     (array_agg(t.balance_after ORDER BY t.created_utc DESC, t.id DESC))[1] AS balance_after
              FROM notifications.credit_transactions t
              WHERE t.brand_id = {brandId} AND t.created_utc >= {sinceUtc}
              GROUP BY t.type, CASE WHEN t.type = 'Consume' THEN to_char(t.created_utc AT TIME ZONE {timeZone}, 'YYYY-MM-DD') ELSE t.id::text END
            ) g
            WHERE (g.occurred_utc, g.id) < ({beforeUtc}, {beforeId})
            ORDER BY g.occurred_utc DESC, g.id DESC
            LIMIT {take}
            """).ToListAsync(ct);
        return [.. rows.Select(r => new PlatformCreditEntry(r.Id, r.OccurredUtc, ContractType(r.Type, r.ReferenceType), r.Credits, r.BalanceAfter))];
    }

    public async Task<int> GetConsumedSinceAsync(Guid brandId, Instant since, CancellationToken ct)
    {
        RequirePlatformScope();
        return -await db.CreditTransactions.AsNoTracking()
            .Where(t => t.BrandId == brandId && t.Type == CreditTransactionType.Consume && t.CreatedUtc >= since)
            .SumAsync(t => t.Amount, ct);
    }

    public async Task<Result<PlatformCreditEntry>> GrantAsync(Guid brandId, int credits, CancellationToken ct)
    {
        RequirePlatformScope();
        var balance = await CreditLedger.LockBalanceAsync(db, brandId, ct);
        if (balance + credits < 0)
        {
            return Error.Conflict(ErrorCodes.InsufficientCredits);
        }

        var entry = CreditTransaction.PlatformGrant(ids.New(), brandId, credits, balance, clock.GetCurrentInstant());
        db.CreditTransactions.Add(entry);
        await db.SaveChangesAsync(ct);
        return new PlatformCreditEntry(entry.Id, entry.CreatedUtc, nameof(CreditTransactionType.Grant), entry.Amount, entry.BalanceAfter);
    }

    public async Task<PlatformSmsSenderInfo> GetSmsSenderAsync(Guid brandId, CancellationToken ct)
    {
        RequirePlatformScope();
        var defaultSender = sms.Value.DefaultSender;
        var sender = await db.SmsSenders.AsNoTracking().SingleOrDefaultAsync(s => s.BrandId == brandId, ct);
        return sender is null
            ? new PlatformSmsSenderInfo(null, nameof(SmsSenderStatus.None), defaultSender, null, null, null)
            : new PlatformSmsSenderInfo(
                sender.Name, sender.Status.ToString(), BrandSmsSender.EffectiveName(sender, defaultSender), sender.SubmittedUtc, sender.DecidedUtc,
                sender.RejectionReason);
    }

    public Task<int> CountPendingSmsSendersAsync(CancellationToken ct)
    {
        RequirePlatformScope();
        return db.SmsSenders.AsNoTracking().CountAsync(s => s.Status == SmsSenderStatus.Pending, ct);
    }

    // Sözleşme CreditTransactionType: Consume → Usage, deneme hediyesi (Subscription kaynaklı Grant) → TrialGift; süresi dolan
    // kontör (Expire, v1'de yazılmıyor) tüketim gibi gösterilir.
    private static string ContractType(string type, string? referenceType) => type switch
    {
        nameof(CreditTransactionType.Consume) or nameof(CreditTransactionType.Expire) => "Usage",
        nameof(CreditTransactionType.Grant) when string.Equals(referenceType, CreditTransaction.TrialGiftReferenceType, StringComparison.Ordinal) => "TrialGift",
        _ => type,
    };

    private void RequirePlatformScope()
    {
        if (!tenant.Scope.IsPlatform)
        {
            throw new DomainException("Platform servisi yalnız platform bağlamında çağrılır (ADR-0006).");
        }
    }

    private sealed class GroupedRow
    {
        public Guid Id { get; init; }

        public Instant OccurredUtc { get; init; }

        public string Type { get; init; } = string.Empty;

        public string? ReferenceType { get; init; }

        public int Credits { get; init; }

        public int BalanceAfter { get; init; }
    }
}
