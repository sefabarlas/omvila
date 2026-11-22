using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using Omvila.Infrastructure.Hosting;

namespace Omvila.Infrastructure.Security;

/// <summary>
/// Ortamdan (kasa/ortam değişkeni) gelen 32 baytlık simetrik anahtar kuralı (IbanProtector deseni, T-73): base64 ve tam 32 bayt;
/// boşsa yalnız Development ve Testing'de ayar adına bağlı süreç başına geçici anahtar, Staging/Production'da açılış durur.
/// ASP.NET DataProtection kullanılmaz: anahtar halkası test sunucusunda kalıcı değildir.
/// </summary>
public static class ConfiguredSecretKey
{
    public const int KeyBytes = 32;

    private static readonly ConcurrentDictionary<string, byte[]> EphemeralKeys = new(StringComparer.Ordinal);

    /// <summary>Anahtar kuralı; geçerliyse null. Ortam bilinmiyorsa güvenli taraf: anahtar zorunlu.</summary>
    /// <param name="value">Ayarın değeri (base64).</param>
    /// <param name="settingName">Ayar adı (ör. <c>Integrations:Calendar:TokenEncryptionKey</c>).</param>
    /// <param name="environment">Ortam.</param>
    public static string? Error(string? value, string settingName, IHostEnvironment? environment)
    {
        ArgumentNullException.ThrowIfNull(settingName);
        if (string.IsNullOrWhiteSpace(value))
        {
            return environment?.IsDevelopmentOrTesting() == true
                ? null
                : $"{settingName} tanımlı değil: {environment?.EnvironmentName ?? "bu"} ortamında zorunlu "
                  + $"(ortam değişkeni {settingName.Replace(":", "__", StringComparison.Ordinal)}, base64, {KeyBytes} bayt; "
                  + $"üretmek için: openssl rand -base64 {KeyBytes}).";
        }

        var buffer = new byte[value.Length];
        return Convert.TryFromBase64String(value, buffer, out var length) && length == KeyBytes
            ? null
            : $"{settingName} base64 ve tam {KeyBytes} bayt olmalı.";
    }

    /// <summary>Anahtarı döner; kural ihlalinde <see cref="InvalidOperationException"/>. Boşsa geçici anahtar (yalnız Development/Testing).</summary>
    /// <param name="value">Ayarın değeri (base64).</param>
    /// <param name="settingName">Ayar adı.</param>
    /// <param name="environment">Ortam.</param>
    /// <param name="isEphemeral">Geçici anahtar mı (çağıran uyarı günlüğü yazar).</param>
    public static byte[] Resolve(string? value, string settingName, IHostEnvironment? environment, out bool isEphemeral)
    {
        if (Error(value, settingName, environment) is { } error)
        {
            throw new InvalidOperationException(error);
        }

        isEphemeral = string.IsNullOrWhiteSpace(value);
        return isEphemeral ? EphemeralKeys.GetOrAdd(settingName, _ => RandomNumberGenerator.GetBytes(KeyBytes)) : Convert.FromBase64String(value!);
    }
}
