using NodaTime;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: alan-modeli.md § AppointmentSeries (WEEKLY;INTERVAL=1|2|4, ilk 3 ay somut, çakışan atlanır, "bu ve sonrakiler"
// seriyi böler), dilim-02 kabul kriteri 13 ("Her 4 haftada bir", yalnız bu / bu ve sonrakiler), musaitlik.md
// § Kavramlar (yerel → anlık dönüşüm, M20 saat dilimi).
public sealed class RecurrenceTests
{
    private static readonly LocalDate Tuesday = new(2026, 11, 25);
    private static readonly LocalTime At1430 = new(14, 30);
    private static readonly WeeklyRecurrence Every4Weeks = WeeklyRecurrence.Create(4).Value;

    [Fact]
    public void Every4Weeks_For3Months_ProducesFourOccurrences()
    {
        var plan = RecurrenceGenerator.Generate(Tuesday, At1430, Offers.Istanbul, Every4Weeks, endsOn: null, canBook: _ => true);

        plan.Planned.Select(o => o.Date).ShouldBe([new(2026, 11, 25), new(2026, 12, 23), new(2027, 1, 20), new(2027, 2, 17)]);
        plan.Skipped.ShouldBeEmpty();
        plan.GeneratedUntil.ShouldBe(new LocalDate(2027, 2, 24));
        plan.Planned[0].StartUtc.ShouldBe(Offers.Tuesday1430);
    }

    [Fact]
    public void Weekly_For3Months_ProducesFourteenOccurrences() =>
        RecurrenceGenerator.Generate(Tuesday, At1430, Offers.Istanbul, WeeklyRecurrence.Create(1).Value, null, _ => true)
            .Planned.Count.ShouldBe(14, "25 Kas + 13 hafta = 24 Şub, ufuk 25 Şub hariç");

    [Fact]
    public void ConflictingOccurrences_AreSkipped()
    {
        var plan = RecurrenceGenerator.Generate(Tuesday, At1430, Offers.Istanbul, Every4Weeks, null, o => o.Date != new LocalDate(2026, 12, 23));

        plan.Skipped.ShouldBe([new LocalDate(2026, 12, 23)]);
        plan.Planned.Count.ShouldBe(3);
    }

    [Fact]
    public void EndsOn_IsInclusive_AndCapsGeneratedUntil()
    {
        var plan = RecurrenceGenerator.Generate(Tuesday, At1430, Offers.Istanbul, Every4Weeks, endsOn: new LocalDate(2026, 12, 23), canBook: _ => true);

        plan.Planned.Select(o => o.Date).ShouldBe([new(2026, 11, 25), new(2026, 12, 23)]);
        plan.GeneratedUntil.ShouldBe(new LocalDate(2026, 12, 23));
    }

    [Fact]
    public void Extend_ContinuesAfterGeneratedUntil_OnTheSameCadence()
    {
        var plan = RecurrenceGenerator.Extend(
            Tuesday, At1430, Offers.Istanbul, Every4Weeks, endsOn: null,
            generatedUntil: new LocalDate(2027, 2, 24), untilExclusive: new LocalDate(2027, 4, 1), canBook: _ => true);

        plan.Planned.Select(o => o.Date).ShouldBe([new(2027, 3, 17)]);
        plan.GeneratedUntil.ShouldBe(new LocalDate(2027, 3, 31));
    }

    [Fact]
    public void LocalStart_IsResolvedInTheBranchZone_AcrossDst()
    {
        // Berlin yaz saatine 28 Mart 2027'de geçer: yerel 10:00 önce 09:00Z, sonra 08:00Z (M20).
        var berlin = DateTimeZoneProviders.Tzdb["Europe/Berlin"];
        var plan = RecurrenceGenerator.Generate(new LocalDate(2027, 3, 21), new LocalTime(10, 0), berlin, WeeklyRecurrence.Create(1).Value, new LocalDate(2027, 3, 28), _ => true);

        plan.Planned.Select(o => o.StartUtc).ShouldBe([Instant.FromUtc(2027, 3, 21, 9, 0), Instant.FromUtc(2027, 3, 28, 8, 0)]);
    }

    [Fact]
    public void Affected_ThisOnly_AndThisAndFollowing()
    {
        LocalDate[] dates = [new(2026, 11, 25), new(2026, 12, 23), new(2027, 1, 20), new(2027, 2, 17)];

        RecurrenceGenerator.Affected(dates, d => d, new LocalDate(2026, 12, 23), SeriesScope.ThisOnly).ShouldBe([new LocalDate(2026, 12, 23)]);
        RecurrenceGenerator.Affected(dates, d => d, new LocalDate(2026, 12, 23), SeriesScope.ThisAndFollowing)
            .ShouldBe([new(2026, 12, 23), new(2027, 1, 20), new(2027, 2, 17)]);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public void Rule_RoundTripsAsRRule(int weeks)
    {
        var rule = WeeklyRecurrence.Create(weeks).Value;

        rule.ToRRule().ShouldBe($"FREQ=WEEKLY;INTERVAL={weeks}");
        WeeklyRecurrence.Parse(rule.ToRRule()).ShouldBe(rule);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(8)]
    public void Rule_AllowsOnly1_2_4Weeks(int weeks) =>
        WeeklyRecurrence.Create(weeks).Error!.FieldErrors["recurrence.intervalWeeks"].ShouldBe(["invalid_format"]);

    [Fact]
    public void Series_ThisAndFollowingCancel_EndsTheSeriesTheDayBefore()
    {
        var series = Series();

        series.EndBefore(new LocalDate(2026, 12, 23), Offers.Tuesday1430);

        series.EndsOn.ShouldBe(new LocalDate(2026, 12, 22));
        series.GeneratedUntil.ShouldBe(new LocalDate(2026, 12, 22));
        series.Status.ShouldBe(SeriesStatus.Active);
        RecurrenceGenerator.Dates(series.StartsOn, series.Rule, new LocalDate(2027, 12, 31), series.EndsOn).ShouldBe([Tuesday]);
    }

    [Fact]
    public void Series_ThisAndFollowingFromTheFirstDate_EndsTheSeries()
    {
        var series = Series();

        series.EndBefore(Tuesday, Offers.Tuesday1430);

        series.Status.ShouldBe(SeriesStatus.Ended);
    }

    [Fact]
    public void Series_SplitFrom_EndsTheOldAndStartsANewSeriesAtTheDate()
    {
        var series = Series();
        var splitDate = new LocalDate(2027, 1, 20);
        var newId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000f2");

        var next = series.SplitFrom(splitDate, newId, new LocalTime(16, 0), [new SeriesItemTemplate(Offers.HaircutId, Offers.Zeynep, 0)], Offers.Tuesday1430);

        series.EndsOn.ShouldBe(new LocalDate(2027, 1, 19));
        next.Id.ShouldBe(newId);
        next.StartsOn.ShouldBe(splitDate);
        next.LocalStartTime.ShouldBe(new LocalTime(16, 0));
        next.RecurrenceRule.ShouldBe(series.RecurrenceRule);
        next.GeneratedUntil.ShouldBe(new LocalDate(2027, 2, 24));
        next.TemplateItems.ShouldHaveSingleItem().StaffId.ShouldBe(Offers.Zeynep);
        RecurrenceGenerator.Dates(series.StartsOn, series.Rule, new LocalDate(2027, 2, 25), series.EndsOn).ShouldBe([new(2026, 11, 25), new(2026, 12, 23)]);
        RecurrenceGenerator.Dates(next.StartsOn, next.Rule, new LocalDate(2027, 2, 25), next.EndsOn).ShouldBe([new(2027, 1, 20), new(2027, 2, 17)]);
    }

    private static AppointmentSeries Series() => AppointmentSeries.Create(
        Guid.Parse("0193a8f0-0000-7000-8000-0000000000f1"),
        Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1"),
        Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2"),
        Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1"),
        Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2"),
        [new SeriesItemTemplate(Offers.HaircutId, Offers.Elif, 0)],
        Every4Weeks,
        Tuesday,
        At1430,
        endsOn: null,
        generatedUntil: new LocalDate(2027, 2, 24),
        Offers.Tuesday1430);
}
