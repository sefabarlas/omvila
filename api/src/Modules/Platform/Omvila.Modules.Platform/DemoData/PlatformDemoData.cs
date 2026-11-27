using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-131) için Platform yazma yolu: geçmişe çekilmiş deneme, Gecikti ve Aktif
/// abonelik, dönem faturası (plan fiyatı tanımlıysa; S-14), havale bildirimleri, banka hareketleri ve denetim kaydı. Geçişler alan
/// metotlarıyla ve verilen anlarla yapılır; olay (outbox) yazılmaz. Çağıranın transaction'ında, markanın kiracı bağlamında çalışır.
/// </summary>
public sealed class PlatformDemoData
{
    /// <summary>Demo banka hareketlerinin referans öneki; sıfırlamada yalnız bunlar silinir.</summary>
    public const string BankReferencePrefix = "DEMO-";

    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly PlatformDbContext _db;
    private readonly SubscriptionBilling _billing;
    private readonly InvoiceNumberAllocator _numbers;
    private readonly IIdGenerator _ids;
    private readonly PlatformOptions _options;

    internal PlatformDemoData(
        PlatformDbContext db, SubscriptionBilling billing, InvoiceNumberAllocator numbers, IIdGenerator ids, IOptions<PlatformOptions> options)
    {
        _db = db;
        _billing = billing;
        _numbers = numbers;
        _ids = ids;
        _options = options.Value;
    }

    /// <summary>Önceki seed'in banka hareketleri (platform verisi, markaya bağlı değil) silinir; dönüş: silinen satır.</summary>
    public Task<int> EraseBankTransactionsAsync(CancellationToken ct) =>
        _db.BankTransactions.Where(t => t.BankReference.StartsWith(BankReferencePrefix)).ExecuteDeleteAsync(ct);

    /// <summary>
    /// Kayıtta açılan denemeyi <paramref name="input"/>'taki gün kadar önce bitmiş sayar ve Gecikti'ye alır (vade = deneme bitişi;
    /// fiyat varsa dönem faturası). <see cref="DemoSubscriptionState.Active"/> ise ertesi gün bildirilen havale, kodu ve tutarı tutan
    /// banka hareketiyle eşleşir: fatura Ödendi, abonelik Aktif, denetim kaydı Match. <see cref="DemoSubscriptionState.PastDue"/> ise
    /// bekleyen bir bildirim ve iki aday hareket yazılır: biri kodu taşır (beklenen tutar biliniyorsa tutarı da tutar), diğeri taşımaz.
    /// </summary>
    public async Task<DemoSubscriptionResult> AgeSubscriptionAsync(DemoSubscription input, Instant now, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var today = BillingCalendar.Today(now);
        var trialEnd = today.PlusDays(-input.DaysSinceTrialEnd);
        var dueUtc = now - Duration.FromDays(input.DaysSinceTrialEnd);
        var subscription = await _db.Subscriptions.SingleAsync(s => s.BrandId == input.BrandId, ct);

        // Deneme geçmişe çekilir (alan metodu yok: gerçek abonelikte tarih geri alınmaz); sonraki geçişler alan metotlarıyladır.
        var entry = _db.Entry(subscription);
        entry.Property(s => s.TrialEndsOn).CurrentValue = trialEnd;
        entry.Property(s => s.CurrentPeriodStart).CurrentValue = trialEnd.PlusDays(-_options.TrialDays);
        entry.Property(s => s.CurrentPeriodEnd).CurrentValue = trialEnd;
        entry.Property(s => s.CreatedUtc).CurrentValue = dueUtc - Duration.FromDays(_options.TrialDays);
        Ensure(subscription.MarkPastDue(trialEnd));
        var invoice = await IssueAsync(subscription, dueUtc, ct);
        var expected = invoice?.TotalMinor;

        var notifiedUtc = input.State == DemoSubscriptionState.Active ? dueUtc + Duration.FromDays(1) : now - input.NoticeAge;
        var notice = PaymentNotice.ForSubscription(
            _ids.New(), subscription, expected, PaymentNoticeChannel.Panel, input.SenderName, input.NotifiedByUserId, notifiedUtc);
        _db.PaymentNotices.Add(notice);
        var withCode = AddBankTransaction(
            input, 1, notifiedUtc - Duration.FromHours(2), expected ?? input.FallbackAmountMinor, $"{subscription.TransferCode} abonelik");
        var transactions = 1;
        if (input.State == DemoSubscriptionState.Active)
        {
            var resolvedUtc = notifiedUtc + Duration.FromHours(3);
            Ensure(notice.Match(withCode.Id, input.PlatformUserId, resolvedUtc));
            _db.Entry(withCode).Property(t => t.MatchedNoticeId).CurrentValue = notice.Id;
            Ensure(subscription.RecordPayment(subscription.CurrentPeriodStart, SubscriptionPaymentMethod.BankTransfer));
            if (invoice is not null)
            {
                Ensure(invoice.MarkPaid(SubscriptionPaymentMethod.BankTransfer, resolvedUtc));
            }

            AddAudit(
                new AuditEntry(
                    input.BrandId, AuditActorType.PlatformUser, input.PlatformUserId, AuditAction.Match, nameof(PaymentNotice), notice.Id,
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["purpose"] = notice.Purpose.ToString(),
                        ["bankTransactionId"] = withCode.Id,
                        ["invoiceId"] = invoice?.Id,
                    }),
                resolvedUtc);
        }
        else
        {
            // Kodu taşımayan aday: açıklamada kod yok, tutar da farklı ("Kontrol edin").
            AddBankTransaction(input, 2, notifiedUtc - Duration.FromHours(5), (expected ?? input.FallbackAmountMinor) + input.MismatchDeltaMinor, "abonelik odemesi");
            transactions++;
        }

