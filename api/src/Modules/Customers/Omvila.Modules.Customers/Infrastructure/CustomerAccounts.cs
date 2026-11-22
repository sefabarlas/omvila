using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Customers.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Infrastructure;

/// <summary><see cref="ICustomerAccounts"/>: global kişi tablosu RLS'sizdir; yalnız kişinin kendi kaydı okunur ve yazılır.</summary>
internal sealed class CustomerAccounts(CustomersDbContext db, CustomerRegistry registry) : ICustomerAccounts
{
    // Kural: sözleşme meVerifyOtp — Customer yoksa açılır, IsVerified ve HasAppAccount olur; BrandCustomer açılmaz (KK1).
    public async Task<CustomerAppSignIn> SignInAsync(PhoneNumber phone, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(phone);
        var customer = await registry.FindOrCreateAsync(phone, string.Empty, verified: true, ct);
        var isNew = customer.OpenAppAccount();
        await db.SaveChangesAsync(ct);
        return new CustomerAppSignIn(customer.Id, isNew);
    }

    public Task<bool> CanSignInAsync(Guid customerId, CancellationToken ct) =>
        db.Customers.AsNoTracking().AnyAsync(c => c.Id == customerId && c.HasAppAccount && c.AnonymizedUtc == null, ct);
}
