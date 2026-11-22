using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Omvila.Infrastructure.Email;
using Omvila.Infrastructure.Files;
using Omvila.Infrastructure.Health;
using Omvila.Infrastructure.Hosting;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Http.Cors;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Infrastructure.Push;
using Omvila.Infrastructure.Sms;

namespace Omvila.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>Uygulama rolünün bağlantı dizesi (ConnectionStrings:Default, omvila_app).</summary>
    public const string AppConnectionName = "Default";

    /// <summary>Migration bağlantı dizesi (ConnectionStrings:Migrator, omvila_migrator).</summary>
    public const string MigratorConnectionName = "Migrator";

    public static IServiceCollection AddOmvilaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Sözleşme JSON'u: camelCase, enum metin (PascalCase), boş isteğe bağlı alan yazılmaz, NodaTime tipleri.
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        });
        AddMessagingProviders(services, configuration);

        // Veri kaynağı ilk kullanımda kurulur: OpenAPI belge üretimi gibi veritabanısız çalıştırmalar etkilenmez.
        // Bağlantılar bu veri kaynağından açılır ve DbContext'lere verilir: NodaTime eşlemesi burada da açık olmalı.
        services.AddSingleton(_ => new NpgsqlDataSourceBuilder(
                configuration.GetConnectionString(AppConnectionName)
                ?? throw new InvalidOperationException($"ConnectionStrings:{AppConnectionName} tanımlı değil."))
            .UseNodaTime()
            .Build());

        services.AddScoped<DbSession>();
        services.AddScoped<IDbSession>(sp => sp.GetRequiredService<DbSession>());
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<DbSession>());
        services.AddScoped<TransactionEndpointFilter>();
        services.AddExceptionHandler<DatabaseExceptionHandler>();
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddModuleDbContext<InfraDbContext>(InfraDbContext.Schema);
        services.AddTransient<MigrationRunner>();

        services.AddOptions<OutboxOptions>()
            .Bind(configuration.GetSection(OutboxOptions.Section))
            .Validate(o => o.IsValid(), "Outbox ayarları eksik veya geçersiz (appsettings \"Outbox\").");
        services.AddSingleton<IntegrationEventRegistry>();
        services.AddScoped<IOutbox, Outbox>();
        services.AddSingleton<JobRunner>();
        services.AddPeriodicJob<OutboxDispatcher, OutboxOptions>(o => Duration.FromSeconds(o.DispatchIntervalSeconds));

        services.AddOptions<IdempotencyOptions>()
            .Bind(configuration.GetSection(IdempotencyOptions.Section))
            .Validate(o => o.IsValid(), "Idempotency ayarları eksik veya geçersiz (appsettings \"Idempotency\").");
        services.AddPeriodicJob<PurgeIdempotencyKeys, IdempotencyOptions>(o => Duration.FromMinutes(o.PurgeIntervalMinutes));

        services.AddSurfaceCors(configuration);
        services.AddTrustedForwardedHeaders(configuration);

        services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
        return services;
    }

    // SMS ve push sağlayıcıları, teslim raporu adaptörleri.
    private static void AddMessagingProviders(IServiceCollection services, IConfiguration configuration)
    {
        // Sahte sağlayıcı izni ortama bağlı (HostEnvironmentExtensions.AllowsFakeProviders); izinsizse açılış durur.
        services.AddOptions<StagingOptions>().Bind(configuration.GetSection(StagingOptions.Section));
        services.AddSingleton<IValidateOptions<SmsOptions>, FakeProviderOptionsValidator>();
        services.AddSingleton<IValidateOptions<PushOptions>, FakeProviderOptionsValidator>();
        services.AddOptions<SmsOptions>().Bind(configuration.GetSection(SmsOptions.Section)).ValidateOnStart();
        services.AddScoped<ISmsSender>(sp => CreateSmsSender(sp));
        services.AddOptions<DevSmsLogOptions>()
            .Bind(configuration.GetSection(DevSmsLogOptions.Section))
            .Validate(o => o.IsValid(), "Sahte SMS günlüğü ayarları geçersiz (appsettings \"Sms:DevLog\": Limit 1–1000, RefreshSeconds 0–3600).");
        services.AddOptions<PushOptions>().Bind(configuration.GetSection(PushOptions.Section)).ValidateOnStart();
        services.AddHttpClient(FcmPushSender.HttpClientName);
        services.AddScoped<IPushSender>(sp => CreatePushSender(sp));
        services.AddSingleton<IValidateOptions<EmailOptions>, FakeProviderOptionsValidator>();
        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.Section))
            .Validate(o => o.IsValid(), "E-posta ayarları eksik veya geçersiz (appsettings \"Email\": Provider boş, Fake veya Smtp).")
            .ValidateOnStart();
        services.AddSingleton<FakeEmailSender>();
        services.AddScoped<IEmailSender>(sp => CreateEmailSender(sp));
        services.AddOptions<FileStorageOptions>()
            .Bind(configuration.GetSection(FileStorageOptions.Section))
            .Validate(o => o.IsValid(), "Dosya deposu ayarı eksik (appsettings \"FileStorage:RootPath\").");
        services.AddSingleton<IFileStorage, DiskFileStorage>();

        // Teslim raporu adaptörleri sağlayıcı koduyla kaydedilir (webhook yolu /webhooks/sms/{provider}); kayıtsız kod 404.
        // Sahte sağlayıcının raporu yalnız sahte sağlayıcı seçiliyken (Development, Testing, izinli Staging) açılır.
        services.AddOptions<FakeSmsProviderOptions>().Bind(configuration.GetSection(FakeSmsProviderOptions.Section));
        if (string.Equals(configuration[$"{SmsOptions.Section}:{nameof(SmsOptions.Provider)}"], SmsOptions.FakeProvider, StringComparison.Ordinal))
        {
            services.AddKeyedSingleton<ISmsDeliveryReportVerifier, FakeSmsDeliveryReportVerifier>(FakeSmsDeliveryReportVerifier.ProviderCode);
        }
    }

    // Sağlayıcı seçimi (T-03): şimdilik yalnız sahte sağlayıcı. Ortam izni FakeProviderOptionsValidator'da (options çözülürken).
    private static FakeSmsSender CreateSmsSender(IServiceProvider services)
    {
        var options = services.GetRequiredService<IOptions<SmsOptions>>().Value;
        if (!string.Equals(options.Provider, SmsOptions.FakeProvider, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Desteklenmeyen SMS sağlayıcısı: '{options.Provider}' (T-03 karar bekliyor).");
        }

        return ActivatorUtilities.CreateInstance<FakeSmsSender>(services);
    }

    // Push sağlayıcısı: boş → kapalı; Fake ortam izniyle (FakeProviderOptionsValidator); Fcm kimlik bilgisi yoksa kapalı.
    private static IPushSender CreatePushSender(IServiceProvider services)
    {
        var options = services.GetRequiredService<IOptions<PushOptions>>().Value;
        return options.Provider switch
        {
            "" => new DisabledPushSender(),
            PushOptions.FakeProvider => ActivatorUtilities.CreateInstance<FakePushSender>(services),
            PushOptions.FcmProvider => options.Fcm.IsConfigured ? ActivatorUtilities.CreateInstance<FcmPushSender>(services) : new DisabledPushSender(),
            _ => throw new InvalidOperationException($"Desteklenmeyen push sağlayıcısı: '{options.Provider}'."),
        };
    }

    // E-posta sağlayıcısı (T-05): boş → kapalı; Fake ortam izniyle (FakeProviderOptionsValidator); Smtp (Mailpit).
    private static IEmailSender CreateEmailSender(IServiceProvider services)
    {
        var options = services.GetRequiredService<IOptions<EmailOptions>>().Value;
        return options.Provider switch
        {
            "" => new DisabledEmailSender(),
            EmailOptions.FakeProvider => services.GetRequiredService<FakeEmailSender>(),
            EmailOptions.SmtpProvider => ActivatorUtilities.CreateInstance<SmtpEmailSender>(services),
            _ => throw new InvalidOperationException($"Desteklenmeyen e-posta sağlayıcısı: '{options.Provider}'."),
        };
    }

    /// <summary>
    /// API uçlarının veritabanı oturumu. UseAuthentication ve UseAuthorization'dan sonra, uçlardan önce çağrılır.
    /// </summary>
    public static IApplicationBuilder UseOmvilaDbSession(this IApplicationBuilder app) =>
        app.UseMiddleware<DbSessionMiddleware>();

    /// <summary>Kayıtlı periyodik işleri zamanlar. Yalnız worker rolünde çağrılır.</summary>
    public static IServiceCollection AddOmvilaWorker(this IServiceCollection services) =>
        services.AddHostedService<JobHostedService>();

    /// <summary>
    /// API yüzeyi grubu (ör. "/api/v1/manage", Audiences.Manage). Gruptaki her uç tek transaction'da çalışır.
    /// Ön ekin yüzeyine göre (manage, admin, me, public) o yüzeyin CORS politikası bağlanır.
    /// Yüzey verilirse uçlar o yüzeyin jetonunu ister; açık uç <c>.AllowAnonymous()</c> + gerekçe taşır.
    /// </summary>
    public static RouteGroupBuilder MapApiGroup(this IEndpointRouteBuilder endpoints, string prefix, string? audience = null)
    {
        var group = endpoints.MapGroup(prefix);
        group.WithMetadata(new TransactionalEndpointMetadata());
        group.AddEndpointFilter<TransactionEndpointFilter>();
        group.RequireSurfaceCors(prefix);
        if (audience is not null)
        {
            group.RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = audience });
        }

        return group;
    }
}
