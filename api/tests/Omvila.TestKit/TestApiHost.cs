using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Http.Cors;
using Omvila.Infrastructure.Idempotency;
using Omvila.SharedKernel;

namespace Omvila.TestKit;

/// <summary>Altyapıyı Host'taki sırayla kuran küçük TestServer uygulaması; deneme uçları "/api/test" grubunda.</summary>
public static class TestApiHost
{
    public static async Task<WebApplication> StartAsync(
        PostgresFixture db,
        Action<RouteGroupBuilder> mapEndpoints,
        Action<IServiceCollection>? configureServices = null,
        Action<WebApplication>? configurePipeline = null,
        string? audience = null)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Testing" });
        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(db.Settings());
        builder.Services.AddSingleton<IClock>(TestClock.Create());
        builder.Services.AddSingleton<IIdGenerator, UuidV7IdGenerator>();
        builder.Services.AddOmvilaProblemDetails();
        builder.Services.AddOmvilaInfrastructure(builder.Configuration);
        configureServices?.Invoke(builder.Services);

        var app = builder.Build();
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseSurfaceCors();
        app.UseIdempotencyRequestBuffering();
        configurePipeline?.Invoke(app);
        app.UseOmvilaDbSession();
        mapEndpoints(app.MapApiGroup("/api/test", audience));
        await app.StartAsync(TestContext.Current.CancellationToken);
        return app;
    }
}
