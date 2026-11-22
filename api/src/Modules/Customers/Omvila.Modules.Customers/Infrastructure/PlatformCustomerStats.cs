using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Infrastructure;

/// <summary><see cref="IPlatformCustomerStats"/>: platform okuma servisi (ADR-0006); başka bağlamda çağrı programlama hatasıdır.</summary>
internal sealed class PlatformCustomerStats(CustomersDbContext db, ITenantContext tenant) : IPlatformCustomerStats
{
    public Task<int> CountBrandCustomersAsync(Guid brandId, CancellationToken ct)
    {
        if (!tenant.Scope.IsPlatform)
        {
            throw new DomainException("Platform okuma servisi yalnız platform bağlamında çağrılır (ADR-0006).");
        }

        return db.BrandCustomers.AsNoTracking().CountAsync(c => c.BrandId == brandId && c.MergedIntoId == null && c.AnonymizedUtc == null, ct);
    }
}
