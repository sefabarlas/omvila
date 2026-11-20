using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Omvila.Infrastructure.Health;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
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
        services.AddOptions<SmsOptions>().Bind(configuration.GetSection(SmsOptions.Section));
        services.AddScoped<ISmsSender>(sp => CreateSmsSender(sp));

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
        services.AddPeriodicJob<OutboxDispatcher>();

        services.AddOptions<IdempotencyOptions>()
            .Bind(configuration.GetSection(IdempotencyOptions.Section))
            .Validate(o => o.IsValid(), "Idempotency ayarları eksik veya geçersiz (appsettings \"Idempotency\").");
        services.AddPeriodicJob<PurgeIdempotencyKeys>();

        services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
        return services;
    }

    // Sağlayıcı seçimi (T-03): şimdilik yalnız sahte sağlayıcı; o da yalnız Development ve test ortamında.
    private static FakeSmsSender CreateSmsSender(IServiceProvider services)
    {
        var options = services.GetRequiredService<Microsoft.Extensions.Options.IOptions<SmsOptions>>().Value;
        var environment = services.GetService<IHostEnvironment>();
        if (!string.Equals(options.Provider, SmsOptions.FakeProvider, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Desteklenmeyen SMS sağlayıcısı: '{options.Provider}' (T-03 karar bekliyor).");
        }

        if (environment is not null && !environment.IsDevelopment() && !environment.IsEnvironment("Testing"))
        {
            throw new InvalidOperationException("Sahte SMS sağlayıcısı yalnız Development ve Testing ortamında kullanılabilir.");
        }

        return ActivatorUtilities.CreateInstance<FakeSmsSender>(services);
    }

    /// <summary>Kayıtlı periyodik işleri zamanlar. Yalnız worker rolünde çağrılır.</summary>
    public static IServiceCollection AddOmvilaWorker(this IServiceCollection services) =>
        services.AddHostedService<JobHostedService>();

    /// <summary>
    /// API yüzeyi grubu (ör. "/api/v1/manage", Audiences.Manage). Gruptaki her uç tek transaction'da çalışır.
    /// Yüzey verilirse uçlar o yüzeyin jetonunu ister; açık uç <c>.AllowAnonymous()</c> + gerekçe taşır.
    /// </summary>
    public static RouteGroupBuilder MapApiGroup(this IEndpointRouteBuilder endpoints, string prefix, string? audience = null)
    {
        var group = endpoints.MapGroup(prefix);
        group.AddEndpointFilter<TransactionEndpointFilter>();
        if (audience is not null)
        {
            group.RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = audience });
        }

        return group;
    }
}
