using System.Reflection;
using System.Text;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Features.ManageSubscription;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Tests;

// Kural: alan-modeli.md § SubscriptionInvoice, § PaymentNotice, § Subscription.TransferCode; durum-makinesi.md § 10 PaymentNotice;
// guvenlik-kvkk.md § Dosya yükleme; T-118 (tutar, KDV, fiyat yoksa fatura yok).
public sealed class BillingTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 12, 9, 6, 0);
    private static readonly Guid Admin = Guid.Parse("0193a8f0-0000-7000-8000-0000000000e1");

    [Fact]
    public void InvoiceNumber_YearPrefixAndSixDigits() =>
        InvoiceNumbers.Format(2026, 184).ShouldBe("OMV-2026-000184");

    [Fact]
    public void Amount_PlanPriceWithoutPrice_IsNull() =>
        InvoiceAmount.Calculate(Plan(null, 1, 25000), BillingCycle.Monthly, 3, 20m).ShouldBeNull();

    [Fact]
    public void Amount_PlanPricePlusExtraBranches_WithVat()
    {
        var amount = InvoiceAmount.Calculate(Plan(100000, 1, 25000), BillingCycle.Monthly, 3, 20m)!;

        amount.NetMinor.ShouldBe(150000, "100.000 + 2 ek şube × 25.000");
        amount.TaxMinor.ShouldBe(30000);
        amount.TotalMinor.ShouldBe(180000);
        amount.Currency.ShouldBe(Currency.TRY);
    }

    [Fact]
    public void Amount_NoVatConfigured_TaxZero_UnlimitedBranchesNoExtra()
    {
        var amount = InvoiceAmount.Calculate(Plan(100000, null, 25000), BillingCycle.Monthly, 5, null)!;

        amount.NetMinor.ShouldBe(100000);
        amount.TaxMinor.ShouldBe(0);
    }

    [Fact]
    public void ExtraBranches_NeverNegative() => InvoiceAmount.ExtraBranches(1, 2).ShouldBe(0);

    [Fact]
    public void Invoice_IssueThenPaid_PaidTwiceInvalid()
    {
        var subscription = SubscriptionTests.Trial();
        var invoice = SubscriptionInvoice.Issue(
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000f1"), subscription, "OMV-2026-000001", new LocalDate(2026, 12, 9),
            new LocalDate(2027, 1, 8), new InvoiceAmount(100000, 20000, Currency.TRY), Now);

        invoice.Status.ShouldBe(SubscriptionInvoiceStatus.Issued);
        invoice.DueOn.ShouldBe(new LocalDate(2026, 12, 9), "vade dönem başı");
        invoice.TotalMinor.ShouldBe(120000);
        invoice.MarkPaid(SubscriptionPaymentMethod.BankTransfer, Now).IsSuccess.ShouldBeTrue();
        invoice.PaidVia.ShouldBe(SubscriptionPaymentMethod.BankTransfer);
        invoice.MarkPaid(SubscriptionPaymentMethod.BankTransfer, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        invoice.MarkVoid().Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    [Theory]
    [InlineData("Kuaför Elif", "KE")]
    [InlineData("şık salon", "SS")]
    [InlineData("İpek Güzellik", "IG")]
    [InlineData("Berber", "BB")]
    [InlineData("", "OM")]
    [InlineData("123 & 456", "OM")]
    public void TransferCodes_InitialsAsciiUpper(string name, string initials) =>
        TransferCodes.Initials(name).ShouldBe(initials);

    [Fact]
    public void TransferCodes_SameSuffixForBothPrefixes()
    {
        var codes = TransferCodes.Create("Kuaför Elif", 4);

        codes.Subscription.ShouldMatch("^ABN-KE-[0-9]{4}$");
        codes.Credits.ShouldBe("KNT" + codes.Subscription[3..]);
    }

    [Fact]
    public void PaymentNotice_TrialingSubscription_PaysFirstPeriodAfterTrial()
    {
        var notice = PaymentNotice.ForSubscription(
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000f2"), SubscriptionTests.Trial(), null, PaymentNoticeChannel.Panel, "  Elif Kaya ", null, Now);

        notice.Status.ShouldBe(PaymentNoticeStatus.Pending);
        notice.TransferCode.ShouldBe("ABN-KE-1125");
        notice.PeriodStart.ShouldBe(new LocalDate(2026, 12, 9));
        notice.SenderName.ShouldBe("Elif Kaya");
        notice.ExpectedAmountMinor.ShouldBeNull();
    }

    [Fact]
    public void PaymentNotice_Credits_UsesCreditCodeAndPackage()
    {
        var package = CreditPackage.Unpriced(Guid.Parse("0193a8f0-0000-7000-8000-0000000000f3"), "medium", "Orta", 5000);
        var notice = PaymentNotice.ForCredits(
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000f4"), SubscriptionTests.Trial(), package, PaymentNoticeChannel.App, null, null, Now);

        notice.TransferCode.ShouldBe("KNT-KE-1125");
        notice.Credits.ShouldBe(5000);
        notice.CreditPackageId.ShouldBe(package.Id);
        notice.Channel.ShouldBe(PaymentNoticeChannel.App);
    }

    [Fact]
    public void PaymentNotice_PendingResolvesOnce()
    {
        var bankTransaction = Guid.Parse("0193a8f0-0000-7000-8000-0000000000f5");
        var matched = Notice();
        matched.Match(bankTransaction, Admin, Now).IsSuccess.ShouldBeTrue();
        matched.Status.ShouldBe(PaymentNoticeStatus.Matched);
        matched.MatchedBankTransactionId.ShouldBe(bankTransaction);
        matched.ResolvedByUserId.ShouldBe(Admin);
        matched.MarkNotFound(Admin, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

        var notFound = Notice();
        notFound.MarkNotFound(Admin, Now).IsSuccess.ShouldBeTrue();
        notFound.Status.ShouldBe(PaymentNoticeStatus.NotFound);
        notFound.Reject(Admin, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

        var rejected = Notice();
        rejected.Reject(Admin, Now).IsSuccess.ShouldBeTrue();
        rejected.Status.ShouldBe(PaymentNoticeStatus.Rejected);
        rejected.Match(bankTransaction, Admin, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    [Theory]
    [InlineData(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31 }, "application/pdf")]
    [InlineData(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "image/png")]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, "image/jpeg")]
    [InlineData(new byte[] { 0x47, 0x49, 0x46, 0x38 }, null)]
    public void ReceiptType_DetectedFromSignature(byte[] head, string? contentType) =>
        ReceiptType.Detect(head)?.ContentType.ShouldBe(contentType);

    [Fact]
    public void SimplePdf_ValidStructure_TurkishLettersMapped()
    {
        var bytes = new SimplePdf().Line("Abonelik faturası · ğüşıöç İĞŞ (bilgi)").Row("Toplam", "1.200,00 TL", bold: true).Build();
        var text = Encoding.Latin1.GetString(bytes);

        text.ShouldStartWith("%PDF-1.4");
        text.TrimEnd().ShouldEndWith("%%EOF");
        text.ShouldContain("/Differences [129 /gbreve");
        var startXref = int.Parse(text[(text.LastIndexOf("startxref", StringComparison.Ordinal) + 10)..].Split('\n')[0], System.Globalization.CultureInfo.InvariantCulture);
        text[startXref..].ShouldStartWith("xref");
        SimplePdf.Escape("(ğ)\\").ShouldBe("\\(\u0081\\)\\\\");
        SimplePdf.Escape("€").ShouldBe("?");
    }

    // Fatura PDF'i görüntüleyicide açılarak denetlendi (T-131): dönem aralığındaki "–" WinAnsi kodu olmadan "?" çıkıyordu.
    [Fact]
    public void SimplePdf_PeriodDashAndCurlyQuotes_UseWinAnsiCodes()
    {
        SimplePdf.Escape("09.12.2026 – 08.01.2027").ShouldBe("09.12.2026 \u0096 08.01.2027");
        SimplePdf.Escape("Elif’in Salonu").ShouldBe("Elif\u0092in Salonu");
        SimplePdf.Escape("“Nar” — güzellik…").ShouldBe("\u0093Nar\u0094 \u0097 g\u00fczellik\u0085");
    }

    private static PaymentNotice Notice() =>
        PaymentNotice.ForSubscription(
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000f6"), SubscriptionTests.Trial(), 120000, PaymentNoticeChannel.Panel, null, null, Now);

    // Plan fiyatı yalnız platform paneli (ayrı PR) ve veritabanından değişir; testte özel alanlara yansımayla yazılır.
    private static Plan Plan(long? monthly, int? includedBranches, long? extraBranchPrice)
    {
        var plan = Domain.Plan.Unpriced(Guid.Parse("0193a8f0-0000-7000-8000-0000000000d1"), "salon", "Salon");
        Set(plan, nameof(Domain.Plan.MonthlyPriceMinor), monthly);
        Set(plan, nameof(Domain.Plan.IncludedBranchCount), includedBranches);
        Set(plan, nameof(Domain.Plan.ExtraBranchPriceMinor), extraBranchPrice);
        return plan;
    }

    private static void Set(object target, string property, object? value) =>
        target.GetType().GetProperty(property, BindingFlags.Instance | BindingFlags.Public)!.SetValue(target, value);
}
