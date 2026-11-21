using System.Globalization;
using NodaTime;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// SMS değişken biçimleri (bildirimler.md § SMS: tarih "Sal 25 Kas", saat "14:30"; urun-kurallari.md § Biçimler, web
/// formatDate ile aynı kısaltmalar). Zaman yalnız şubenin saat diliminde gösterilir.
/// </summary>
internal static class SmsFormat
{
    private static readonly string[] Months = ["Oca", "Şub", "Mar", "Nis", "May", "Haz", "Tem", "Ağu", "Eyl", "Eki", "Kas", "Ara"];

    // IsoDayOfWeek: 1 = Pazartesi.
    private static readonly string[] Weekdays = ["Pzt", "Sal", "Çar", "Per", "Cum", "Cmt", "Paz"];

    public static string Date(LocalDate date) =>
        $"{Weekdays[(int)date.DayOfWeek - 1]} {date.Day.ToString(CultureInfo.InvariantCulture)} {Months[date.Month - 1]}";

    public static string Time(LocalTime time) => time.ToString("HH:mm", CultureInfo.InvariantCulture);

    /// <summary>
    /// Bağlantı şemasız yazılır ("omvila.com/r/…", bildirimler.md § Bağlantılar): 8 birim kazanılır, telefonlar
    /// alan adını yine bağlantı olarak tanır. Yalnız https kaldırılır (geliştirme ortamındaki http olduğu gibi kalır).
    /// </summary>
    public static string Link(Uri url)
    {
        ArgumentNullException.ThrowIfNull(url);
        var text = url.AbsoluteUri;
        return text.StartsWith("https://", StringComparison.Ordinal) ? text["https://".Length..] : text;
    }

    /// <summary>Birden çok kalemde ilk ad ve kalan sayı: "Saç kesimi +1".</summary>
    public static string FirstAndRest(IReadOnlyList<string> names) =>
        names.Count switch
        {
            0 => string.Empty,
            1 => names[0],
            _ => $"{names[0]} +{(names.Count - 1).ToString(CultureInfo.InvariantCulture)}",
        };
}
