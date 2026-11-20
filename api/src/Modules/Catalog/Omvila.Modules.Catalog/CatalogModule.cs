using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Features;
using Omvila.Modules.Catalog.Features.BranchPrices;
using Omvila.Modules.Catalog.Features.Categories;
using Omvila.Modules.Catalog.Features.Services;
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
        services.AddIntegrationEventHandler<BranchCreated, CopyBranchPricesOnBranchCreated>();
        return services;
    }

    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        CategoryEndpoints.Map(app);
        ServiceEndpoints.Map(app);
        BranchPriceEndpoints.Map(app);
        return app;
    }
}
