using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Scheduling.Infrastructure.ConflictGuard;

namespace Omvila.Modules.Scheduling;

/// <summary>
/// Scheduling modülü girişi (D2: randevu alanı ve kalıcılığı, müsaitlik motoru ve anlık görüntü yükleyicisi, çakışma
/// engeli ve yazma yolu: AppointmentWriter). Uçlar sonraki parçalardadır. Yükleyici Tenancy (IBranchScheduleQueries) ve
/// Catalog (IServiceOffers) sözleşmelerine dayanır.
/// </summary>
public static class SchedulingModule
{
    public static IServiceCollection AddSchedulingModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<SchedulingDbContext>(SchedulingDbContext.Schema);
        services.AddScoped<AvailabilitySnapshotLoader>();
        services.AddScoped<AppointmentLocks>();
        services.AddScoped<AppointmentWriter>();
        return services;
    }

    /// <summary>Henüz uç yok; Host eşlemesi şimdiden bağlıdır.</summary>
    public static IEndpointRouteBuilder MapSchedulingEndpoints(this IEndpointRouteBuilder app) => app;
}
