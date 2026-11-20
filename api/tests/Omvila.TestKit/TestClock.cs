using NodaTime;
using NodaTime.Testing;

namespace Omvila.TestKit;

/// <summary>Testlerin sabit saati: 2026-11-25T09:00:00Z (Salı, 12:00 İstanbul). Testler gerçek saate bağlı olamaz.</summary>
public static class TestClock
{
    public static readonly Instant Start = Instant.FromUtc(2026, 11, 25, 9, 0);

    public static FakeClock Create() => new(Start);
}
