using System.Text.Json;

namespace Omvila.Modules.Tenancy.Features.Brand;

/// <summary>
/// PATCH gövdesi: alanın hiç gelmemesi ile <c>null</c> gelmesi farklıdır (api-kurallari.md: null değeri temizler).
/// Tip uyuşmazlığı alan hatası olarak toplanır.
/// </summary>
internal sealed class JsonBody(JsonElement root)
{
    public Dictionary<string, string[]> Errors { get; } = new(StringComparer.Ordinal);

    public bool IsObject => root.ValueKind == JsonValueKind.Object;

    public bool Has(string name) => IsObject && root.TryGetProperty(name, out _);

    /// <summary>Gelmediyse <paramref name="current"/>; null geldiyse null; metin değilse hata.</summary>
    public string? String(string name, string? current)
    {
        if (!IsObject || !root.TryGetProperty(name, out var value))
        {
            return current;
        }

        switch (value.ValueKind)
        {
            case JsonValueKind.Null:
                return null;
            case JsonValueKind.String:
                var text = value.GetString()!.Trim();
                return text.Length == 0 ? null : text;
            default:
                Errors[name] = ["invalid_format"];
                return current;
        }
    }
}
