using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NodaTime;
using Omvila.Host;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Http.Cors;
using Omvila.Infrastructure.Idempotency;
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
builder.Services.AddNotificationsModule(builder.Configuration);
builder.Services.AddTenancyModule(builder.Configuration);
builder.Services.AddPlatformModule(builder.Configuration);
builder.Services.AddCatalogModule();
builder.Services.AddCustomersModule();
builder.Services.AddSalesModule();
builder.Services.AddReviewsModule();
builder.Services.AddSchedulingModule(builder.Configuration);
builder.Services.AddIntegrationsModule(builder.Configuration);
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

// `--role=migrate` (veya eski biçim `migrate`): migration'ları migrator bağlantısıyla uygular ve çıkar; sunucu başlamaz.
// Platform:SeedPlans doluysa (geliştirme ve test sunucusu yapılandırması) planlar da eklenir (S-14).
if (role == HostRole.Migrate || args.Contains("migrate", StringComparer.Ordinal))
{
    var migrator = builder.Configuration.GetConnectionString(InfrastructureServiceCollectionExtensions.MigratorConnectionName)
        ?? throw new InvalidOperationException("ConnectionStrings:Migrator tanımlı değil.");
    await app.Services.GetRequiredService<MigrationRunner>().RunAsync(migrator, CancellationToken.None);
    await app.Services.GetRequiredService<PlanSeeder>().SeedAsync(migrator, CancellationToken.None);
    return;
}

// `--role=seed [--reset]`: demo verisini yazar ve çıkar; Production'da hata ile çıkar (Seeding/DemoSeedCommand, T-74).
if (role == HostRole.Seed)
{
    var reset = args.Contains("--reset", StringComparer.Ordinal);
    Environment.ExitCode = await Omvila.Host.Seeding.DemoSeedCommand.RunAsync(app.Services, app.Environment, reset, Console.Out, CancellationToken.None);
    return;
}

// `--role=platform-user --platform-phone=… --platform-name=… --platform-role=…`: platform kullanıcısını oluşturur veya günceller
// ve çıkar (K2: web'den kayıt yok; idempotent; PlatformUsers/PlatformUserCommand, T-119).
if (role == HostRole.PlatformUser)
{
    Environment.ExitCode = await Omvila.Host.PlatformUsers.PlatformUserCommand.RunAsync(app.Services, builder.Configuration, Console.Out, CancellationToken.None);
    return;
}

// Ters vekil başlıkları en başta: hız sınırı ve günlük gerçek istemci IP'sini ve şemayı görür (yalnız güvenilen vekilden).
app.UseTrustedForwardedHeaders();
app.UseExceptionHandler();
app.UseStatusCodePages();
// CORS kimlik doğrulama, hız sınırı ve idempotency'den önce: ön uçuş (OPTIONS) jetonsuz yanıtlanır.
app.UseSurfaceCors();
app.UseIdempotencyRequestBuffering();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseOmvilaDbSession();

// Canlılık: süreç ayakta. Hazırlık: bağımlılıklar (veritabanı, outbox gecikmesi) "ready" etiketiyle eklenir.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

if (role.HasFlag(HostRole.Api))
{
    app.MapOpenApi("/openapi/{documentName}.json");
    app.MapIdentityEndpoints();
    app.MapTenancyEndpoints();
    app.MapCatalogEndpoints();
    app.MapCustomersEndpoints();
    app.MapSchedulingEndpoints();
    app.MapSalesEndpoints();
    app.MapReviewsEndpoints();
    app.MapNotificationsEndpoints();
    app.MapPlatformEndpoints();
    app.MapIntegrationsEndpoints();
    // Yalnız sahte SMS sağlayıcısı etkinken (Production'da yok); kimlik doğrulamasız, koruma ters vekilde (T-76).
    app.MapDevSmsLogPage();
}

HostLog.Started(app.Logger, role);
await app.RunAsync();

/// <summary>WebApplicationFactory için giriş noktası.</summary>
public partial class Program;
