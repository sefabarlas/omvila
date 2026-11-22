using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Tests;

// Kural: dilim-06 kabul kriteri 6 (peşin paket hak edilen gelirde seans kullanıldıkça); alan-modeli.md § CommissionEntry (paket seans
// payı); sözleşme ReportOverview (gelmedi kaybı = hizmet tutarı − alınmış kapora; oranlar sonuçlanan / bütün randevulara); T-120.
public sealed class ReportFiguresTests
{
    private static readonly Guid PackageItem = Guid.Parse("0193a8f0-0000-7000-8000-0000000000f1");
    private static readonly Dictionary<Guid, long> Shares = new() { [PackageItem] = 15000 };

    [Fact]
    public void EarnedMinor_PaidItem_IsItemAmount()
    {
        ReportFigures.EarnedMinor(45000, null, Shares).ShouldBe(45000);
    }

    // 10 seanslık 1.500 ₺ Fön paketi: kullanılan her seans 150 ₺ hak edilir (kalemin liste fiyatı değil).
    [Fact]
    public void EarnedMinor_PackageItem_IsSessionShare()
    {
        ReportFigures.EarnedMinor(19000, PackageItem, Shares).ShouldBe(15000);
    }

    [Fact]
    public void EarnedMinor_UnknownPackageItem_IsZero()
    {
        ReportFigures.EarnedMinor(19000, Guid.Parse("0193a8f0-0000-7000-8000-0000000000f2"), Shares).ShouldBe(0);
    }

    [Theory]
    [InlineData(45000, 0, 45000)]
    [InlineData(45000, 15000, 30000)]
    [InlineData(10000, 15000, 0)]
    public void NoShowLossMinor_IsPayableMinusRetainedDeposit_NotNegative(long payable, long retained, long expected)
    {
        ReportFigures.NoShowLossMinor(payable, retained).ShouldBe(expected);
    }

    [Fact]
    public void Percent_OneDecimal_HalfUp_NullWithoutWhole()
    {
        ReportFigures.Percent(5, 123).ShouldBe(4.1);
        ReportFigures.Percent(1, 8).ShouldBe(12.5);
        ReportFigures.Percent(54, 117).ShouldBe(46.2);
        ReportFigures.Percent(0, 10).ShouldBe(0);
        ReportFigures.Percent(3, 0).ShouldBeNull();
    }

    [Fact]
    public void ChangePercent_AgainstPrevious_NullWhenPreviousEmpty()
    {
        ReportFigures.ChangePercent(6120000, 5615000).ShouldBe(9);
        ReportFigures.ChangePercent(50000, 100000).ShouldBe(-50);
        ReportFigures.ChangePercent(50000, 0).ShouldBeNull();
    }
}
