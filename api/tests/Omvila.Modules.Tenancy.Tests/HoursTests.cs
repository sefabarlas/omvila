using NodaTime;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Tests;

// Kural: dilim-01 kabul kriteri 5 (Salı 10:00–13:00 ve 13:45–19:00; örtüşen aralık ve gece yarısını aşan
// aralık validation_failed), kriter 6 (1 Ocak tek gün, Ramazan Bayramı 3 gün), alan-modeli.md § WorkingHoursOverride
public sealed class HoursTests
{
    private static LocalTime T(int hour, int minute = 0) => new(hour, minute);

    [Fact]
    public void Tuesday_WithLunchBreak_IsValid()
    {
        var hours = WeeklyHours.Create([(IsoDayOfWeek.Tuesday, [(T(13, 45), T(19)), (T(10), T(13))])]);

        hours.IsSuccess.ShouldBeTrue();
        hours.Value.Days[IsoDayOfWeek.Tuesday].Select(i => (i.Start, i.End)).ShouldBe([(T(10), T(13)), (T(13, 45), T(19))], "sıralı");
    }

    [Fact]
    public void OverlappingIntervals_AreValidationFailed()
    {
        var hours = WeeklyHours.Create([(IsoDayOfWeek.Tuesday, [(T(10), T(14)), (T(13, 45), T(19))])]);

        hours.Error!.Code.ShouldBe(ErrorCodes.ValidationFailed);
        hours.Error.FieldErrors["days[0].intervals"].ShouldBe(["overlapping"]);
    }

    [Fact]
    public void AdjacentIntervals_AreAllowed() =>
        WeeklyHours.Create([(IsoDayOfWeek.Friday, [(T(10), T(13)), (T(13), T(21))])]).IsSuccess.ShouldBeTrue();

    [Theory]
    [InlineData(22, 2)]
    [InlineData(19, 10)]
    [InlineData(10, 10)]
    public void IntervalEndingAtOrBeforeStart_IncludingAcrossMidnight_IsValidationFailed(int start, int end)
    {
        var hours = WeeklyHours.Create([(IsoDayOfWeek.Saturday, [(T(start), T(end))])]);

        hours.Error!.FieldErrors["days[0].intervals[0]"].ShouldBe(["invalid_range"]);
    }

    [Fact]
    public void MoreThanFourIntervals_IsValidationFailed() =>
        WeeklyHours.Create([(IsoDayOfWeek.Monday, [(T(8), T(9)), (T(10), T(11)), (T(12), T(13)), (T(14), T(15)), (T(16), T(17))])])
            .Error!.FieldErrors["days[0].intervals"].ShouldBe(["out_of_range"]);

    [Fact]
    public void SameDayTwice_IsValidationFailed() =>
        WeeklyHours.Create([(IsoDayOfWeek.Monday, [(T(9), T(12))]), (IsoDayOfWeek.Monday, [(T(13), T(18))])])
            .Error!.FieldErrors.ContainsKey("days[1].dayOfWeek").ShouldBeTrue();

    [Fact]
    public void EmptyWeek_IsClosed() =>
        WeeklyHours.Create([]).Value.HasAnyOpening.ShouldBeFalse();

    [Fact]
    public void NewYear_ClosedSingleDay()
    {
        var days = DayOverrides.Create(new LocalDate(2027, 1, 1), null, OverrideKind.Holiday, isClosed: true, [], "Yılbaşı").Value;

        days.Count.ShouldBe(1);
        days[0].ShouldBe(days[0] with { Date = new LocalDate(2027, 1, 1), IsClosed = true, Note = "Yılbaşı" });
    }

    [Fact]
    public void RamadanFeast_ThreeDays_ExpandsToEachDate()
    {
        var days = DayOverrides.Create(new LocalDate(2027, 3, 9), new LocalDate(2027, 3, 11), OverrideKind.Holiday, isClosed: true, [], "Ramazan Bayramı").Value;

        days.Select(d => d.Date).ShouldBe([new LocalDate(2027, 3, 9), new LocalDate(2027, 3, 10), new LocalDate(2027, 3, 11)]);
        days.ShouldAllBe(d => d.IsClosed && d.Note == "Ramazan Bayramı");
    }

    [Fact]
    public void ShortDay_WithHours_IsValid() =>
        DayOverrides.Create(new LocalDate(2026, 12, 31), null, OverrideKind.ShortDay, isClosed: false, [(T(10), T(16))], "Erken kapanış")
            .Value.Single().Intervals.Single().End.ShouldBe(T(16));

    [Fact]
    public void ClosedDayWithHours_IsValidationFailed() =>
        DayOverrides.Create(new LocalDate(2027, 1, 1), null, OverrideKind.Holiday, isClosed: true, [(T(10), T(12))], null)
            .Error!.FieldErrors["intervals"].ShouldBe(["invalid_format"]);

    [Theory]
    [InlineData("Closure", false)]
    [InlineData("ShortDay", true)]
    [InlineData("ExtraOpening", true)]
    public void KindAndClosedStateMustAgree(string kind, bool isClosed) =>
        DayOverrides.Create(new LocalDate(2027, 1, 2), null, Enum.Parse<OverrideKind>(kind), isClosed, isClosed ? [] : [(T(10), T(12))], null)
            .Error!.FieldErrors.ContainsKey("isClosed").ShouldBeTrue();

    [Fact]
    public void ToDateBeforeFromDate_IsValidationFailed() =>
        DayOverrides.Create(new LocalDate(2027, 1, 5), new LocalDate(2027, 1, 4), OverrideKind.Closure, isClosed: true, [], null)
            .Error!.FieldErrors["toDate"].ShouldBe(["invalid_range"]);
}
