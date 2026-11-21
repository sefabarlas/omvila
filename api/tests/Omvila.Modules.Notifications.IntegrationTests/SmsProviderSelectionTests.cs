using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Hosting;
using Omvila.Infrastructure.Sms;
using Omvila.TestKit;

namespace Omvila.Modules.Notifications.IntegrationTests;

// Kural: kodlama-dotnet.md — sahte sağlayıcı (OTP'yi günlüğe yazar) yalnız Development ve test ortamında;
// T-73: test sunucusunda (Staging) yalnız Staging:AllowFakeProviders=true iken, Production'da hiçbir ayarla.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class SmsProviderSelectionTests(PostgresFixture db)
{
    [Theory]
    [InlineData("Production", false)]
    [InlineData("Production", true)]
    [InlineData("Staging", false)]
    public async Task FakeProvider_WithoutEnvironmentPermission_IsRejected(string environment, bool stagingFlag)
    {
        await using var services = db.CreateServices(configure: s => s
            .AddSingleton<IHostEnvironment>(new Env(environment))
            .Configure<StagingOptions>(o => o.AllowFakeProviders = stagingFlag));
        await using var scope = services.CreateAsyncScope();

        Should.Throw<OptionsValidationException>(() => scope.ServiceProvider.GetRequiredService<ISmsSender>());
    }

    [Fact]
    public async Task FakeProvider_InStagingWithFlag_IsAllowed()
    {
        await using var services = db.CreateServices(configure: s => s
            .AddSingleton<IHostEnvironment>(new Env(Environments.Staging))
            .Configure<StagingOptions>(o => o.AllowFakeProviders = true));
        await using var scope = services.CreateAsyncScope();

        // Ortam denetimi seçenekler çözülürken yapılır; gönderici veritabanı oturumu ister, burada yalnız izin denetlenir.
        scope.ServiceProvider.GetRequiredService<IOptions<SmsOptions>>().Value.Provider.ShouldBe(SmsOptions.FakeProvider);
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
