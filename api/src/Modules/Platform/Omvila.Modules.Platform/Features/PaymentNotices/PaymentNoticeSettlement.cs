using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.PaymentNotices;

internal sealed class PaymentNoticeSettlement(
    PlatformDbContext db,
    IDbSession session,
    SubscriptionBilling billing,
    ICreditLedger credits,
    IAuditLog audit,
    IOutbox outbox,
    IClock clock) : IPaymentNoticeSettlement
{
    // Kural: dilim-06 kabul kriteri 2; alan-modeli.md § PaymentNotice "Eşleştirme sonucu"; durum-makinesi.md § 8 (PastDue/ReadOnly → Active)
    public async Task<Result<PaymentNoticeSettled>> MatchAsync(
        Guid paymentNoticeId, Guid bankTransactionId, Guid platformUserId, CancellationToken ct)
    {
        var notice = await db.PaymentNotices.SingleOrDefaultAsync(n => n.Id == paymentNoticeId, ct);
        if (notice is null)
        {
            return Error.NotFound();
        }

        // Yazımlar markanın bağlamında: defter ve denetim kaydı markaya aittir.
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.ForBrand(notice.BrandId), ct);
        try
        {
            var now = clock.GetCurrentInstant();
            var matched = notice.Match(bankTransactionId, platformUserId, now);
            if (!matched.IsSuccess)
            {
                return matched.Error;
            }

            Guid? invoiceId = null;
            int? creditsAdded = null;
            if (notice.Purpose == PaymentNoticePurpose.Subscription)
            {
                var paid = await PaySubscriptionAsync(notice, now, ct);
                if (!paid.IsSuccess)
                {
                    return paid.Error;
                }

                invoiceId = paid.Value;
            }
            else
            {
                creditsAdded = notice.Credits!.Value;
                await credits.PurchaseAsync(notice.BrandId, creditsAdded.Value, notice.ExpectedAmountMinor, nameof(PaymentNotice), notice.Id, ct);
            }

            await db.SaveChangesAsync(ct);
            await WriteAuditAsync(notice, bankTransactionId, platformUserId, invoiceId, creditsAdded, ct);
            await outbox.EnqueueAsync(new PaymentNoticeMatched(notice.BrandId, notice.Id, notice.Purpose.ToString(), creditsAdded), ct);
            return new PaymentNoticeSettled(notice.Id, notice.BrandId, invoiceId, creditsAdded);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }

    private Task WriteAuditAsync(
        PaymentNotice notice, Guid bankTransactionId, Guid platformUserId, Guid? invoiceId, int? creditsAdded, CancellationToken ct) =>
        audit.WriteAsync(
            new AuditEntry(
                notice.BrandId,
                AuditActorType.PlatformUser,
                platformUserId,
                AuditAction.Match,
                nameof(PaymentNotice),
                notice.Id,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["purpose"] = notice.Purpose.ToString(),
                    ["bankTransactionId"] = bankTransactionId,
                    ["invoiceId"] = invoiceId,
                    ["credits"] = creditsAdded,
                }),
            ct);

    /// <summary>
    /// Ödenen dönem: denemede deneme bitişinden başlayan ilk dönem, diğer durumlarda geçerli (ödenmemiş) dönem. Dönemin faturası
    /// yoksa ve fiyat tanımlıysa kesilir; Issued/Failed fatura Paid olur.
    /// </summary>
    private async Task<Result<Guid?>> PaySubscriptionAsync(PaymentNotice notice, Instant now, CancellationToken ct)
    {
        var subscription = await db.Subscriptions.SingleAsync(s => s.BrandId == notice.BrandId, ct);
        var periodStart = subscription.Status == SubscriptionStatus.Trialing ? subscription.NextPeriod().Start : subscription.CurrentPeriodStart;
        var recorded = subscription.RecordPayment(periodStart, SubscriptionPaymentMethod.BankTransfer);
        if (!recorded.IsSuccess)
        {
            return recorded.Error;
        }

        var invoice = await db.Invoices.SingleOrDefaultAsync(
                i => i.SubscriptionId == subscription.Id && i.PeriodStart == subscription.CurrentPeriodStart, ct)
            ?? await billing.IssueForCurrentPeriodAsync(subscription, now, ct);
        if (invoice is null)
        {
            return (Guid?)null;
        }

        if (invoice.Status != SubscriptionInvoiceStatus.Paid)
        {
            var paid = invoice.MarkPaid(SubscriptionPaymentMethod.BankTransfer, now);
            if (!paid.IsSuccess)
            {
                return paid.Error;
            }
        }

        return invoice.Id;
    }
}
