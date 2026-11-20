using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

/// <summary>Altyapıyı Host'taki sırayla kuran küçük TestServer uygulaması; deneme uçları "/api/test" grubunda.</summary>
internal static class TestApiHost
{
    public static async Task<WebApplication> StartAsync(
        PostgresFixture db,
        Action<RouteGroupBuilder> mapEndpoints,
        Action<IServiceCollection>? configureServices = null)
    {
        var builder = WebApplication.CreateBuilder();
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
        app.UseIdempotencyRequestBuffering();
        mapEndpoints(app.MapApiGroup("/api/test"));
        await app.StartAsync(TestContext.Current.CancellationToken);
        return app;
    }
}
