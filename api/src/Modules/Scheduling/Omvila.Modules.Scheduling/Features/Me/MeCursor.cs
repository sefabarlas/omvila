using System.Buffers.Text;
using System.Globalization;
using System.Text;

namespace Omvila.Modules.Scheduling.Features.Me;

/// <summary>
/// Müşteri uygulaması randevu listesinin opak imleci (api-kurallari.md § Liste ve sayfalama): sabit sıralı listede atlanacak satır
/// sayısı. Bir müşterinin randevuları azdır ve "Değerlendirilmedi" süzgeci Reviews'ta uygulandığından anahtar imleci yerine konum
/// kullanılır.
/// </summary>
internal static class MeCursor
{
    public const int DefaultLimit = 50;
    public const int MaxLimit = 100;
    private const string Prefix = "o:";

    public static string Encode(int offset) =>
        Base64Url.EncodeToString(Encoding.UTF8.GetBytes(Prefix + offset.ToString(CultureInfo.InvariantCulture)));

    /// <summary>Boş imleç 0; bozuk imleç false.</summary>
    public static bool TryDecode(string? cursor, out int offset)
    {
        offset = 0;
        if (string.IsNullOrEmpty(cursor))
        {
            return true;
        }

        if (!Base64Url.IsValid(cursor))
        {
            return false;
        }

        var text = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(cursor));
        return text.StartsWith(Prefix, StringComparison.Ordinal)
            && int.TryParse(text.AsSpan(Prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out offset);
    }

    /// <summary>limit 1–100 (sözleşme Limit, varsayılan 50); geçersizse false.</summary>
    public static bool TryLimit(int? limit, out int value)
    {
        value = limit ?? DefaultLimit;
        return value is >= 1 and <= MaxLimit;
    }
}
