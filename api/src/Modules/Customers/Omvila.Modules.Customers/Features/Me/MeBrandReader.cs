using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Me;

/// <summary>
/// "Salonlarım" (sözleşme meListBrands, meSaveBrand; dilim-05 KK1). Randevu alınan markalar (kişinin kartları) ile QR veya
/// bağlantıyla kaydedilenler (CustomerSavedBrand) birlikte. Kartlar marka RLS'indedir: müşteri bağlamında görünmediğinden
/// platform bağlamında yalnız bu kişinin kimliğiyle süzülerek okunur. Kaydetmek kart açmaz; salon bu kişiyi görmez.
/// </summary>
internal sealed class MeBrandReader(
    CustomersDbContext db,
    IDbSession session,
    ICustomerBrandDirectory brands,
    ICustomerPackageBadges packages,
    IIdGenerator ids,
    IClock clock)
{
    // Kural: sözleşme meListBrands — son ziyarete göre azalan; ziyareti olmayanlar kaydedilme anına göre (yeniden eskiye) sonda.
    public async Task<MeBrandList> ListAsync(Guid customerId, CancellationToken ct)
    {
        var cards = await CardsAsync(customerId, brandId: null, ct);
        var saved = await db.SavedBrands.AsNoTracking().Where(s => s.CustomerId == customerId).ToListAsync(ct);
        var rows = await BuildAsync(cards, saved, ct);
        return new MeBrandList([.. rows
            .OrderByDescending(r => r.LastVisitOn.HasValue)
            .ThenByDescending(r => r.LastVisitOn)
            .ThenByDescending(r => r.SavedUtc)
            .ThenBy(r => r.Name, StringComparer.Ordinal)]);
    }

    /// <summary>Bağlantı adıyla markayı kaydeder; zaten kayıtlı veya kartı varsa satır yine döner. Bilinmeyen ad null.</summary>
    // Kural: alan-modeli.md § CustomerSavedBrand (BrandCustomer oluşturmaz), sözleşme meSaveBrand (kendiliğinden idempotent)
    public async Task<MeBrand?> SaveAsync(Guid customerId, string slug, string source, CancellationToken ct)
    {
        if (await brands.ResolveSlugAsync(slug, ct) is not { } brandId)
        {
            return null;
        }

        var saved = CustomerSavedBrand.Create(ids.New(), customerId, brandId, source, clock.GetCurrentInstant());
        await db.Database.ExecuteSqlAsync($"""
            INSERT INTO customers.customer_saved_brands (id, customer_id, brand_id, added_utc, source)
            VALUES ({saved.Id}, {saved.CustomerId}, {saved.BrandId}, {saved.AddedUtc}, {saved.Source})
            ON CONFLICT (customer_id, brand_id) DO NOTHING
            """, ct);
        var rows = await db.SavedBrands.AsNoTracking().Where(s => s.CustomerId == customerId && s.BrandId == brandId).ToListAsync(ct);
        var built = await BuildAsync(await CardsAsync(customerId, brandId, ct), rows, ct);
        return built.SingleOrDefault();
    }

    private async Task<List<MeBrand>> BuildAsync(List<CardRow> cards, List<CustomerSavedBrand> saved, CancellationToken ct)
    {
        var brandIds = cards.Select(c => c.BrandId).Concat(saved.Select(s => s.BrandId)).Distinct().ToList();
        var brandCards = await brands.GetCardsAsync(brandIds, ct);
        var badges = await packages.ListActiveAsync([.. cards.Select(c => c.Id)], ct);
        var cardByBrand = cards.ToDictionary(c => c.BrandId);
        var savedByBrand = saved.ToDictionary(s => s.BrandId);
        var rows = new List<MeBrand>();
        foreach (var brandId in brandIds)
        {
            if (!brandCards.TryGetValue(brandId, out var brand))
            {
                continue;
            }

            var card = cardByBrand.GetValueOrDefault(brandId);
            rows.Add(new MeBrand(
                brand.Id,
                brand.Name,
                brand.Slug,
                brand.LogoUrl,
                brand.CoverColor,
                brand.BookingEnabled,
                card is not null,
                savedByBrand.GetValueOrDefault(brandId)?.AddedUtc,
                card?.LastVisitOn,
                [.. brand.Branches.Select(b => new MeBranchRef(b.Id, b.Name, b.District))],
                card is not null && badges.TryGetValue(card.Id, out var list)
                    ? [.. list.Select(p => new MeBrandPackageBadge(p.CustomerPackageId, p.PackageName, p.RemainingSessions))]
                    : []));
        }

        return rows;
    }

    // Arşiv kart (birleştirilmiş) listede yoktur; anonimleştirilen kart anonim kişiye bağlandığından bu kişide görünmez.
    private async Task<List<CardRow>> CardsAsync(Guid customerId, Guid? brandId, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            return await db.BrandCustomers.AsNoTracking()
                .Where(b => b.CustomerId == customerId && b.MergedIntoId == null && (brandId == null || b.BrandId == brandId))
                .Select(b => new CardRow(b.Id, b.BrandId, b.LastVisitOn))
                .ToListAsync(ct);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }

    private sealed record CardRow(Guid Id, Guid BrandId, LocalDate? LastVisitOn);
}
