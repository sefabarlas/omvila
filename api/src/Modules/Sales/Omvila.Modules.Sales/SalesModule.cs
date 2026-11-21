using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Sales.Features.Cash;
using Omvila.Modules.Sales.Features.CustomerPayments;
using Omvila.Modules.Sales.Infrastructure.Configurations;
using Omvila.SharedKernel;
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
/// paketleri, satış ve iade. ExpireCustomerPackages işi. Kasa günü ve gün kapatma
/// (CashDayClose), müşteri kartı Ödemeler sekmesi.
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
        services.AddScoped<PaymentRows>();
        services.AddScoped<CashDays>();
        services.AddScoped<CustomerPaymentReader>();
        services.AddUniqueConstraintError(CashDayCloseConfiguration.BranchDayIndex, ErrorCodes.CashDayAlreadyClosed);
        // Müşteri birleştirme ve KVKK silmesi (T-92): tahsilatlar.
        services.AddScoped<CustomerPaymentsParticipant>();
        services.AddScoped<Customers.Contracts.ICustomerMergeParticipant>(sp => sp.GetRequiredService<CustomerPaymentsParticipant>());
        services.AddScoped<Customers.Contracts.ICustomerErasureParticipant>(sp => sp.GetRequiredService<CustomerPaymentsParticipant>());
        services.AddScoped<CustomerPackagesParticipant>();
        services.AddScoped<Customers.Contracts.ICustomerMergeParticipant>(sp => sp.GetRequiredService<CustomerPackagesParticipant>());
        services.AddScoped<Customers.Contracts.ICustomerErasureParticipant>(sp => sp.GetRequiredService<CustomerPackagesParticipant>());
        // Demo verisi --reset (T-74): markanın tahsilat, paket ve prim kayıtları.
        services.AddBrandDataEraser<SalesDbContext>();
        return services;
    }

    public static IEndpointRouteBuilder MapSalesEndpoints(this IEndpointRouteBuilder app)
    {
        PackageEndpoints.Map(app);
        CustomerPackageEndpoints.Map(app);
        CashDayEndpoints.Map(app);
        CustomerPaymentEndpoints.Map(app);
        return app;
    }
}
