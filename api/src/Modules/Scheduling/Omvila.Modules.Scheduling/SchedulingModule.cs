using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Features.AppointmentLink;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Features.Approvals;
using Omvila.Modules.Scheduling.Features.Calendar;
using Omvila.Modules.Scheduling.Features.PublicBooking;
using Omvila.Modules.Scheduling.Features.Slots;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Scheduling.Infrastructure.ConflictGuard;
using Omvila.Modules.Scheduling.Jobs;

namespace Omvila.Modules.Scheduling;

/// <summary>
/// Scheduling modülü girişi (D2: randevu alanı ve kalıcılığı, müsaitlik motoru ve anlık görüntü yükleyicisi, çakışma
/// engeli ve yazma yolu: AppointmentWriter; panel takvimi, boş saat arama ve randevu uçları; rezervasyon sayfası ve SMS bağlantısı (/r/) uçları; IAppointmentQueries; MarkNoShows ve ExtendRecurringSeries işleri). D3: onay akışı (onay, red, kuyruk) ve
/// ExpireAppointmentHolds işi.
/// Yükleyici Tenancy (IBranchScheduleQueries) ve Catalog (IServiceOffers), randevu uçları Customers (ICustomerDirectory)
/// ve Platform (ISubscriptionQueries) sözleşmelerine dayanır.
/// </summary>
public static class SchedulingModule
{
    public static IServiceCollection AddSchedulingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SchedulingOptions>()
            .Bind(configuration.GetSection(SchedulingOptions.Section))
            .Validate(o => o.IsValid(), "Scheduling ayarları eksik veya geçersiz (appsettings \"Scheduling\").");
        services.AddModuleDbContext<SchedulingDbContext>(SchedulingDbContext.Schema);
        services.AddScoped<AvailabilitySnapshotLoader>();
        services.AddScoped<AppointmentLocks>();
        services.AddScoped<AppointmentWriter>();
        services.AddScoped<AppointmentDetailReader>();
        services.AddScoped<ManageAppointmentCreator>();
        services.AddScoped<PublicAppointmentCreator>();
        services.AddScoped<ManageAppointmentChanges>();
        services.AddScoped<AppointmentLinkResolver>();
        services.AddScoped<PublicAppointmentLinkChanges>();
        services.AddScoped<CalendarReader>();
        services.AddScoped<IAppointmentQueries, AppointmentQueries>();
        services.AddScoped<AppointmentApprovals>();
        services.AddScoped<ApprovalQueueReader>();
        services.AddPeriodicJob<ExpireAppointmentHolds, SchedulingOptions>(
            o => Duration.FromSeconds(o.ExpireAppointmentHoldsIntervalSeconds));
        services.AddPeriodicJob<MarkNoShows, SchedulingOptions>(o => Duration.FromSeconds(o.MarkNoShowsIntervalSeconds));
        services.AddPeriodicJob<ExtendRecurringSeries, SchedulingOptions>(
            o => Duration.FromMinutes(o.ExtendRecurringSeriesIntervalMinutes));
        return services;
    }

    public static IEndpointRouteBuilder MapSchedulingEndpoints(this IEndpointRouteBuilder app)
    {
        CalendarEndpoints.Map(app);
        SlotSearchEndpoints.Map(app);
        ManageAppointmentEndpoints.Map(app);
        ApprovalEndpoints.Map(app);
        PublicAvailabilityEndpoints.Map(app);
        PublicAppointmentEndpoints.Map(app);
        PublicAppointmentLinkEndpoints.Map(app);
        return app;
    }
}
