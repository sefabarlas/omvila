using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Markanın sabit havale kodları (alan-modeli.md § Subscription): abonelik <c>ABN-KE-1125</c>, kontör <c>KNT-KE-1125</c>. Orta
/// kısım işletme adının ilk iki sözcüğünün baş harfleri (ASCII), son kısım rastgele rakamlar; ikisi aynı soneki paylaşır.
/// Benzersizlik veritabanı indeksiyle güvence altındadır; üretici çakışmayı önceden sorar.
/// </summary>
internal sealed record TransferCodes(string Subscription, string Credits)
{
    public const string SubscriptionPrefix = "ABN";
    public const string CreditsPrefix = "KNT";
    private const string FallbackInitials = "OM";

    public static TransferCodes Create(string? brandName, int digits)
    {
        var max = 1;
        for (var i = 0; i < digits; i++)
        {
            max *= 10;
        }

        var number = RandomNumberGenerator.GetInt32(0, max).ToString(new string('0', digits), CultureInfo.InvariantCulture);
        var suffix = $"{Initials(brandName)}-{number}";
        return new TransferCodes($"{SubscriptionPrefix}-{suffix}", $"{CreditsPrefix}-{suffix}");
    }

    internal static string Initials(string? brandName)
    {
        var letters = (brandName ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(word => Fold(word).FirstOrDefault(char.IsAsciiLetter))
            .Where(c => c != default)
            .Take(2)
            .Select(char.ToUpperInvariant)
            .ToArray();
        return letters.Length switch
        {
            0 => FallbackInitials,
            1 => new string([letters[0], letters[0]]),
            _ => new string(letters),
        };
    }

    // Türkçe harfleri ASCII karşılığına çevirir (Ş → S, İ → I, ğ → g); diğer işaretler ayrıştırılıp atılır.
    private static string Fold(string word)
    {
        var mapped = word.Replace('ı', 'i').Replace('İ', 'I');
        var decomposed = mapped.Normalize(NormalizationForm.FormD);
        return new string([.. decomposed.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)]);
    }
}
