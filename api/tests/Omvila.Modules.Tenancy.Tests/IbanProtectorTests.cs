using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Omvila.Modules.Tenancy.Infrastructure.BankAccounts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Tests;

// Kural: guvenlik-kvkk.md (IBAN uygulama düzeyinde şifreli), T-78 — AES-256-GCM, anahtar Tenancy:IbanEncryptionKey
// (base64 32 bayt); Development/Testing'de yoksa geçici, Staging/Production'da eksikse açılış durur (T-73 deseni).
public sealed class IbanProtectorTests
{
    private static readonly Guid BrandA = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid BrandB = Guid.Parse("0193a8f0-0000-7000-8000-00000000000b");
    private static readonly string Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    [Fact]
    public void ProtectThenUnprotect_ReturnsSameIban_AndCiphertextHidesIt()
    {
        var protector = Create(Key, Environments.Staging);
        var iban = Parse("TR29 0006 4000 0011 2345 6789 01");

        var encrypted = protector.Protect(BrandA, iban);

        encrypted.Last4.ShouldBe("8901");
        Encoding.ASCII.GetString(encrypted.Ciphertext).ShouldNotContain("0006400000112345");
        protector.Unprotect(BrandA, encrypted.Ciphertext).ShouldBe(iban);
        protector.Protect(BrandA, iban).Ciphertext.ShouldNotBe(encrypted.Ciphertext, "her şifrelemede yeni nonce");
    }

    [Fact]
    public void Unprotect_WithAnotherBrand_Fails()
    {
        var protector = Create(Key, Environments.Staging);
        var encrypted = protector.Protect(BrandA, Parse("TR290006400000112345678901"));

        Should.Throw<CryptographicException>(() => protector.Unprotect(BrandB, encrypted.Ciphertext));
    }

    [Fact]
    public void Unprotect_WithAnotherKey_Fails()
    {
        var encrypted = Create(Key, Environments.Staging).Protect(BrandA, Parse("TR290006400000112345678901"));
        var other = Create(Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), Environments.Staging);

        Should.Throw<CryptographicException>(() => other.Unprotect(BrandA, encrypted.Ciphertext));
    }

    [Theory]
    [InlineData("Staging")]
    [InlineData("Production")]
    public void Validator_MissingKey_FailsAndNamesEnvironmentVariable(string environment)
    {
        var result = new TenancySecretsValidator(new Env(environment)).Validate(null, new TenancyOptions());

        result.Failed.ShouldBeTrue();
        result.FailureMessage.ShouldContain("Tenancy__IbanEncryptionKey");
    }

    [Theory]
    [InlineData("not-base64!")]
    [InlineData("c2hvcnQ=")]
    public void Validator_InvalidKey_Fails(string key) =>
        new TenancySecretsValidator(new Env(Environments.Staging)).Validate(null, new TenancyOptions { IbanEncryptionKey = key }).Failed.ShouldBeTrue();

    [Fact]
    public void Validator_Staging_WithKey_Succeeds() =>
        new TenancySecretsValidator(new Env(Environments.Staging)).Validate(null, new TenancyOptions { IbanEncryptionKey = Key }).Succeeded.ShouldBeTrue();

    [Theory]
    [InlineData("Development")]
    [InlineData("Testing")]
    public void Validator_LocalEnvironments_AllowEphemeralKey(string environment)
    {
        new TenancySecretsValidator(new Env(environment)).Validate(null, new TenancyOptions()).Succeeded.ShouldBeTrue();
        var first = Create(null, environment);
        var second = Create(null, environment);

        var encrypted = first.Protect(BrandA, Parse("TR290006400000112345678901"));

        second.Unprotect(BrandA, encrypted.Ciphertext).Value.ShouldBe("TR290006400000112345678901", "geçici anahtar süreç boyunca ortak");
    }

    [Fact]
    public void Protector_InStagingWithoutKey_FailsFast() =>
        Should.Throw<InvalidOperationException>(() => Create(null, Environments.Staging));

    private static IbanProtector Create(string? key, string environment) =>
        new(Options.Create(new TenancyOptions { IbanEncryptionKey = key }), NullLogger<IbanProtector>.Instance, new Env(environment));

    private static Iban Parse(string value) => Iban.TryParse(value, out var iban) ? iban : throw new ArgumentException("geçersiz test IBAN'ı", nameof(value));

    private sealed class Env(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;

        public string ApplicationName { get; set; } = "Omvila.Tests";

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
