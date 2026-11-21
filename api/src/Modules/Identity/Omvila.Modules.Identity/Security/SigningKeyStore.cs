using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// ES256 anahtarları (kid ile). Yapılandırmada anahtar yoksa yalnız Development'ta geçici anahtar üretilir
/// (yeniden başlatınca jetonlar düşer); diğer ortamlarda (Staging, Production) açılış durur (T-73).
/// </summary>
internal sealed partial class SigningKeyStore
{
    public SigningKeyStore(IOptions<AuthOptions> options, IHostEnvironment environment, ILogger<SigningKeyStore> logger)
    {
        if (IdentitySecrets.SigningKeysError(options.Value, environment) is { } error)
        {
            throw new InvalidOperationException(error);
        }

        var configured = IdentitySecrets.ConfiguredSigningKeys(options.Value);
        if (configured.Count == 0)
        {
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
