namespace Omvila.Modules.Identity.Security;

public sealed class SigningKeyOptions
{
    public string Kid { get; set; } = string.Empty;

    /// <summary>P-256 özel anahtar, PEM (PKCS#8 veya EC).</summary>
    public string PrivateKeyPem { get; set; } = string.Empty;
}
