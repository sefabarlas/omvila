using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Platform.Jobs;

/// <summary>
/// AdvanceSubscriptionStatuses (durum-makinesi.md § 8; saatlik, idempotent). Platform bağlamında, OMVILA'nın faturalama gününe göre:
/// deneme bitişine TrialEndingNoticeDays kala owner.trial_ending (bir kez); deneme bitti veya ödenmiş dönem bitti, ödeme yok
/// → PastDue + dönem faturası (fiyat varsa) + owner.subscription_payment_failed (kart yok, T-06: havale bekleniyor);
/// PastDue + PastDueGraceDays → ReadOnly; ReadOnly + ReadOnlyToCancelledDays → Cancelled; Cancelled + CancelledArchiveDays
/// → marka Archived (Tenancy.Contracts <see cref="IBrandBilling"/>, ADR-0006 K4). Ödeme (havale eşleşmesi) durumu ayrıca
/// <c>IPaymentNoticeSettlement</c> ile değiştirir.
/// </summary>
internal sealed partial class AdvanceSubscriptionStatuses(
    PlatformDbContext db,
    SubscriptionBilling billing,
    IBrandBilling brands,
    IOutbox outbox,
    IClock clock,
    IOptions<PlatformOptions> options,
    ILogger<AdvanceSubscriptionStatuses> logger) : IPeriodicJob
{
    public string Key => "AdvanceSubscriptionStatuses";

    public async Task RunAsync(CancellationToken ct)
    {
        var settings = options.Value;
        var now = clock.GetCurrentInstant();
        var today = BillingCalendar.Today(now);
        var plans = await db.Plans.AsNoTracking().ToDictionaryAsync(p => p.Id, p => p.Name, ct);

        await NoticeTrialEndingAsync(today, now, settings, plans, ct);
        await MarkPastDueAsync(today, now, settings, plans, ct);
        await MarkReadOnlyAsync(today, settings, ct);
        await CancelAsync(today, now, settings, ct);
        await ArchiveAsync(today, now, settings, ct);
        await db.SaveChangesAsync(ct);
    }

    // Kural: bildirimler.md § E-posta owner.trial_ending ("Deneme bitmeden 3 gün önce")
    private async Task NoticeTrialEndingAsync(
        LocalDate today, Instant now, PlatformOptions settings, Dictionary<Guid, string> plans, CancellationToken ct)
    {
        var horizon = today.PlusDays(settings.TrialEndingNoticeDays);
        var due = await db.Subscriptions
            .Where(s => s.Status == SubscriptionStatus.Trialing && s.TrialEndingNotifiedUtc == null && s.TrialEndsOn <= horizon && s.TrialEndsOn > today)
            .ToListAsync(ct);
        foreach (var subscription in due.Where(s => s.IsDueForTrialEndingNotice(today, settings.TrialEndingNoticeDays)))
        {
            subscription.MarkTrialEndingNotified(now);
            await outbox.EnqueueAsync(
                new SubscriptionTrialEnding(
                    subscription.BrandId, subscription.TrialEndsOn!.Value, plans[subscription.PlanId], settings.TrialDays, settings.PastDueGraceDays),
                ct);
        }
    }

    // Kural: durum-makinesi.md § 8 (Trialing → PastDue, Active → PastDue); T-118 (fatura deneme bitişinde ve dönem başında)
    private async Task MarkPastDueAsync(
        LocalDate today, Instant now, PlatformOptions settings, Dictionary<Guid, string> plans, CancellationToken ct)
    {
        var due = await db.Subscriptions
            .Where(s => (s.Status == SubscriptionStatus.Trialing && s.TrialEndsOn <= today)
                || (s.Status == SubscriptionStatus.Active && s.CurrentPeriodEnd < today))
            .ToListAsync(ct);
        foreach (var subscription in due)
        {
            if (!subscription.MarkPastDue(today).IsSuccess)
            {
                continue;
            }

            var invoice = await billing.IssueForCurrentPeriodAsync(subscription, now, ct);
            var readOnlyOn = subscription.ReadOnlyOn(settings.PastDueGraceDays)!.Value;
            await outbox.EnqueueAsync(
                new SubscriptionPastDue(
                    subscription.BrandId, subscription.PastDueSince!.Value, readOnlyOn, subscription.CurrentPeriodStart, plans[subscription.PlanId]),
                ct);
            LogPastDue(logger, subscription.BrandId, invoice is not null);
        }
    }

    // Kural: durum-makinesi.md § 8 (PastDue → ReadOnly "7 gün geçti")
    private async Task MarkReadOnlyAsync(LocalDate today, PlatformOptions settings, CancellationToken ct)
    {
        var since = today.PlusDays(-settings.PastDueGraceDays);
        var due = await db.Subscriptions.Where(s => s.Status == SubscriptionStatus.PastDue && s.PastDueSince <= since).ToListAsync(ct);
        foreach (var subscription in due)
        {
            if (subscription.MarkReadOnly(today, settings.PastDueGraceDays).IsSuccess)
            {
                LogReadOnly(logger, subscription.BrandId);
            }
        }
    }

    // Kural: durum-makinesi.md § 8 (ReadOnly → Cancelled "30 gün"; T-118 ReadOnlyToCancelledDays)
    private async Task CancelAsync(LocalDate today, Instant now, PlatformOptions settings, CancellationToken ct)
    {
        var since = today.PlusDays(-settings.ReadOnlyToCancelledDays);
        var due = await db.Subscriptions.Where(s => s.Status == SubscriptionStatus.ReadOnly && s.ReadOnlySince <= since).ToListAsync(ct);
        foreach (var subscription in due.Where(s => s.IsReadOnlyExpired(today, settings.ReadOnlyToCancelledDays)))
        {
            subscription.Cancel(today, now);
        }
    }

    // Kural: durum-makinesi.md § 8 Cancelled ("30 gün sonra arşiv"; CancelledArchiveDays)
    private async Task ArchiveAsync(LocalDate today, Instant now, PlatformOptions settings, CancellationToken ct)
    {
        var since = today.PlusDays(-settings.CancelledArchiveDays);
        var due = await db.Subscriptions
            .Where(s => s.Status == SubscriptionStatus.Cancelled && s.BrandArchivedUtc == null && s.CancelledOn <= since)
            .ToListAsync(ct);
        foreach (var subscription in due.Where(s => s.IsDueForArchive(today, settings.CancelledArchiveDays)))
        {
            await brands.ArchiveAsync(subscription.BrandId, ct);
            subscription.MarkBrandArchived(now);
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscription of brand {BrandId} is past due (invoice issued: {InvoiceIssued})")]
    private static partial void LogPastDue(ILogger logger, Guid brandId, bool invoiceIssued);

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscription of brand {BrandId} is read-only")]
    private static partial void LogReadOnly(ILogger logger, Guid brandId);
}
