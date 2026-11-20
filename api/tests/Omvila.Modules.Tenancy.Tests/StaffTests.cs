using NodaTime;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Tests;

// Kural: dilim-01 kabul kriteri 12 (Elif'e 2–4 Aralık yıllık izin),
// alan-modeli.md § StaffWorkingHours (şubeler arası örtüşme yok), § TimeOff, § Plan
public sealed class StaffTests
{
    private static readonly DateTimeZone Istanbul = DateTimeZoneProviders.Tzdb["Europe/Istanbul"];
    private static readonly Guid Kadikoy = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    private static readonly Guid Nisantasi = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2");

    private static LocalTime T(int hour, int minute = 0) => new(hour, minute);

    [Fact]
    public void AnnualLeave_TwoToFourDecember_CoversThreeFullIstanbulDays()
    {
        var period = TimeOffPeriod.AllDay(new LocalDate(2026, 12, 2), new LocalDate(2026, 12, 4), Istanbul).Value;

        period.IsAllDay.ShouldBeTrue();
        period.StartUtc.ShouldBe(Instant.FromUtc(2026, 12, 1, 21, 0), "2 Aralık 00:00 İstanbul");
        period.EndUtc.ShouldBe(Instant.FromUtc(2026, 12, 4, 21, 0), "5 Aralık 00:00 İstanbul (4 Aralık dahil)");
        (period.EndUtc - period.StartUtc).ShouldBe(Duration.FromDays(3));
    }

    [Fact]
    public void AllDayLeave_EndingBeforeStart_IsValidationFailed() =>
        TimeOffPeriod.AllDay(new LocalDate(2026, 12, 4), new LocalDate(2026, 12, 2), Istanbul).Error!.FieldErrors["toDate"].ShouldBe(["invalid_range"]);

    [Fact]
    public void TimedLeave_RequiresStartBeforeEnd()
    {
        var start = Instant.FromUtc(2026, 11, 25, 10, 0);

        TimeOffPeriod.Timed(start, start + Duration.FromHours(2)).IsSuccess.ShouldBeTrue();
        TimeOffPeriod.Timed(start, start).Error!.FieldErrors["endUtc"].ShouldBe(["invalid_range"]);
        TimeOffPeriod.Timed(null, start).Error!.FieldErrors["startUtc"].ShouldBe(["required"]);
    }

    [Fact]
    public void StaffHours_AtTwoBranchesOnDifferentDays_AreValid() =>
        StaffHoursRules.CheckAgainstOtherBranches(
            [IsoDayOfWeek.Tuesday], Hours(IsoDayOfWeek.Tuesday, (T(10), T(19))), [Hours(IsoDayOfWeek.Monday, (T(10), T(19)))]).IsSuccess.ShouldBeTrue();

    [Fact]
    public void StaffHours_OverlappingAcrossBranches_ReportsDayIndexPerContract() =>
        StaffHoursRules.CheckAgainstOtherBranches(
            [IsoDayOfWeek.Tuesday], Hours(IsoDayOfWeek.Tuesday, (T(10), T(14))), [Hours(IsoDayOfWeek.Tuesday, (T(13), T(19)))])
            .Error!.FieldErrors["days[0].intervals"].ShouldBe(["overlaps_other_branch"]);

    [Fact]
    public void StaffHours_SameDayBackToBackAtTwoBranches_AreValid() =>
        StaffHoursRules.CheckAgainstOtherBranches(
            [IsoDayOfWeek.Saturday], Hours(IsoDayOfWeek.Saturday, (T(9), T(13))), [Hours(IsoDayOfWeek.Saturday, (T(14), T(18)))]).IsSuccess.ShouldBeTrue();

    [Fact]
    public void StaffLimit_IsStrict_AndCarriesPlanLimitMeta()
    {
        var limits = new PlanLimits(StaffLimit: 3, IncludedBranchCount: 1, AllowsExtraBranches: true);

        limits.CanAddStaff(2).IsSuccess.ShouldBeTrue();
        var denied = limits.CanAddStaff(3);
        denied.Error!.Code.ShouldBe(ErrorCodes.Forbidden);
        denied.Error.Meta.ContainsKey("planLimit").ShouldBeTrue();
    }

    [Fact]
    public void BranchBeyondIncluded_IsAllowedAsExtra_WhenPlanHasExtraBranchPrice()
    {
        var limits = new PlanLimits(StaffLimit: null, IncludedBranchCount: 1, AllowsExtraBranches: true);

        limits.CanAddBranch(0).Value.ShouldBeFalse("dahil şube");
        limits.CanAddBranch(1).Value.ShouldBeTrue("ek şube, faturaya yansır");
    }

    [Fact]
    public void BranchBeyondIncluded_IsForbidden_WhenPlanHasNoExtraBranches() =>
        new PlanLimits(null, IncludedBranchCount: 1, AllowsExtraBranches: false).CanAddBranch(1).Error!.Code.ShouldBe(ErrorCodes.Forbidden);

    [Fact]
    public void UnlimitedPlan_AllowsEverything()
    {
        PlanLimits.Unlimited.CanAddStaff(500).IsSuccess.ShouldBeTrue();
        PlanLimits.Unlimited.CanAddBranch(50).Value.ShouldBeFalse();
    }

    [Fact]
    public void NewStaffWithInvite_NormalizesMobilePhone() =>
        StaffRules.Validate(Draft(phone: "0538 274 19 05", sendInvite: true)).Value!.E164.ShouldBe("+905382741905");

    [Fact]
    public void Invite_WithoutPhone_IsValidationFailed() =>
        StaffRules.Validate(Draft(phone: null, sendInvite: true)).Error!.FieldErrors["phone"].ShouldBe(["required"]);

    [Fact]
    public void StaffWithoutPhoneAndInvite_IsAllowed() =>
        StaffRules.Validate(Draft(phone: null, sendInvite: false)).Value.ShouldBeNull();

    [Theory]
    [InlineData("0216 330 40 40")]
    [InlineData("abc")]
    public void LandlineOrInvalidPhone_IsValidationFailed(string phone) =>
        StaffRules.Validate(Draft(phone: phone, sendInvite: true)).Error!.FieldErrors["phone"].ShouldBe(["invalid_format"]);

    [Fact]
    public void InvalidFields_AreAllReported()
    {
        var result = StaffRules.Validate(new StaffDraft("D", null, StaffRole.Staff, "yeşil", [], 120, -1, SendInvite: false));

        result.Error!.FieldErrors.Keys.Order(StringComparer.Ordinal)
            .ShouldBe(["branchIds", "calendarColor", "fullName", "saleCommissionPercent", "serviceCommissionPercent"]);
    }

    private static StaffDraft Draft(string? phone, bool sendInvite) =>
        new("Deniz Kara", phone, StaffRole.Staff, "#2E7D32", [Kadikoy], 22, 10, sendInvite);

    private static WeeklyHours Hours(IsoDayOfWeek day, (LocalTime Start, LocalTime End) interval) =>
        WeeklyHours.Create([(day, [interval])]).Value;
}
