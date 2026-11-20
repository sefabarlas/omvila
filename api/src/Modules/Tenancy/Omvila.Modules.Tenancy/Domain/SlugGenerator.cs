using System.Globalization;
using System.Text;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// İşletme adından bağlantı adı (dilim-01 § Alan kuralları): Türkçe dönüşüm (ç→c, ğ→g, ı→i, İ→i, ö→o, ş→s, ü→u),
/// harf ve rakam dışındaki karakterler tire, kesme işareti yok sayılır. Alınmışsa veya ayrılmışsa -2, -3…
/// </summary>
internal static class SlugGenerator
{
    // Ad yalnız işaretlerden oluşuyorsa (ör. "&&") kullanılacak taban.
    private const string Fallback = "isletme";

    public static string BaseFrom(string businessName)
    {
        ArgumentNullException.ThrowIfNull(businessName);
        var builder = new StringBuilder(businessName.Length);
        var pendingHyphen = false;
        foreach (var raw in businessName.Trim())
        {
            if (raw is '\'' or '’' or '`')
            {
                continue;
            }

            var c = Transliterate(raw);
            if (char.IsAsciiLetterOrDigit(c))
            {
                if (pendingHyphen && builder.Length > 0)
                {
                    builder.Append('-');
                }

                builder.Append(char.ToLowerInvariant(c));
                pendingHyphen = false;
            }
            else
            {
                pendingHyphen = true;
            }
        }

        var slug = builder.Length == 0 ? Fallback : builder.ToString();
        return Trim(slug, Slug.MaxLength);
    }

    /// <summary>Taban kullanılabilirse onu, değilse ilk boş "-n" adayını döner.</summary>
    public static Slug FirstAvailable(string businessName, Func<string, bool> isTaken)
    {
        ArgumentNullException.ThrowIfNull(isTaken);
        var baseSlug = BaseFrom(businessName);
        for (var n = 1; ; n++)
        {
            var suffix = n == 1 ? string.Empty : "-" + n.ToString(CultureInfo.InvariantCulture);
            var candidate = Trim(baseSlug, Slug.MaxLength - suffix.Length) + suffix;
            if (Slug.TryCreate(candidate, out var slug) && !isTaken(candidate))
            {
                return slug;
            }
        }
    }

    private static char Transliterate(char c) => c switch
    {
        'ç' or 'Ç' => 'c',
        'ğ' or 'Ğ' => 'g',
        'ı' or 'I' or 'İ' or 'i' => 'i',
        'ö' or 'Ö' => 'o',
        'ş' or 'Ş' => 's',
        'ü' or 'Ü' => 'u',
        'â' or 'Â' => 'a',
        'î' or 'Î' => 'i',
        'û' or 'Û' => 'u',
        _ => c,
    };

    // Sınır aşılırsa kelime ortasında değil, son tireden kesilir; sonda tire kalmaz.
    private static string Trim(string slug, int max)
    {
        if (slug.Length <= max)
        {
            return slug;
        }

        var cut = slug[..max];
        var lastHyphen = cut.LastIndexOf('-');
        return (lastHyphen >= Slug.MinLength ? cut[..lastHyphen] : cut).TrimEnd('-');
    }
}
