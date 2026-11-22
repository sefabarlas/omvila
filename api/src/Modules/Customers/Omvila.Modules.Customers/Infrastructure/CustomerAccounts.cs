using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Features.Deletion;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Infrastructure;

/// <summary><see cref="ICustomerAccounts"/>: global kişi tablosu RLS'sizdir; yalnız kişinin kendi kaydı okunur ve yazılır.</summary>
internal sealed class CustomerAccounts(
    CustomersDbContext db, CustomerRegistry registry, IDbSession session, CustomerAccountEraser eraser, IIdGenerator ids, IClock clock) : ICustomerAccounts
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

    public async Task<CustomerAppIdentity?> GetIdentityAsync(Guid customerId, CancellationToken ct)
    {
        var customer = await db.Customers.AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == customerId && c.HasAppAccount && c.AnonymizedUtc == null, ct);
        return customer is not null && PhoneNumber.TryParse(customer.PhoneE164, out var phone)
            ? new CustomerAppIdentity(phone, customer.HasName ? customer.FullName : null)
            : null;
    }

    public async Task<IReadOnlyList<CustomerBrandCardRef>> ListBrandCardsAsync(Guid customerId, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            return await db.BrandCustomers.AsNoTracking()
                .Where(b => b.CustomerId == customerId && b.MergedIntoId == null && b.AnonymizedUtc == null)
                .Select(b => new CustomerBrandCardRef(b.BrandId, b.Id))
                .ToListAsync(ct);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }

    // Kural: alan-modeli.md § CustomerSavedBrand (Source Booking yalnız sunucu yazar); sözleşme publicCreateAppointment (meBearer)
    public async Task SaveBookedBrandAsync(Guid customerId, Guid brandId, CancellationToken ct)
    {
        var saved = CustomerSavedBrand.Create(ids.New(), customerId, brandId, CustomerSavedBrand.BookingSource, clock.GetCurrentInstant());
        await db.Database.ExecuteSqlAsync($"""
            INSERT INTO customers.customer_saved_brands (id, customer_id, brand_id, added_utc, source)
            VALUES ({saved.Id}, {saved.CustomerId}, {saved.BrandId}, {saved.AddedUtc}, {saved.Source})
            ON CONFLICT (customer_id, brand_id) DO NOTHING
            """, ct);
    }

    public Task<CustomerAccountDeletion?> DeleteAccountAsync(Guid customerId, string? ipAddress, string? userAgent, CancellationToken ct) =>
        eraser.DeleteAsync(customerId, ipAddress, userAgent, ct);
}
