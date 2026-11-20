using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Infrastructure;

namespace Omvila.Modules.Platform;

/// <summary>Platform modülü girişi (D1 kapsamı: plan ve deneme aboneliği).</summary>
public static class PlatformModule
{
    public static IServiceCollection AddPlatformModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PlatformOptions>()
            .Bind(configuration.GetSection(PlatformOptions.Section))
            .Validate(o => o.IsValid(), "Platform ayarları eksik veya geçersiz (appsettings \"Platform\").");
        services.AddModuleDbContext<PlatformDbContext>(PlatformDbContext.Schema);
        services.AddScoped<ISubscriptionProvisioning, SubscriptionProvisioning>();
        services.AddSingleton<PlanSeeder>();
        return services;
    }
}
