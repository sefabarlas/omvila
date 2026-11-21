using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Infrastructure;

namespace Omvila.Modules.Sales;

/// <summary>
/// Sales modülü girişi (alan-modeli.md § Modüller: Payment, PaymentAllocation). Dilim 4: kapora havalesinin tahsilat kaydı
/// (<see cref="IDepositPayments"/>; Scheduling "Kapora geldi" ile aynı transaction'da çağırır). Kendi HTTP ucu henüz yok.
/// </summary>
public static class SalesModule
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services)
    {
        services.AddModuleDbContext<SalesDbContext>(SalesDbContext.Schema);
        services.AddScoped<IDepositPayments, DepositPayments>();
        // Müşteri birleştirme ve KVKK silmesi (T-92): tahsilatlar.
        services.AddScoped<CustomerPaymentsParticipant>();
        services.AddScoped<Customers.Contracts.ICustomerMergeParticipant>(sp => sp.GetRequiredService<CustomerPaymentsParticipant>());
        services.AddScoped<Customers.Contracts.ICustomerErasureParticipant>(sp => sp.GetRequiredService<CustomerPaymentsParticipant>());
        // Demo verisi --reset (T-74): markanın tahsilat kayıtları.
        services.AddBrandDataEraser<SalesDbContext>();
        return services;
    }
}
