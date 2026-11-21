namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// SMS parça hesabı (3GPP TS 23.038/23.040). Kontör = parça sayısı. GSM-7 Türkçe: Türkçe ulusal dil başlığı (3 oktet)
/// her mesajda varsayılır, tek mesaj <see cref="GsmLength.SingleSmsLimit"/> (155) birim; birleştirilmiş mesajda başlığa
/// birleştirme bilgisi (6 oktet) eklenir, parça başına 149 birim ve kaydırma karakteri (2 birim) iki parçaya bölünmez.
/// UCS-2: tek mesaj 70, parça başına 67 kod birimi; vekil çift bölünmez.
/// </summary>
internal static class SmsEncoding
{
    public const int Gsm7MultipartLimit = 149;

    public const int Ucs2SingleLimit = 70;

    public const int Ucs2MultipartLimit = 67;

    public static SmsMeasure Measure(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (text.Length == 0)
        {
            return new SmsMeasure(SmsEncodingKind.Gsm7Turkish, 0, 1);
        }

        return GsmLength.Units(text) is { } units ? MeasureGsm(text, units) : MeasureUcs2(text);
    }

    private static SmsMeasure MeasureGsm(string text, int units)
    {
        if (units <= GsmLength.SingleSmsLimit)
        {
            return new SmsMeasure(SmsEncodingKind.Gsm7Turkish, units, 1);
        }

        var segments = 1;
        var used = 0;
        foreach (var c in text)
        {
            var charUnits = GsmLength.CharUnits(c)!.Value;
            if (used + charUnits > Gsm7MultipartLimit)
            {
                segments++;
                used = 0;
            }

            used += charUnits;
        }

        return new SmsMeasure(SmsEncodingKind.Gsm7Turkish, units, segments);
    }

    private static SmsMeasure MeasureUcs2(string text)
    {
        if (text.Length <= Ucs2SingleLimit)
        {
            return new SmsMeasure(SmsEncodingKind.Ucs2, text.Length, 1);
        }

        var segments = 1;
        var used = 0;
        for (var i = 0; i < text.Length; i++)
        {
            var width = char.IsHighSurrogate(text[i]) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]) ? 2 : 1;
            if (used + width > Ucs2MultipartLimit)
            {
                segments++;
                used = 0;
            }

            used += width;
            i += width - 1;
        }

        return new SmsMeasure(SmsEncodingKind.Ucs2, text.Length, segments);
    }
}
