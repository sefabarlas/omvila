using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Security;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>
/// OAuth jetonlarının uygulama düzeyinde şifrelenmesi (guvenlik-kvkk.md: integrations.calendar_links "Gizli, şifreli"; IbanProtector
/// deseni). AES-256-GCM; anahtar <c>Integrations:Calendar:TokenEncryptionKey</c> (ortamdan). Biçim: sürüm (1) | nonce (12) | etiket (16)
/// | şifreli metin; bağlantı kimliği ek doğrulanan veridir. Aynı anahtardan HMAC ile ayrı bir alt anahtar state imzası için türetilir.
/// </summary>
internal sealed partial class CalendarTokenProtector
{
    private const byte FormatVersion = 1;
    private const int NonceBytes = 12;
    private const int TagBytes = 16;
    private const int HeaderBytes = 1 + NonceBytes + TagBytes;

    private readonly byte[] _key;

    public CalendarTokenProtector(IOptions<CalendarOptions> options, ILogger<CalendarTokenProtector> logger, IHostEnvironment? environment = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        _key = ConfiguredSecretKey.Resolve(options.Value.TokenEncryptionKey, CalendarOptions.TokenEncryptionKeySetting, environment, out var ephemeral);
        StateKey = HMACSHA256.HashData(_key, "omvila-calendar-state"u8);
        if (ephemeral)
        {
            LogEphemeralKey(logger);
        }
    }

    /// <summary>connect-url state imzasının anahtarı (şifreleme anahtarından türetilir).</summary>
    public byte[] StateKey { get; }

    public byte[] Protect(Guid linkId, string token)
    {
        var plaintext = Encoding.UTF8.GetBytes(token);
        var output = new byte[HeaderBytes + plaintext.Length];
        output[0] = FormatVersion;
        var nonce = output.AsSpan(1, NonceBytes);
        RandomNumberGenerator.Fill(nonce);
        using var aes = new AesGcm(_key, TagBytes);
        aes.Encrypt(nonce, plaintext, output.AsSpan(HeaderBytes), output.AsSpan(1 + NonceBytes, TagBytes), linkId.ToByteArray());
        return output;
    }

    public string Unprotect(Guid linkId, byte[] ciphertext)
    {
        ArgumentNullException.ThrowIfNull(ciphertext);
        if (ciphertext.Length <= HeaderBytes || ciphertext[0] != FormatVersion)
        {
            throw new CryptographicException("Unknown calendar token format.");
        }

        var plaintext = new byte[ciphertext.Length - HeaderBytes];
        using var aes = new AesGcm(_key, TagBytes);
        aes.Decrypt(ciphertext.AsSpan(1, NonceBytes), ciphertext.AsSpan(HeaderBytes), ciphertext.AsSpan(1 + NonceBytes, TagBytes), plaintext, linkId.ToByteArray());
        return Encoding.UTF8.GetString(plaintext);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Integrations:Calendar:TokenEncryptionKey not configured; using an ephemeral key (calendar links break after restart)")]
    private static partial void LogEphemeralKey(ILogger logger);
}
