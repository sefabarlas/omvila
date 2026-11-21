using System.Globalization;
using NodaTime;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// İşletme bildirimi değişkenleri (bildirimler.md § Push: kilit ekranında "Ad S."; panelde kalan süre "1 sa 35 dk";
/// sayılar Türkçe binlik ayırıcıyla "1.240").
/// </summary>
internal static class StaffText
{
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    /// <summary>"Pınar Yalçın" → "Pınar Y."; tek kelimelik ad olduğu gibi kalır.</summary>
    public static string ShortName(string fullName)
    {
        ArgumentNullException.ThrowIfNull(fullName);
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length switch
        {
            0 => string.Empty,
            1 => parts[0],
            _ => $"{string.Join(' ', parts[..^1])} {char.ToUpper(parts[^1][0], Turkish)}.",
        };
    }

    /// <summary>Kalan süre dakikaya yukarı yuvarlanır: "1 sa 35 dk", "2 sa", "45 dk"; geçmişse "0 dk".</summary>
    public static string Remaining(Duration remaining)
    {
        var minutes = (long)Math.Max(0, Math.Ceiling(remaining.TotalMinutes));
        var (hours, rest) = (minutes / 60, minutes % 60);
        return (hours, rest) switch
        {
            (0, _) => $"{rest.ToString(CultureInfo.InvariantCulture)} dk",
            (_, 0) => $"{hours.ToString(CultureInfo.InvariantCulture)} sa",
            _ => $"{hours.ToString(CultureInfo.InvariantCulture)} sa {rest.ToString(CultureInfo.InvariantCulture)} dk",
        };
    }

    /// <summary>Kapora tutarı ₺ ile: 84000 → "840 ₺", 125050 → "1.250,50 ₺" (urun-kurallari.md § Biçimler).</summary>
    public static string Amount(long amountMinor) => $"{SmsFormat.Amount(amountMinor)} ₺";

    /// <summary>1240 → "1.240".</summary>
    public static string Count(int value) => value.ToString("N0", Turkish);
}
