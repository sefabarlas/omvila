using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Sms;
using Omvila.Modules.Catalog;
using Omvila.Modules.Customers;
using Omvila.Modules.Identity;
using Omvila.Modules.Integrations;
using Omvila.Modules.Notifications;
using Omvila.Modules.Platform;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Reviews;
using Omvila.Modules.Sales;
using Omvila.Modules.Scheduling;
using Omvila.Modules.Tenancy;
using Omvila.SharedKernel;
using Testcontainers.PostgreSql;
using Xunit;

namespace Omvila.TestKit;

/// <summary>
/// Gerçek PostgreSQL 17 (Testcontainers). Yerel geliştirmeyle aynı rol betiğini (api/db/init) çalıştırır
/// ve bütün migration'ları migrator rolüyle uygular. Testler uygulama rolüyle (omvila_app) bağlanır.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private const string Password = "dev_only_password";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17")
        .WithDatabase("omvila")
        .WithUsername("omvila_migrator")
        .WithPassword(Password)
        .WithResourceMapping(new FileInfo(InitScriptPath()), new DirectoryInfo("/docker-entrypoint-initdb.d/"))
        .Build();

    public string MigratorConnectionString => _container.GetConnectionString();

    public string AppConnectionString => new NpgsqlConnectionStringBuilder(MigratorConnectionString)
    {
        Username = "omvila_app",
        Password = Password,
    }.ConnectionString;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(Settings()).Build();

        // Bütün modüllerin migration'ları (her modül kendi DbContext'ini AddModuleDbContext ile kaydeder).
        await using var services = CreateServices(configure: s => s
            .AddIdentityModule(configuration)
            .AddNotificationsModule(configuration)
            .AddTenancyModule(configuration)
            .AddPlatformModule(configuration)
            .AddCatalogModule()
            .AddCustomersModule()
            .AddSalesModule()
            .AddReviewsModule()
            .AddSchedulingModule(configuration)
            .AddIntegrationsModule(configuration));
        await services.GetRequiredService<MigrationRunner>().RunAsync(MigratorConnectionString, CancellationToken.None);
        await services.GetRequiredService<PlanSeeder>().SeedAsync(MigratorConnectionString, CancellationToken.None);
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();

    /// <summary>
    /// Uygulama rolüne bağlı altyapı servisleri (DbSession, DbContext'ler, outbox, iş çalıştırıcısı).
    /// Saat TestClock (FakeClock); testte çözülüp ilerletilebilir. Her test kendi kapsamını açar.
    /// </summary>
    public ServiceProvider CreateServices(int? maxPoolSize = null, Action<IServiceCollection>? configure = null)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(Settings(maxPoolSize)).Build();

        var services = new ServiceCollection()
            .AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(NullLogger<>))
            .AddSingleton<IClock>(TestClock.Create())
            .AddSingleton<IIdGenerator, UuidV7IdGenerator>()
            .AddOmvilaInfrastructure(configuration);
        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }

    /// <summary>Settings() değerlerinden yapılandırma (modül kayıtları için).</summary>
    public IConfiguration Configuration() => new ConfigurationBuilder().AddInMemoryCollection(Settings()).Build();

    /// <summary>Uygulamanın test yapılandırması: bağlantı dizeleri ve outbox ayarları (TestServer uygulamaları da kullanır).</summary>
    public IDictionary<string, string?> Settings(int? maxPoolSize = null)
    {
        var app = new NpgsqlConnectionStringBuilder(AppConnectionString);
        if (maxPoolSize is { } size)
        {
            app.MaxPoolSize = size;
        }

        return new Dictionary<string, string?>(BaseSettings, StringComparer.Ordinal)
        {
            [$"ConnectionStrings:{InfrastructureServiceCollectionExtensions.AppConnectionName}"] = app.ConnectionString,
            [$"ConnectionStrings:{InfrastructureServiceCollectionExtensions.MigratorConnectionName}"] = MigratorConnectionString,
        };
    }

    // Bağlantı dizeleri dışındaki test ayarları (outbox, iş aralıkları, kimlik, sahte sağlayıcılar).
    private static readonly Dictionary<string, string?> BaseSettings = new(StringComparer.Ordinal)
    {
        ["Outbox:BatchSize"] = "50",
        ["Outbox:DispatchIntervalSeconds"] = "1",
        ["Outbox:RetryBaseDelaySeconds"] = "30",
        ["Outbox:RetryMaxDelaySeconds"] = "3600",
        ["Idempotency:RetentionHours"] = "24",
        ["Idempotency:PurgeIntervalMinutes"] = "60",
        ["Cors:PreflightMaxAgeSeconds"] = "600",
        ["Notifications:DispatchIntervalSeconds"] = "1",
        ["Notifications:BatchSize"] = "50",
        ["Notifications:MaxAttempts"] = "5",
        ["Notifications:RetryBaseDelaySeconds"] = "30",
        ["Notifications:RetryMaxDelaySeconds"] = "3600",
        ["Sales:ExpireCustomerPackagesIntervalMinutes"] = "60",
        ["Customers:DataExport:ProcessIntervalSeconds"] = "60",
        ["Customers:DataExport:ApplyRetentionIntervalMinutes"] = "1440",
        ["Customers:DataExport:DownloadBaseUrl"] = "https://api.omvila.com/",
        ["Scheduling:ExpireAppointmentHoldsIntervalSeconds"] = "60",
        ["Scheduling:MarkNoShowsIntervalSeconds"] = "60",
        ["Scheduling:ExtendRecurringSeriesIntervalMinutes"] = "60",
        ["Scheduling:ExpireWaitlistOffersIntervalSeconds"] = "60",
        ["Scheduling:ExpireWaitlistEntriesIntervalMinutes"] = "60",
        ["Integrations:Calendar:Provider"] = "Fake",
        ["Integrations:Calendar:RedirectUris:Web"] = "https://salon.omvila.com/ayarlar/google-takvim",
        ["Integrations:Calendar:StateMinutes"] = "10",
        ["Integrations:Calendar:SyncWindowDays"] = "60",
        ["Integrations:Calendar:SyncIntervalSeconds"] = "300",
        ["Integrations:Calendar:RefreshAheadMinutes"] = "10",
        ["Scheduling:SendDailySummariesIntervalSeconds"] = "300",
        ["Sms:Provider"] = "Fake",
        ["Push:Provider"] = "Fake",
        ["Sms:DefaultSender"] = "OMVILA",
        [$"{FakeSmsProviderOptions.Section}:{nameof(FakeSmsProviderOptions.WebhookSecret)}"] = TestSmsWebhook.Secret,
        ["Auth:Issuer"] = "omvila",
        ["Auth:AccessTokenMinutes"] = "15",
        ["Auth:RefreshTokenDaysRemembered"] = "30",
        ["Auth:RefreshTokenDaysSession"] = "1",
        ["Auth:Otp:Length"] = "6",
        ["Auth:Otp:TtlSeconds"] = "180",
        ["Auth:Otp:ResendCooldownSeconds"] = "60",
        ["Auth:Otp:MaxAttempts"] = "5",
        ["Auth:Otp:LockoutMinutes"] = "5",
        ["Auth:Otp:MaxPerPhonePerHour"] = "5",
        ["Auth:Otp:MaxPerIpPerHour"] = "20",
        ["Auth:SignupTokenMinutes"] = "15",
        ["Auth:PublicBookingTokenMinutes"] = "30",
        ["Auth:TrustedDeviceDays"] = "30",
        ["Auth:PanelSignInUrl"] = "https://omvila.com/giris",
        ["Legal:TermsOfUseVersion"] = "2026-11-01",
        ["Legal:PrivacyNoticeVersion"] = "2026-11-01",
        ["Tenancy:BookingPageBaseUrl"] = "https://omvila.com/",
        ["Platform:TrialDays"] = "14",
        ["Platform:PastDueGraceDays"] = "7",
        ["Platform:TrialPlanCode"] = "salon_plus",
        ["Platform:TrialCredits"] = "100",
        ["Platform:SeedPlans:0:Code"] = "salon_plus",
        ["Platform:SeedPlans:0:Name"] = "Salon Plus",
        ["Platform:ReadOnlyToCancelledDays"] = "30",
        ["Platform:CancelledArchiveDays"] = "30",
        ["Platform:TrialEndingNoticeDays"] = "3",
        ["Platform:AdvanceSubscriptionStatusesIntervalMinutes"] = "60",
        ["Platform:SeedCreditPackages:0:Code"] = "starter",
        ["Platform:SeedCreditPackages:0:Name"] = "Başlangıç",
        ["Platform:SeedCreditPackages:0:Credits"] = "1000",
        ["Platform:SeedCreditPackages:1:Code"] = "medium",
        ["Platform:SeedCreditPackages:1:Name"] = "Orta",
        ["Platform:SeedCreditPackages:1:Credits"] = "5000",
        ["Email:Provider"] = "Fake",
        ["Email:FromAddress"] = "bildirim@omvila.com",
        ["Email:FromName"] = "OMVILA",
        ["Email:PanelBaseUrl"] = "https://salon.omvila.com/",
        ["FileStorage:RootPath"] = Path.Combine(Path.GetTempPath(), "omvila-test-files"),
        ["Auth:Otp:HmacKey"] = TestSigningKey.OtpHmacKey,
        ["Auth:SigningKeys:0:Kid"] = TestSigningKey.Kid,
        ["Auth:SigningKeys:0:PrivateKeyPem"] = TestSigningKey.PrivateKeyPem,
        ["Tenancy:IbanEncryptionKey"] = TestSigningKey.IbanEncryptionKey,
        ["Customers:DataExport:LinkSigningKey"] = TestSigningKey.DataExportLinkKey,
        ["Integrations:Calendar:TokenEncryptionKey"] = TestSigningKey.CalendarTokenKey,
    };

    /// <summary>Migrator rolüyle (tablo sahibi, RLS'i atlar) açık bağlantı: test verisi ve şema kurulumu için.</summary>
    public async Task<NpgsqlConnection> OpenMigratorConnectionAsync(CancellationToken ct)
    {
        var connection = new NpgsqlConnection(MigratorConnectionString);
        await connection.OpenAsync(ct);
        return connection;
    }

    private static string InitScriptPath()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Omvila.slnx")))
        {
            dir = dir.Parent;
        }

        return dir is null
            ? throw new InvalidOperationException("api/ kökü bulunamadı.")
            : Path.Combine(dir.FullName, "db", "init", "01-roles.sql");
    }
}
