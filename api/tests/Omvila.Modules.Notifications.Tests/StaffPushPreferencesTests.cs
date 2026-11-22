using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Tests;

// Kural: sözleşme MyNotificationPreferences (anahtar → olay eşlemesi; gün sonu özeti kapalı varsayılan),
// manageGetMyNotificationPreferences (staff.deposit_reported, owner.credits_low kapatılamaz); T-105.
public sealed class StaffPushPreferencesTests
{
    private static readonly Guid BrandId = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid UserId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1");

    [Theory]
    [InlineData(NotificationEvents.StaffAppointmentCreated, true)]
    [InlineData(NotificationEvents.StaffApprovalNeeded, true)]
    [InlineData(NotificationEvents.StaffAppointmentChanged, true)]
    [InlineData(NotificationEvents.StaffCustomerCheckedIn, true)]
    [InlineData(NotificationEvents.StaffWalkInQueue, true)]
    [InlineData(NotificationEvents.StaffReviewReceived, true)]
    [InlineData(NotificationEvents.StaffDailySummary, false)]
    [InlineData(NotificationEvents.StaffDepositReported, true)]
    [InlineData(NotificationEvents.OwnerCreditsLow, true)]
    public void Defaults_AllOnExceptDailySummary(string eventCode, bool expected) =>
        StaffPushPreferences.AllowsPush(NotificationPreference.Defaults(BrandId, UserId), eventCode).ShouldBe(expected);

    [Theory]
    [InlineData(NotificationEvents.StaffAppointmentCreated, false)]
    [InlineData(NotificationEvents.StaffApprovalNeeded, false)]
    [InlineData(NotificationEvents.StaffAppointmentChanged, false)]
    [InlineData(NotificationEvents.StaffCustomerCheckedIn, false)]
    [InlineData(NotificationEvents.StaffWalkInQueue, false)]
    [InlineData(NotificationEvents.StaffReviewReceived, false)]
    [InlineData(NotificationEvents.StaffDailySummary, false)]
    [InlineData(NotificationEvents.StaffDepositReported, true)]
    [InlineData(NotificationEvents.OwnerCreditsLow, true)]
    public void AllKeysOff_OnlyNonClosableEventsPass(string eventCode, bool expected)
    {
        var off = NotificationPreference.Create(BrandId, UserId, new NotificationPreferenceValues(false, false, false, false, false, false, false), NodaTime.Instant.FromUtc(2026, 11, 25, 9, 0));

        StaffPushPreferences.AllowsPush(off, eventCode).ShouldBe(expected);
    }
}
