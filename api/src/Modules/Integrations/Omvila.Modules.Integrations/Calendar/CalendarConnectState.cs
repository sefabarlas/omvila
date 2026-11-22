using System.Buffers.Binary;
using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using NodaTime;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>
/// connect-url state değeri (sözleşme: 10 dk geçerli, kullanıcıya bağlı, tek kullanımlık; T-121): marka (16) | kullanıcı (16) |
/// personel (16) | tek kullanımlık değer (16) | geçerlilik sonu (Unix sn, 8) | istemci (1: web 0, mobil 1) | HMAC-SHA256 (32), base64url. Sunucuda saklanmaz;
/// tek kullanım bağlantının <c>ConnectNonce</c>'uyla sağlanır.
/// </summary>
internal sealed record CalendarConnectState(Guid BrandId, Guid UserId, Guid StaffId, Guid Nonce, Instant ExpiresUtc, bool Mobile)
{
    private const int PayloadBytes = (16 * 4) + 8 + 1;
    private const int MacBytes = 32;

    public string Sign(byte[] key)
    {
        var buffer = new byte[PayloadBytes + MacBytes];
        BrandId.TryWriteBytes(buffer.AsSpan(0, 16));
        UserId.TryWriteBytes(buffer.AsSpan(16, 16));
        StaffId.TryWriteBytes(buffer.AsSpan(32, 16));
        Nonce.TryWriteBytes(buffer.AsSpan(48, 16));
        BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan(64, 8), ExpiresUtc.ToUnixTimeSeconds());
        buffer[72] = Mobile ? (byte)1 : (byte)0;
        HMACSHA256.HashData(key, buffer.AsSpan(0, PayloadBytes), buffer.AsSpan(PayloadBytes));
        return WebEncoders.Base64UrlEncode(buffer);
    }

    /// <summary>İmza geçerli ve süresi dolmamışsa state; değilse null.</summary>
    public static CalendarConnectState? Read(string? value, byte[] key, Instant now)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        byte[] buffer;
        try
        {
            buffer = WebEncoders.Base64UrlDecode(value);
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
        HMACSHA256.HashData(key, buffer.AsSpan(0, PayloadBytes), expected);
        if (!CryptographicOperations.FixedTimeEquals(expected, buffer.AsSpan(PayloadBytes)))
        {
            return null;
        }

        var state = new CalendarConnectState(
            new Guid(buffer.AsSpan(0, 16)),
            new Guid(buffer.AsSpan(16, 16)),
            new Guid(buffer.AsSpan(32, 16)),
            new Guid(buffer.AsSpan(48, 16)),
            Instant.FromUnixTimeSeconds(BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan(64, 8))),
            buffer[72] == 1);
        return state.ExpiresUtc <= now ? null : state;
    }
}
