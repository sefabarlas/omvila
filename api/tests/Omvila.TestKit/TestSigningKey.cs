using System.Security.Cryptography;

namespace Omvila.TestKit;

/// <summary>Test çalışması başına üretilen ES256 anahtarı; depoda gizli anahtar tutulmaz.</summary>
public static class TestSigningKey
{
    public const string Kid = "test-key-1";

    private static readonly Lazy<string> Pem = new(() =>
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        return ecdsa.ExportPkcs8PrivateKeyPem();
    });

    public static string PrivateKeyPem => Pem.Value;

    /// <summary>OTP HMAC anahtarı (base64, 32 bayt); çalışma başına üretilir.</summary>
    public static string OtpHmacKey { get; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
}
