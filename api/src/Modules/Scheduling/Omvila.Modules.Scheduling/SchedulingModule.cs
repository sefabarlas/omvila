using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;

namespace Omvila.Modules.Scheduling;

/// <summary>
/// Scheduling modülü girişi (D2: randevu alanı ve kalıcılığı, müsaitlik motoru ve anlık görüntü yükleyicisi). Çakışma
/// engeli (kilit + exclusion constraint) ve uçlar sonraki parçalardadır. Yükleyici Tenancy (IBranchScheduleQueries) ve
/// Catalog (IServiceOffers) sözleşmelerine dayanır.
/// </summary>
public static class SchedulingModule
{
    public static IServiceCollection AddSchedulingModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<SchedulingDbContext>(SchedulingDbContext.Schema);
        services.AddScoped<AvailabilitySnapshotLoader>();
        return services;
    }

    /// <summary>Henüz uç yok; Host eşlemesi şimdiden bağlıdır.</summary>
    public static IEndpointRouteBuilder MapSchedulingEndpoints(this IEndpointRouteBuilder app) => app;
}
