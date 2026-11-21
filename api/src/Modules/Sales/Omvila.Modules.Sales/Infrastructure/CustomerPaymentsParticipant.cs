using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Contracts;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary>
/// Müşteri birleştirme ve KVKK silmesinde Sales'in payı (T-92): tahsilatlar hedef karta taşınır; silmede ödeme kayıtları
/// anonim karta bağlı kalır (yasal saklama), yalnız sayılır. Paketler <see cref="CustomerPackagesParticipant"/>.
/// </summary>
internal sealed class CustomerPaymentsParticipant(SalesDbContext db) : ICustomerMergeParticipant, ICustomerErasureParticipant
{
    // Kural: alan-modeli.md § Birleştirme (ödemeler hedefe taşınır)
    public async Task MergeAsync(CustomerMerge merge, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(merge);
        await db.Payments
            .Where(p => p.BrandId == merge.BrandId && p.BrandCustomerId == merge.SourceBrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.BrandCustomerId, merge.TargetBrandCustomerId), ct);
    }

    public async Task<CustomerErasureFootprint> GetFootprintAsync(Guid brandId, Guid brandCustomerId, Instant now, CancellationToken ct) =>
        new(Payments: await db.Payments.CountAsync(p => p.BrandId == brandId && p.BrandCustomerId == brandCustomerId, ct));

    // Ödeme satırları karta (BrandCustomerId) bağlıdır; kart anonimleştiğinde müşteri bağı da anonim olur.
    public Task EraseAsync(CustomerErasure erasure, CancellationToken ct) => Task.CompletedTask;
}
