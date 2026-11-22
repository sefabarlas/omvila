using System.Buffers.Binary;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Security;

namespace Omvila.Modules.Customers.Features.DataExports;

/// <summary>
/// Dışa aktarma indirme bağlantısının jetonu (süreli, imzalı; T-121): marka (16) | istek (16) | geçerlilik sonu (Unix sn, 8) |
/// HMAC-SHA256 (32), base64url. Jeton veritabanında saklanmaz; e-posta gönderilirken üretilir. Süre jetondadır ve isteğin
/// <c>ExpiresUtc</c>'siyle de denetlenir (arşiv o anda silinir).
/// </summary>
internal sealed partial class DataExportLinks
{
    private const int PayloadBytes = 16 + 16 + 8;
    private const int MacBytes = 32;

    private readonly byte[] _key;

    public DataExportLinks(IOptions<DataExportOptions> options, ILogger<DataExportLinks> logger, IHostEnvironment? environment = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        _key = ConfiguredSecretKey.Resolve(options.Value.LinkSigningKey, DataExportOptions.LinkSigningKeySetting, environment, out var ephemeral);
        if (ephemeral)
        {
            LogEphemeralKey(logger);
        }
    }

    public string CreateToken(Guid brandId, Guid exportId, Instant expiresUtc)
    {
        var buffer = new byte[PayloadBytes + MacBytes];
        brandId.TryWriteBytes(buffer.AsSpan(0, 16));
        exportId.TryWriteBytes(buffer.AsSpan(16, 16));
        BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan(32, 8), expiresUtc.ToUnixTimeSeconds());
        HMACSHA256.HashData(_key, buffer.AsSpan(0, PayloadBytes), buffer.AsSpan(PayloadBytes));
        return WebEncoders.Base64UrlEncode(buffer);
    }

    /// <summary>İmza geçerli ve süresi dolmamışsa marka ve istek; değilse null.</summary>
    public (Guid BrandId, Guid ExportId)? Read(string token, Instant now)
    {
        byte[] buffer;
        try
        {
            buffer = WebEncoders.Base64UrlDecode(token);
        }
        catch (FormatException)
        {
            return null;
        }

        if (buffer.Length != PayloadBytes + MacBytes)
        {
            return null;
        }

        Span<byte> expected = stackalloc byte[MacBytes];
        HMACSHA256.HashData(_key, buffer.AsSpan(0, PayloadBytes), expected);
        if (!CryptographicOperations.FixedTimeEquals(expected, buffer.AsSpan(PayloadBytes)))
        {
            return null;
        }

        var expires = Instant.FromUnixTimeSeconds(BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan(32, 8)));
        return expires <= now ? null : (new Guid(buffer.AsSpan(0, 16)), new Guid(buffer.AsSpan(16, 16)));
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Customers:DataExport:LinkSigningKey not configured; using an ephemeral key (links stop working after restart)")]
    private static partial void LogEphemeralKey(ILogger logger);
}
