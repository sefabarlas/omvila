using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Tests;

// Kural: sözleşme BrandSettings aralıkları, sureler-ve-ayarlar.md (varsayılanlar geçerli olmalı).
public sealed class BrandSettingsTests
{
    private static readonly BrandSettingsValues Defaults = new(
        false, 120, true, true, 24, 12, 15, 120, 60, 15, 30, "RiskyCustomersOnly", 2, true, [1440, 120], true);

    [Fact]
    public void Defaults_AreValid() => BrandSettingsRules.Validate(Defaults).ShouldBeEmpty();

    [Fact]
    public void OutOfRangeValues_AreReportedPerField()
    {
        var errors = BrandSettingsRules.Validate(Defaults with
        {
            ApprovalTimeoutMinutes = 45,
            CancellationCutoffHours = 169,
            NoShowAfterMinutes = 4,
            MaxAdvanceDays = 0,
            SlotIntervalMinutes = 20,
            DepositPolicy = "Sometimes",
            RiskyCustomerNoShowThreshold = 11,
        });

        errors.Keys.Order(StringComparer.Ordinal).ShouldBe([
            "approvalTimeoutMinutes", "cancellationCutoffHours", "depositPolicy", "maxAdvanceDays", "noShowAfterMinutes",
            "riskyCustomerNoShowThreshold", "slotIntervalMinutes",
        ]);
        errors["depositPolicy"].ShouldBe(["invalid_format"]);
    }

    [Theory]
    [InlineData(new[] { 1440, 120, 60, 30 }, "too_long")]
    [InlineData(new[] { 20 }, "out_of_range")]
    [InlineData(new[] { 4321 }, "out_of_range")]
    public void ReminderOffsets_AreLimited(int[] offsets, string code) =>
        BrandSettingsRules.Validate(Defaults with { ReminderOffsetsMinutes = offsets })["reminderOffsetsMinutes"].ShouldBe([code]);

    [Fact]
    public void Apply_StoresRemindersDistinctAndDescending()
    {
        var settings = BrandSettings.CreateDefault(Guid.Empty);

        settings.Apply(Defaults with { ReminderOffsetsMinutes = [120, 1440, 120] });

        settings.ReminderOffsetsMinutes.ShouldBe([1440, 120]);
    }

    [Theory]
    [InlineData(new[] { 120, 120 }, "invalid_format")]
    [InlineData(new[] { 1440, 120, 60, 30 }, "too_long")]
    [InlineData(new[] { 20 }, "out_of_range")]
    [InlineData(new int[0], null)]
    public void NotificationOffsets_RejectDuplicates_OnTopOfTheRanges(int[] offsets, string? code) =>
        BrandSettingsRules.ValidateNotificationOffsets(offsets).ShouldBe(code);

    [Fact]
    public void ApplyNotificationSettings_ChangesOnlyRemindersAndReviewInvite()
    {
        var settings = BrandSettings.CreateDefault(Guid.Empty);

        settings.ApplyNotificationSettings([120, 2880], reviewInviteEnabled: false);

        settings.ReminderOffsetsMinutes.ShouldBe([2880, 120]);
        settings.ReviewInviteEnabled.ShouldBeFalse();
        settings.CancellationCutoffHours.ShouldBe(24);
    }
}
