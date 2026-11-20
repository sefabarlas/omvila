using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using NodaTime.Testing;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

/// <summary>
/// Gerçek uygulama (Program, role=api) + test veritabanı + sabit saat (TestClock). Ayarlar test sınıfına göre ezilebilir.
/// </summary>
internal sealed class OmvilaApiFactory(PostgresFixture db, IDictionary<string, string?>? overrides = null) : WebApplicationFactory<Program>
{
    public FakeClock Clock { get; } = TestClock.Create();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("role", "api");
        foreach (var (key, value) in db.Settings())
        {
            builder.UseSetting(key, value);
        }

        foreach (var (key, value) in overrides ?? new Dictionary<string, string?>(StringComparer.Ordinal))
        {
            builder.UseSetting(key, value);
        }

        builder.ConfigureTestServices(services => services.Replace(ServiceDescriptor.Singleton<IClock>(Clock)));
    }
}
