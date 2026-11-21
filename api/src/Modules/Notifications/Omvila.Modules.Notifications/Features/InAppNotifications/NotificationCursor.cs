using System.Buffers.Text;
using System.Globalization;
using System.Text;
using NodaTime;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>
/// Bildirim paneli imleci (api-kurallari.md § Liste ve sayfalama): sıra oluşma anı yeniden eskiye, eşitlikte kimlik büyükten
/// küçüğe; imleç son satırın (an, kimlik) çiftidir, istemci için opaktır.
/// </summary>
internal readonly record struct NotificationCursor(Instant CreatedUtc, Guid Id)
{
    public string Encode() =>
        Base64Url.EncodeToString(Encoding.UTF8.GetBytes($"{CreatedUtc.ToUnixTimeTicks().ToString(CultureInfo.InvariantCulture)}|{Id:N}"));

    public static bool TryDecode(string? text, out NotificationCursor cursor)
    {
        cursor = default;
        if (string.IsNullOrEmpty(text) || !Base64Url.IsValid(text))
        {
            return false;
        }

        var parts = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(text)).Split('|');
        if (parts.Length != 2
            || !long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var ticks)
            || !Guid.TryParseExact(parts[1], "N", out var id))
        {
            return false;
        }

        cursor = new NotificationCursor(Instant.FromUnixTimeTicks(ticks), id);
        return true;
    }
}
