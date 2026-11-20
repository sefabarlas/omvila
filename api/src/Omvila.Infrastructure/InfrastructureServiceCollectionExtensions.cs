using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Omvila.Infrastructure.Health;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>Uygulama rolünün bağlantı dizesi (ConnectionStrings:Default, omvila_app).</summary>
    public const string AppConnectionName = "Default";

    /// <summary>Migration bağlantı dizesi (ConnectionStrings:Migrator, omvila_migrator).</summary>
    public const string MigratorConnectionName = "Migrator";

    public static IServiceCollection AddOmvilaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Veri kaynağı ilk kullanımda kurulur: OpenAPI belge üretimi gibi veritabanısız çalıştırmalar etkilenmez.
        services.AddSingleton(_ => NpgsqlDataSource.Create(
            configuration.GetConnectionString(AppConnectionName)
            ?? throw new InvalidOperationException($"ConnectionStrings:{AppConnectionName} tanımlı değil.")));

        services.AddScoped<DbSession>();
        services.AddScoped<IDbSession>(sp => sp.GetRequiredService<DbSession>());
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<DbSession>());
        services.AddScoped<TransactionEndpointFilter>();
        services.AddModuleDbContext<InfraDbContext>(InfraDbContext.Schema);
        services.AddTransient<MigrationRunner>();

        services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
        return services;
    }

    /// <summary>
    /// API yüzeyi grubu (ör. "/api/v1/manage"). Gruptaki her uç tek transaction'da çalışır.
    /// </summary>
    public static RouteGroupBuilder MapApiGroup(this IEndpointRouteBuilder endpoints, string prefix)
    {
        var group = endpoints.MapGroup(prefix);
        group.AddEndpointFilter<TransactionEndpointFilter>();
        return group;
    }
}
