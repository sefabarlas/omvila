using Omvila.Infrastructure.Http.Cors;

namespace Omvila.Infrastructure.Tests;

// Kural: guvenlik-kvkk.md § CORS (yüzey bazında izinli kökenler, joker yok); T-60.
public sealed class SurfaceCorsOptionsValidatorTests
{
    [Fact]
    public void Collect_ValidHttpsOrigins_HasNoErrors()
    {
        var options = Options(("Manage", "https://panel.omvila.com"), ("Public", "https://omvila.com"));

        SurfaceCorsOptionsValidator.Collect(options, requireHttps: true).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("*")]
    [InlineData("https://*.omvila.com")]
    [InlineData("")]
    [InlineData("panel.omvila.com")]
    [InlineData("https://panel.omvila.com/")]
    [InlineData("https://panel.omvila.com/giris")]
    [InlineData("ftp://panel.omvila.com")]
    public void Collect_InvalidOrigin_IsRejected(string origin)
    {
        var options = Options(("Manage", origin));

        SurfaceCorsOptionsValidator.Collect(options, requireHttps: false).ShouldHaveSingleItem().ShouldContain("Cors:Surfaces:Manage:Origins");
    }

    [Fact]
    public void Collect_HttpOrigin_RejectedOnlyWhenHttpsRequired()
    {
        var options = Options(("Public", "http://localhost:3000"));

        SurfaceCorsOptionsValidator.Collect(options, requireHttps: false).ShouldBeEmpty();
        SurfaceCorsOptionsValidator.Collect(options, requireHttps: true).ShouldHaveSingleItem().ShouldContain("https");
    }

    [Fact]
    public void Collect_UnknownSurface_IsRejected()
    {
        var options = Options(("Panel", "https://panel.omvila.com"));

        SurfaceCorsOptionsValidator.Collect(options, requireHttps: true).ShouldHaveSingleItem().ShouldContain("bilinmeyen yüzey");
    }

    [Fact]
    public void Collect_NonPositivePreflightMaxAge_IsRejected()
    {
        var options = new SurfaceCorsOptions { PreflightMaxAgeSeconds = 0 };

        SurfaceCorsOptionsValidator.Collect(options, requireHttps: true).ShouldHaveSingleItem().ShouldContain("PreflightMaxAgeSeconds");
    }

    [Fact]
    public void FromPrefix_MapsGroupPrefixesToSurfaces()
    {
        ApiSurface.FromPrefix("/api/v1/manage/customers").ShouldBe(ApiSurface.Manage);
        ApiSurface.FromPrefix("/api/v1/manage").ShouldBe(ApiSurface.Manage);
        ApiSurface.FromPrefix("/api/v1/public/otp").ShouldBe(ApiSurface.Public);
        ApiSurface.FromPrefix("/api/v1/admin/brands").ShouldBe(ApiSurface.Admin);
        ApiSurface.FromPrefix("/api/v1/me/appointments").ShouldBe(ApiSurface.Me);
        ApiSurface.FromPrefix("/api/v1/manager").ShouldBeNull();
        ApiSurface.FromPrefix("/api/test").ShouldBeNull();
    }

    private static SurfaceCorsOptions Options(params (string Surface, string Origin)[] entries)
    {
        var options = new SurfaceCorsOptions { PreflightMaxAgeSeconds = 600 };
        foreach (var (surface, origin) in entries)
        {
            options.Surfaces[surface] = new SurfaceCorsOrigins { Origins = [origin] };
        }

        return options;
    }
}
