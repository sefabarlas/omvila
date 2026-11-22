using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Features.Devices;
using Omvila.Modules.Notifications.Features.InAppNotifications;
using Omvila.Modules.Notifications.Features.Sms;
using Omvila.Modules.Notifications.Features.SmsDeliveryReports;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Notifications.IntegrationEventHandlers;
using Omvila.Modules.Notifications.Jobs;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Scheduling.Contracts;

namespace Omvila.Modules.Notifications;

/// <summary>
/// Notifications modülü girişi: telefona SMS (sistem şablonu, sahte sağlayıcı), kontör defteri; D2'de randevu olaylarından
/// müşteri SMS'leri ve zamanlanmış hatırlatmalar (Scheduling olayları; Customers ve Tenancy sözleşmeleriyle); D3'te
/// kontör düşümü, gönderen adı, geri çekilmeli tekrar, teslim raporu webhook'u, cihaz kaydı ve push (SMS'e geri düşme).
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
        services.AddPeriodicJob<DispatchNotifications, NotificationsOptions>(o => Duration.FromSeconds(o.DispatchIntervalSeconds));
        services.AddScoped<SmsDeliveryReportHandler>();
        services.AddScoped<AppointmentSms>();
        services.AddScoped<DepositSms>();
        services.AddScoped<InAppNotifier>();
        services.AddScoped<ICustomerDevices, CustomerDevices>();
        services.AddScoped<SmsUsage>();
        // Demo verisi --reset (T-74): markanın bildirim kuyruğu, kontör defteri, cihazları ve panel bildirimleri.
        services.AddBrandDataEraser<NotificationsDbContext>();
        services.AddIntegrationEventHandler<SmsCreditsLow, NotifyCreditsLow>();
        services.AddIntegrationEventHandler<SmsCreditsInsufficient, NotifyCreditsLow>();
        services.AddIntegrationEventHandler<AppointmentConfirmed, SendAppointmentConfirmation>();
        services.AddIntegrationEventHandler<AppointmentRescheduled, NotifyAppointmentRescheduled>();
        services.AddIntegrationEventHandler<AppointmentCancelledBySalon, NotifyAppointmentCancelledBySalon>();
        services.AddIntegrationEventHandler<AppointmentCancelledByCustomer, CancelRemindersOnCustomerCancel>();
        services.AddIntegrationEventHandler<AppointmentRequested, NotifyAppointmentRequested>();
        services.AddIntegrationEventHandler<AppointmentApproved, NotifyAppointmentApproved>();
        services.AddIntegrationEventHandler<AppointmentDeclined, NotifyAppointmentDeclined>();
        services.AddIntegrationEventHandler<AppointmentApprovalExpired, NotifyAppointmentApprovalExpired>();
        services.AddIntegrationEventHandler<AppointmentRequested, NotifyApprovalNeeded>();
        services.AddIntegrationEventHandler<AppointmentApproved, ResolveApprovalNotifications>();
        services.AddIntegrationEventHandler<AppointmentDeclined, ResolveApprovalNotifications>();
        services.AddIntegrationEventHandler<AppointmentApprovalExpired, ResolveApprovalNotifications>();
        services.AddIntegrationEventHandler<AppointmentDepositReceived, NotifyDepositReceived>();
        services.AddIntegrationEventHandler<AppointmentDepositReceived, ResolveApprovalNotifications>();
        services.AddIntegrationEventHandler<AppointmentDepositReported, NotifyDepositReported>();
        services.AddIntegrationEventHandler<AppointmentDepositReminded, NotifyDepositReminded>();
        services.AddIntegrationEventHandler<ReviewInviteScheduled, NotifyReviewInvite>();
        services.AddIntegrationEventHandler<ReviewReceived, NotifyReviewReceived>();
        services.AddIntegrationEventHandler<WaitlistOfferCreated, NotifyWaitlistOffer>();
        return services;
    }

    /// <summary>Notifications uçları: SMS teslim raporu webhook'u, cihaz kaydı, bildirim paneli, SMS özeti ve gönderen adı.</summary>
    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        SmsDeliveryReportEndpoints.Map(app);
        DeviceEndpoints.Map(app);
        MeDeviceEndpoints.Map(app);
        InAppNotificationEndpoints.Map(app);
        MeNotificationEndpoints.Map(app);
        SmsEndpoints.Map(app);
        return app;
    }
}
