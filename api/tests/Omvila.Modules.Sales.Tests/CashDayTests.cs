using NodaTime;
using Omvila.Modules.Sales.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Tests;

// Kural: alan-modeli.md § CashDayClose; dilim-05 kabul kriteri 7 (beklenen 3.450, sayılan 3.400 → fark −50 ve açıklama);
// docs/acik-sorular.md W3 (beklenen nakit = nakit tahsilatlar − nakit iadeler), T-86, T-96; sözleşme CashDay, CashDayClosing.
public sealed class CashDayTests
{
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    private static readonly Guid Branch = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2");
    private static readonly Guid User = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
    private static readonly LocalDate Today = new(2026, 11, 25);
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 17, 0);

    [Fact]
    public void Totals_ByMethod_TransferSplitsDeposit_PackageRedemptionCountsWithoutAmount_VoidedIgnored()
    {
        var totals = CashDayTotals.Calculate(
            [
                Entry(PaymentMethod.Cash, 45000),
                Entry(PaymentMethod.Cash, 300000),
                Entry(PaymentMethod.Card, 124000),
                Entry(PaymentMethod.Transfer, 15000, isDeposit: true),
                Entry(PaymentMethod.Transfer, 50000),
                Entry(PaymentMethod.PackageRedemption, 0),
                Entry(PaymentMethod.Cash, 99900, status: PaymentStatus.Voided),
                Entry(PaymentMethod.Card, 10000, status: PaymentStatus.Refunded),
            ],
            0);

        totals.Total.ShouldBe(new MethodTotal(544000, 7), "sayı tüm kayıtlar (Paketten dahil), tutar Paketten hariç; iptal kayıt yok");
        totals.Cash.ShouldBe(new MethodTotal(345000, 2));
        totals.Card.ShouldBe(new MethodTotal(134000, 2), "iade işaretli kayıt brüt sayılır");
        totals.Transfer.ShouldBe(new MethodTotal(65000, 2));
        totals.TransferDepositMinor.ShouldBe(15000);
        totals.TransferOtherMinor.ShouldBe(50000);
        totals.CashRefundedMinor.ShouldBe(0);
        totals.ExpectedCashMinor.ShouldBe(345000);
    }

    // Kural: docs/acik-sorular.md W3: o gün işaretlenen nakit iadeler beklenen nakitten düşülür; tutar eksiye inmez.
    [Theory]
    [InlineData(345000, 20000, 325000)]
    [InlineData(10000, 20000, 0)]
    public void ExpectedCash_IsCashMinusCashRefunds_NotBelowZero(long cash, long refunded, long expected)
    {
        var totals = CashDayTotals.Calculate([Entry(PaymentMethod.Cash, cash)], refunded);

        totals.CashRefundedMinor.ShouldBe(refunded);
        totals.ExpectedCashMinor.ShouldBe(expected);
    }

    [Fact]
    public void Totals_UnknownMethod_CountsInTotalOnly()
    {
        var totals = CashDayTotals.Calculate([Entry((PaymentMethod)99, 7000), Entry(PaymentMethod.Cash, 1000)], 0);

        totals.Total.ShouldBe(new MethodTotal(8000, 2));
        totals.Cash.ShouldBe(new MethodTotal(1000, 1));
        totals.Card.ShouldBe(default);
        totals.Transfer.ShouldBe(default);
    }

    [Fact]
    public void Totals_Empty_AreZero()
    {
        var totals = CashDayTotals.Calculate([], 0);

        totals.Total.ShouldBe(default);
        totals.ExpectedCashMinor.ShouldBe(0);
        Should.Throw<ArgumentNullException>(() => CashDayTotals.Calculate(null!, 0));
    }

    [Fact]
    public void Criterion7_Expected3450Counted3400_DifferenceMinus50_Shortage_WithNote()
    {
        var closed = Close(Money.Try(345000), Money.Try(340000), "  Bozuk para verildi ");

        closed.IsSuccess.ShouldBeTrue();
        var close = closed.Value;
        close.ExpectedCashMinor.ShouldBe(345000);
        close.CountedCashMinor.ShouldBe(340000);
        close.DifferenceMinor.ShouldBe(-5000);
        close.Difference.ShouldBe(CashDifference.Shortage);
        close.DifferenceNote.ShouldBe("Bozuk para verildi");
        close.CurrencyCode.ShouldBe("TRY");
        close.BrandId.ShouldBe(Brand);
        close.BranchId.ShouldBe(Branch);
        close.BusinessDate.ShouldBe(Today);
        close.ClosedByUserId.ShouldBe(User);
        close.ClosedUtc.ShouldBe(Now);
        close.Id.ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void Criterion7_DifferenceWithoutNote_IsValidationRequired(string? note)
    {
        var closed = Close(Money.Try(345000), Money.Try(340000), note);

        closed.IsSuccess.ShouldBeFalse();
        closed.Error.Code.ShouldBe(ErrorCodes.ValidationFailed);
        closed.Error.FieldErrors["differenceNote"].ShouldBe(["required"]);
    }

    [Fact]
    public void NoDifference_NoteOptional_None()
    {
        var closed = Close(Money.Try(345000), Money.Try(345000), null);

        closed.Value.Difference.ShouldBe(CashDifference.None);
        closed.Value.DifferenceMinor.ShouldBe(0);
        closed.Value.DifferenceNote.ShouldBeNull();
    }

    [Fact]
    public void MoreCounted_IsSurplus()
    {
        var closed = Close(Money.Try(345000), Money.Try(350000), "Bahşiş kasaya kondu");

        closed.Value.DifferenceMinor.ShouldBe(5000);
        closed.Value.Difference.ShouldBe(CashDifference.Surplus);
    }

    [Fact]
    public void NoteTooLong_AndFutureDay_AreValidationErrors()
    {
        var closed = CashDayClose.Close(
            new Ids(), Brand, Branch, Today.PlusDays(1), Today, Money.Try(0), Money.Try(100), new string('a', CashDayClose.DifferenceNoteMaxLength + 1), User, Now);

        closed.IsSuccess.ShouldBeFalse();
        closed.Error.FieldErrors["date"].ShouldBe(["out_of_range"]);
        closed.Error.FieldErrors["differenceNote"].ShouldBe(["too_long"]);
    }

    [Fact]
    public void PastDay_CanBeClosed()
    {
        var closed = CashDayClose.Close(new Ids(), Brand, Branch, Today.PlusDays(-3), Today, Money.Try(0), Money.Try(0), null, null, Now);

        closed.IsSuccess.ShouldBeTrue();
        closed.Value.ClosedByUserId.ShouldBeNull();
    }

    [Fact]
    public void ProgrammerErrors_Throw()
    {
        Should.Throw<DomainException>(() => Close(Money.Try(100), new Money(100, (Currency)99), null));
        Should.Throw<ArgumentNullException>(() =>
            CashDayClose.Close(null!, Brand, Branch, Today, Today, Money.Try(0), Money.Try(0), null, User, Now));
    }

    // Kural: alan-modeli.md § CashDayClose ("kapanmış güne yeni tahsilat yazılabilir ama raporda 'kapanıştan sonra' diye işaretlenir").
    [Fact]
    public void IsRecordedAfter_OnlyWhenPaidAfterClosing()
    {
        var close = Close(Money.Try(0), Money.Try(0), null).Value;

        close.IsRecordedAfter(Now.Plus(Duration.FromMinutes(1))).ShouldBeTrue();
        close.IsRecordedAfter(Now).ShouldBeFalse();
        close.IsRecordedAfter(Now.Minus(Duration.FromHours(1))).ShouldBeFalse();
    }

    private static Result<CashDayClose> Close(Money expected, Money counted, string? note) =>
        CashDayClose.Close(new Ids(), Brand, Branch, Today, Today, expected, counted, note, User, Now);

    private static CashEntry Entry(PaymentMethod method, long amountMinor, bool isDeposit = false, PaymentStatus status = PaymentStatus.Recorded) =>
        new(method, amountMinor, isDeposit, status);

    private sealed class Ids : IIdGenerator
    {
        private int _next;

        public Guid New() => Guid.Parse($"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _next):D12}");
    }
}
