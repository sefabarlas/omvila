using System.Globalization;
using NodaTime;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// SMS değişken biçimleri (bildirimler.md § SMS: tarih "Sal 25 Kas", saat "14:30"; urun-kurallari.md § Biçimler, web
/// formatDate ile aynı kısaltmalar). Zaman yalnız şubenin saat diliminde gösterilir.
/// </summary>
internal static class SmsFormat
{
    private static readonly CultureInfo TurkishNumbers = CultureInfo.GetCultureInfo("tr-TR");

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

    /// <summary>Tutar ₺ işaretsiz, binlik ayraçlı ("150", "1.250"; kuruş varsa "1.250,50"); şablon "TL" ekler.</summary>
    public static string Amount(long amountMinor)
    {
        var lira = amountMinor / 100m;
        return lira.ToString(amountMinor % 100 == 0 ? "#,0" : "#,0.00", TurkishNumbers);
    }

    /// <summary>
    /// Süre "2 saat", "1 sa 30 dk", "45 dk"; dakikaya yukarı yuvarlanır (gönderim birkaç saniye gecikse de havale süresi
    /// "2 saat" yazılır).
    /// </summary>
    public static string Duration(NodaTime.Duration duration)
    {
        var minutes = (long)Math.Ceiling(Math.Max(0, duration.TotalMinutes));
        var (hours, rest) = (minutes / 60, minutes % 60);
        return (hours, rest) switch
        {
            (0, _) => $"{rest.ToString(CultureInfo.InvariantCulture)} dk",
            (_, 0) => $"{hours.ToString(CultureInfo.InvariantCulture)} saat",
            _ => $"{hours.ToString(CultureInfo.InvariantCulture)} sa {rest.ToString(CultureInfo.InvariantCulture)} dk",
        };
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
