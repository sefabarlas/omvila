using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Infrastructure;

namespace Omvila.Modules.Sales;

/// <summary>
/// Sales modülü girişi (alan-modeli.md § Modüller: Payment, PaymentAllocation, CommissionEntry). Kapora havalesinin tahsilat
/// kaydı (<see cref="IDepositPayments"/>) ve "Tamamla ve ödeme al"ın tahsilatları ile hizmet primi
/// (<see cref="IAppointmentPayments"/>); ikisini de Scheduling istek transaction'ında çağırır. Kendi HTTP ucu henüz yok.
/// </summary>
public static class SalesModule
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<SalesDbContext>(SalesDbContext.Schema);
        services.AddScoped<IDepositPayments, DepositPayments>();
        services.AddScoped<IAppointmentPayments, AppointmentPayments>();
        // Müşteri birleştirme ve KVKK silmesi (T-92): tahsilatlar.
        services.AddScoped<CustomerPaymentsParticipant>();
        services.AddScoped<Customers.Contracts.ICustomerMergeParticipant>(sp => sp.GetRequiredService<CustomerPaymentsParticipant>());
        services.AddScoped<Customers.Contracts.ICustomerErasureParticipant>(sp => sp.GetRequiredService<CustomerPaymentsParticipant>());
        // Demo verisi --reset (T-74): markanın tahsilat kayıtları.
        services.AddBrandDataEraser<SalesDbContext>();
        return services;
    }
}
