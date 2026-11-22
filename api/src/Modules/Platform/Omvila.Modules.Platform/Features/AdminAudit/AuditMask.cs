using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.AdminAudit;

/// <summary>
/// Denetim kaydı ayrıntısının maskesi (sözleşme adminListAuditLog: "changes'te telefon ve ad maskelidir"; guvenlik-kvkk.md).
/// Kayıt kişisel veri taşımamalıdır (IAuditLog); bu katman ikinci güvencedir: ad anahtarlarındaki değer "Ad S." olur, metin içinde
/// geçen Türkiye telefon numaraları "0532 *** ** 67" biçimine çevrilir.
/// </summary>
internal static partial class AuditMask
{
    private const int RegexTimeoutMilliseconds = 1000;

    private static readonly HashSet<string> NameKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "name", "fullName", "customerName", "senderName", "ownerName", "staffName",
    };

    private static readonly HashSet<string> PhoneKeys = new(StringComparer.OrdinalIgnoreCase) { "phone", "phoneE164", "phoneNumber" };

    /// <summary>JSON nesnesini maskeler; boş veya geçersizse boş nesne.</summary>
    public static JsonObject Changes(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return JsonNode.Parse(json) is JsonObject node ? (JsonObject)Walk(node, null)! : [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    /// <summary>"Elif Kaya" → "Elif K."; tek sözcük olduğu gibi kalır.</summary>
    public static string Name(string fullName)
    {
        ArgumentNullException.ThrowIfNull(fullName);
        var words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return words.Length <= 1 ? fullName.Trim() : $"{words[0]} {char.ToUpperInvariant(words[^1][0])}.";
    }

    /// <summary>Metin içindeki telefonları maskeler.</summary>
    public static string Phones(string text) =>
        PhonePattern().Replace(text, m => PhoneNumber.TryParse(m.Value, out var phone) ? phone.Masked : m.Value);

    private static JsonNode? Walk(JsonNode? node, string? key)
    {
        switch (node)
        {
            case JsonObject obj:
                var copy = new JsonObject();
                foreach (var (name, value) in obj)
                {
                    copy[name] = Walk(value, name);
                }

                return copy;
            case JsonArray array:
                return new JsonArray([.. array.Select(item => Walk(item, key))]);
            case JsonValue value when value.TryGetValue<string>(out var text):
                if (key is not null && PhoneKeys.Contains(key))
                {
                    return PhoneNumber.TryParse(text, out var phone) ? phone.Masked : "***";
                }

                return key is not null && NameKeys.Contains(key) ? Name(text) : Phones(text);
            default:
                return node?.DeepClone();
        }
    }

    [GeneratedRegex(@"(?<![\d])(?:\+?90[\s-]?|0)?5\d{2}[\s-]?\d{3}[\s-]?\d{2}[\s-]?\d{2}(?![\d])", RegexOptions.None, RegexTimeoutMilliseconds)]
    private static partial Regex PhonePattern();
}
