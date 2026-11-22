using NodaTime;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Tests;

// Kural: durum-makinesi.md § 10 (Brand.Status: Onboarding → Active → Suspended / Archived); sözleşme adminChangeBrandStatus
// (yalnız Active ↔ Suspended; Onboarding veya Archived markada ya da aynı durumda 409 invalid_state_transition).
public sealed class BrandStatusTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);

    [Fact]
    public void Platform_SuspendsActiveBrand_AndReopensIt()
    {
        var brand = ActiveBrand();

        brand.ChangeStatusByPlatform(BrandStatus.Suspended).IsSuccess.ShouldBeTrue();
        brand.Status.ShouldBe(BrandStatus.Suspended);

        brand.ChangeStatusByPlatform(BrandStatus.Active).IsSuccess.ShouldBeTrue();
        brand.Status.ShouldBe(BrandStatus.Active);
    }

    [Fact]
    public void Platform_CannotChangeOnboardingBrand_OrSetTheSameStatus()
    {
        var onboarding = Brand.Register(Guid.Parse("0193a8f0-0000-7000-8000-00000000000a"), "Kuaför Elif", Slug.Create("kuafor-elif").Value, "hair_salon", "TRY", Now);

        var fromOnboarding = onboarding.ChangeStatusByPlatform(BrandStatus.Suspended);
        fromOnboarding.Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        onboarding.Status.ShouldBe(BrandStatus.Onboarding);

        var active = ActiveBrand();
        active.ChangeStatusByPlatform(BrandStatus.Active).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        active.ChangeStatusByPlatform(BrandStatus.Archived).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        active.ChangeStatusByPlatform(BrandStatus.Onboarding).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        active.Status.ShouldBe(BrandStatus.Active);
    }

    private static Brand ActiveBrand()
    {
        var brand = Brand.Register(Guid.Parse("0193a8f0-0000-7000-8000-00000000000b"), "Berber Kaan", Slug.Create("berber-kaan").Value, "barber", "TRY", Now);
        brand.CompleteOnboarding().ShouldBeTrue();
        return brand;
    }
}
