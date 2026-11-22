using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Omvila.TestKit;

namespace Omvila.Host.Tests;

// Kural: T-73 — test sunucusu (Staging): sahte SMS/push yalnız Staging:AllowFakeProviders=true iken, Production'da hiçbir
// ayarla; imza, OTP ve IBAN (T-78) anahtarı gizli değerden, eksikse açılış durur; CORS kökenleri https (T-60).
// appsettings.Staging.json gerçek dosyadır (Sms/Push Provider=Fake); gizli değerler ortam değişkeni gibi verilir.
public sealed class StagingStartupTests
{
    [Fact]
    public async Task Staging_WithSecretsAndFlag_Starts()
    {
        await using var factory = Create("Staging", Secrets(allowFake: "true"));
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/health/live", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Staging_WithoutFakeProviderFlag_FailsAtStartup()
    {
        var settings = Secrets(allowFake: null);

        var error = await StartupErrorAsync("Staging", settings);

        error.ShouldContain("Staging__AllowFakeProviders");
    }

    [Fact]
    public async Task Production_WithFakeProviderAndFlag_FailsAtStartup()
    {
        var settings = Secrets(allowFake: "true");
        settings["Sms:Provider"] = "Fake";

        var error = await StartupErrorAsync("Production", settings);

        error.ShouldContain("Sms:Provider=Fake Production");
    }

    [Theory]
    [InlineData("Auth:Otp:HmacKey", "Auth__Otp__HmacKey")]
    [InlineData("Auth:SigningKeys:0:PrivateKeyPem", "Auth__SigningKeys__0__PrivateKeyPem")]
    [InlineData("Tenancy:IbanEncryptionKey", "Tenancy__IbanEncryptionKey")]
    [InlineData("Customers:DataExport:LinkSigningKey", "Customers__DataExport__LinkSigningKey")]
    public async Task Staging_WithoutSecret_FailsAtStartup(string missingKey, string expectedHint)
    {
        var settings = Secrets(allowFake: "true");
        settings.Remove(missingKey);

        var error = await StartupErrorAsync("Staging", settings);

        error.ShouldContain(expectedHint);
    }

    [Fact]
    public async Task Staging_WithHttpCorsOrigin_FailsAtStartup()
    {
        var settings = Secrets(allowFake: "true");
        settings["Cors:Surfaces:Manage:Origins:0"] = "http://salon.test.omvila.com.tr";

        var error = await StartupErrorAsync("Staging", settings);

        error.ShouldContain("https");
    }

    private static Dictionary<string, string?> Secrets(string? allowFake)
    {
        var settings = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["Auth:Otp:HmacKey"] = TestSigningKey.OtpHmacKey,
            ["Auth:SigningKeys:0:Kid"] = TestSigningKey.Kid,
            ["Auth:SigningKeys:0:PrivateKeyPem"] = TestSigningKey.PrivateKeyPem,
            ["Tenancy:IbanEncryptionKey"] = TestSigningKey.IbanEncryptionKey,
            ["Customers:DataExport:LinkSigningKey"] = TestSigningKey.DataExportLinkKey,
            ["Cors:Surfaces:Manage:Origins:0"] = "https://salon.test.omvila.com.tr",
            ["Cors:Surfaces:Public:Origins:0"] = "https://randevu.test.omvila.com.tr",
        };
        if (allowFake is not null)
        {
            settings["Staging:AllowFakeProviders"] = allowFake;
        }

        return settings;
    }

    private static WebApplicationFactory<Program> Create(string environment, Dictionary<string, string?> settings) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
            // Veritabanı yok: yalnız api rolü (iş döngüsü yok), veri kaynağı ilk kullanımda kurulur.
            builder.UseSetting("role", "api");
            builder.UseSetting("ConnectionStrings:Default", "Host=127.0.0.1;Port=1;Database=x;Username=x;Password=x;Timeout=1");
            foreach (var (key, value) in settings)
            {
                builder.UseSetting(key, value);
            }
        });

    private static async Task<string> StartupErrorAsync(string environment, Dictionary<string, string?> settings)
    {
        await using var factory = Create(environment, settings);
        var exception = Should.Throw<Exception>(() => factory.CreateClient().Dispose());
        return exception.ToString();
    }
}
