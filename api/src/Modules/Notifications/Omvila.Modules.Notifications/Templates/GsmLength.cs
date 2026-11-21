namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// SMS uzunluğu, GSM 03.38 + Türkçe tekli kaydırma tablosu (3GPP TS 23.038): temel tablodaki karakter 1,
/// kaydırma tablosundaki (ğ ı ş ç İ Ğ Ş € ^ { } [ ] ~ \ |) 2 birim. Ulusal dil başlığıyla tek mesaj 155 birim
/// (bildirimler.md: ≤ 155 = 1 kontör). Tabloda olmayan karakter (emoji vb.) UCS-2'ye düşürür; şablonlarda yasak.
/// </summary>
internal static class GsmLength
{
    public const int SingleSmsLimit = 155;

    private const string Basic =
        "@£$¥èéùìòÇ\nØø\rÅåΔ_ΦΓΛΩΠΨΣΘΞÆæßÉ !\"#¤%&'()*+,-./0123456789:;<=>?¡ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÑÜ§¿abcdefghijklmnopqrstuvwxyzäöñüà";

    private const string TurkishShift = "ğışçİĞŞ€^{}[]~\\|\f";

    /// <summary>Birim sayısı; tabloda olmayan karakter varsa null.</summary>
    public static int? Units(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var units = 0;
        foreach (var c in text)
        {
            if (CharUnits(c) is not { } charUnits)
            {
                return null;
            }

            units += charUnits;
        }

        return units;
    }

    /// <summary>Tek karakterin birimi: temel tablo 1, kaydırma tablosu 2 (kaçış + kod); tabloda yoksa null.</summary>
    public static int? CharUnits(char c)
    {
        if (Basic.Contains(c, StringComparison.Ordinal))
        {
            return 1;
        }

        return TurkishShift.Contains(c, StringComparison.Ordinal) ? 2 : null;
    }
}
