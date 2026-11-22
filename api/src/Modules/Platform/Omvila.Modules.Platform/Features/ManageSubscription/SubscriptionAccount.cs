using System.Buffers.Text;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>
/// Salonun abonelik okumaları (manageGetSubscription, manageListInvoices, manageGetInvoicePdf, manageListCreditPackages,
/// managePurchaseCredits). Markanın kiracı bağlamında. Fiyat ve OMVILA hesabı yapılandırılmamışsa ilgili alan null (S-14).
/// </summary>
internal sealed class SubscriptionAccount(
    PlatformDbContext db, SubscriptionBilling billing, IBrandBilling brands, IOptions<PlatformOptions> options)
{
    private const int DefaultLimit = 50;
    private const int MaxLimit = 100;
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    public async Task<SubscriptionDetail?> GetDetailAsync(Guid brandId, CancellationToken ct)
    {
        var subscription = await db.Subscriptions.AsNoTracking().SingleOrDefaultAsync(s => s.BrandId == brandId, ct);
        if (subscription is null)
        {
            return null;
        }

        var plan = await db.Plans.AsNoTracking().SingleAsync(p => p.Id == subscription.PlanId, ct);
        var profile = await brands.GetProfileAsync(brandId, ct);
        var branches = profile?.ActiveBranchCount ?? subscription.BranchCount;
        var amount = await billing.AmountAsync(subscription, ct);
        var pending = await db.PaymentNotices.AsNoTracking()
            .Where(n => n.BrandId == brandId && n.Purpose == PaymentNoticePurpose.Subscription && n.Status == PaymentNoticeStatus.Pending)
            .OrderByDescending(n => n.NotifiedUtc)
            .FirstOrDefaultAsync(ct);
        return new SubscriptionDetail(
            subscription.Status,
            new PlanRef(plan.Code, plan.Name),
            subscription.BillingCycle,
            subscription.PaymentMethod,
            subscription.Status == SubscriptionStatus.Trialing ? subscription.TrialEndsOn : null,
            subscription.CurrentPeriodStart,
            subscription.CurrentPeriodEnd,
            subscription.PastDueSince,
            subscription.ReadOnlyOn(options.Value.PastDueGraceDays),
            subscription.Status == SubscriptionStatus.ReadOnly ? subscription.ReadOnlySince : null,
            new BranchCounts(branches, plan.IncludedBranchCount, InvoiceAmount.ExtraBranches(branches, plan.IncludedBranchCount)),
            new StaffCounts(profile?.ActiveStaffCount ?? subscription.StaffCount, plan.StaffLimit),
            amount is null ? null : new Money(amount.TotalMinor, amount.Currency),
            subscription.TransferCode,
            BankAccount(),
            pending is null ? null : PaymentNoticeSummary.From(pending));
    }

    public async Task<Result<SubscriptionInvoicePage>> ListInvoicesAsync(Guid brandId, int? limit, string? cursor, CancellationToken ct)
    {
        if (limit is < 1 or > MaxLimit)
        {
            return Error.Validation("limit", "out_of_range");
        }

        (Instant IssuedUtc, Guid Id)? after = null;
        if (cursor is not null)
        {
            if (!TryDecode(cursor, out var decoded))
            {
                return Error.Validation("cursor", "invalid_format");
            }

            after = decoded;
        }

        var query = db.Invoices.AsNoTracking().Where(i => i.BrandId == brandId);
        if (after is { } a)
        {
            query = query.Where(i => i.IssuedUtc < a.IssuedUtc || (i.IssuedUtc == a.IssuedUtc && i.Id.CompareTo(a.Id) < 0));
        }

        var take = limit ?? DefaultLimit;
        var rows = await query.OrderByDescending(i => i.IssuedUtc).ThenByDescending(i => i.Id).Take(take + 1).ToListAsync(ct);
        var page = rows.Take(take).ToList();
        var next = rows.Count > take ? Encode(page[^1]) : null;
        return new SubscriptionInvoicePage([.. page.Select(SubscriptionInvoiceView.From)], next);
    }

    /// <summary>Bilgi amaçlı fatura PDF'i (e-Arşiv değil); fatura markada yoksa null.</summary>
    public async Task<(string FileName, byte[] Content)?> GetInvoicePdfAsync(Guid brandId, Guid invoiceId, CancellationToken ct)
    {
        var invoice = await db.Invoices.AsNoTracking().SingleOrDefaultAsync(i => i.BrandId == brandId && i.Id == invoiceId, ct);
        if (invoice is null)
        {
            return null;
        }

        var plan = await (from s in db.Subscriptions.AsNoTracking()
                          join p in db.Plans.AsNoTracking() on s.PlanId equals p.Id
                          where s.Id == invoice.SubscriptionId
                          select new { p.Name, s.BillingCycle }).SingleAsync(ct);
        var profile = await brands.GetProfileAsync(brandId, ct);
        var pdf = new SimplePdf()
            .Line("OMVILA", 20, bold: true, spacingAfter: 4)
            .Line("Abonelik faturası · bilgi amaçlıdır, e-Arşiv faturası değildir", 9, spacingAfter: 18)
            .Row("Fatura no", invoice.InvoiceNumber, bold: true)
            .Row("Kesim tarihi", Date(invoice.IssuedUtc.InZone(BillingCalendar.Zone).Date))
            .Row("Durum", StatusText(invoice.Status));
        if (options.Value.OmvilaBankAccount is { IsConfigured: true } account)
        {
            pdf.Row("Düzenleyen", account.AccountHolderName!);
        }

        pdf.Space(10)
            .Row("İşletme", profile?.LegalName ?? profile?.Name ?? string.Empty);
        if (profile?.TaxOffice is { } office && profile.TaxNumber is { } number)
        {
            pdf.Row("Vergi dairesi / no", $"{office} / {number}");
        }

        pdf.Space(10)
            .Row("Plan", $"{plan.Name} · {(plan.BillingCycle == BillingCycle.Yearly ? "yıllık" : "aylık")}")
            .Row("Dönem", $"{Date(invoice.PeriodStart)} – {Date(invoice.PeriodEnd)}")
            .Space(10)
            .Row("Tutar (KDV hariç)", Lira(invoice.AmountMinor))
            .Row("KDV", Lira(invoice.TaxMinor))
            .Row("Toplam", Lira(invoice.TotalMinor), bold: true);
        if (invoice.PaidUtc is { } paid)
        {
            pdf.Row("Ödeme tarihi", Date(paid.InZone(BillingCalendar.Zone).Date));
        }

        return ($"{invoice.InvoiceNumber}.pdf", pdf.Build());
    }

    public async Task<CreditPackageList> ListCreditPackagesAsync(CancellationToken ct)
    {
        var packages = await db.CreditPackages.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.Credits).ToListAsync(ct);
        return new CreditPackageList([.. packages.Select(CreditPackageView.From)]);
    }

    // Kural: K1 (kontör yalnız havaleyle; kayıt oluşmaz, salon manageNotifyTransfer ile bildirir)
    public async Task<Result<CreditPurchaseInstructions>> PurchaseInstructionsAsync(Guid brandId, CreditPurchaseRequest? body, CancellationToken ct)
    {
        if (body?.CreditPackageId is not { } packageId)
        {
            return Error.Validation("creditPackageId", "required");
        }

        var package = await db.CreditPackages.AsNoTracking().SingleOrDefaultAsync(p => p.Id == packageId && p.IsActive, ct);
        var subscription = await db.Subscriptions.AsNoTracking().SingleOrDefaultAsync(s => s.BrandId == brandId, ct);
        if (package is null || subscription is null)
        {
            return Error.NotFound();
        }

        return new CreditPurchaseInstructions(
            package.Id, package.Credits, package.PriceMinor is { } price ? Money.Try(price) : null, subscription.CreditTransferCode, BankAccount());
    }

    internal OmvilaBankAccountView? BankAccount() =>
        options.Value.OmvilaBankAccount is { IsConfigured: true } account
            ? new OmvilaBankAccountView(account.AccountHolderName!, account.Iban!, string.IsNullOrWhiteSpace(account.BankName) ? null : account.BankName)
            : null;

    private static string Lira(long minor) => (minor / 100m).ToString("N2", Turkish) + " TL";

    private static string Date(LocalDate date) => date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

    private static string StatusText(SubscriptionInvoiceStatus status) => status switch
    {
        SubscriptionInvoiceStatus.Paid => "Ödendi",
        SubscriptionInvoiceStatus.Failed => "Ödenemedi",
        SubscriptionInvoiceStatus.Void => "İptal",
        _ => "Ödeme bekleniyor",
    };

    private static string Encode(SubscriptionInvoice last) =>
        Base64Url.EncodeToString(Encoding.UTF8.GetBytes(
            string.Create(CultureInfo.InvariantCulture, $"{last.IssuedUtc.ToUnixTimeTicks()}|{last.Id}")));

    private static bool TryDecode(string cursor, out (Instant IssuedUtc, Guid Id) value)
    {
        value = default;
        try
        {
            var parts = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(cursor)).Split('|');
            if (parts.Length == 2 && long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var ticks)
                && Guid.TryParse(parts[1], out var id))
            {
                value = (Instant.FromUnixTimeTicks(ticks), id);
                return true;
            }
        }
        catch (FormatException)
        {
            return false;
        }

        return false;
    }
}
