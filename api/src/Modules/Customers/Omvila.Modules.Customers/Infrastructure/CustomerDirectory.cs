using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Domain;
using Omvila.SharedKernel;
using ContractConsentChannel = Omvila.Modules.Customers.Contracts.ConsentChannel;
using ContractConsentType = Omvila.Modules.Customers.Contracts.ConsentType;

namespace Omvila.Modules.Customers.Infrastructure;

internal sealed class CustomerDirectory(CustomersDbContext db, CustomerRegistry registry, IIdGenerator ids, IClock clock) : ICustomerDirectory
{
    public async Task<EnsuredBrandCustomer> EnsureBrandCustomerAsync(EnsureBrandCustomerRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        var customer = await registry.FindOrCreateAsync(request.Phone, request.FullName, request.PhoneVerified, ct);
        var (card, created) = await registry.EnsureCardAsync(request.BrandId, customer.Id, request.FullName, ToDomain(request.Source), ct);
        await db.SaveChangesAsync(ct);
        return new EnsuredBrandCustomer(card.Id, customer.Id, created, card.IsBlocked, card.NoShowCount);
    }

    public async Task<EnsuredBrandCustomer> CreateWithoutPhoneAsync(Guid brandId, string fullName, CustomerSource source, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var customer = Customer.CreateWithoutPhone(ids.New(), fullName, now);
        var card = BrandCustomer.Create(
            ids.New(), brandId, customer.Id, customer.FullName, birthYear: null, notes: null, ToDomain(source), appointmentSmsEnabled: true, now);
        db.Customers.Add(customer);
        db.BrandCustomers.Add(card);
        await db.SaveChangesAsync(ct);
        return new EnsuredBrandCustomer(card.Id, customer.Id, Created: true, card.IsBlocked, card.NoShowCount);
    }

    public async Task<IReadOnlyDictionary<Guid, CustomerDisplayInfo>> GetDisplayInfoAsync(
        Guid brandId, IReadOnlyCollection<Guid> brandCustomerIds, CancellationToken ct)
    {
        var rows = await (from b in db.BrandCustomers.AsNoTracking()
                          join c in db.Customers.AsNoTracking() on b.CustomerId equals c.Id
                          where b.BrandId == brandId && brandCustomerIds.Contains(b.Id)
                          select new { b.Id, b.CustomerId, Name = b.DisplayName ?? c.FullName, c.PhoneE164, b.NoShowCount, b.IsBlocked, b.AppointmentSmsEnabled, b.VisitCount, b.FirstVisitOn, b.LastVisitOn })
            .ToListAsync(ct);
        return rows.ToDictionary(
            r => r.Id,
            r => new CustomerDisplayInfo(
                r.Id, r.CustomerId, r.Name, r.PhoneE164, CustomerPhoneDisplay.For(r.PhoneE164, canSeeFullPhone: false).PhoneMasked,
                r.NoShowCount, r.IsBlocked, r.AppointmentSmsEnabled, r.VisitCount, r.FirstVisitOn, r.LastVisitOn));
    }

    public async Task<Result> ApplyCountersAsync(Guid brandId, Guid brandCustomerId, CustomerCounterChange change, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(change);
        // Birleştirmeyle arşivlenen karta gelen geç olay kalan karta uygulanır.
        var card = await registry.ResolveMergedAsync(await db.BrandCustomers.SingleOrDefaultAsync(b => b.Id == brandCustomerId && b.BrandId == brandId, ct), ct);
        if (card is null)
        {
            return Error.NotFound();
        }

        card.ApplyCounters(change.Visits, change.Cancels, change.NoShows, change.SpentMinor, change.VisitedOn);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<string?> FindBrandDisplayNameAsync(Guid brandId, PhoneNumber phone, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(phone);
        return await (from b in db.BrandCustomers.AsNoTracking()
                      join c in db.Customers.AsNoTracking() on b.CustomerId equals c.Id
                      where b.BrandId == brandId && c.PhoneE164 == phone.E164 && b.MergedIntoId == null
                      select b.DisplayName ?? c.FullName).SingleOrDefaultAsync(ct);
    }

    public async Task RecordConsentAsync(ConsentEntry entry, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        db.ConsentRecords.Add(ConsentRecord.Create(
            ids.New(), entry.CustomerId, entry.BrandId, ToDomain(entry.Type), entry.IsGranted, entry.DocumentVersion, ToDomain(entry.Channel),
            entry.IpAddress, entry.OccurredUtc));
        await db.SaveChangesAsync(ct);
    }

    private static BrandCustomerSource ToDomain(CustomerSource source) => Enum.Parse<BrandCustomerSource>(source.ToString());

    private static ConsentKind ToDomain(ContractConsentType type) => Enum.Parse<ConsentKind>(type.ToString());

    private static ConsentSource ToDomain(ContractConsentChannel channel) => Enum.Parse<ConsentSource>(channel.ToString());
}
