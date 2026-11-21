using NodaTime;
using Omvila.Modules.Sales.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Tests;

// Kural: durum-makinesi.md A11 ("prim doğar"), § 7 (Accrued); alan-modeli.md § CommissionEntry ("Taban, kalemin tahsil edilen
// tutarıdır"), § ServiceStaff (CommissionPercentOverride); docs/acik-sorular.md W2, T-88.
public sealed class CommissionTests
{
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    private static readonly Guid Branch = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2");
    private static readonly Guid Elif = Guid.Parse("0193a8f0-0000-7000-8000-0000000000e1");
    private static readonly Guid Item = Guid.Parse("0193a8f0-0000-7000-8000-0000000000f1");
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 5);

    // dilim-05 KK5: Boya 1.250 + Fön 190, kapora 200 + Kart 1.240 = 1.440 tahsil edildi; taban kalem tutarlarıdır.
    [Fact]
    public void Distribute_Criterion5_DepositAndCardCoverTotal_BasisIsItemAmount()
    {
        CommissionBasis.Distribute([125000, 19000], 124000 + 20000).ShouldBe([125000L, 19000L]);
    }

    [Fact]
    public void Distribute_LessThanTotal_SpreadsByAmountShare_RemainderToLargestFraction()
    {
        // 124.000 × 125/144 = 107.638,9 ; 124.000 × 19/144 = 16.361,1 → artık kuruş ilk kaleme.
        var bases = CommissionBasis.Distribute([125000, 19000], 124000);

        bases.ShouldBe([107639L, 16361L]);
        bases.Sum().ShouldBe(124000);
    }

    [Fact]
    public void Distribute_EqualRemainders_EarlierItemsGetTheCents()
    {
        CommissionBasis.Distribute([1, 1, 1], 2).ShouldBe([1L, 1L, 0L]);
    }

    [Fact]
    public void Distribute_CollectedAboveTotal_IsCappedAtItemAmounts()
    {
        // Kapora toplamdan büyükse fazlası prim tabanına girmez.
        CommissionBasis.Distribute([10000, 5000], 20000).ShouldBe([10000L, 5000L]);
    }

    [Fact]
    public void Distribute_ZeroTotalOrNoItems_IsZero()
    {
        CommissionBasis.Distribute([0, 0], 5000).ShouldBe([0L, 0L]);
        CommissionBasis.Distribute([], 5000).ShouldBeEmpty();
    }

    [Fact]
    public void Distribute_NothingCollected_IsZero()
    {
        CommissionBasis.Distribute([125000, 19000], 0).ShouldBe([0L, 0L]);
    }

    [Fact]
    public void Distribute_NegativeInput_Throws()
    {
        Should.Throw<DomainException>(() => CommissionBasis.Distribute([100], -1));
        Should.Throw<DomainException>(() => CommissionBasis.Distribute([100, -1], 50));
        Should.Throw<ArgumentNullException>(() => CommissionBasis.Distribute(null!, 50));
    }

    [Fact]
    public void Distribute_LargeAmounts_DoNotOverflow()
    {
        var bases = CommissionBasis.Distribute([long.MaxValue / 2, long.MaxValue / 2], long.MaxValue - 1);

        bases.Sum().ShouldBe(long.MaxValue - 1);
    }

    [Fact]
    public void AccrueService_Criterion5_Elif24Percent_AccruedWithCopiedPercentAndLocalPeriod()
    {
        var entry = CommissionEntry.AccrueService(new Ids(), Brand, Branch, Elif, Item, Money.Try(125000), 24m, new LocalDate(2026, 11, 25), Now);

        entry.Type.ShouldBe(CommissionType.Service);
        entry.Status.ShouldBe(CommissionStatus.Accrued);
        entry.BasisMinor.ShouldBe(125000);
        entry.Percent.ShouldBe(24m);
        entry.AmountMinor.ShouldBe(30000);
        entry.CurrencyCode.ShouldBe("TRY");
        entry.PeriodYearMonth.ShouldBe(202611);
        entry.StaffId.ShouldBe(Elif);
        entry.BrandId.ShouldBe(Brand);
        entry.BranchId.ShouldBe(Branch);
        entry.AppointmentItemId.ShouldBe(Item);
        entry.CustomerPackageId.ShouldBeNull();
        entry.ReversesEntryId.ShouldBeNull();
        entry.CreatedUtc.ShouldBe(Now);
        entry.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void AccrueService_HalfCent_RoundsAwayFromZero()
    {
        // 19.000 × %12,5 = 2.375 ; 19.001 × %12,5 = 2.375,125 → 2.375 ; 19.004 × %12,5 = 2.375,5 → 2.376
        CommissionEntry.AccrueService(new Ids(), Brand, Branch, Elif, Item, Money.Try(19004), 12.5m, new LocalDate(2027, 1, 1), Now)
            .ShouldSatisfyAllConditions(e => e.AmountMinor.ShouldBe(2376), e => e.PeriodYearMonth.ShouldBe(202701));
    }

    [Fact]
    public void AccrueService_InvalidPercent_Throws()
    {
        Should.Throw<DomainException>(() => CommissionEntry.AccrueService(new Ids(), Brand, Branch, Elif, Item, Money.Try(100), 101m, new LocalDate(2026, 11, 25), Now));
        Should.Throw<ArgumentNullException>(() => CommissionEntry.AccrueService(null!, Brand, Branch, Elif, Item, Money.Try(100), 10m, new LocalDate(2026, 11, 25), Now));
    }

    [Theory]
    [InlineData(10.0, 24.0, 10.0)]
    [InlineData(null, 24.0, 24.0)]
    [InlineData(0.0, 24.0, 0.0)]
    [InlineData(null, null, 0.0)]
    public void ServicePercent_OverrideBeatsStaffRate(double? serviceStaff, double? staff, double expected)
    {
        CommissionEntry.ServicePercent((decimal?)serviceStaff, (decimal?)staff).ShouldBe((decimal)expected);
    }

    private sealed class Ids : IIdGenerator
    {
        private int _next;

        public Guid New() => Guid.Parse($"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _next):D12}");
    }
}
