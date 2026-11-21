using NodaTime;
using Omvila.Modules.Notifications.Templates;

namespace Omvila.Modules.Notifications.Tests;

// Kural: bildirimler.md § SMS (tutar "150 TL", ₺ işareti SMS'te yok); deposit.awaiting "{süre} içinde"
public sealed class SmsFormatTests
{
    [Theory]
    [InlineData(84_000, "840")]
    [InlineData(125_000, "1.250")]
    [InlineData(125_050, "1.250,50")]
    [InlineData(1_250_000, "12.500")]
    public void Amount_IsLiraWithThousandsSeparator(long minor, string expected) => SmsFormat.Amount(minor).ShouldBe(expected);

    [Theory]
    [InlineData(120 * 60, "2 saat")]
    [InlineData((120 * 60) - 3, "2 saat")]
    [InlineData(90 * 60, "1 sa 30 dk")]
    [InlineData(45 * 60, "45 dk")]
    [InlineData(-5, "0 dk")]
    public void Duration_RoundsUpToMinutes(int seconds, string expected) => SmsFormat.Duration(Duration.FromSeconds(seconds)).ShouldBe(expected);
}
