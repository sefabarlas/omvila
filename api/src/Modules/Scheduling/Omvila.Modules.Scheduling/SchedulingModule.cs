using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Features.Calendar;
using Omvila.Modules.Scheduling.Features.Slots;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Scheduling.Infrastructure.ConflictGuard;

namespace Omvila.Modules.Scheduling;

/// <summary>
/// Scheduling modülü girişi (D2: randevu alanı ve kalıcılığı, müsaitlik motoru ve anlık görüntü yükleyicisi, çakışma
/// engeli ve yazma yolu: AppointmentWriter; panel takvimi, boş saat arama ve randevu uçları; IAppointmentQueries).
/// Yükleyici Tenancy (IBranchScheduleQueries) ve Catalog (IServiceOffers), randevu uçları Customers (ICustomerDirectory)
/// ve Platform (ISubscriptionQueries) sözleşmelerine dayanır.
/// </summary>
public static class SchedulingModule
{
    public static IServiceCollection AddSchedulingModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<SchedulingDbContext>(SchedulingDbContext.Schema);
        services.AddScoped<AvailabilitySnapshotLoader>();
        services.AddScoped<AppointmentLocks>();
        services.AddScoped<AppointmentWriter>();
        services.AddScoped<AppointmentDetailReader>();
        services.AddScoped<ManageAppointmentCreator>();
        services.AddScoped<ManageAppointmentChanges>();
        services.AddScoped<CalendarReader>();
        services.AddScoped<IAppointmentQueries, AppointmentQueries>();
        return services;
    }

    public static IEndpointRouteBuilder MapSchedulingEndpoints(this IEndpointRouteBuilder app)
    {
        CalendarEndpoints.Map(app);
        SlotSearchEndpoints.Map(app);
        ManageAppointmentEndpoints.Map(app);
        return app;
    }
}
