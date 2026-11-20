using System.Globalization;

namespace Omvila.Infrastructure.Http;

/// <summary>Kaynak sürümü: PostgreSQL xmin, HTTP'de zayıf ETag <c>W/"8812"</c> (docs/sozlesme/api-kurallari.md § Eşzamanlı düzenleme).</summary>
public static class ETagVersion
{
    public static string Format(uint version) => $"W/\"{version.ToString(CultureInfo.InvariantCulture)}\"";

    /// <summary><c>W/"8812"</c>, <c>"8812"</c> veya <c>8812</c> kabul edilir.</summary>
    public static bool TryParse(string? value, out uint version)
    {
        version = 0;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var text = value.Trim();
        if (text.StartsWith("W/", StringComparison.Ordinal))
        {
            text = text[2..];
        }

        if (text.Length >= 2 && text[0] == '"' && text[^1] == '"')
        {
            text = text[1..^1];
        }

        return uint.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out version);
    }
}
