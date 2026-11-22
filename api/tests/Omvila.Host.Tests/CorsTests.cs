using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Omvila.Host.Tests;

// Kural: guvenlik-kvkk.md § CORS (yüzey bazında izinli kökenler, joker yok); mimari.md § API yüzeyleri; T-59 (3) ETag; T-60.
// Development ayarı: Manage → http://localhost:5173, Public → http://localhost:3000.
public sealed class CorsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private const string PanelOrigin = "http://localhost:5173";
    private const string BookingOrigin = "http://localhost:3000";

    [Fact]
    public async Task Preflight_ManageFromPanelOrigin_AllowsClientHeadersAndCredentials()
    {
        using var client = factory.CreateClient();

        using var response = await PreflightAsync(client, "/api/v1/manage/customers", PanelOrigin, "POST",
            "authorization,content-type,if-match,idempotency-key,x-client-type,x-client,x-requested-with");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        Header(response, "Access-Control-Allow-Origin").ShouldBe(PanelOrigin);
        Header(response, "Access-Control-Allow-Credentials").ShouldBe("true");
        Header(response, "Access-Control-Allow-Methods")!.ShouldContain("POST");
        var allowed = Header(response, "Access-Control-Allow-Headers")!.ToLowerInvariant();
        foreach (var header in new[] { "authorization", "content-type", "if-match", "idempotency-key", "x-client-type", "x-client", "x-requested-with" })
        {
            allowed.ShouldContain(header);
        }

        Header(response, "Access-Control-Max-Age").ShouldBe("600");
    }

    [Fact]
    public async Task Preflight_RefreshCookieEndpoint_SucceedsWithoutToken()
    {
        using var client = factory.CreateClient();

        using var response = await PreflightAsync(client, "/api/v1/manage/auth/refresh", PanelOrigin, "POST", "content-type,x-requested-with");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        Header(response, "Access-Control-Allow-Origin").ShouldBe(PanelOrigin);
        Header(response, "Access-Control-Allow-Credentials").ShouldBe("true");
    }

    [Fact]
    public async Task Preflight_PublicOtpFromBookingOrigin_AllowsCredentialsBeforeRateLimiter()
    {
        using var client = factory.CreateClient();

        using var response = await PreflightAsync(client, "/api/v1/public/otp/request", BookingOrigin, "POST", "content-type,idempotency-key");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        Header(response, "Access-Control-Allow-Origin").ShouldBe(BookingOrigin);
        Header(response, "Access-Control-Allow-Credentials").ShouldBe("true");
    }

    [Fact]
    public async Task Preflight_BookingOriginOnManage_IsNotAllowed()
    {
        using var client = factory.CreateClient();

        using var response = await PreflightAsync(client, "/api/v1/manage/customers", BookingOrigin, "GET", "authorization");

        Header(response, "Access-Control-Allow-Origin").ShouldBeNull();
        Header(response, "Access-Control-Allow-Credentials").ShouldBeNull();
    }

    [Fact]
    public async Task Preflight_PanelOriginOnPublic_IsNotAllowed()
    {
        using var client = factory.CreateClient();

        using var response = await PreflightAsync(client, "/api/v1/public/otp/request", PanelOrigin, "POST", "content-type");

        Header(response, "Access-Control-Allow-Origin").ShouldBeNull();
    }

    [Fact]
    public async Task Preflight_UnknownOrigin_GetsNoAllowOrigin()
    {
        using var client = factory.CreateClient();

        using var response = await PreflightAsync(client, "/api/v1/manage/customers", "https://kotu.example.test", "GET", "authorization");

        Header(response, "Access-Control-Allow-Origin").ShouldBeNull();
    }

    [Fact]
    public async Task CrossOriginResponse_ExposesETagAndOtherHeaders_EvenOn401()
    {
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/v1/manage/session", UriKind.Relative));
        request.Headers.Add("Origin", PanelOrigin);

        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        Header(response, "Access-Control-Allow-Origin").ShouldBe(PanelOrigin);
        var exposed = Header(response, "Access-Control-Expose-Headers")!;
        foreach (var header in new[] { "ETag", "Location", "Idempotent-Replayed", "Retry-After" })
        {
            exposed.ShouldContain(header);
        }
    }

    [Fact]
    public async Task HealthEndpoint_HasNoCors()
    {
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/health/live", UriKind.Relative));
        request.Headers.Add("Origin", PanelOrigin);

        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        Header(response, "Access-Control-Allow-Origin").ShouldBeNull();
    }

    [Fact]
    public void EveryApiEndpoint_HasItsSurfaceCorsPolicy()
    {
        using var client = factory.CreateClient();
        var endpoints = factory.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .OfType<RouteEndpoint>()
            .Where(e => e.RoutePattern.RawText?.StartsWith("/api/v1/", StringComparison.Ordinal) == true)
            .ToList();

        endpoints.ShouldNotBeEmpty();
        foreach (var endpoint in endpoints)
        {
            // Sağlayıcı geri çağrısı sunucudan sunucuya: tarayıcıya açılmaz, CORS politikası yoktur (T-64).
            if (endpoint.RoutePattern.RawText!.StartsWith("/api/v1/webhooks/", StringComparison.Ordinal))
            {
                endpoint.Metadata.GetMetadata<IEnableCorsAttribute>().ShouldBeNull(endpoint.RoutePattern.RawText);
                continue;
            }

            var path = endpoint.RoutePattern.RawText!;
            var expected = path.StartsWith("/api/v1/manage", StringComparison.Ordinal) ? "cors:manage"
                : path.StartsWith("/api/v1/me/", StringComparison.Ordinal) ? "cors:me"
                : "cors:public";
            var cors = endpoint.Metadata.GetMetadata<IEnableCorsAttribute>();
            cors.ShouldNotBeNull(endpoint.RoutePattern.RawText);
            cors.PolicyName.ShouldBe(expected, endpoint.RoutePattern.RawText);
        }
    }

    [Fact]
    public async Task AdminAndMePolicies_DoNotAllowCredentials()
    {
        using var configured = factory.WithWebHostBuilder(b =>
        {
            b.UseSetting("Cors:Surfaces:Admin:Origins:0", "http://localhost:5174");
            b.UseSetting("Cors:Surfaces:Me:Origins:0", "http://localhost:5175");
        });
        using var client = configured.CreateClient();
        var provider = configured.Services.GetRequiredService<ICorsPolicyProvider>();
        var context = new DefaultHttpContext();

        var admin = await provider.GetPolicyAsync(context, "cors:admin");
        var me = await provider.GetPolicyAsync(context, "cors:me");
        var manage = await provider.GetPolicyAsync(context, "cors:manage");

        admin!.Origins.ShouldBe(["http://localhost:5174"]);
        admin.SupportsCredentials.ShouldBeFalse();
        me!.SupportsCredentials.ShouldBeFalse();
        manage!.SupportsCredentials.ShouldBeTrue();
        manage.Origins.ShouldBe([PanelOrigin]);
    }

    [Theory]
    [InlineData("*")]
    [InlineData("https://*.omvila.com")]
    [InlineData("http://localhost:3000/r")]
    public void Startup_RejectsInvalidOrigin(string origin)
    {
        using var configured = factory.WithWebHostBuilder(b => b.UseSetting("Cors:Surfaces:Public:Origins:0", origin));

        var error = Should.Throw<Exception>(() => configured.CreateClient());

        error.ToString().ShouldContain("Cors:Surfaces:Public:Origins");
    }

    private static async Task<HttpResponseMessage> PreflightAsync(HttpClient client, string path, string origin, string method, string headers)
    {
        using var request = new HttpRequestMessage(HttpMethod.Options, new Uri(path, UriKind.Relative));
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", method);
        request.Headers.Add("Access-Control-Request-Headers", headers);
        return await client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    private static string? Header(HttpResponseMessage response, string name) =>
        response.Headers.TryGetValues(name, out var values) ? string.Join(",", values) : null;
}
