using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Features.AdminBrands;
using Omvila.Modules.Platform.Features.ManageSubscription;
using Omvila.Modules.Platform.Features.PaymentNotices;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Platform.Infrastructure.Configurations;
using Omvila.Modules.Platform.Jobs;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform;

/// <summary>
/// Platform modülü girişi (D1: plan ve deneme aboneliği; D5: denetim kaydı IAuditLog; D6: platform paneli, abonelik durum makinesi ve
/// AdvanceSubscriptionStatuses işi, fatura, havale bildirimi, salonun abonelik uçları, eşleştirme servisi IPaymentNoticeSettlement).
/// </summary>
public static class PlatformModule
{
    public static IServiceCollection AddPlatformModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PlatformOptions>()
            .Bind(configuration.GetSection(PlatformOptions.Section))
            .Validate(o => o.IsValid(), "Platform ayarları eksik veya geçersiz (appsettings \"Platform\").");
        services.AddModuleDbContext<PlatformDbContext>(PlatformDbContext.Schema);
        services.AddScoped<ISubscriptionProvisioning, SubscriptionProvisioning>();
        services.AddScoped<ISubscriptionQueries, SubscriptionQueries>();
        services.AddScoped<IAuditLog, AuditLog>();
        services.AddScoped<IAuditLogRetention, AuditLogRetention>();
        services.AddSingleton<PlanSeeder>();
        services.AddScoped<InvoiceNumberAllocator>();
        services.AddScoped<SubscriptionBilling>();
        services.AddScoped<SubscriptionAccount>();
        services.AddScoped<PaymentNoticeIntake>();
        // Platform paneli eşleştirmesi (adminMatchPaymentNotice) bunu çağırır (T-118).
        services.AddScoped<IPaymentNoticeSettlement, PaymentNoticeSettlement>();
        services.AddPeriodicJob<AdvanceSubscriptionStatuses, PlatformOptions>(o => Duration.FromMinutes(o.AdvanceSubscriptionStatusesIntervalMinutes));
        // Eşzamanlı iki bildirimden ikincisi bekleyen bildirim indeksine takılır.
        services.AddUniqueConstraintError(PaymentNoticeConfiguration.PendingIndex, ErrorCodes.PaymentNoticeAlreadyPending);
        // Platform paneli (admin yüzeyi; T-119): onay sayıları, genel bakış ve markalar okuma modeli.
        services.AddScoped<IPendingApprovalCounts, PendingApprovals>();
        services.AddScoped<AdminBrandReader>();
        // Demo verisi --reset (T-74): markanın aboneliği.
        services.AddBrandDataEraser<PlatformDbContext>();
        return services;
    }

    /// <summary>Platform uçlarını eşler (admin-overview, admin-brands, manage-subscription).</summary>
    public static IEndpointRouteBuilder MapPlatformEndpoints(this IEndpointRouteBuilder app)
    {
        AdminBrandEndpoints.Map(app);
        ManageSubscriptionEndpoints.Map(app);
        return app;
    }
}
