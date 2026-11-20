using Microsoft.AspNetCore.Mvc.Testing;
using Omvila.TestKit;

namespace Omvila.Host.Tests;

// Kural: roller-yetkiler.md § Uygulama kuralları 1 — her API ucu ya izin ister ya da açıkça anonimdir.
public sealed class EndpointAuditTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public void EveryApiEndpoint_DeclaresPermissionOrAnonymous()
    {
        using var client = factory.CreateClient();

        EndpointAudit.FindUnmarkedApiEndpoints(factory.Services).ShouldBeEmpty();
    }
}
