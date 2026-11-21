using CsCheck;
using NodaTime;
using Omvila.Modules.Scheduling.Domain.Availability;

namespace Omvila.Modules.Scheduling.Tests.Availability;

// Kural: musaitlik.md § Kavramlar ([Start, End) yarı açık aralık; Intersect, Subtract, Union/Merge, Split özellik tabanlı
// testlerle). Model: 0–200 dakikalık bit dizisi; her işlem bit dizisi işlemine eşit olmalı.
public sealed class IntervalSetTests
{
    private const int Horizon = 200;
    private static readonly Instant Origin = Instant.FromUtc(2026, 11, 24, 7, 0);

    private static readonly Gen<Interval> GenInterval =
        Gen.Select(Gen.Int[0, Horizon], Gen.Int[0, 40], (start, length) => Span(start, Math.Min(Horizon, start + length)));

    private static readonly Gen<IntervalSet> GenSet = GenInterval.Array[0, 8].Select(intervals => IntervalSet.From(intervals));

    [Fact]
    public void From_IsNormal_SortedDisjointNonAdjacentNonEmpty() =>
        GenSet.Sample(set =>
        {
            set.Intervals.ShouldAllBe(i => i.Start < i.End);
            set.Intervals.Zip(set.Intervals.Skip(1)).ShouldAllBe(pair => pair.First.End < pair.Second.Start);
        });

    [Fact]
    public void Union_MatchesBitwiseOr() =>
        Gen.Select(GenSet, GenSet).Sample((a, b) => Bits(a.Union(b)).ShouldBe(Zip(Bits(a), Bits(b), (x, y) => x || y)));

    [Fact]
    public void Intersect_MatchesBitwiseAnd() =>
        Gen.Select(GenSet, GenSet).Sample((a, b) => Bits(a.Intersect(b)).ShouldBe(Zip(Bits(a), Bits(b), (x, y) => x && y)));

    [Fact]
    public void Subtract_MatchesAndNot() =>
        Gen.Select(GenSet, GenSet).Sample((a, b) => Bits(a.Subtract(b)).ShouldBe(Zip(Bits(a), Bits(b), (x, y) => x && !y)));

    [Fact]
    public void Operations_ReturnNormalSets() =>
        Gen.Select(GenSet, GenSet).Sample((a, b) =>
        {
            foreach (var result in new[] { a.Union(b), a.Intersect(b), a.Subtract(b) })
            {
                IntervalSet.From(result.Intervals).ShouldBe(result);
            }
        });

    [Fact]
    public void Split_PartitionsTheSetAtTheInstant() =>
        Gen.Select(GenSet, Gen.Int[0, Horizon]).Sample((set, minute) =>
        {
            var at = Origin + Duration.FromMinutes(minute);
            var (before, after) = set.Split(at);
            before.Union(after).ShouldBe(set);
            before.Intersect(after).IsEmpty.ShouldBeTrue();
            before.Intervals.ShouldAllBe(i => i.End <= at);
            after.Intervals.ShouldAllBe(i => i.Start >= at);
        });

    [Fact]
    public void Contains_And_Overlaps_MatchTheBitModel() =>
        Gen.Select(GenSet, GenInterval).Sample((set, interval) =>
        {
            var bits = Bits(set);
            var from = Minute(interval.Start);
            var to = Minute(interval.End);
            set.Contains(interval).ShouldBe(Enumerable.Range(from, to - from).All(m => bits[m]));
            set.Overlaps(interval).ShouldBe(Enumerable.Range(from, to - from).Any(m => bits[m]));
            set.Contains(interval.Start).ShouldBe(from < Horizon && bits[from]);
        });

    [Fact]
    public void HalfOpen_TouchingIntervalsMerge_ButDoNotOverlap()
    {
        var set = IntervalSet.Of(Span(0, 60), Span(60, 90));

        set.Intervals.ShouldBe([Span(0, 90)]);
        IntervalSet.Of(Span(0, 60)).Overlaps(Span(60, 90)).ShouldBeFalse("14:00'te biten ile 14:00'te başlayan çakışmaz");
        IntervalSet.Of(Span(0, 61)).Overlaps(Span(60, 90)).ShouldBeTrue("bir dakika örtüşen çakışır");
        set.TotalDuration.ShouldBe(Duration.FromMinutes(90));
    }

    private static Interval Span(int fromMinute, int toMinute) =>
        new(Origin + Duration.FromMinutes(fromMinute), Origin + Duration.FromMinutes(toMinute));

    private static int Minute(Instant instant) => (int)(instant - Origin).TotalMinutes;

    private static bool[] Bits(IntervalSet set)
    {
        var bits = new bool[Horizon + 1];
        foreach (var interval in set.Intervals)
        {
            for (var m = Minute(interval.Start); m < Minute(interval.End); m++)
            {
                bits[m] = true;
            }
        }

        return bits;
    }

    private static bool[] Zip(bool[] a, bool[] b, Func<bool, bool, bool> op) => [.. a.Zip(b, op)];
}
