using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Catalog.Features;
using Omvila.Modules.Catalog.Features.BranchPrices;
using Omvila.Modules.Catalog.Features.Categories;
using Omvila.Modules.Catalog.Features.PublicCatalog;
using Omvila.Modules.Catalog.Features.Services;
using Omvila.Modules.Catalog.Features.Templates;
using Omvila.Modules.Catalog.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Catalog;

/// <summary>Catalog modülü girişi (D1: kategori, hizmet, aşama, hizmeti veren personel, şube fiyatı).</summary>
public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<CatalogDbContext>(CatalogDbContext.Schema);
        services.AddScoped<IOnboardingStepProvider, ServicesOnboardingStep>();
        services.AddScoped<IStaffServiceAssignments, StaffServiceAssignments>();
        services.AddScoped<IServiceOffers, ServiceOffers>();
        // Platform paneli aktivasyon hunisi (yalnız platform bağlamında).
        services.AddScoped<IPlatformCatalogStats, Infrastructure.PlatformCatalogStats>();
        services.AddScoped<IServiceDeposits, ServiceDeposits>();
        services.AddScoped<IServiceDirectory, ServiceDirectory>();
        services.AddIntegrationEventHandler<BranchCreated, CopyBranchPricesOnBranchCreated>();
        // Demo verisi (--role=seed; T-74).
        services.AddScoped(sp => new DemoData.CatalogDemoData(
            sp.GetRequiredService<CatalogDbContext>(), sp.GetRequiredService<SharedKernel.IIdGenerator>(), sp.GetRequiredService<NodaTime.IClock>()));
        services.AddBrandDataEraser<CatalogDbContext>();
        return services;
    }

    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        CategoryEndpoints.Map(app);
        ServiceEndpoints.Map(app);
        BranchPriceEndpoints.Map(app);
        TemplateEndpoints.Map(app);
        PublicCatalogEndpoints.Map(app);
        return app;
    }
}
