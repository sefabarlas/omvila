using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Omvila.Host.Tests;

// Kural: T-73 (7), T-76 — ters vekil arkasında X-Forwarded-For/Proto yalnız güvenilen vekilden
// (ForwardedHeaders:KnownProxies) veya ağdan (ForwardedHeaders:KnownNetworks) kabul edilir; böylece IP başına OTP sınırı
// gerçek istemci IP'sini görür. Güvenilmeyen kaynağın başlığı ve yapılandırma boşken (Development) başlık yok sayılır.
// Bağlantı IP'si TestServer'da boş olduğundan test önce onu verir, istekten sonra uygulamanın gördüğü IP ve şemayı okur.
public sealed class ForwardedHeadersTests
{
    private const string ProxyIp = "10.0.0.5";
    private const string ClientIp = "203.0.113.7";

    [Fact]
    public async Task FromKnownProxy_UsesForwardedClientIpAndScheme()
    {
        var seen = await SendAsync(ProxyIp, Settings(("ForwardedHeaders:KnownProxies:0", ProxyIp)));

        seen.RemoteIp.ShouldBe(ClientIp);
        seen.Scheme.ShouldBe("https");
    }

    [Fact]
    public async Task FromKnownNetwork_UsesForwardedClientIp()
    {
        var seen = await SendAsync("172.18.0.3", Settings(("ForwardedHeaders:KnownNetworks:0", "172.18.0.0/16")));

        seen.RemoteIp.ShouldBe(ClientIp);
        seen.Scheme.ShouldBe("https");
    }

    [Fact]
    public async Task FromUntrustedSource_IgnoresForwardedHeaders()
    {
        var seen = await SendAsync("198.51.100.9", Settings(("ForwardedHeaders:KnownProxies:0", ProxyIp), ("ForwardedHeaders:KnownNetworks:0", "172.18.0.0/16")));

        seen.RemoteIp.ShouldBe("198.51.100.9");
        seen.Scheme.ShouldBe("http");
    }

    [Fact]
    public async Task WithoutTrustedProxies_IgnoresForwardedHeaders_EvenFromLoopback()
    {
        var seen = await SendAsync("127.0.0.1", Settings());

        seen.RemoteIp.ShouldBe("127.0.0.1");
        seen.Scheme.ShouldBe("http");
    }

    [Theory]
    [InlineData("ForwardedHeaders:KnownProxies:0", "caddy")]
    [InlineData("ForwardedHeaders:KnownNetworks:0", "172.18.0.0")]
    [InlineData("ForwardedHeaders:KnownNetworks:0", "172.18.0.0/40")]
    public void InvalidTrustedProxyConfiguration_FailsAtStartup(string key, string value)
    {
        using var factory = Create(Settings((key, value)), new Probe(IPAddress.Loopback));

        var exception = Should.Throw<Exception>(() => factory.CreateClient().Dispose());

        exception.ToString().ShouldContain("ForwardedHeaders");
    }

    private static Dictionary<string, string?> Settings(params (string Key, string Value)[] pairs) =>
        pairs.ToDictionary(p => p.Key, string? (p) => p.Value, StringComparer.Ordinal);

    private static async Task<Probe> SendAsync(string remoteIp, Dictionary<string, string?> settings)
    {
        var probe = new Probe(IPAddress.Parse(remoteIp));
        await using var factory = Create(settings, probe);
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/health/live", UriKind.Relative));
        request.Headers.Add("X-Forwarded-For", ClientIp);
        request.Headers.Add("X-Forwarded-Proto", "https");

        using var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return probe;
    }

    private static WebApplicationFactory<Program> Create(Dictionary<string, string?> settings, Probe probe) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("role", "api");
            foreach (var (key, value) in settings)
            {
                builder.UseSetting(key, value);
            }

            builder.ConfigureTestServices(services => services.AddSingleton<IStartupFilter>(new ProbeStartupFilter(probe)));
        });

    private sealed class Probe(IPAddress connectionIp)
    {
        public IPAddress ConnectionIp { get; } = connectionIp;

        public string? RemoteIp { get; set; }

        public string? Scheme { get; set; }
    }

    // Uygulamanın bütün ara katmanlarının dışında: bağlantı IP'sini verir, istek bitince uygulamanın gördüğünü kaydeder.
    private sealed class ProbeStartupFilter(Probe probe) : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            app.Use(async (HttpContext context, RequestDelegate nextMiddleware) =>
            {
                context.Connection.RemoteIpAddress = probe.ConnectionIp;
                await nextMiddleware(context);
                probe.RemoteIp = context.Connection.RemoteIpAddress?.ToString();
                probe.Scheme = context.Request.Scheme;
            });
            next(app);
        };
    }
}
