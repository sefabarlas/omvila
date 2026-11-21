using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Sales.Features.Cash;
using Omvila.Modules.Sales.Features.CustomerPayments;
using Omvila.Modules.Sales.Infrastructure.Configurations;
using Omvila.SharedKernel;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Infrastructure;

namespace Omvila.Modules.Sales;

/// <summary>
/// Sales modülü girişi (alan-modeli.md § Modüller: Payment, PaymentAllocation, CommissionEntry). Kapora havalesinin tahsilat
/// kaydı (<see cref="IDepositPayments"/>) ve "Tamamla ve ödeme al"ın tahsilatları ile hizmet primi
/// (<see cref="IAppointmentPayments"/>); ikisini de Scheduling istek transaction'ında çağırır. Kasa günü ve gün kapatma uçları
/// (CashDayClose) ve müşteri kartı Ödemeler sekmesi.
/// </summary>
public static class SalesModule
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<SalesDbContext>(SalesDbContext.Schema);
        services.AddScoped<IDepositPayments, DepositPayments>();
        services.AddScoped<IAppointmentPayments, AppointmentPayments>();
        services.AddScoped<PaymentRows>();
        services.AddScoped<CashDays>();
        services.AddScoped<CustomerPaymentReader>();
        services.AddUniqueConstraintError(CashDayCloseConfiguration.BranchDayIndex, ErrorCodes.CashDayAlreadyClosed);
        // Demo verisi --reset (T-74): markanın tahsilat kayıtları.
        services.AddBrandDataEraser<SalesDbContext>();
        return services;
    }

    public static IEndpointRouteBuilder MapSalesEndpoints(this IEndpointRouteBuilder app)
    {
        CashDayEndpoints.Map(app);
        CustomerPaymentEndpoints.Map(app);
        return app;
    }
}
