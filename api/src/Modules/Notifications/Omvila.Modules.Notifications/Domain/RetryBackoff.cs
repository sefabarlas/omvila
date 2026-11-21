using NodaTime;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>
/// Üstel geri çekilme (dilim-03 kriter 10): n. başarısız denemeden sonra bekleme = taban × 2^(n−1), üst sınırla.
/// Taban 30 sn ile 30 sn, 1 dk, 2 dk, 4 dk; 5. başarısızlıkta mesaj Failed olur.
/// </summary>
internal static class RetryBackoff
{
    /// <param name="failedAttempts">Şimdiye kadarki başarısız deneme sayısı (≥ 1).</param>
    /// <param name="baseDelay">İlk gecikme.</param>
    /// <param name="maxDelay">Üst sınır.</param>
    public static Duration Delay(int failedAttempts, Duration baseDelay, Duration maxDelay)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(failedAttempts, 1);
        var exponent = Math.Min(failedAttempts - 1, 30);
        var ticks = baseDelay.BclCompatibleTicks * Math.Pow(2, exponent);
        return ticks >= maxDelay.BclCompatibleTicks ? maxDelay : Duration.FromTicks((long)ticks);
    }
}
