using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Persistence;
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
        await using var services = CreateServices();
        await services.GetRequiredService<MigrationRunner>().RunAsync(MigratorConnectionString, CancellationToken.None);
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();

    /// <summary>Uygulama rolüne bağlı altyapı servisleri (DbSession, DbContext'ler). Her test kendi kapsamını açar.</summary>
    public ServiceProvider CreateServices(int? maxPoolSize = null)
    {
        var app = new NpgsqlConnectionStringBuilder(AppConnectionString);
        if (maxPoolSize is { } size)
        {
            app.MaxPoolSize = size;
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                [$"ConnectionStrings:{InfrastructureServiceCollectionExtensions.AppConnectionName}"] = app.ConnectionString,
                [$"ConnectionStrings:{InfrastructureServiceCollectionExtensions.MigratorConnectionName}"] = MigratorConnectionString,
            })
            .Build();

        return new ServiceCollection()
            .AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory>(NullLoggerFactory.Instance)
            .AddSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(NullLogger<>))
            .AddOmvilaInfrastructure(configuration)
            .BuildServiceProvider();
    }

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
