using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.TestKit;

namespace Omvila.Host.Tests;

// Kural: T-76 — /dev/sms yalnız sahte SMS sağlayıcısı etkinken eşlenir (Development, Testing, izinli Staging);
// Production'da yol yoktur (404). Sayfanın içeriği Omvila.Api.IntegrationTests/DevSmsLogPageTests'te.
public sealed class DevSmsLogMappingTests
{
    private const string Path = "/dev/sms";

    [Fact]
    public async Task Production_HasNoDevSmsPage()
    {
        await using var factory = Create("Production", allowFake: null, smsProvider: null);
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri(Path, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        IsMapped(factory).ShouldBeFalse();
    }

    [Fact]
    public async Task StagingWithFakeProviders_MapsDevSmsPage()
    {
        await using var factory = Create("Staging", allowFake: "true", smsProvider: null);
        using var client = factory.CreateClient();

        IsMapped(factory).ShouldBeTrue();
    }

    [Fact]
    public async Task Development_WithoutFakeSmsProvider_HasNoDevSmsPage()
    {
        await using var factory = Create("Development", allowFake: null, smsProvider: "");
        using var client = factory.CreateClient();

        IsMapped(factory).ShouldBeFalse();
    }

    [Fact]
    public async Task DevSmsPage_IsNotInOpenApiDocument()
    {
        await using var factory = Create("Development", allowFake: null, smsProvider: null);
        using var client = factory.CreateClient();

        var document = await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);

        IsMapped(factory).ShouldBeTrue();
        document.ShouldNotContain("/dev/sms");
    }

    private static bool IsMapped(WebApplicationFactory<Program> factory) =>
        factory.Services.GetServices<EndpointDataSource>()
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Any(e => string.Equals(e.RoutePattern.RawText, Path, StringComparison.Ordinal));

    private static WebApplicationFactory<Program> Create(string environment, string? allowFake, string? smsProvider) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
            // Veritabanı yok: yalnız api rolü, veri kaynağı ilk kullanımda kurulur (404 veritabanına gitmez).
            builder.UseSetting("role", "api");
            builder.UseSetting("ConnectionStrings:Default", "Host=127.0.0.1;Port=1;Database=x;Username=x;Password=x;Timeout=1");
            builder.UseSetting("Auth:Otp:HmacKey", TestSigningKey.OtpHmacKey);
            builder.UseSetting("Auth:SigningKeys:0:Kid", TestSigningKey.Kid);
            builder.UseSetting("Auth:SigningKeys:0:PrivateKeyPem", TestSigningKey.PrivateKeyPem);
            if (allowFake is not null)
            {
                builder.UseSetting("Staging:AllowFakeProviders", allowFake);
            }

            if (smsProvider is not null)
            {
                builder.UseSetting("Sms:Provider", smsProvider);
            }
        });
}
