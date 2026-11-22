using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Features.AdminAudit;

namespace Omvila.Modules.Platform.Tests;

// Kural: sözleşme adminGetPaymentNotice ("Kod tutuyor" = kod VE tutar; aralık 7 gün önce – 3 gün sonra; T-116 (3)), dilim-06 KK3,
// adminImportBankTransactions (CSV biçimi, T-124), adminListSubscriptions nextStep (PastDueGraceDays), adminListAuditLog (maske).
public sealed class AdminApprovalRulesTests
{
    private static readonly DateTimeZone Istanbul = DateTimeZoneProviders.Tzdb["Europe/Istanbul"];

    [Theory]
    [InlineData("ABN-KE-1125", true)]
    [InlineData("havale abn ke 1125 kasım", true)]
    [InlineData("ABNKE1125", true)]
    [InlineData("ABN-KE-1126", false)]
    [InlineData(null, false)]
    public void CodeFound_IgnoresCaseSpacesAndDashes(string? description, bool expected) =>
        TransferMatch.ContainsCode(description, "ABN-KE-1125").ShouldBe(expected);

    [Fact]
    public void Verdict_CodeMatchesOnlyWhenCodeAndAmountBothMatch()
    {
        TransferMatch.Compare("KNT-SM-0412", 50000, "KNT-SM-0412", 50000).Verdict.ShouldBe(MatchVerdict.CodeMatches);
        TransferMatch.Compare("KNT-SM-0412", 50000, "KNT-SM-0412", 45000).Verdict.ShouldBe(MatchVerdict.CheckRequired);
        TransferMatch.Compare("KNT-SM-0412", 50000, null, 50000).Verdict.ShouldBe(MatchVerdict.CheckRequired);
        TransferMatch.Compare("KNT-SM-0412", null, "KNT-SM-0412", 50000).ShouldBe(new TransferMatch(true, false), "beklenen tutar yoksa tutar tutmaz");
    }

    [Fact]
    public void CandidateWindow_SevenDaysBeforeToThreeDaysAfter() =>
        TransferMatch.Window(new LocalDate(2026, 11, 18)).ShouldBe((new LocalDate(2026, 11, 11), new LocalDate(2026, 11, 21)));

    [Theory]
    [InlineData("1.250,50", 125050)]
    [InlineData("1250,5", 125050)]
    [InlineData("1250", 125000)]
    [InlineData("12.500.000,00", 1250000000)]
    [InlineData("1250.50", 125050)]
    [InlineData("1.250", 125000)]
    [InlineData("-300,00", -30000)]
    [InlineData("(300,00)", -30000)]
    [InlineData("1.000,00 TL", 100000)]
    public void Amount_TurkishFormat(string text, long expected) => BankStatementCsv.ParseAmount(text).ShouldBe(expected);

    [Theory]
    [InlineData("abc")]
    [InlineData("1.25,00")]
    [InlineData("1,234,56")]
    [InlineData("")]
    public void Amount_Invalid(string text) => BankStatementCsv.ParseAmount(text).ShouldBeNull();

    [Fact]
    public void Csv_CommaDelimited_HeaderInAnyOrder_TimeColumnInIstanbul()
    {
        var parsed = BankStatementCsv.Parse("REFERANS,saat,tutar,GONDEREN,tarih\nR-1,14:05,\"1.000,00\",ELİF KAYA,2026-11-18\n", Istanbul)!;

        var row = parsed.Rows.ShouldHaveSingleItem();
        row.ShouldBe(new BankStatementCsv.Row(2, new LocalDate(2026, 11, 18), Instant.FromUtc(2026, 11, 18, 11, 5), 100000, "ELİF KAYA", null, "R-1"));
        parsed.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Csv_MissingRequiredHeader_IsInvalidFile_RowErrorsCarryColumn()
    {
        BankStatementCsv.Parse("Tarih;Tutar;Gönderen\n18.11.2026;10;A\n", Istanbul).ShouldBeNull();
        var parsed = BankStatementCsv.Parse("Tarih;Tutar;Gönderen;Referans\n18.11.2026;0;A;R\n18.11.2026;10;;R2\n18.11.2026 25:00;10;A;R3\n", Istanbul)!;
        parsed.Errors.ShouldBe([
            new BankStatementCsv.RowError(2, "Tutar", "out_of_range"),
            new BankStatementCsv.RowError(3, "Gönderen", "required"),
            new BankStatementCsv.RowError(4, "Tarih", "invalid_format"),
        ]);
        parsed.TotalRows.ShouldBe(3);
    }

    [Fact]
    public void NextStep_PastDue_ReadOnlyOnIsPastDueSincePlusGraceDays()
    {
        var subscription = Trial();
        subscription.MarkPastDue(new LocalDate(2026, 12, 9)).IsSuccess.ShouldBeTrue();

        SubscriptionNextStep.For(subscription, 500, 7, 30).ShouldBe(new SubscriptionNextStep(SubscriptionNextStep.ReadOnlyOn, new LocalDate(2026, 12, 16)));
    }

    [Fact]
    public void NextStep_Trial_Active_Credits_Cancelled()
    {
        var trial = Trial();
        SubscriptionNextStep.For(trial, 500, 7, 30).ShouldBe(new SubscriptionNextStep(SubscriptionNextStep.TrialEnds, new LocalDate(2026, 12, 9)));

        trial.RecordPayment(trial.NextPeriod().Start, SubscriptionPaymentMethod.BankTransfer).IsSuccess.ShouldBeTrue();
        SubscriptionNextStep.For(trial, 500, 7, 30).ShouldBe(new SubscriptionNextStep(SubscriptionNextStep.Renews, trial.CurrentPeriodEnd.PlusDays(1)));
        SubscriptionNextStep.For(trial, 0, 7, 30).ShouldBe(new SubscriptionNextStep(SubscriptionNextStep.SmsStopped, null));
        SubscriptionNextStep.IsCreditsLow(199).ShouldBeTrue();
        SubscriptionNextStep.IsCreditsLow(200).ShouldBeFalse();
    }

    [Fact]
    public void AuditMask_NamesAndPhonesMasked_OtherValuesKept()
    {
        var masked = AuditMask.Changes("""{"fullName":"Selin Öztürk","phone":"+905324187705","reason":"Aradı: 0532 418 77 05","credits":500,"section":"Summary"}""");

        string.Join('|', masked.Select(p => $"{p.Key}={p.Value}")).ShouldBe(
            "fullName=Selin Ö.|phone=0532 *** ** 05|reason=Aradı: 0532 *** ** 05|credits=500|section=Summary");
        AuditMask.Changes(null).Count.ShouldBe(0);
        AuditMask.Changes("not json").Count.ShouldBe(0);
    }

    private static Subscription Trial() =>
        Subscription.StartTrial(
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1"), Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1"),
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1"), new LocalDate(2026, 11, 25), 14, TransferCodes.Create("Kuaför Elif", 4),
            Instant.FromUtc(2026, 11, 25, 9, 0));
}
