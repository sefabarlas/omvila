using Omvila.Modules.Notifications.Contracts;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// Sistem SMS şablonları (docs/standartlar/bildirimler.md § Olay kataloğu, metinler birebir).
/// Salonun şablon ezmesi ve NotificationTemplate tablosu D3'te.
/// </summary>
internal static class SmsTemplates
{
    private static readonly Dictionary<string, string> Bodies = new(StringComparer.Ordinal)
    {
        [NotificationEvents.OtpCode] = "OMVILA giriş kodunuz: {kod}. Bu kodu kimseyle paylaşmayın.",
    };

    public static IReadOnlyCollection<string> EventCodes => Bodies.Keys;

    public static string Render(string eventCode, IReadOnlyDictionary<string, string> values)
    {
        if (!Bodies.TryGetValue(eventCode, out var body))
        {
            throw new InvalidOperationException($"SMS şablonu yok: {eventCode}");
        }

        foreach (var (key, value) in values)
        {
            body = body.Replace("{" + key + "}", value, StringComparison.Ordinal);
        }

        return body.Contains('{', StringComparison.Ordinal)
            ? throw new InvalidOperationException($"Şablonda doldurulmamış değişken: {eventCode}")
            : body;
    }
}
