using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Infrastructure;

namespace Omvila.Modules.Platform;

/// <summary>Platform modülü girişi (D1: plan ve deneme aboneliği; D5: denetim kaydı IAuditLog).</summary>
public static class PlatformModule
{
    public static IServiceCollection AddPlatformModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PlatformOptions>()
            .Bind(configuration.GetSection(PlatformOptions.Section))
            .Validate(o => o.IsValid(), "Platform ayarları eksik veya geçersiz (appsettings \"Platform\").");
        services.AddModuleDbContext<PlatformDbContext>(PlatformDbContext.Schema);
        services.AddScoped<ISubscriptionProvisioning, SubscriptionProvisioning>();
        services.AddScoped<ISubscriptionQueries, SubscriptionQueries>();
        services.AddScoped<IAuditLog, AuditLog>();
        services.AddSingleton<PlanSeeder>();
        // Demo verisi --reset (T-74): markanın aboneliği.
        services.AddBrandDataEraser<PlatformDbContext>();
        return services;
    }
}
