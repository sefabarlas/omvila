using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Features.SmsDeliveryReports;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.IntegrationEventHandlers;
using Omvila.Modules.Notifications.Jobs;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications;

/// <summary>
/// Notifications modülü girişi: telefona SMS (sistem şablonu, sahte sağlayıcı), kontör defteri; D2'de randevu olaylarından
/// müşteri SMS'leri ve zamanlanmış hatırlatmalar (Scheduling olayları; Customers ve Tenancy sözleşmeleriyle); D3'te
/// kontör düşümü, gönderen adı, geri çekilmeli tekrar ve teslim raporu webhook'u.
/// </summary>
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
        services.AddScoped<SmsDeliveryReportHandler>();
        services.AddScoped<AppointmentSms>();
        services.AddIntegrationEventHandler<AppointmentConfirmed, SendAppointmentConfirmation>();
        services.AddIntegrationEventHandler<AppointmentRescheduled, NotifyAppointmentRescheduled>();
        services.AddIntegrationEventHandler<AppointmentCancelledBySalon, NotifyAppointmentCancelledBySalon>();
        services.AddIntegrationEventHandler<AppointmentCancelledByCustomer, CancelRemindersOnCustomerCancel>();
        services.AddIntegrationEventHandler<AppointmentRequested, NotifyAppointmentRequested>();
        services.AddIntegrationEventHandler<AppointmentApproved, NotifyAppointmentApproved>();
        services.AddIntegrationEventHandler<AppointmentDeclined, NotifyAppointmentDeclined>();
        services.AddIntegrationEventHandler<AppointmentApprovalExpired, NotifyAppointmentApprovalExpired>();
        return services;
    }

    /// <summary>Notifications uçları: SMS teslim raporu webhook'u.</summary>
    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        SmsDeliveryReportEndpoints.Map(app);
        return app;
    }
}
