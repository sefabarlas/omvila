using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Omvila.Infrastructure.Persistence;

/// <summary>Modül DbContext'lerinin ortak kaydı: aynı bağlantı ve transaction, NodaTime, snake_case, şemada migration geçmişi.</summary>
public static class ModuleDbContexts
{
    public const string MigrationsHistoryTable = "__ef_migrations_history";

    /// <summary>
    /// İstek oturumuna bağlı DbContext kaydı. DbContext oturum başladıktan sonra çözülmelidir
    /// (TransactionEndpointFilter veya iş için <see cref="IDbSession.BeginAsync"/>).
    /// </summary>
    public static IServiceCollection AddModuleDbContext<TContext>(this IServiceCollection services, string schema)
        where TContext : DbContext
    {
        services.AddSingleton(new MigratableContext(typeof(TContext), schema));
        services.AddScoped(sp =>
        {
            var session = sp.GetRequiredService<IDbSession>();
            var options = Configure(new DbContextOptionsBuilder<TContext>(), session.Connection, schema).Options;
            var context = ActivatorUtilities.CreateInstance<TContext>(sp, options);
            context.Database.UseTransaction(session.Transaction);
            return context;
        });
        return services;
    }

    /// <summary>Modül DbContext seçenekleri; modüllerin tasarım zamanı fabrikaları da kullanır.</summary>
    public static DbContextOptionsBuilder<TContext> Configure<TContext>(
        DbContextOptionsBuilder<TContext> builder, NpgsqlConnection connection, string schema)
        where TContext : DbContext =>
        builder
            .UseNpgsql(connection, npgsql => npgsql
                .UseNodaTime()
                .MigrationsHistoryTable(MigrationsHistoryTable, schema))
            .UseSnakeCaseNamingConvention();
}
