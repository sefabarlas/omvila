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
            if (Basic.Contains(c, StringComparison.Ordinal))
            {
                units += 1;
            }
            else if (TurkishShift.Contains(c, StringComparison.Ordinal))
            {
                units += 2;
            }
            else
            {
                return null;
            }
        }

        return units;
    }
}
