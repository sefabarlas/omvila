using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Omvila.Host.Tests;

// Periyodik işler yalnız worker rolünde zamanlanır (docs/mimari/mimari.md § Genel resim).
public sealed class WorkerRoleTests
{
    [Theory]
    [InlineData("api", false)]
    [InlineData("worker", true)]
    [InlineData("all", true)]
    public async Task JobHostedService_IsRegisteredOnlyForWorkerRole(string role, bool expected)
    {
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder
            .UseSetting("role", role)
            // Veritabanı yok: iş döngüsü hatayı günlüğe yazıp sürmeli, uygulama düşmemeli.
            .UseSetting("ConnectionStrings:Default", "Host=127.0.0.1;Port=1;Database=x;Username=x;Password=x;Timeout=1"));

        var hosted = factory.Services.GetServices<IHostedService>().Select(s => s.GetType().Name);

        hosted.Contains("JobHostedService", StringComparer.Ordinal).ShouldBe(expected);
    }
}
