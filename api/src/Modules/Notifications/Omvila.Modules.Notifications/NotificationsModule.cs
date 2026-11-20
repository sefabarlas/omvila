using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.Jobs;

namespace Omvila.Modules.Notifications;

/// <summary>Notifications modülü girişi (asgari D1 kapsamı: telefona SMS, sistem şablonu, sahte sağlayıcı).</summary>
public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<NotificationsOptions>()
            .Bind(configuration.GetSection(NotificationsOptions.Section))
            .Validate(o => o.IsValid(), "Notifications ayarları eksik veya geçersiz (appsettings \"Notifications\").");
        services.AddModuleDbContext<NotificationsDbContext>(NotificationsDbContext.Schema);
        services.AddScoped<INotificationOutbox, NotificationOutbox>();
        services.AddScoped<ICreditLedger, CreditLedger>();
        services.AddPeriodicJob<DispatchNotifications>();
        return services;
    }
}
