using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Infrastructure;

namespace Omvila.Modules.Scheduling;

/// <summary>
/// Scheduling modülü girişi (D2 temel: randevu alanı ve kalıcılığı). Müsaitlik motoru, çakışma engeli (kilit +
/// exclusion constraint) ve uçlar sonraki parçalardadır.
/// </summary>
public static class SchedulingModule
{
    public static IServiceCollection AddSchedulingModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<SchedulingDbContext>(SchedulingDbContext.Schema);
        return services;
    }

    /// <summary>Henüz uç yok; Host eşlemesi şimdiden bağlıdır.</summary>
    public static IEndpointRouteBuilder MapSchedulingEndpoints(this IEndpointRouteBuilder app) => app;
}
