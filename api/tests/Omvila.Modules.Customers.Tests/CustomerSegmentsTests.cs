using NodaTime;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Tests;

// Kural: sozluk.md § Müşteri listesi filtreleri (Düzenli, Yeni, Gelmeme riski, 60+ gündür gelmeyen), alan-modeli.md
// § BrandCustomer ("Riskli müşteri" = NoShowCount ≥ RiskyCustomerNoShowThreshold) ve § Tag (sistem etiketleri hesaplanır),
// Musteriler ekranı etiketleri ("Düzenli", "Yeni", "2 kez gelmedi").
public sealed class CustomerSegmentsTests
{
    private static readonly LocalDate Today = new(2026, 11, 25);
    private const int Threshold = 2;

    [Fact]
    public void NeverCompleted_IsNew_AndTaggedYeni()
    {
        var a = new CustomerActivity(VisitCount: 0, NoShowCount: 0, FirstVisitOn: null, LastVisitOn: null);

        CustomerSegments.IsNew(a, Today).ShouldBeTrue();
        CustomerSegments.Tags(a, Today, Threshold).ShouldBe(["Yeni"]);
    }

    [Theory]
    [InlineData(30, true)]
    [InlineData(31, false)]
    public void FirstVisitWithin30Days_IsNew(int daysAgo, bool expected) =>
        CustomerSegments.IsNew(new CustomerActivity(1, 0, Today.PlusDays(-daysAgo), Today.PlusDays(-daysAgo)), Today).ShouldBe(expected);

    [Theory]
    [InlineData(3, 180, true)]
    [InlineData(3, 181, false)]
    [InlineData(2, 0, false)]
    public void Regular_NeedsThreeVisits_AndARecentVisit(int visits, int lastVisitDaysAgo, bool expected) =>
        CustomerSegments.IsRegular(new CustomerActivity(visits, 0, Today.PlusDays(-400), Today.PlusDays(-lastVisitDaysAgo)), Today).ShouldBe(expected);

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    public void NoShowRisk_UsesBrandThreshold(int noShows, bool expected) =>
        CustomerSegments.IsNoShowRisk(new CustomerActivity(5, noShows, null, null), Threshold).ShouldBe(expected);

    [Fact]
    public void RiskyCustomer_IsTaggedWithNoShowCount()
    {
        var a = new CustomerActivity(VisitCount: 2, NoShowCount: 2, FirstVisitOn: Today.PlusDays(-300), LastVisitOn: Today.PlusDays(-70));

        CustomerSegments.Tags(a, Today, Threshold).ShouldBe(["2 kez gelmedi"]);
    }

    [Fact]
    public void RegularCustomer_IsTaggedDuzenli()
    {
        var a = new CustomerActivity(VisitCount: 12, NoShowCount: 0, FirstVisitOn: Today.PlusDays(-500), LastVisitOn: Today);

        CustomerSegments.Tags(a, Today, Threshold).ShouldBe(["Düzenli"]);
    }

    [Theory]
    [InlineData(60, false)]
    [InlineData(61, true)]
    public void Lapsed_LastVisitOlderThan60Days(int daysAgo, bool expected) =>
        CustomerSegments.IsLapsed(new CustomerActivity(4, 0, Today.PlusDays(-200), Today.PlusDays(-daysAgo)), Today).ShouldBe(expected);

    [Fact]
    public void NeverVisited_IsNotLapsed() =>
        CustomerSegments.IsLapsed(new CustomerActivity(0, 0, null, null), Today).ShouldBeFalse();
}
