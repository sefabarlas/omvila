using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// OTP özeti: HMAC-SHA256(anahtar, amaç | telefon | kod). Amaç ve telefon özete bağlıdır: bir amacın kodu
/// başka amaçta veya başka telefonda geçmez (ADR-0007). Anahtar yoksa yalnız Development'ta geçici anahtar.
/// </summary>
internal sealed partial class OtpHasher
{
    private readonly byte[] _key;

    public OtpHasher(IOptions<OtpOptions> options, ILogger<OtpHasher> logger, IHostEnvironment? environment = null)
    {
        var configured = options.Value.HmacKey;
        if (string.IsNullOrWhiteSpace(configured))
        {
            // Ortam bilinmiyorsa güvenli taraf: anahtar zorunlu.
            if (environment is null || (!environment.IsDevelopment() && !environment.IsEnvironment("Testing")))
            {
                throw new InvalidOperationException("Auth:Otp:HmacKey tanımlı değil (base64, kasadan).");
            }

            LogEphemeralKey(logger);
            _key = RandomNumberGenerator.GetBytes(32);
        }
        else
        {
            _key = Convert.FromBase64String(configured);
            if (_key.Length < 32)
            {
                throw new InvalidOperationException("Auth:Otp:HmacKey en az 32 bayt olmalı.");
            }
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
