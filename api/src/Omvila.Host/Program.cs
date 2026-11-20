using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NodaTime;
using Omvila.Host;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity;
using Omvila.SharedKernel;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);
var role = HostRoleParser.Parse(builder.Configuration["role"]);

builder.Services.AddSerilog((services, logger) => logger
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter()));

builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddSingleton<IIdGenerator, UuidV7IdGenerator>();
builder.Services.AddOmvilaProblemDetails();
builder.Services.AddOmvilaInfrastructure(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);
// OpenAPI belgesi derleme sırasında üretilirken (GetDocument.Insider) uygulama veritabanısız başlar; iş zamanlanmaz.
var isOpenApiGeneration = string.Equals(Assembly.GetEntryAssembly()?.GetName().Name, "GetDocument.Insider", StringComparison.Ordinal);
if (role.HasFlag(HostRole.Worker) && !isOpenApiGeneration)
{
    builder.Services.AddOmvilaWorker();
}

builder.Services.AddOpenApi("v1", options => options.AddDocumentTransformer((document, _, _) =>
{
    document.Info.Title = "OMVILA API";
    document.Info.Version = "0.1.0";
    return Task.CompletedTask;
}));

var app = builder.Build();

// `dotnet run --project src/Omvila.Host -- migrate`: migration'ları migrator rolüyle uygular ve çıkar.
if (args.Contains("migrate", StringComparer.Ordinal))
{
    var migrator = builder.Configuration.GetConnectionString(InfrastructureServiceCollectionExtensions.MigratorConnectionName)
        ?? throw new InvalidOperationException("ConnectionStrings:Migrator tanımlı değil.");
    await app.Services.GetRequiredService<MigrationRunner>().RunAsync(migrator, CancellationToken.None);
    return;
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseIdempotencyRequestBuffering();
app.UseAuthentication();
app.UseAuthorization();

// Canlılık: süreç ayakta. Hazırlık: bağımlılıklar (veritabanı, outbox gecikmesi) "ready" etiketiyle eklenir.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

if (role.HasFlag(HostRole.Api))
{
    app.MapOpenApi("/openapi/{documentName}.json");
}

HostLog.Started(app.Logger, role);
await app.RunAsync();

/// <summary>WebApplicationFactory için giriş noktası.</summary>
public partial class Program;
