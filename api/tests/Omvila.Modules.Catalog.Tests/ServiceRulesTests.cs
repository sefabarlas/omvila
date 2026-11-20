using Omvila.Modules.Catalog.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Tests;

// Kural: dilim-01 kabul kriteri 7 ("Saç kesimi · 45 dk · 450 ₺"; "Boya" 45 + 40 bekleme + 35 → 120 dk;
// IBAN yokken "Her zaman iste" → deposit_unavailable_no_iban), alan-modeli.md § Service değişmezleri
public sealed class ServiceRulesTests
{
    private static readonly ServiceStage[] BoyaStages =
    [
        new("Uygulama", 45, StaffBusy: true),
        new("Bekleme", 40, StaffBusy: false),
        new("Yıkama ve fön", 35, StaffBusy: true),
    ];

    [Fact]
    public void HaircutWithoutStages_IsValid() =>
        ServiceRules.Validate(Draft("Saç kesimi", 45, 45_000), hasActiveBankAccount: false).IsSuccess.ShouldBeTrue();

    [Fact]
    public void ColourWithStages_TotalMustBe120() =>
        ServiceRules.Validate(Draft("Boya", 120, 120_000, stages: BoyaStages), hasActiveBankAccount: false).IsSuccess.ShouldBeTrue();

    [Fact]
    public void StageSumMismatch_IsValidationFailed() =>
        ServiceRules.Validate(Draft("Boya", 110, 120_000, stages: BoyaStages), false)
            .Error!.FieldErrors["durationMinutes"].ShouldBe(["stage_sum_mismatch"]);

    [Fact]
    public void StagesWithoutAnyBusyStage_AreRejected() =>
        ServiceRules.Validate(Draft("Bekleme", 40, 0, stages: [new ServiceStage("Bekleme", 40, false)]), false)
            .Error!.FieldErrors["stages"].ShouldBe(["no_busy_stage"]);

    [Fact]
    public void AlwaysDeposit_WithoutIban_IsDepositUnavailable()
    {
        var result = ServiceRules.Validate(
            Draft("Balyaj", 180, 280_000, deposit: new DepositRule(DepositMode.Always, null, 30m)), hasActiveBankAccount: false);

        result.Error!.Code.ShouldBe(ErrorCodes.DepositUnavailableNoIban);
        result.Error.Kind.ShouldBe(ErrorKind.Conflict);
    }

    [Fact]
    public void AlwaysDeposit_WithIban_IsValid() =>
        ServiceRules.Validate(Draft("Balyaj", 180, 280_000, deposit: new DepositRule(DepositMode.Always, null, 30m)), true)
            .IsSuccess.ShouldBeTrue();

    [Theory]
    [InlineData("amountAndPercent")]
    [InlineData("alwaysWithoutValue")]
    [InlineData("percentOutOfRange")]
    [InlineData("amountAbovePrice")]
    public void InvalidDeposit_IsValidationFailed(string scenario)
    {
        var deposit = scenario switch
        {
            "amountAndPercent" => new DepositRule(DepositMode.Inherit, Money.Try(10_000), 30m),
            "alwaysWithoutValue" => new DepositRule(DepositMode.Always, null, null),
            "percentOutOfRange" => new DepositRule(DepositMode.Inherit, null, 0.5m),
            _ => new DepositRule(DepositMode.Inherit, Money.Try(50_000), null),
        };

        ServiceRules.Validate(Draft("Saç kesimi", 45, 45_000, deposit: deposit), true).Error!.Code.ShouldBe(ErrorCodes.ValidationFailed);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(721)]
    public void DurationOutOfRange_IsValidationFailed(int minutes) =>
        ServiceRules.Validate(Draft("Saç kesimi", minutes, 45_000), false).Error!.FieldErrors["durationMinutes"].ShouldBe(["out_of_range"]);

    [Fact]
    public void MoreThanSixStages_IsValidationFailed() =>
        ServiceRules.Validate(Draft("Uzun", 70, 0, stages: Enumerable.Range(0, 7).Select(i => new ServiceStage($"A{i}", 10, true)).ToArray()), false)
            .Error!.FieldErrors["stages"].ShouldBe(["out_of_range"]);

    private static ServiceDraft Draft(string name, int minutes, long priceMinor, ServiceStage[]? stages = null, DepositRule? deposit = null) =>
        new(name, minutes, Money.Try(priceMinor), PriceDisplay.Fixed, 0, 0, deposit ?? DepositRule.Inherit, stages ?? []);
}
