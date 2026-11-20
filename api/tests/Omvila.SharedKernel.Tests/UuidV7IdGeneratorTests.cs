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
}
