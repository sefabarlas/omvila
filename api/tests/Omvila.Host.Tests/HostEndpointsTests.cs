using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Omvila.Host.Tests;

public sealed class HostEndpointsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task Health_ReturnsHealthy(string path)
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri(path, UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldBe("Healthy");
    }

    // Kural: docs/sozlesme/api-kurallari.md § Hatalar (Problem: type, title, status, code zorunlu)
    [Fact]
    public async Task UnknownRoute_ReturnsProblemJsonWithContractFields()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/api/v1/manage/yok", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/problem+json");
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var root = body.RootElement;
        root.GetProperty("code").GetString().ShouldBe("not_found");
        root.GetProperty("type").GetString().ShouldBe("https://docs.omvila.com/errors/not_found");
        root.GetProperty("status").GetInt32().ShouldBe(404);
        root.GetProperty("title").GetString().ShouldNotBeNullOrWhiteSpace();
        root.GetProperty("traceId").GetString().ShouldNotBeNullOrWhiteSpace();
        root.TryGetProperty("detail", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task OpenApiDocument_IsServed()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        body.RootElement.GetProperty("info").GetProperty("title").GetString().ShouldBe("OMVILA API");
    }
}
