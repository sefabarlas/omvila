using System.Text;
using System.Text.RegularExpressions;

namespace Omvila.Modules.Identity.Domain;

/// <summary>
/// Kişinin kendi yazdığı ad soyad (alan-modeli.md § User, T-130; sözleşme SignupRequest.ownerFullName). Tek değerdir,
/// ad ve soyad ayrılmaz. Ad kişisel veridir: günlüğe ve hata iletisine yazılmaz; doğrulama yalnız hata kodu döner.
/// </summary>
internal static partial class PersonName
{
    public const int MinLength = 2;

    public const int MaxLength = 120;

    /// <summary>Baştaki ve sondaki boşluk kırpılır, aradaki boşluk dizileri tek boşluğa iner (NFC).</summary>
    public static string Normalize(string? input) =>
        string.IsNullOrWhiteSpace(input) ? string.Empty : Whitespace().Replace(input.Normalize(NormalizationForm.FormC).Trim(), " ");

    /// <summary>Normalleştirilmiş adın hata kodu (api-kurallari.md alan hataları); geçerliyse <c>null</c>.</summary>
    public static string? Validate(string normalized)
    {
        if (normalized.Length == 0)
        {
            return "required";
        }

        if (normalized.Length < MinLength)
        {
            return "too_short";
        }

        if (normalized.Length > MaxLength)
        {
            return "too_long";
        }

        return Allowed().IsMatch(normalized) ? null : "invalid_format";
    }

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex Whitespace();

    // Sözleşmedeki desenle aynı: Latin harfleri (Türkçe dahil; U+00C0–U+024F, × ve ÷ hariç), kesme işareti (' ve ’),
    // tire ve boşluk; harfle başlar. Rakam ve diğer işaretler kabul edilmez.
    [GeneratedRegex(
        @"^[A-Za-zÀ-ÖØ-öø-ɏ][A-Za-zÀ-ÖØ-öø-ɏ'’ -]*\z",
        RegexOptions.CultureInvariant,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex Allowed();
}
