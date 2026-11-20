using Omvila.Modules.Catalog.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Tests;

// Kural: dilim-01 kabul kriteri 9 (Nişantaşı "Saç kesimi 550 ₺" ve isBranchPrice, Kadıköy 450 ₺), kriter 11
// ("Keratin 75 dk (özel)", "Balyaj +300 ₺"), alan-modeli.md § BranchServicePrice (fiyat ve süre çözümü)
public sealed class OfferResolverTests
{
    private static readonly Money Haircut = Money.Try(45_000);

    [Fact]
    public void BranchPrice_OverridesBasePrice_AndIsFlagged()
    {
        var nisantasi = OfferResolver.Resolve(Haircut, 45, [], new BranchPrice(Money.Try(55_000), null, true), null).Value;
        var kadikoy = OfferResolver.Resolve(Haircut, 45, [], null, null).Value;

        nisantasi.Price.ShouldBe(Money.Try(55_000));
        nisantasi.IsBranchPrice.ShouldBeTrue();
        kadikoy.Price.ShouldBe(Haircut);
        kadikoy.IsBranchPrice.ShouldBeFalse();
    }

    [Fact]
    public void StaffSurcharge_IsAddedToPrice() =>
        OfferResolver.Resolve(Money.Try(280_000), 180, [], null, new StaffServiceOverride(null, Money.Try(30_000))).Value
            .Price.ShouldBe(Money.Try(310_000));

    [Fact]
    public void StaffDuration_WinsOverBranchAndBase() =>
        OfferResolver.Resolve(Money.Try(180_000), 60, [], new BranchPrice(Money.Try(180_000), 70, true), new StaffServiceOverride(75, null))
            .Value.DurationMinutes.ShouldBe(75);

    [Fact]
    public void DurationDifference_GoesToLastBusyStage()
    {
        ServiceStage[] stages = [new("Uygulama", 45, true), new("Bekleme", 40, false), new("Yıkama ve fön", 35, true)];

        var offer = OfferResolver.Resolve(Money.Try(120_000), 120, stages, null, new StaffServiceOverride(130, null)).Value;

        offer.Stages.Select(s => s.DurationMinutes).ShouldBe([45, 40, 45]);
    }

    [Fact]
    public void Override_ThatWouldShrinkLastBusyStageBelowFiveMinutes_IsRejected()
    {
        ServiceStage[] stages = [new("Uygulama", 45, true), new("Bekleme", 40, false), new("Yıkama ve fön", 35, true)];

        OfferResolver.FitsStages(stages, 120, 85).ShouldBeFalse();
        OfferResolver.FitsStages(stages, 120, 90).ShouldBeTrue();
        OfferResolver.Resolve(Money.Try(120_000), 120, stages, null, new StaffServiceOverride(85, null)).IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void UnavailableAtBranch_IsNotFound() =>
        OfferResolver.Resolve(Haircut, 45, [], new BranchPrice(Haircut, null, IsAvailable: false), null).Error!.Code.ShouldBe(ErrorCodes.NotFound);
}
