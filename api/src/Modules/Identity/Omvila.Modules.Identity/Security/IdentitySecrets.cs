using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using Omvila.Infrastructure.Hosting;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// İmza ve OTP anahtarı kuralları (T-73). Anahtar yoksa geçici anahtar yalnız yerelde: imza için Development,
/// OTP için Development ve Testing. Staging ve Production'da gizli değer (ortam değişkeni/kasa) zorunlu.
/// Hem açılış denetimi (<see cref="IdentitySecretsValidator"/>) hem anahtar sınıfları bu kuralları kullanır.
/// </summary>
internal static class IdentitySecrets
{
    public const int MinOtpKeyBytes = 32;

    public static bool AllowsEphemeralSigningKey(IHostEnvironment environment) => environment.IsDevelopment();

    public static bool AllowsEphemeralOtpKey(IHostEnvironment? environment) => environment?.IsDevelopmentOrTesting() == true;

    public static IReadOnlyList<SigningKeyOptions> ConfiguredSigningKeys(AuthOptions options) =>
        options.SigningKeys.Where(k => !string.IsNullOrWhiteSpace(k.PrivateKeyPem)).ToList();

    public static string? SigningKeysError(AuthOptions options, IHostEnvironment environment)
    {
        var configured = ConfiguredSigningKeys(options);
        if (configured.Count == 0)
        {
            return AllowsEphemeralSigningKey(environment)
                ? null
                : $"Auth:SigningKeys tanımlı değil: {environment.EnvironmentName} ortamında ES256 özel anahtarı zorunlu "
                  + "(ortam değişkenleri Auth__SigningKeys__0__Kid ve Auth__SigningKeys__0__PrivateKeyPem, kasadan).";
        }

        for (var i = 0; i < configured.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(configured[i].Kid))
            {
                return $"Auth:SigningKeys:{i}:Kid boş.";
            }

            if (!IsP256PrivateKey(configured[i].PrivateKeyPem))
            {
                return $"Auth:SigningKeys:{i}:PrivateKeyPem okunamadı (P-256 özel anahtar, PEM).";
            }
        }

        return null;
    }

    public static string? OtpKeyError(OtpOptions options, IHostEnvironment? environment)
    {
        if (string.IsNullOrWhiteSpace(options.HmacKey))
        {
            return AllowsEphemeralOtpKey(environment)
                ? null
                : $"Auth:Otp:HmacKey tanımlı değil: {environment?.EnvironmentName ?? "bu"} ortamında zorunlu "
                  + $"(ortam değişkeni Auth__Otp__HmacKey, base64, en az {MinOtpKeyBytes} bayt, kasadan).";
        }

        var buffer = new byte[options.HmacKey.Length];
        return Convert.TryFromBase64String(options.HmacKey, buffer, out var length) && length >= MinOtpKeyBytes
            ? null
            : $"Auth:Otp:HmacKey base64 ve en az {MinOtpKeyBytes} bayt olmalı.";
    }

    private static bool IsP256PrivateKey(string pem)
    {
        try
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(pem);
            return ecdsa.KeySize == 256;
        }
        catch (Exception ex) when (ex is ArgumentException or CryptographicException)
        {
            return false;
        }
    }
}
