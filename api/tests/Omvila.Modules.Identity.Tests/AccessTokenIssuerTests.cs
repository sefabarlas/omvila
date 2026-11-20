using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using NodaTime;
using NodaTime.Testing;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Security;

namespace Omvila.Modules.Identity.Tests;

// Kural: ADR-0007 (manage iddiaları, 15 dk ömür, ES256, kid; izinler jetona yazılmaz)
public sealed class AccessTokenIssuerTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);
    private static readonly Guid User = Guid.Parse("0193a8f0-0000-7000-8000-000000000001");
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid Membership = Guid.Parse("0193a8f0-0000-7000-8000-000000000002");
    private static readonly Guid Kadikoy = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");

    [Fact]
    public void IssueManage_WritesContractClaims_SignedWithActiveKey()
    {
        var (issuer, _) = Create();

        var (token, expiresIn) = issuer.IssueManage(new ManageTokenClaims(User, Brand, Membership, Roles.BranchManager, [Kadikoy], null));

        expiresIn.ShouldBe(15 * 60);
        var jwt = new JsonWebToken(token);
        jwt.Alg.ShouldBe("ES256");
        jwt.Kid.ShouldBe("k1");
        jwt.Issuer.ShouldBe("omvila");
        jwt.Audiences.ShouldBe([Audiences.Manage]);
        jwt.GetClaim(OmvilaClaims.Subject).Value.ShouldBe(User.ToString());
        jwt.GetClaim(OmvilaClaims.BrandId).Value.ShouldBe(Brand.ToString());
        jwt.GetClaim(OmvilaClaims.Role).Value.ShouldBe(Roles.BranchManager);
        jwt.GetClaim(OmvilaClaims.BranchIds).Value.ShouldBe(Kadikoy.ToString());
        jwt.IssuedAt.ShouldBe(Now.ToDateTimeUtc());
        jwt.ValidTo.ShouldBe((Now + Duration.FromMinutes(15)).ToDateTimeUtc());
        jwt.Claims.Any(c => c.Type.Contains("permission", StringComparison.OrdinalIgnoreCase)).ShouldBeFalse();
    }

    [Fact]
    public void IssueManage_ForOwner_UsesAllBranchesMarker()
    {
        var (issuer, _) = Create();

        var (token, _) = issuer.IssueManage(new ManageTokenClaims(User, Brand, Membership, Roles.Owner, null, null));

        new JsonWebToken(token).GetClaim(OmvilaClaims.BranchIds).Value.ShouldBe(OmvilaClaims.AllBranches);
    }

    [Fact]
    public void SigningKeyStore_WithoutConfiguredKeys_OutsideDevelopment_FailsFast() =>
        Should.Throw<InvalidOperationException>(() =>
            new SigningKeyStore(Options.Create(new AuthOptions()), new FakeEnvironment(Environments.Production), NullLogger<SigningKeyStore>.Instance));

    private static (AccessTokenIssuer Issuer, FakeClock Clock) Create()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var options = new AuthOptions { Issuer = "omvila", AccessTokenMinutes = 15 };
        options.SigningKeys.Add(new SigningKeyOptions { Kid = "k1", PrivateKeyPem = ecdsa.ExportPkcs8PrivateKeyPem() });
        var keys = new SigningKeyStore(Options.Create(options), new FakeEnvironment(Environments.Production), NullLogger<SigningKeyStore>.Instance);
        var clock = new FakeClock(Now);
        return (new AccessTokenIssuer(keys, clock, Options.Create(options)), clock);
    }

    private sealed class FakeEnvironment(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;

        public string ApplicationName { get; set; } = "Omvila.Tests";

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
