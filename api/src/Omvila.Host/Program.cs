using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NodaTime;
using Omvila.Host;
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
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi("v1", options => options.AddDocumentTransformer((document, _, _) =>
{
    document.Info.Title = "OMVILA API";
    document.Info.Version = "0.1.0";
    return Task.CompletedTask;
}));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

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
