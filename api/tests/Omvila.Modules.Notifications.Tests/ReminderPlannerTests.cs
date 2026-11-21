using NodaTime;
using Omvila.Modules.Notifications.Templates;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Tests;

// Kural: durum-makinesi.md § ScheduleReminders, sureler-ve-ayarlar.md ReminderOffsetsMinutes [1440, 120] ve QuietHoursLocal
// (22:00–08:00, müşteri hatırlatması 08:00'e kayar), dilim-02 kabul kriteri 1 (24 sa / 2 sa hatırlatmaları).
public sealed class ReminderPlannerTests
{
    private static readonly DateTimeZone Istanbul = DateTimeZoneProviders.Tzdb["Europe/Istanbul"];
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0); // Çarşamba 12:00

    // Marka varsayılanı (BrandSettings.CreateDefault; sureler-ve-ayarlar.md ReminderOffsetsMinutes).
    private static readonly int[] DefaultOffsets = [1440, 120];

    [Fact]
    public void Afternoon_Appointment_DayBeforeAndTwoHoursBefore()
    {
        var start = Local(2026, 11, 27, 14, 30);

        var plan = ReminderPlanner.Plan(start, Now, Istanbul, DefaultOffsets);

        plan.ShouldBe([new PlannedReminder(Local(2026, 11, 26, 14, 30), null), new PlannedReminder(Local(2026, 11, 27, 12, 30), SmsTemplates.SameDayVariant)]);
    }

    [Fact]
    public void OffsetsAlreadyInThePast_AreSkipped()
    {
        ReminderPlanner.Plan(Local(2026, 11, 25, 13, 0), Now, Istanbul, DefaultOffsets).ShouldBeEmpty("12:00'de 13:00'e");
        ReminderPlanner.Plan(Local(2026, 11, 26, 11, 0), Now, Istanbul, DefaultOffsets)
            .ShouldBe([new PlannedReminder(Local(2026, 11, 26, 9, 0), SmsTemplates.SameDayVariant)]);
    }

    [Fact]
    public void QuietHours_MorningReminderMovesTo0800()
    {
        var plan = ReminderPlanner.Plan(Local(2026, 11, 27, 9, 0), Now, Istanbul, DefaultOffsets);

        plan.ShouldBe([new PlannedReminder(Local(2026, 11, 26, 9, 0), null), new PlannedReminder(Local(2026, 11, 27, 8, 0), SmsTemplates.SameDayVariant)]);
    }

    [Fact]
    public void QuietHours_LateAppointment_DayBeforeShiftsIntoSameDay_OnlyOneSameDayReminderKept()
    {
        var plan = ReminderPlanner.Plan(Local(2026, 11, 27, 23, 30), Now, Istanbul, DefaultOffsets);

        plan.ShouldBe([new PlannedReminder(Local(2026, 11, 27, 21, 30), SmsTemplates.SameDayVariant)]);
    }

    [Fact]
    public void ReminderShiftedPastStart_IsSkipped() =>
        ReminderPlanner.Plan(Local(2026, 11, 27, 7, 30), Now, Istanbul, [120])
            .ShouldBeEmpty("05:30 → 08:00 randevudan sonra");

    private static Instant Local(int year, int month, int day, int hour, int minute) =>
        Istanbul.AtStrictly(new LocalDateTime(year, month, day, hour, minute)).ToInstant();
}
