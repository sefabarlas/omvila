using NodaTime;
using Omvila.Modules.Notifications.Templates;

namespace Omvila.Modules.Notifications.Tests;

// Kural: bildirimler.md § Push (kilit ekranında müşteri "Ad S."), dilim-03 § Kapsam (kalan süre "1 sa 35 dk"),
// sözleşme InAppNotification örneği ("1.240 kontör").
public sealed class StaffTextTests
{
    [Theory]
    [InlineData("Pınar Yalçın", "Pınar Y.")]
    [InlineData("Ayşe Nur  ışık", "Ayşe Nur I.")]
    [InlineData("Cem", "Cem")]
    [InlineData("  ", "")]
    public void ShortName_KeepsFirstNames_AndInitialOfSurname(string fullName, string expected) =>
        StaffText.ShortName(fullName).ShouldBe(expected);

    [Theory]
    [InlineData(95, "1 sa 35 dk")]
    [InlineData(120, "2 sa")]
    [InlineData(45, "45 dk")]
    [InlineData(-3, "0 dk")]
    public void Remaining_IsShortHoursAndMinutes(int minutes, string expected) =>
        StaffText.Remaining(Duration.FromMinutes(minutes)).ShouldBe(expected);

    [Fact]
    public void Remaining_RoundsUpToTheMinute() => StaffText.Remaining(Duration.FromSeconds(30)).ShouldBe("1 dk");

    [Fact]
    public void Count_UsesTurkishThousandsSeparator() => StaffText.Count(1240).ShouldBe("1.240");
}
