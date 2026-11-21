using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Omvila.Infrastructure.Sms;

/// <summary>
/// Sahte sağlayıcının teslim raporu (geliştirme ve test). Biçim gerçek adaptörler için örnektir:
/// başlık <c>X-Signature: sha256=&lt;hex&gt;</c> = HMAC-SHA256(gizli, ham gövde); gövde
/// <c>{"messageId":"fake-…","status":"delivered"|"failed","reason":"…"}</c>.
/// </summary>
public sealed class FakeSmsDeliveryReportVerifier(IOptions<FakeSmsProviderOptions> options) : ISmsDeliveryReportVerifier
{
    /// <summary>Webhook yolundaki sağlayıcı kodu.</summary>
    public const string ProviderCode = "fake";

    public const string SignatureHeader = "X-Signature";

    private const string SignaturePrefix = "sha256=";

    private const int Sha256HexLength = 64;

    public SmsDeliveryReportVerification Verify(IHeaderDictionary headers, ReadOnlySpan<byte> body)
    {
        ArgumentNullException.ThrowIfNull(headers);
        var secret = options.Value.WebhookSecret;
        if (string.IsNullOrEmpty(secret) || !TryReadSignature(headers, out var signature))
        {
            return SmsDeliveryReportVerification.Unauthorized;
        }

        var expected = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), body);
        if (!CryptographicOperations.FixedTimeEquals(expected, signature))
        {
            return SmsDeliveryReportVerification.Unauthorized;
        }

        try
        {
            using var document = JsonDocument.Parse(body.ToArray());
            return Parse(document.RootElement);
        }
        catch (JsonException)
        {
            return SmsDeliveryReportVerification.Malformed;
        }
    }

    /// <summary>İmza başlığının değeri (testler ve geliştirme araçları için).</summary>
    /// <param name="secret">Gizli.</param>
    /// <param name="body">Ham gövde.</param>
    public static string Sign(string secret, ReadOnlySpan<byte> body)
    {
        ArgumentNullException.ThrowIfNull(secret);
        return SignaturePrefix + Convert.ToHexStringLower(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), body));
    }

    private static SmsDeliveryReportVerification Parse(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object
            || !root.TryGetProperty("messageId", out var id) || id.ValueKind != JsonValueKind.String
            || string.IsNullOrWhiteSpace(id.GetString())
            || !root.TryGetProperty("status", out var status) || status.ValueKind != JsonValueKind.String)
        {
            return SmsDeliveryReportVerification.Malformed;
        }

        var messageId = id.GetString()!;
        var reason = root.TryGetProperty("reason", out var r) && r.ValueKind == JsonValueKind.String ? r.GetString() : null;
        return status.GetString() switch
        {
            "delivered" => SmsDeliveryReportVerification.Valid([new SmsDeliveryReport(messageId, Delivered: true, null)]),
            "failed" => SmsDeliveryReportVerification.Valid([new SmsDeliveryReport(messageId, Delivered: false, reason ?? "undelivered")]),
            _ => SmsDeliveryReportVerification.Malformed,
        };
    }

    private static bool TryReadSignature(IHeaderDictionary headers, out byte[] signature)
    {
        signature = [];
        var value = headers[SignatureHeader].ToString();
        if (!value.StartsWith(SignaturePrefix, StringComparison.Ordinal))
        {
            return false;
        }

        var hex = value[SignaturePrefix.Length..];
        if (hex.Length != Sha256HexLength || !hex.All(char.IsAsciiHexDigit))
        {
            return false;
        }

        signature = Convert.FromHexString(hex);
        return true;
    }
}
