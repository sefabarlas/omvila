using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Integrations.Calendar;
using Omvila.Modules.Integrations.Contracts;
using Omvila.Modules.Integrations.Features.GoogleCalendar;
using Omvila.Modules.Integrations.Infrastructure;
using Omvila.Modules.Integrations.Jobs;

namespace Omvila.Modules.Integrations;

/// <summary>
/// Integrations modülü girişi (alan-modeli.md § Integrations: CalendarLink, AppointmentCalendarSync; K7). Google Takvim'e tek
/// yönlü aktarım: bağlantı uçları, jeton yenileme ve olay aktarımı işleri, sağlayıcı seçimi (boş → kapalı, Fake ortam izniyle,
/// Google istemci kimliğiyle).
/// </summary>
public static class IntegrationsModule
{
    public static IServiceCollection AddIntegrationsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CalendarOptions>()
            .Bind(configuration.GetSection(CalendarOptions.Section))
            .Validate(o => o.IsValid(), "Integrations:Calendar ayarları eksik veya geçersiz.")
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<CalendarOptions>, CalendarOptionsValidator>();
        services.AddModuleDbContext<IntegrationsDbContext>(IntegrationsDbContext.Schema);
        services.AddSingleton<CalendarTokenProtector>();
        services.AddSingleton<FakeCalendarStore>();
        services.AddHttpClient(GoogleCalendarProvider.HttpClientName);
        services.AddScoped<ICalendarProvider>(CreateProvider);
        services.AddScoped<CalendarConnections>();
        services.AddScoped<ICalendarLinkQueries, CalendarLinkQueries>();
        services.AddPeriodicJob<RefreshCalendarTokens, CalendarOptions>(o => Duration.FromSeconds(o.SyncIntervalSeconds));
        services.AddPeriodicJob<SyncCalendarEvents, CalendarOptions>(o => Duration.FromSeconds(o.SyncIntervalSeconds));
        // Demo verisi --reset (T-74): markanın bağlantıları ve aktarım kayıtları.
        services.AddBrandDataEraser<IntegrationsDbContext>();
        return services;
    }

    public static IEndpointRouteBuilder MapIntegrationsEndpoints(this IEndpointRouteBuilder app)
    {
        GoogleCalendarEndpoints.Map(app);
        return app;
    }

    private static ICalendarProvider CreateProvider(IServiceProvider services)
    {
        var options = services.GetRequiredService<IOptions<CalendarOptions>>().Value;
        return options.Provider switch
        {
            CalendarOptions.FakeProvider => ActivatorUtilities.CreateInstance<FakeCalendarProvider>(services),
            CalendarOptions.GoogleProvider when options.Google.IsConfigured => ActivatorUtilities.CreateInstance<GoogleCalendarProvider>(services),
            _ => new DisabledCalendarProvider(),
        };
    }
}
