using Omvila.Modules.Notifications.Templates;

namespace Omvila.Modules.Notifications.Tests;

// Kural: bildirimler.md § ek uyumu ("{salon|bulunma}"; sabit "'te" yazılmaz).
public sealed class TurkishSuffixTests
{
    [Theory]
    [InlineData("Kuaför Elif", "Kuaför Elif'te")]
    [InlineData("Berber Ali", "Berber Ali'de")]
    [InlineData("Saç Stüdyo", "Saç Stüdyo'da")]
    [InlineData("Güzel Kadın", "Güzel Kadın'da")]
    [InlineData("Salon Kırk", "Salon Kırk'ta")]
    [InlineData("14:30", "14:30'da")]
    [InlineData("14:15", "14:15'te")]
    [InlineData("14:00", "14:00'te")]
    [InlineData("10:00", "10:00'da")]
    [InlineData("12:00", "12:00'de")]
    [InlineData("09:40", "09:40'ta")]
    [InlineData("16:20", "16:20'de")]
    [InlineData("11:06", "11:06'da")]
    public void Locative_FollowsVowelHarmonyAndConsonantAssimilation(string value, string expected) =>
        TurkishSuffix.Locative(value).ShouldBe(expected);
}
