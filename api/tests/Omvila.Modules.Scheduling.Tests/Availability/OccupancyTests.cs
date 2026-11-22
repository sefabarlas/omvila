using NodaTime;
using Omvila.Modules.Scheduling.Domain.Availability;

namespace Omvila.Modules.Scheduling.Tests.Availability;

// Kural: sözleşme manageGetOverview (doluluk = dolu dakika / çalışma dakikası; izin ve kapalı gün düşülür); musaitlik.md § 1–3;
// docs/acik-sorular.md T-120. Hafta: Pazartesi 23 – Pazar 29 Kasım 2026; şube Pzt–Cmt 10:00–20:00, pazar kapalı.
public sealed class OccupancyTests
{
    private static readonly LocalDate Monday = new(2026, 11, 23);
    private static readonly LocalDate Sunday = Monday.PlusDays(6);
    private static readonly long Hour = (long)Duration.FromHours(1).TotalSeconds;

    [Fact]
    public void Measure_WorkingIsStaffHoursWithinBranchHours_SundayClosed()
    {
        var scenario = new Scenario().Staff(Scenario.Elif, hours: Scenario.Week(new LocalTime(9, 0), new LocalTime(21, 0), sunday: true));
        var snapshot = scenario.Build();

        var (booked, working) = Occupancy.Measure(snapshot, snapshot.Staff[Scenario.Elif], Monday, Sunday, []);

        working.ShouldBe(60 * Hour, "6 gün × 10 saat; şube saatleri dışı ve kapalı pazar sayılmaz");
        booked.ShouldBe(0);
    }

    [Fact]
    public void Measure_TimeOffIsDeducted_BookingOutsideWorkingIgnored()
    {
        var scenario = new Scenario().Staff(Scenario.Elif);
        scenario.TimeOff(Scenario.Elif, scenario.At(Scenario.Tuesday, 0, 0), scenario.At(Scenario.Tuesday.PlusDays(1), 0, 0));
        var snapshot = scenario.Build();
        var wednesday = Scenario.Tuesday.PlusDays(1);
        Interval[] busy =
        [
            new(scenario.At(wednesday, 10, 0), scenario.At(wednesday, 12, 0)),
            new(scenario.At(wednesday, 11, 0), scenario.At(wednesday, 12, 30)),
            new(scenario.At(wednesday, 20, 0), scenario.At(wednesday, 21, 0)),
        ];

        var (booked, working) = Occupancy.Measure(snapshot, snapshot.Staff[Scenario.Elif], Monday, Sunday, busy);

        working.ShouldBe(50 * Hour, "izinli salı düşülür");
        booked.ShouldBe(Hour * 5 / 2, "örtüşen dilimler bir kez, çalışma saati dışı sayılmaz");
        Occupancy.Percent(booked, working).ShouldBe(5);
    }

    [Fact]
    public void Measure_BranchClosedOverride_IsDeducted()
    {
        var snapshot = new Scenario().Staff(Scenario.Elif).BranchOverride(Scenario.Tuesday, []).Build();

        Occupancy.Measure(snapshot, snapshot.Staff[Scenario.Elif], Monday, Sunday, []).WorkingSeconds.ShouldBe(50 * Hour);
    }

    [Theory]
    [InlineData(0L, 0L, null)]
    [InlineData(37L, 50L, 74)]
    [InlineData(1L, 200L, 1)]
    [InlineData(1L, 201L, 0)]
    [InlineData(10L, 10L, 100)]
    public void Percent_RoundsHalfUp_NullWithoutWorkingTime(long booked, long working, int? expected)
    {
        Occupancy.Percent(booked, working).ShouldBe(expected);
    }
}
