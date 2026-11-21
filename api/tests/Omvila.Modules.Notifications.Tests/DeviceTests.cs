using NodaTime;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Templates;

namespace Omvila.Modules.Notifications.Tests;

// Kural: alan-modeli.md § Device (jeton doğal anahtar; aynı jeton yeni sahibe geçer; geçersiz jeton pasif) ve
// sureler-ve-ayarlar.md QuietHoursLocal (22:00–08:00 şube yerel; sabaha ertelenir).
public sealed class DeviceTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid Elif = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1");
    private static readonly Guid Mert = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2");

    [Fact]
    public void Refresh_MovesTokenToNewOwner_AndReactivates()
    {
        var device = Device.RegisterForUser(Guid.Parse("0193a8f0-0000-7000-8000-0000000000e1"), Brand, Elif, DevicePlatform.iOS, "token-0123456789abcdefgh", "1.0.0", Now);
        device.Deactivate();

        device.Refresh(Mert, DevicePlatform.Android, "1.0.1", Now.Plus(Duration.FromMinutes(5)));

        device.OwnerId.ShouldBe(Mert);
        device.IsActive.ShouldBeTrue();
        device.Platform.ShouldBe(DevicePlatform.Android);
        device.AppVersion.ShouldBe("1.0.1");
        device.LastSeenUtc.ShouldBe(Now.Plus(Duration.FromMinutes(5)));
    }

    [Theory]
    [InlineData(19, 30, 2026, 11, 26, 5, 0)] // 22:30 → ertesi gün 08:00
    [InlineData(3, 0, 2026, 11, 25, 5, 0)] // 06:00 → aynı gün 08:00
    [InlineData(5, 0, 2026, 11, 25, 5, 0)] // 08:00 → olduğu gibi
    [InlineData(18, 59, 2026, 11, 25, 18, 59)] // 21:59 → olduğu gibi
    public void QuietHours_DeferToEightLocal(int hour, int minute, int y, int mo, int d, int expectedHour, int expectedMinute)
    {
        var zone = DateTimeZoneProviders.Tzdb["Europe/Istanbul"];

        QuietHours.Defer(Instant.FromUtc(2026, 11, 25, hour, minute), zone).ShouldBe(Instant.FromUtc(y, mo, d, expectedHour, expectedMinute));
    }
}