        await _db.SaveChangesAsync(ct);
        return new DemoSubscriptionResult(subscription.Status.ToString(), invoice?.InvoiceNumber, notice.Id, transactions);
    }

    /// <summary>
    /// Bekleyen kontör satın alma bildirimi: en küçük etkin paket (KNT- kodu; tutar paketin fiyatı, tanımlı değilse boş).
    /// Paket yoksa null.
    /// </summary>
    public async Task<Guid?> AddCreditNoticeAsync(Guid brandId, string senderName, Guid? userId, Instant notifiedUtc, CancellationToken ct)
    {
        var package = await _db.CreditPackages.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.Credits).FirstOrDefaultAsync(ct);
        if (package is null)
        {
            return null;
        }

        var subscription = await _db.Subscriptions.AsNoTracking().SingleAsync(s => s.BrandId == brandId, ct);
        var notice = PaymentNotice.ForCredits(_ids.New(), subscription, package, PaymentNoticeChannel.Panel, senderName, userId, notifiedUtc);
        _db.PaymentNotices.Add(notice);
        await _db.SaveChangesAsync(ct);
        return notice.Id;
    }

    /// <summary>Denetim kaydı satırı, verilen anla (kayıtlar kişisel veri taşımaz). Marka kaydı markanın, diğerleri platform bağlamında yazılır.</summary>
    public async Task AddAuditAsync(AuditEntry entry, Instant occurredUtc, CancellationToken ct)
    {
        AddAudit(entry, occurredUtc);
        await _db.SaveChangesAsync(ct);
    }

    private static void Ensure(Result result)
    {
        if (!result.IsSuccess)
        {
            throw new DomainException("Demo aboneliği beklenen duruma getirilemedi.");
        }
    }

    private void AddAudit(AuditEntry entry, Instant occurredUtc) =>
        _db.AuditLogs.Add(AuditLogEntry.Create(entry, entry.Changes is { Count: > 0 } c ? JsonSerializer.Serialize(c, Json) : null, occurredUtc));

    private async Task<SubscriptionInvoice?> IssueAsync(Subscription subscription, Instant issuedUtc, CancellationToken ct)
    {
        if (await _billing.AmountAsync(subscription, ct) is not { } amount)
        {
            return null;
        }

        var number = await _numbers.NextAsync(subscription.CurrentPeriodStart.Year, ct);
        var invoice = SubscriptionInvoice.Issue(
            _ids.New(), subscription, number, subscription.CurrentPeriodStart, subscription.CurrentPeriodEnd, amount, issuedUtc);
        _db.Invoices.Add(invoice);
        return invoice;
    }

    private PlatformBankTransaction AddBankTransaction(DemoSubscription input, int ordinal, Instant receivedUtc, long amountMinor, string description)
    {
        var transaction = PlatformBankTransaction.Create(
            _ids.New(), BillingCalendar.Today(receivedUtc), receivedUtc, amountMinor, input.SenderName, description,
            $"{BankReferencePrefix}{input.BrandId:N}-{ordinal}", BankTransactionSource.Manual, input.PlatformUserId, receivedUtc);
        _db.BankTransactions.Add(transaction);
        return transaction;
    }
}
