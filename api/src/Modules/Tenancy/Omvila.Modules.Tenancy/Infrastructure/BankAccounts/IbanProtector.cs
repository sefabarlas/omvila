using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Hosting;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Infrastructure.BankAccounts;

/// <summary>
/// IBAN'ın uygulama düzeyinde şifrelenmesi (guvenlik-kvkk.md; alan-modeli.md § BrandBankAccount.IbanEncrypted).
/// AES-256-GCM; anahtar <c>Tenancy:IbanEncryptionKey</c> (base64, 32 bayt; kasadan/ortam değişkeninden). ASP.NET
/// DataProtection kullanılmaz: anahtar halkası test sunucusunda kalıcı değildir. Biçim: sürüm (1) | nonce (12) | etiket (16) |
/// şifreli metin; marka kimliği ek doğrulanan veridir (bir markanın şifreli IBAN'ı başka markanın satırında çözülmez).
/// Anahtar yoksa yalnız Development ve Testing'de süreç boyunca geçici anahtar; Staging/Production'da açılış durur
/// (<see cref="TenancySecretsValidator"/>).
/// </summary>
internal sealed partial class IbanProtector
{
    public const int KeyBytes = 32;
    private const byte FormatVersion = 1;
    private const int NonceBytes = 12;
    private const int TagBytes = 16;
    private const int HeaderBytes = 1 + NonceBytes + TagBytes;

    // Süreç başına tek geçici anahtar: aynı süreçteki test sunucuları aynı veritabanını okuyabilsin.
    private static readonly Lazy<byte[]> EphemeralKey = new(() => RandomNumberGenerator.GetBytes(KeyBytes));

    private readonly byte[] _key;

    public IbanProtector(IOptions<TenancyOptions> options, ILogger<IbanProtector> logger, IHostEnvironment? environment = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (KeyError(options.Value, environment) is { } error)
        {
            throw new InvalidOperationException(error);
        }

        if (string.IsNullOrWhiteSpace(options.Value.IbanEncryptionKey))
        {
            LogEphemeralKey(logger);
            _key = EphemeralKey.Value;
        }
        else
        {
            _key = Convert.FromBase64String(options.Value.IbanEncryptionKey);
        }
    }

    /// <summary>Anahtar kuralı; geçerliyse null. Ortam bilinmiyorsa güvenli taraf: anahtar zorunlu.</summary>
    public static string? KeyError(TenancyOptions options, IHostEnvironment? environment)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (string.IsNullOrWhiteSpace(options.IbanEncryptionKey))
        {
            return environment?.IsDevelopmentOrTesting() == true
                ? null
                : $"Tenancy:IbanEncryptionKey tanımlı değil: {environment?.EnvironmentName ?? "bu"} ortamında zorunlu "
                  + $"(ortam değişkeni Tenancy__IbanEncryptionKey, base64, {KeyBytes} bayt; üretmek için: openssl rand -base64 {KeyBytes}).";
        }

        var buffer = new byte[options.IbanEncryptionKey.Length];
        return Convert.TryFromBase64String(options.IbanEncryptionKey, buffer, out var length) && length == KeyBytes
            ? null
            : $"Tenancy:IbanEncryptionKey base64 ve tam {KeyBytes} bayt olmalı.";
    }

    public EncryptedIban Protect(Guid brandId, Iban iban)
    {
        ArgumentNullException.ThrowIfNull(iban);
        var plaintext = Encoding.ASCII.GetBytes(iban.Value);
        var output = new byte[HeaderBytes + plaintext.Length];
        output[0] = FormatVersion;
        var nonce = output.AsSpan(1, NonceBytes);
        RandomNumberGenerator.Fill(nonce);
        using var aes = new AesGcm(_key, TagBytes);
        aes.Encrypt(nonce, plaintext, output.AsSpan(HeaderBytes), output.AsSpan(1 + NonceBytes, TagBytes), brandId.ToByteArray());
        return new EncryptedIban(output, iban.Last4);
    }

    /// <summary>Şifreyi çözer. Anahtar veya marka uyuşmazsa <see cref="CryptographicException"/> (programcı/yapılandırma hatası).</summary>
    public Iban Unprotect(Guid brandId, byte[] ciphertext)
    {
        ArgumentNullException.ThrowIfNull(ciphertext);
        if (ciphertext.Length <= HeaderBytes || ciphertext[0] != FormatVersion)
        {
            throw new CryptographicException("Unknown IBAN ciphertext format.");
        }

        var plaintext = new byte[ciphertext.Length - HeaderBytes];
        using var aes = new AesGcm(_key, TagBytes);
        aes.Decrypt(
            ciphertext.AsSpan(1, NonceBytes), ciphertext.AsSpan(HeaderBytes), ciphertext.AsSpan(1 + NonceBytes, TagBytes), plaintext, brandId.ToByteArray());
        return Iban.TryParse(Encoding.ASCII.GetString(plaintext), out var iban)
            ? iban
            : throw new CryptographicException("Decrypted IBAN is not valid.");
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Tenancy:IbanEncryptionKey not configured; using an ephemeral key (IBANs saved now cannot be read after restart)")]
    private static partial void LogEphemeralKey(ILogger logger);
}
