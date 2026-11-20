using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Infrastructure;

/// <summary>
/// Global kişi ve marka kartı oluşturma (panel ekleme ve ICustomerDirectory ortak yolu). Eşzamanlı istekler aynı
/// telefonda buluşur: ekleme ON CONFLICT DO NOTHING ile yapılır, ardından satır okunur.
/// </summary>
internal sealed class CustomerRegistry(CustomersDbContext db, IIdGenerator ids, IClock clock)
{
    public Task<Customer?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>
        db.Customers.SingleOrDefaultAsync(c => c.PhoneE164 == phone.E164, ct);

    public Task<BrandCustomer?> FindCardAsync(Guid brandId, Guid customerId, CancellationToken ct) =>
        db.BrandCustomers.SingleOrDefaultAsync(b => b.BrandId == brandId && b.CustomerId == customerId, ct);

    /// <summary>
    /// Telefonla kişiyi bulur, yoksa verilen adla oluşturur. Mevcut kişinin adı değişmez (başka markanın veya müşterinin
    /// kendi verdiği addır). <paramref name="verified"/> ise kişi doğrulanmış işaretlenir.
    /// </summary>
    public async Task<Customer> FindOrCreateAsync(PhoneNumber phone, string fullName, bool verified, CancellationToken ct)
    {
        var customer = await FindByPhoneAsync(phone, ct);
        if (customer is null)
        {
            var c = Customer.Create(ids.New(), phone, fullName, birthYear: null, verified, clock.GetCurrentInstant());
            await db.Database.ExecuteSqlAsync($"""
                INSERT INTO customers.customers (id, phone_e164, full_name, language_code, is_verified, created_utc)
                VALUES ({c.Id}, {c.PhoneE164}, {c.FullName}, {c.LanguageCode}, {c.IsVerified}, {c.CreatedUtc})
                ON CONFLICT (phone_e164) DO NOTHING
                """, ct);
            customer = await db.Customers.SingleAsync(x => x.PhoneE164 == c.PhoneE164, ct);
        }

        if (verified && !customer.IsVerified)
        {
            customer.MarkVerified();
        }

        return customer;
    }

    /// <summary>Markada kart yoksa açar (eşzamanlı çağrılar aynı kartta buluşur). Created: bu çağrı mı açtı.</summary>
    public async Task<(BrandCustomer Card, bool Created)> EnsureCardAsync(
        Guid brandId, Guid customerId, string displayName, BrandCustomerSource source, CancellationToken ct)
    {
        var existing = await FindCardAsync(brandId, customerId, ct);
        if (existing is not null)
        {
            return (existing, false);
        }

        var card = BrandCustomer.Create(ids.New(), brandId, customerId, displayName, birthYear: null, notes: null, source, appointmentSmsEnabled: true, clock.GetCurrentInstant());
        var sourceText = card.Source.ToString();
        await db.Database.ExecuteSqlAsync($"""
            INSERT INTO customers.brand_customers (id, brand_id, customer_id, display_name, source, visit_count, cancel_count, no_show_count,
                total_spent_minor, is_blocked, appointment_sms_enabled, created_utc)
            VALUES ({card.Id}, {card.BrandId}, {card.CustomerId}, {card.DisplayName}, {sourceText}, 0, 0, 0, 0, false, {card.AppointmentSmsEnabled}, {card.CreatedUtc})
            ON CONFLICT (brand_id, customer_id) DO NOTHING
            """, ct);
        var saved = await db.BrandCustomers.SingleAsync(b => b.BrandId == brandId && b.CustomerId == customerId, ct);
        return (saved, saved.Id == card.Id);
    }
}
