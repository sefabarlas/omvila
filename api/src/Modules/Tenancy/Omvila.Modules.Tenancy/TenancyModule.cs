using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Infrastructure;

namespace Omvila.Modules.Tenancy;

/// <summary>Tenancy modülü girişi: marka, şube, personel ve ayarların kalıcılığı; marka kurma sözleşmesi.</summary>
public static class TenancyModule
{
    public static IServiceCollection AddTenancyModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<TenancyDbContext>(TenancyDbContext.Schema);
        services.AddScoped<ITenantProvisioning, TenantProvisioning>();
        return services;
    }
}
