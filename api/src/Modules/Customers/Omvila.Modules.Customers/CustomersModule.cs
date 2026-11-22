using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Jobs;
using Omvila.Modules.Customers.Features.DataExports;
using Omvila.Modules.Customers.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Features;
using Omvila.Modules.Customers.Features.Customers;
using Omvila.Modules.Customers.Features.Deletion;
using Omvila.Modules.Customers.Features.Export;
using Omvila.Modules.Customers.Features.Imports;
using Omvila.Modules.Customers.Features.Me;
using Omvila.Modules.Customers.Features.Merge;
using Omvila.Modules.Customers.Features.PhoneReveal;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Customers.Infrastructure.Configurations;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers;

/// <summary>Customers modülü girişi (D2: müşteri listesi, arama, ekleme, kart; ICustomerDirectory. D5: birleştirme, içe/dışa aktarma, KVKK silme).</summary>
public static class CustomersModule
{
    public static IServiceCollection AddCustomersModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<CustomersDbContext>(CustomersDbContext.Schema);
        services.AddScoped<CustomerRegistry>();
        services.AddScoped<CustomerReader>();
        services.AddScoped<CustomerWriter>();
        services.AddScoped<ICustomerDirectory, CustomerDirectory>();
        // D5 müşteri uygulaması (T-103): hesap, profil, Salonlarım.
        services.AddScoped<ICustomerAccounts, CustomerAccounts>();
        services.AddScoped<MeBrandReader>();
        // D5 müşteri uygulaması (T-104): rızalar ve hesap silme.
        services.AddScoped<MarketingConsentLedger>();
        services.AddScoped<CustomerAccountEraser>();
        // D5 (T-92): birleştirme, içe/dışa aktarma, KVKK silme.
        services.AddOptions<CustomerImportOptions>()
            .BindConfiguration(CustomerImportOptions.Section)
            .Validate(o => o.IsValid(), "İçe aktarma ayarları geçersiz (appsettings \"Customers:Import\").");
        services.AddScoped<CustomerMerger>();
        services.AddScoped<CustomerImporter>();
        services.AddScoped<CustomerExporter>();
        services.AddScoped<CustomerEraser>();
        services.AddScoped<DeletionRequestReader>();
        // Aynı numarayla eşzamanlı iki ekleme: ikincisi kart benzersizliğine takılır (meta'sız phone_already_exists).
        services.AddUniqueConstraintError(BrandCustomerConfiguration.BrandCustomerIndex, ErrorCodes.PhoneAlreadyExists);
        // D6 (T-121): tüm verileri dışa aktarma ve saklama süresi.
        services.AddOptions<DataExportOptions>()
            .BindConfiguration(DataExportOptions.Section)
            .Validate(o => o.IsValid(), "Dışa aktarma ayarları geçersiz (appsettings \"Customers:DataExport\").")
            .ValidateOnStart();
        services.AddSingleton<IValidateOptions<DataExportOptions>, CustomersSecretsValidator>();
        services.AddSingleton<DataExportLinks>();
        services.AddScoped<IBrandDataExportParticipant, CustomersExportParticipant>();
        services.AddPeriodicJob<ProcessDataExports, DataExportOptions>(o => Duration.FromSeconds(o.ProcessIntervalSeconds));
        services.AddPeriodicJob<ApplyRetention, DataExportOptions>(o => Duration.FromMinutes(o.ApplyRetentionIntervalMinutes));
        // Demo verisi --reset (T-74): markanın kartları ve rızaları; global kişi kalır.
        services.AddBrandDataEraser<CustomersDbContext>();
        return services;
    }

    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        CustomerEndpoints.Map(app);
        CustomerRecordsEndpoints.Map(app);
        PhoneRevealEndpoints.Map(app);
        Features.Me.MeProfileEndpoints.Map(app);
        Features.Me.MeBrandEndpoints.Map(app);
        Features.Me.MeConsentEndpoints.Map(app);
        DataExportEndpoints.Map(app);
        return app;
    }
}
