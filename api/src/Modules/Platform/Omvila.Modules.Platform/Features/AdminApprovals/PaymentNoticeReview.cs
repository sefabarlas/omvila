using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Features.PaymentNotices;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;
using static Omvila.Modules.Platform.Features.AdminApprovals.AdminApprovalViews;

namespace Omvila.Modules.Platform.Features.AdminApprovals;

/// <summary>
/// Havale bildirimi incelemesi (sözleşme adminListApprovalQueue, adminGetPaymentNotice, adminMatchPaymentNotice,
/// adminMarkPaymentNoticeNotFound, adminRejectPaymentNotice; dilim-06 KK2, KK3; T-116 (3)). Adaylar eşleşmemiş banka hareketleridir
/// (<see cref="TransferMatch"/>); eşleştirme sonucu <see cref="IPaymentNoticeSettlement"/>'tadır. Platform bağlamında çalışır.
/// </summary>
internal sealed class PaymentNoticeReview(
    PlatformDbContext db,
    IPaymentNoticeSettlement settlement,
    IPlatformBrandDirectory brands,
    IUserNameDirectory users,
    IAuditLog audit,
    IOutbox outbox,
    IClock clock)
{
    private const string Entity = nameof(PaymentNotice);

    private static DateTimeZone Zone => DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];

    /// <summary>Bekleyen bildirimler (en eskiden yeniye) ve her birinin en iyi adayı.</summary>
    public async Task<IReadOnlyList<AdminApprovalQueueItem>> QueueAsync(CancellationToken ct)
    {
        var notices = await db.PaymentNotices.AsNoTracking()
            .Where(n => n.Status == PaymentNoticeStatus.Pending)
            .OrderBy(n => n.NotifiedUtc).ThenBy(n => n.Id)
            .ToListAsync(ct);
        if (notices.Count == 0)
        {
            return [];
        }

        var names = (await brands.ListAsync(ct)).ToDictionary(b => b.Id, b => b.Name);
        var from = TransferMatch.Window(Day(notices[0].NotifiedUtc)).From;
        var to = TransferMatch.Window(Day(notices[^1].NotifiedUtc)).To;
        var pool = await UnmatchedAsync(from, to, ct);
        return [.. notices.Select(n =>
        {
            var best = Candidates(n, pool).FirstOrDefault();
            return new AdminApprovalQueueItem(
                "PaymentNotice",
                n.Id,
                n.BrandId,
                names.GetValueOrDefault(n.BrandId) ?? string.Empty,
                n.NotifiedUtc,
                new QueuedPaymentNotice(
                    n.Purpose, n.PeriodStart, n.Credits, Amount(n.ExpectedAmountMinor), n.TransferCode, n.SenderName, n.ReceiptFileKey is not null,
                    ETagVersion.Format(n.Version),
                    best is null ? null : new QueuedCandidate(best.BankTransactionId, best.Verdict, best.CodeFound, best.AmountMatches)),
                null);
        })];
    }

    /// <summary>Ayrıntı ve adaylar; bildirim yoksa null.</summary>
    public async Task<AdminPaymentNoticeDetail?> GetAsync(Guid noticeId, CancellationToken ct)
    {
        var notice = await db.PaymentNotices.AsNoTracking().SingleOrDefaultAsync(n => n.Id == noticeId, ct);
        return notice is null ? null : await DetailAsync(notice, ct);
    }

    /// <summary>Bildirim (dekont indirme için); yoksa null.</summary>
    public Task<PaymentNotice?> FindAsync(Guid noticeId, CancellationToken ct) =>
        db.PaymentNotices.AsNoTracking().SingleOrDefaultAsync(n => n.Id == noticeId, ct);

    // Kural: sözleşme adminMatchPaymentNotice — hareket başka bildirime bağlıysa 409 bank_transaction_already_matched; tutar uyuşmasa da
    // eşleştirilebilir; sonuç IPaymentNoticeSettlement (fatura Paid, abonelik Active veya kontör, denetim, owner.payment_notice_matched).
    public async Task<Result<AdminPaymentNoticeDetail>> MatchAsync(Guid noticeId, Guid bankTransactionId, uint expectedVersion, Guid userId, CancellationToken ct)
    {
        var loaded = await LoadPendingAsync(noticeId, expectedVersion, ct);
        if (!loaded.IsSuccess)
        {
            return loaded.Error;
        }

        if (!await db.BankTransactions.AnyAsync(t => t.Id == bankTransactionId, ct))
        {
            return Error.Validation("bankTransactionId", "invalid_format");
        }

        // Koşullu güncelleme satırı kilitler: eşzamanlı ikinci eşleştirme ilki bitince 0 satır görür.
        var claimed = await db.BankTransactions
            .Where(t => t.Id == bankTransactionId && t.MatchedNoticeId == null)
            .ExecuteUpdateAsync(set => set.SetProperty(t => t.MatchedNoticeId, noticeId), ct);
        if (claimed == 0)
        {
            return Error.Conflict(ErrorCodes.BankTransactionAlreadyMatched);
        }

        var settled = await settlement.MatchAsync(noticeId, bankTransactionId, userId, ct);
        if (!settled.IsSuccess)
        {
            return settled.Error;
        }

        return await DetailAsync(loaded.Value, ct);
    }

    // Kural: durum-makinesi.md § 10 PaymentNotice (Pending → NotFound / Rejected); sözleşme: abonelik durumu değişmez, markaya bildirim.
    public async Task<Result<AdminPaymentNoticeDetail>> ResolveAsync(
        Guid noticeId, PaymentNoticeStatus outcome, string? message, uint expectedVersion, AuditEntry auditTemplate, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(auditTemplate);
        var loaded = await LoadPendingAsync(noticeId, expectedVersion, ct);
        if (!loaded.IsSuccess)
        {
            return loaded.Error;
        }

        var notice = loaded.Value;
        var userId = auditTemplate.ActorId ?? Guid.Empty;
        var now = clock.GetCurrentInstant();
        var resolved = outcome == PaymentNoticeStatus.Rejected ? notice.Reject(userId, now, message) : notice.MarkNotFound(userId, now, message);
        if (!resolved.IsSuccess)
        {
            return resolved.Error;
        }

        await db.SaveChangesAsync(ct);
        var changes = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["status"] = notice.Status.ToString(),
            ["purpose"] = notice.Purpose.ToString(),
            ["transferCode"] = notice.TransferCode,
        };
        await audit.WriteAsync(auditTemplate with { BrandId = notice.BrandId, EntityId = notice.Id, Changes = changes }, ct);
        var purpose = notice.Purpose.ToString();
        if (outcome == PaymentNoticeStatus.Rejected)
        {
            await outbox.EnqueueAsync(new PaymentNoticeRejected(notice.BrandId, notice.Id, purpose, notice.ResolutionMessage ?? string.Empty), ct);
        }
        else
        {
            await outbox.EnqueueAsync(new PaymentNoticeNotFound(notice.BrandId, notice.Id, purpose, notice.ResolutionMessage), ct);
        }

        return await DetailAsync(notice, ct);
    }

    private async Task<Result<PaymentNotice>> LoadPendingAsync(Guid noticeId, uint expectedVersion, CancellationToken ct)
    {
        var notice = await db.PaymentNotices.SingleOrDefaultAsync(n => n.Id == noticeId, ct);
        if (notice is null)
        {
            return Error.NotFound();
        }

        if (notice.Version != expectedVersion)
        {
            return Error.PreconditionFailed();
        }

        if (notice.Status != PaymentNoticeStatus.Pending)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        db.Entry(notice).Property(n => n.Version).OriginalValue = expectedVersion;
        return notice;
    }

    private async Task<AdminPaymentNoticeDetail> DetailAsync(PaymentNotice notice, CancellationToken ct)
    {
        // Yazımdan sonra sürüm (xmin) veritabanından okunur.
        var version = await db.PaymentNotices.AsNoTracking().Where(n => n.Id == notice.Id).Select(n => n.Version).SingleAsync(ct);
        var brand = await brands.GetAsync(notice.BrandId, ct);
        var window = TransferMatch.Window(Day(notice.NotifiedUtc));
        var candidates = notice.Status == PaymentNoticeStatus.Pending ? Candidates(notice, await UnmatchedAsync(window.From, window.To, ct)) : [];
        string? planName = null;
        LocalDate? periodEnd = null;
        if (notice.Purpose == PaymentNoticePurpose.Subscription)
        {
            var plan = await (from s in db.Subscriptions.AsNoTracking()
                              join p in db.Plans.AsNoTracking() on s.PlanId equals p.Id
                              where s.BrandId == notice.BrandId
                              select new { p.Name, s.BillingCycle }).SingleOrDefaultAsync(ct);
            planName = plan?.Name;
            periodEnd = plan is not null && notice.PeriodStart is { } start ? BillingCalendar.PeriodEnd(start, plan.BillingCycle) : null;
        }

        return new AdminPaymentNoticeDetail(
            notice.Id,
            notice.BrandId,
            brand?.Brand.Name ?? string.Empty,
            notice.Purpose,
            planName,
            notice.PeriodStart,
            periodEnd,
            notice.Credits,
            Amount(notice.ExpectedAmountMinor),
            notice.TransferCode,
            notice.SenderName,
            notice.NotifiedUtc,
            notice.Channel,
            notice.Status,
            Receipt(notice),
            await ResolutionAsync(notice, ct),
            new CandidateWindow(window.From, window.To),
            candidates,
            ETagVersion.Format(version));
    }

    private async Task<ResolutionInfo?> ResolutionAsync(PaymentNotice notice, CancellationToken ct)
    {
        if (notice.ResolvedUtc is not { } resolvedUtc)
        {
            return null;
        }

        var name = notice.ResolvedByUserId is { } by && (await users.GetAsync([by], ct)).TryGetValue(by, out var info) ? info.FullName : string.Empty;
        return new ResolutionInfo(resolvedUtc, name, notice.MatchedBankTransactionId, notice.ResolutionMessage);
    }

    private static ReceiptInfo? Receipt(PaymentNotice notice) =>
        notice.ReceiptFileKey is null
            ? null
            : new ReceiptInfo(
                notice.ReceiptFileName ?? $"dekont{Path.GetExtension(notice.ReceiptFileKey)}",
                notice.ReceiptContentType ?? "application/pdf",
                notice.ReceiptSizeBytes ?? 0,
                notice.NotifiedUtc);

    // Kural: sözleşme adminGetPaymentNotice — sıralama: kodu bulunan, sonra tutarı tutan, sonra tarihe yakın olan.
    private static List<BankTransactionCandidate> Candidates(PaymentNotice notice, IReadOnlyList<PlatformBankTransaction> pool)
    {
        var day = Day(notice.NotifiedUtc);
        var (from, to) = TransferMatch.Window(day);
        return [.. pool
            .Where(t => t.ValueDate >= from && t.ValueDate <= to)
            .Select(t => (Tx: t, Match: TransferMatch.Compare(notice.TransferCode, notice.ExpectedAmountMinor, t.Description, t.AmountMinor)))
            .OrderByDescending(c => c.Match.CodeFound)
            .ThenByDescending(c => c.Match.AmountMatches)
            .ThenBy(c => Math.Abs(Period.DaysBetween(day, c.Tx.ValueDate)))
            .ThenBy(c => c.Tx.ReceivedUtc ?? Instant.MaxValue)
            .ThenBy(c => c.Tx.Id)
            .Select(c => new BankTransactionCandidate(
                c.Tx.Id, c.Tx.ValueDate, c.Tx.ReceivedUtc, Money.Try(c.Tx.AmountMinor), c.Tx.SenderName, c.Tx.Description, c.Match.CodeFound,
                c.Match.AmountMatches, c.Match.Verdict))];
    }

    private async Task<IReadOnlyList<PlatformBankTransaction>> UnmatchedAsync(LocalDate from, LocalDate to, CancellationToken ct) =>
        await db.BankTransactions.AsNoTracking()
            .Where(t => t.MatchedNoticeId == null && t.ValueDate >= from && t.ValueDate <= to)
            .ToListAsync(ct);

    private static Money? Amount(long? minor) => minor is { } m ? Money.Try(m) : null;

    private static LocalDate Day(Instant instant) => instant.InZone(Zone).Date;
}
