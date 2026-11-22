using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Infrastructure;

/// <summary>
/// Dönem tutarı ve fatura kesimi (T-118). Tutar plan fiyatı + ek şube ücretidir; fiyat tanımlı değilse (S-14) tutar yoktur ve
/// fatura kesilmez (abonelik durumları yine ilerler). Markanın kiracı bağlamında veya platform bağlamında çağrılır.
/// </summary>
internal sealed class SubscriptionBilling(
    PlatformDbContext db, InvoiceNumberAllocator numbers, IBrandBilling brands, IIdGenerator ids, IOptions<PlatformOptions> options)
{
    /// <summary>Bir dönemin tutarı (etkin şube sayısıyla); fiyat yoksa null.</summary>
    public async Task<InvoiceAmount?> AmountAsync(Subscription subscription, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        var plan = await db.Plans.AsNoTracking().SingleAsync(p => p.Id == subscription.PlanId, ct);
        var profile = await brands.GetProfileAsync(subscription.BrandId, ct);
        return InvoiceAmount.Calculate(plan, subscription.BillingCycle, profile?.ActiveBranchCount ?? subscription.BranchCount, options.Value.VatRatePercent);
    }

    /// <summary>
    /// Aboneliğin geçerli dönemi için fatura keser (vade = dönem başı). Dönem zaten faturalandıysa veya fiyat tanımlı değilse null.
    /// Faturalanan şube ve personel sayısı abonelikte güncellenir.
    /// </summary>
    // Kural: T-118 (deneme bitişinde ilk fatura, sonra her dönem başında)
    public async Task<SubscriptionInvoice?> IssueForCurrentPeriodAsync(Subscription subscription, Instant now, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        if (await db.Invoices.AnyAsync(i => i.SubscriptionId == subscription.Id && i.PeriodStart == subscription.CurrentPeriodStart, ct))
        {
            return null;
        }

        var plan = await db.Plans.AsNoTracking().SingleAsync(p => p.Id == subscription.PlanId, ct);
        var profile = await brands.GetProfileAsync(subscription.BrandId, ct);
        if (profile is not null)
        {
            subscription.UpdateCounts(profile.ActiveBranchCount, profile.ActiveStaffCount);
        }

        var amount = InvoiceAmount.Calculate(plan, subscription.BillingCycle, subscription.BranchCount, options.Value.VatRatePercent);
        if (amount is null)
        {
            return null;
        }

        var number = await numbers.NextAsync(BillingCalendar.Today(now).Year, ct);
        var invoice = SubscriptionInvoice.Issue(
            ids.New(), subscription, number, subscription.CurrentPeriodStart, subscription.CurrentPeriodEnd, amount, now);
        db.Invoices.Add(invoice);
        return invoice;
    }
}
