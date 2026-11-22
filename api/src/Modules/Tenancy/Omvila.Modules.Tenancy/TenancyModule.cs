using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Features.BankAccount;
using Omvila.Modules.Tenancy.Features.Brand;
using Omvila.Modules.Tenancy.Features.Branches;
using Omvila.Modules.Tenancy.Features.NotificationSettings;
using Omvila.Modules.Tenancy.Features.Onboarding;
using Omvila.Modules.Tenancy.Features.PublicBrand;
using Omvila.Modules.Tenancy.Features.Staff;
using Omvila.SharedKernel;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.Modules.Tenancy.Infrastructure.BankAccounts;

namespace Omvila.Modules.Tenancy;

/// <summary>Tenancy modülü girişi: marka, şube, personel ve ayarların kalıcılığı; marka kurma sözleşmesi.</summary>
public static class TenancyModule
{
    public static IServiceCollection AddTenancyModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TenancyOptions>()
            .Bind(configuration.GetSection(TenancyOptions.Section))
            .Validate(o => o.IsValid(), "Tenancy ayarları eksik veya geçersiz (appsettings \"Tenancy\").")
            .ValidateOnStart();
        // IBAN şifreleme anahtarı Staging/Production'da gizli değerden; eksikse açılış durur (T-73 deseni, T-78).
        services.AddSingleton<IValidateOptions<TenancyOptions>, TenancySecretsValidator>();
        services.AddSingleton<IbanProtector>();
        services.AddScoped<IBrandBankAccounts, BrandBankAccounts>();
        services.AddModuleDbContext<TenancyDbContext>(TenancyDbContext.Schema);
        services.AddScoped<SlugRegistry>();
        services.AddScoped<ITenantProvisioning, TenantProvisioning>();
        // Aynı anda iki işletme aynı adı alırsa ikincisi 409 olur (kilit kaçarsa son savunma: tek ad dizini).
        services.AddUniqueConstraintError("ix_brands_slug", ErrorCodes.SlugUnavailable);
        services.AddScoped<ITenantQueries, TenantQueries>();
        services.AddScoped<IBranchScheduleQueries, BranchScheduleQueries>();
        services.AddScoped<PublicBookingQueries>();
        services.AddScoped<IPublicBookingQueries>(sp => sp.GetRequiredService<PublicBookingQueries>());
        services.AddScoped<ICustomerBrandDirectory, CustomerBrandDirectory>();
        services.AddScoped<OnboardingService>();
        services.AddScoped<StaffReader>();
        services.AddScoped<StaffWriter>();
        services.AddScoped<IOnboardingStepProvider, BusinessCreatedStep>();
        services.AddScoped<IOnboardingStepProvider, WorkingHoursStep>();
        services.AddScoped<IOnboardingStepProvider, StaffStep>();
        services.AddScoped<IOnboardingStepProvider, ShareBookingLinkStep>();
        // Demo verisi (--role=seed; T-74): şube, saat, personel, kurulum; --reset'te markanın satırları (marka en son).
        services.AddScoped(sp => new DemoData.TenancyDemoData(
            sp.GetRequiredService<TenancyDbContext>(), sp.GetRequiredService<Identity.Contracts.IStaffMemberships>(),
            sp.GetRequiredService<OnboardingService>(), sp.GetRequiredService<IbanProtector>(), sp.GetRequiredService<IIdGenerator>(), sp.GetRequiredService<NodaTime.IClock>()));
        services.AddBrandDataEraser<TenancyDbContext>(typeof(Domain.Brand));
        return services;
    }

    /// <summary>Tenancy uçlarını eşler (kurulum, marka, şube, personel, rezervasyon sayfası salon kartı).</summary>
    public static IEndpointRouteBuilder MapTenancyEndpoints(this IEndpointRouteBuilder app)
    {
        OnboardingEndpoints.Map(app);
        BrandEndpoints.Map(app);
        BankAccountEndpoints.Map(app);
        NotificationSettingsEndpoints.Map(app);
        BranchEndpoints.Map(app);
        StaffEndpoints.Map(app);
        PublicBrandEndpoints.Map(app);
        return app;
    }
}
