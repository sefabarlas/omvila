using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Omvila.Infrastructure.Sms;
using Omvila.TestKit;

namespace Omvila.Modules.Notifications.IntegrationTests;

// Kural: kodlama-dotnet.md — sahte sağlayıcı (OTP'yi günlüğe yazar) yalnız Development ve test ortamında.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class SmsProviderSelectionTests(PostgresFixture db)
{
    [Theory]
    [InlineData("Production")]
    [InlineData("Staging")]
    public async Task FakeProvider_OutsideDevelopmentOrTesting_IsRejected(string environment)
    {
        await using var services = db.CreateServices(configure: s => s.AddSingleton<IHostEnvironment>(new Env(environment)));
        await using var scope = services.CreateAsyncScope();

        Should.Throw<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<ISmsSender>());
    }

    [Fact]
    public async Task UnknownProvider_IsRejected()
    {
        // Sağlayıcı seçimi açık soru T-03; karar verilene kadar yalnız Fake.
        await using var services = db.CreateServices(configure: s => s.Configure<SmsOptions>(o => o.Provider = "Netgsm"));
        await using var scope = services.CreateAsyncScope();

        Should.Throw<InvalidOperationException>(() => scope.ServiceProvider.GetRequiredService<ISmsSender>());
    }

    private sealed class Env(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;

        public string ApplicationName { get; set; } = "Omvila.Tests";

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
