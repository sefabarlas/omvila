using NodaTime;
using NodaTime.Testing;
using Omvila.SharedKernel;

namespace Omvila.SharedKernel.Tests;

public sealed class UuidV7IdGeneratorTests
{
    private static readonly Instant Start = Instant.FromUtc(2026, 11, 25, 9, 0);

    [Fact]
    public void New_IsVersion7_AndEmbedsInjectedClockTime()
    {
        var generator = new UuidV7IdGenerator(new FakeClock(Start));

        var id = generator.New();

        id.Version.ShouldBe(7);
        var bytes = id.ToByteArray(bigEndian: true);
        var unixMs = ((long)bytes[0] << 40) | ((long)bytes[1] << 32) | ((long)bytes[2] << 24)
            | ((long)bytes[3] << 16) | ((long)bytes[4] << 8) | bytes[5];
        unixMs.ShouldBe(Start.ToUnixTimeMilliseconds());
    }

    [Fact]
    public void New_IsOrderedByClockTime()
    {
        var clock = new FakeClock(Start);
        var generator = new UuidV7IdGenerator(clock);

        var first = generator.New();
        clock.Advance(Duration.FromMilliseconds(1));
        var second = generator.New();

        string.CompareOrdinal(first.ToString(), second.ToString()).ShouldBeLessThan(0);
    }

    // Sabit saatte (test) veya aynı milisaniyede üretilen kimlikler de üretim sırasını korur: "son kayıt" seçen sorgular
    // (ör. rıza defteri, ORDER BY occurred_utc, id) aynı an damgalı satırlarda belirsiz kalmaz (T-113).
    [Fact]
    public void New_IsStrictlyIncreasing_WithinTheSameMillisecond()
    {
        var generator = new UuidV7IdGenerator(new FakeClock(Start));

        var ids = Enumerable.Range(0, 2_000).Select(_ => generator.New()).ToList();

        for (var i = 1; i < ids.Count; i++)
        {
            ids[i].CompareTo(ids[i - 1]).ShouldBeGreaterThan(0, "Guid sıralaması (bellekte)");
            ids[i].ToByteArray(bigEndian: true).AsSpan().SequenceCompareTo(ids[i - 1].ToByteArray(bigEndian: true)).ShouldBeGreaterThan(0, "uuid sıralaması (PostgreSQL)");
            ids[i].Version.ShouldBe(7);
            (ids[i].Variant & 0b1100).ShouldBe(0b1000, "RFC 9562 varyantı");
        }
    }

    [Fact]
    public void New_StaysIncreasing_WhenTheClockGoesBack()
    {
        var clock = new FakeClock(Start);
        var generator = new UuidV7IdGenerator(clock);

        var first = generator.New();
        clock.Reset(Start - Duration.FromSeconds(1));
        var second = generator.New();

        second.CompareTo(first).ShouldBeGreaterThan(0);
    }
}
