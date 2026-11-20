using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// ES256 anahtarları (kid ile). Yapılandırmada anahtar yoksa yalnız Development'ta geçici anahtar üretilir
/// (yeniden başlatınca jetonlar düşer); diğer ortamlarda açılış durur.
/// </summary>
internal sealed partial class SigningKeyStore
{
    public SigningKeyStore(IOptions<AuthOptions> options, IHostEnvironment environment, ILogger<SigningKeyStore> logger)
    {
        var configured = options.Value.SigningKeys.Where(k => !string.IsNullOrWhiteSpace(k.PrivateKeyPem)).ToList();
        if (configured.Count == 0)
        {
            if (!environment.IsDevelopment())
            {
                throw new InvalidOperationException("Auth:SigningKeys tanımlı değil (ES256 PEM, kasadan).");
            }

            LogEphemeralKey(logger);
            Keys = [Create("dev-ephemeral", ECDsa.Create(ECCurve.NamedCurves.nistP256))];
        }
        else
        {
            Keys = configured.Select(k =>
            {
                var ecdsa = ECDsa.Create();
                ecdsa.ImportFromPem(k.PrivateKeyPem);
                return Create(k.Kid, ecdsa);
            }).ToList();
        }
    }

    /// <summary>Doğrulama anahtarları; ilki imzalar.</summary>
    public IReadOnlyList<ECDsaSecurityKey> Keys { get; }

    public SigningCredentials SigningCredentials => new(Keys[0], SecurityAlgorithms.EcdsaSha256);

    private static ECDsaSecurityKey Create(string kid, ECDsa ecdsa) => new(ecdsa) { KeyId = kid };

    [LoggerMessage(Level = LogLevel.Warning, Message = "Auth:SigningKeys not configured; using an ephemeral development key")]
    private static partial void LogEphemeralKey(ILogger logger);
}
