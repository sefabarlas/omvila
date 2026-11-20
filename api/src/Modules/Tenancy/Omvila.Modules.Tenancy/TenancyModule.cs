using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Features.Brand;
using Omvila.Modules.Tenancy.Features.Onboarding;
using Omvila.SharedKernel;
using Omvila.Modules.Tenancy.Infrastructure;

namespace Omvila.Modules.Tenancy;

/// <summary>Tenancy modülü girişi: marka, şube, personel ve ayarların kalıcılığı; marka kurma sözleşmesi.</summary>
public static class TenancyModule
{
    public static IServiceCollection AddTenancyModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TenancyOptions>()
            .Bind(configuration.GetSection(TenancyOptions.Section))
            .Validate(o => o.IsValid(), "Tenancy ayarları eksik veya geçersiz (appsettings \"Tenancy\").");
        services.AddModuleDbContext<TenancyDbContext>(TenancyDbContext.Schema);
        services.AddScoped<SlugRegistry>();
        services.AddScoped<ITenantProvisioning, TenantProvisioning>();
        // Aynı anda iki işletme aynı adı alırsa ikincisi 409 olur (kilit kaçarsa son savunma: tek ad dizini).
        services.AddUniqueConstraintError("ix_brands_slug", ErrorCodes.SlugUnavailable);
        services.AddScoped<ITenantQueries, TenantQueries>();
        services.AddScoped<OnboardingService>();
        services.AddScoped<IOnboardingStepProvider, BusinessCreatedStep>();
        services.AddScoped<IOnboardingStepProvider, StaffStep>();
        services.AddScoped<IOnboardingStepProvider, ShareBookingLinkStep>();
        return services;
    }

    /// <summary>Tenancy uçlarını eşler (kurulum, marka).</summary>
    public static IEndpointRouteBuilder MapTenancyEndpoints(this IEndpointRouteBuilder app)
    {
        OnboardingEndpoints.Map(app);
        BrandEndpoints.Map(app);
        return app;
    }
}
