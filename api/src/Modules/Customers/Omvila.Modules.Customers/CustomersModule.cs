using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Features.Customers;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Customers.Infrastructure.Configurations;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers;

/// <summary>Customers modülü girişi (D2: müşteri listesi, arama, ekleme, kart; ICustomerDirectory).</summary>
public static class CustomersModule
{
    public static IServiceCollection AddCustomersModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<CustomersDbContext>(CustomersDbContext.Schema);
        services.AddScoped<CustomerRegistry>();
        services.AddScoped<CustomerReader>();
        services.AddScoped<CustomerWriter>();
        services.AddScoped<ICustomerDirectory, CustomerDirectory>();
        // Aynı numarayla eşzamanlı iki ekleme: ikincisi kart benzersizliğine takılır (meta'sız phone_already_exists).
        services.AddUniqueConstraintError(BrandCustomerConfiguration.BrandCustomerIndex, ErrorCodes.PhoneAlreadyExists);
        // Demo verisi --reset (T-74): markanın kartları ve rızaları; global kişi kalır.
        services.AddBrandDataEraser<CustomersDbContext>();
        return services;
    }

    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        CustomerEndpoints.Map(app);
        return app;
    }
}
