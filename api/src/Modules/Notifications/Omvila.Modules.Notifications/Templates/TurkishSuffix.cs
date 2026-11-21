namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// Türkçe bulunma eki ('da/'de/'ta/'te; bildirimler.md § ek uyumu: sabit "'te" yazılmaz). Ünlü uyumu son ünlüden,
/// ünsüz benzeşmesi son sesten (f s t k ç ş h p → t). Sayıda okunuşun son kelimesi esas alınır: saat "14:30" → "otuz"
/// → 'da, "14:00" → "on dört" → 'te.
/// </summary>
internal static class TurkishSuffix
{
    private const string BackVowels = "aıouAIOU";
    private const string FrontVowels = "eiöüEİÖÜ";
    private const string HardConsonants = "fstkçşhpFSTKÇŞHP";

    // Rakamların ve onlukların okunuşunun son sesi: (kalın ünlü mü, sert ünsüzle mi biter).
    private static readonly (bool Back, bool Hard)[] Units =
    [
        (true, false), // sıfır
        (false, false), // bir
        (false, false), // iki
        (false, true), // üç
        (false, true), // dört
        (false, true), // beş
        (true, false), // altı
        (false, false), // yedi
        (false, false), // sekiz
        (true, false), // dokuz
    ];

    private static readonly (bool Back, bool Hard)[] Tens =
    [
        (true, false), // (kullanılmaz)
        (true, false), // on
        (false, false), // yirmi
        (true, false), // otuz
        (true, true), // kırk
        (false, false), // elli
        (true, true), // altmış
        (false, true), // yetmiş
        (false, false), // seksen
        (true, false), // doksan
    ];

    /// <summary>Değer + kesme işareti + bulunma eki.</summary>
    public static string Locative(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var trimmed = value.TrimEnd();
        var (back, hard) = trimmed.Length > 0 && char.IsAsciiDigit(trimmed[^1]) ? OfNumber(trimmed) : OfWord(trimmed);
        return $"{trimmed}'{(hard ? 't' : 'd')}{(back ? 'a' : 'e')}";
    }

    private static (bool Back, bool Hard) OfWord(string word)
    {
        var last = word.LastOrDefault(char.IsLetter);
        var vowel = word.LastOrDefault(c => BackVowels.Contains(c, StringComparison.Ordinal) || FrontVowels.Contains(c, StringComparison.Ordinal));
        var back = vowel != default && BackVowels.Contains(vowel, StringComparison.Ordinal);
        return (back, last != default && HardConsonants.Contains(last, StringComparison.Ordinal));
    }

    /// <summary>Sondaki sayı; saatte dakika "00" ise saat okunur.</summary>
    private static (bool Back, bool Hard) OfNumber(string text)
    {
        var digits = TrailingDigits(text);
        if (digits.All(c => c == '0') && text.Length > digits.Length + 1 && text[^(digits.Length + 1)] == ':')
        {
            var hour = TrailingDigits(text[..^(digits.Length + 1)]);
            if (hour.Length > 0)
            {
                digits = hour;
            }
        }

        var number = 0;
        foreach (var c in digits.Length > 4 ? digits[^4..] : digits)
        {
            number = (number * 10) + (c - '0');
        }

        if (number % 10 != 0)
        {
            return Units[number % 10];
        }

        if (number % 100 != 0)
        {
            return Tens[number % 100 / 10];
        }

        // sıfır → 'da; yüz, bin → 'de.
        return number == 0 ? Units[0] : (false, false);
    }

    private static string TrailingDigits(string text)
    {
        var start = text.Length;
        while (start > 0 && char.IsAsciiDigit(text[start - 1]))
        {
            start--;
        }

        return text[start..];
    }
}
