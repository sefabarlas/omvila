using System.Security.Cryptography;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Omvila.Modules.Identity.Security;

namespace Omvila.Modules.Identity.Tests;

// Kural: T-73 — imza ve OTP anahtarı Staging/Production'da gizli değerden; eksikse açılış anlaşılır hatayla durur.
// Geçici anahtar yalnız yerelde (imza: Development; OTP: Development ve Testing).
public sealed class IdentitySecretsTests
{
    [Theory]
    [InlineData("Staging")]
    [InlineData("Production")]
    public void Validator_MissingSigningKey_FailsAndNamesEnvironmentVariables(string environment)
    {
        var result = new IdentitySecretsValidator(new Env(environment)).Validate(null, new AuthOptions());

        result.Failed.ShouldBeTrue();
        result.FailureMessage.ShouldContain("Auth__SigningKeys__0__PrivateKeyPem");
    }

    [Theory]
    [InlineData("Staging")]
    [InlineData("Production")]
    public void Validator_MissingOtpKey_FailsAndNamesEnvironmentVariable(string environment)
    {
        var result = new IdentitySecretsValidator(new Env(environment)).Validate(null, new OtpOptions());

        result.Failed.ShouldBeTrue();
        result.FailureMessage.ShouldContain("Auth__Otp__HmacKey");
    }

    [Fact]
    public void Validator_Staging_WithConfiguredSecrets_Succeeds()
    {
        var validator = new IdentitySecretsValidator(new Env(Environments.Staging));

        validator.Validate(null, WithSigningKey()).Succeeded.ShouldBeTrue();
        validator.Validate(null, new OtpOptions { HmacKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)) }).Succeeded.ShouldBeTrue();
    }

    [Theory]
    [InlineData("not-base64!")]
    [InlineData("c2hvcnQ=")]
    public void Validator_InvalidOtpKey_Fails(string key) =>
        new IdentitySecretsValidator(new Env(Environments.Staging)).Validate(null, new OtpOptions { HmacKey = key }).Failed.ShouldBeTrue();

    [Fact]
    public void Validator_UnreadableSigningKey_Fails()
    {
        var options = new AuthOptions();
        options.SigningKeys.Add(new SigningKeyOptions { Kid = "k1", PrivateKeyPem = "-----BEGIN PRIVATE KEY-----\nAAAA\n-----END PRIVATE KEY-----" });

        new IdentitySecretsValidator(new Env(Environments.Staging)).Validate(null, options).Failed.ShouldBeTrue();
    }

    [Fact]
    public void Validator_Development_WithoutSecrets_AllowsEphemeralKeys()
    {
        var validator = new IdentitySecretsValidator(new Env(Environments.Development));

        validator.Validate(null, new AuthOptions()).Succeeded.ShouldBeTrue();
        validator.Validate(null, new OtpOptions()).Succeeded.ShouldBeTrue();
    }

    [Fact]
    public void OtpHasher_InStagingWithoutKey_FailsFast() =>
        Should.Throw<InvalidOperationException>(() =>
            new OtpHasher(Options.Create(new OtpOptions()), NullLogger<OtpHasher>.Instance, new Env(Environments.Staging)));

    [Fact]
    public void SigningKeyStore_InStagingWithoutKey_FailsFast() =>
        Should.Throw<InvalidOperationException>(() =>
            new SigningKeyStore(Options.Create(new AuthOptions()), new Env(Environments.Staging), NullLogger<SigningKeyStore>.Instance));

    private static AuthOptions WithSigningKey()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var options = new AuthOptions();
        options.SigningKeys.Add(new SigningKeyOptions { Kid = "k1", PrivateKeyPem = ecdsa.ExportPkcs8PrivateKeyPem() });
        return options;
    }

    private sealed class Env(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;

        public string ApplicationName { get; set; } = "Omvila.Tests";

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
