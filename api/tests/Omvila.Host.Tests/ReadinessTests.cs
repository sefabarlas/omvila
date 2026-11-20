using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Omvila.TestKit;

namespace Omvila.Host.Tests;

// /health/ready veritabanı bağlantısını denetler (docs/mimari/mimari.md § Kesişen konular: Sağlık).
[Trait("Category", "Integration")]
public sealed class ReadinessTests(PostgresFixture db) : IClassFixture<PostgresFixture>
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Ready_WithReachableDatabase_ReturnsHealthy()
    {
        await using var factory = WithConnectionString(db.AppConnectionString);
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative), Ct);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Ready_WithUnreachableDatabase_ReturnsServiceUnavailable()
    {
        await using var factory = WithConnectionString(
            "Host=127.0.0.1;Port=1;Database=omvila;Username=omvila_app;Password=x;Timeout=2");
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative), Ct);

        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
    }

    private static WebApplicationFactory<Program> WithConnectionString(string connectionString) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseSetting("ConnectionStrings:Default", connectionString));
}
