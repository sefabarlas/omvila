namespace Omvila.Modules.Customers.Tests;

/// <summary>Testlerin sabit kimlikleri (Guid üretimi IIdGenerator'a aittir; testte sıralı sabitler yeterli).</summary>
internal static class TestIds
{
    private static int _next;

    public static Guid Next() => Guid.Parse($"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _next):D12}");
}
