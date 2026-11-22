using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Features.Me;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Deletion;

/// <summary>
/// Müşterinin kendi hesabını silmesi (sözleşme meRequestAccountDeletion; dilim-05 KK9; W7 hemen uygulanır; guvenlik-kvkk.md §
/// Haklar ve işleyiş, § Anonimleştirme). Her markadaki kart, panel silmesiyle aynı yoldan anonim bir kişiye bağlanır ve katılımcılar
/// müşteri adına çalışır (<see cref="CustomerErasure.ByCustomer"/>: yaklaşan randevular AccountDeleted ile iptal, seanslar geri döner,
/// salona bildirim). Randevu, ödeme ve prim kayıtları anonim kartta kalır. Kampanya izinleri geri çekilir, kayıtlı markalar silinir,
/// global kayıt yerinde anonimleşir; <c>DataDeletionRequest</c> (CustomerApp, Completed) ve her marka için ve platform genelinde
/// <c>AuditLog(Anonymize)</c> yazılır. Kartlar ve rızalar marka RLS'inde olduğundan her kart kendi markasının bağlamında işlenir.
/// </summary>
internal sealed class CustomerAccountEraser(
    CustomersDbContext db,
    IDbSession session,
    IEnumerable<ICustomerErasureParticipant> participants,
    MarketingConsentLedger consents,
    IAuditLog audit,
    IIdGenerator ids,
    IClock clock)
{
    // Kural: guvenlik-kvkk.md § Haklar ve işleyiş (Silme, müşteri: bütün markalarda anonimleştirme, yaklaşan randevular iptal)
    public async Task<CustomerAccountDeletion?> DeleteAsync(Guid customerId, string? ipAddress, string? userAgent, CancellationToken ct)
    {
        var customer = await db.Customers.SingleOrDefaultAsync(c => c.Id == customerId && c.AnonymizedUtc == null, ct);
        if (customer is null)
        {
            return null;
        }

        var previous = session.Scope;
        var now = clock.GetCurrentInstant();
        try
        {
            await session.SetTenantAsync(TenantScope.Platform, ct);
            var cards = await db.BrandCustomers
                .Where(b => b.CustomerId == customerId && b.MergedIntoId == null && b.AnonymizedUtc == null)
                .ToListAsync(ct);
            var cancelled = 0;
            foreach (var card in cards)
            {
                await session.SetTenantAsync(TenantScope.ForBrand(card.BrandId), ct);
                cancelled += await EraseCardAsync(customerId, card, ipAddress, userAgent, now, ct);
            }

            await session.SetTenantAsync(TenantScope.Platform, ct);
            await db.SavedBrands.Where(s => s.CustomerId == customerId).ExecuteDeleteAsync(ct);
            var zone = DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
            var request = DataDeletionRequest.CompletedFromApp(
                ids.New(), customerId, customer.FullName, now.InZone(zone).Date.PlusDays(PlatformDefaults.DeletionRequestDueDays), now);
            db.DataDeletionRequests.Add(request);
            customer.AnonymizeAccount(now);
            await db.SaveChangesAsync(ct);
            await audit.WriteAsync(
                new AuditEntry(null, AuditActorType.Customer, customerId, AuditAction.Anonymize, nameof(Customer), customerId,
                    new Dictionary<string, object?>(StringComparer.Ordinal)
                    {
                        ["dataDeletionRequestId"] = request.Id,
                        ["brands"] = cards.Count,
                        ["cancelledAppointments"] = cancelled,
                    },
                    ipAddress, userAgent),
                ct);
            return new CustomerAccountDeletion(now, cancelled);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }

    /// <summary>Bir markadaki kart: önizleme sayıları, anonim kişiye bağlama, katılımcılar, kampanya izninin geri çekilmesi, denetim.</summary>
    /// <returns>İptal edilen yaklaşan randevu sayısı.</returns>
    private async Task<int> EraseCardAsync(Guid customerId, BrandCustomer card, string? ipAddress, string? userAgent, Instant now, CancellationToken ct)
    {
        var footprint = CustomerErasureFootprint.None;
        foreach (var participant in participants)
        {
            footprint = footprint.Add(await participant.GetFootprintAsync(card.BrandId, card.Id, now, ct));
        }

        var anonymous = Customer.CreateAnonymous(ids.New(), now);
        db.Customers.Add(anonymous);
        card.Anonymize(anonymous.Id, now);
        var pending = await db.DataDeletionRequests
            .Where(r => r.BrandId == card.BrandId && r.CustomerId == customerId && r.Status == DeletionRequestStatus.Pending)
            .ToListAsync(ct);
        pending.ForEach(r => r.Complete(now));
        await db.SaveChangesAsync(ct);

        var erasure = new CustomerErasure(card.BrandId, card.Id, anonymous.Id, UserId: null, now, ByCustomer: true);
        foreach (var participant in participants)
        {
            await participant.EraseAsync(erasure, ct);
        }

        // Rıza kayıtları silinmez (yalnız eklenir); kampanya izni geri çekilmiş olarak kapanır.
        await consents.SetAsync(customerId, card.BrandId, granted: false, ipAddress, now, ct);
        await audit.WriteAsync(
            new AuditEntry(card.BrandId, AuditActorType.Customer, customerId, AuditAction.Anonymize, nameof(BrandCustomer), card.Id,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["requestedVia"] = nameof(DeletionRequestedVia.CustomerApp),
                    ["keptAppointments"] = footprint.Appointments,
                    ["keptPayments"] = footprint.Payments,
                    ["cancelledAppointments"] = footprint.UpcomingAppointments,
                },
                ipAddress, userAgent),
            ct);
        return footprint.UpcomingAppointments;
    }
}
