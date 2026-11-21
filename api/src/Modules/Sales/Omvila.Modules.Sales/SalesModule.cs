using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Features.Packages;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Sales.Jobs;

namespace Omvila.Modules.Sales;

/// <summary>
/// Sales modülü girişi (alan-modeli.md § Modüller: Payment, PaymentAllocation, Package, PackageItem, CustomerPackage,
/// CustomerPackageItem, PackageSessionUsage, CommissionEntry). Kapora havalesinin tahsilat kaydı (<see cref="IDepositPayments"/>),
/// "Tamamla ve ödeme al"ın tahsilatları ile hizmet primi (<see cref="IAppointmentPayments"/>) ve paket seansı defteri
/// (<see cref="IPackageSessions"/>); üçünü de Scheduling istek transaction'ında çağırır. Kendi uçları: paket tanımları, müşteri
/// paketleri, satış ve iade. ExpireCustomerPackages işi.
/// </summary>
public static class SalesModule
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services)
    {
        services.AddOptions<SalesOptions>()
            .BindConfiguration(SalesOptions.Section)
            .Validate(o => o.IsValid(), "Sales ayarları eksik veya geçersiz (appsettings \"Sales\").");
        services.AddModuleDbContext<SalesDbContext>(SalesDbContext.Schema);
        services.AddScoped<IDepositPayments, DepositPayments>();
        services.AddScoped<IAppointmentPayments, AppointmentPayments>();
        services.AddScoped<IPackageSessions, PackageSessions>();
        services.AddScoped<PackageReader>();
        services.AddScoped<PackageSales>();
        services.AddPeriodicJob<ExpireCustomerPackages, SalesOptions>(o => Duration.FromMinutes(o.ExpireCustomerPackagesIntervalMinutes));
        // Demo verisi --reset (T-74): markanın tahsilat, paket ve prim kayıtları.
        services.AddBrandDataEraser<SalesDbContext>();
        return services;
    }

    public static IEndpointRouteBuilder MapSalesEndpoints(this IEndpointRouteBuilder app)
    {
        PackageEndpoints.Map(app);
        CustomerPackageEndpoints.Map(app);
        return app;
    }
}
