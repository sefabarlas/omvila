using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// OTP özeti: HMAC-SHA256(anahtar, amaç | telefon | kod). Amaç ve telefon özete bağlıdır: bir amacın kodu
/// başka amaçta veya başka telefonda geçmez (ADR-0007). Anahtar yoksa yalnız Development ve Testing'de geçici anahtar; Staging/Production'da açılış durur (T-73).
/// </summary>
internal sealed partial class OtpHasher
{
    private readonly byte[] _key;

    public OtpHasher(IOptions<OtpOptions> options, ILogger<OtpHasher> logger, IHostEnvironment? environment = null)
    {
        // Ortam bilinmiyorsa güvenli taraf: anahtar zorunlu (IdentitySecrets).
        if (IdentitySecrets.OtpKeyError(options.Value, environment) is { } error)
        {
            throw new InvalidOperationException(error);
        }

        if (string.IsNullOrWhiteSpace(options.Value.HmacKey))
        {
            LogEphemeralKey(logger);
            _key = RandomNumberGenerator.GetBytes(IdentitySecrets.MinOtpKeyBytes);
        }
        else
        {
            _key = Convert.FromBase64String(options.Value.HmacKey);
        }
    }

    public string Hash(OtpPurpose purpose, string phoneE164, string code) =>
        Convert.ToHexStringLower(HMACSHA256.HashData(_key, Encoding.UTF8.GetBytes($"{purpose}|{phoneE164}|{code}")));

    public bool Matches(string expectedHash, OtpPurpose purpose, string phoneE164, string code) =>
        CryptographicOperations.FixedTimeEquals(
            Encoding.ASCII.GetBytes(expectedHash), Encoding.ASCII.GetBytes(Hash(purpose, phoneE164, code)));

    [LoggerMessage(Level = LogLevel.Warning, Message = "Auth:Otp:HmacKey not configured; using an ephemeral development key")]
    private static partial void LogEphemeralKey(ILogger logger);
}
