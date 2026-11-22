using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Me;

/// <summary>
/// "Kampanya ve indirim mesajları" anahtarı (sözleşme meUpdateMarketingConsent; alan-modeli.md § ConsentRecord): tek anahtar o marka
/// için MarketingSms, MarketingEmail ve MarketingPush kayıtlarını aynı değerle ekler. Defter yalnız eklenir; türün son değeri zaten
/// aynıysa (kayıt yoksa geri çekilmiş sayılır) yeni satır yazılmaz. Rıza kayıtları marka RLS'indedir: çağıran markanın veya platformun
/// bağlamında çağırır.
/// </summary>
internal sealed class MarketingConsentLedger(CustomersDbContext db, IIdGenerator ids)
{
    private static readonly ConsentKind[] MarketingKinds = [ConsentKind.MarketingSms, ConsentKind.MarketingEmail, ConsentKind.MarketingPush];

    /// <summary>Son kampanya rızası ve anı, marka başına (kayıt yoksa satır yok).</summary>
    public async Task<Dictionary<Guid, (bool Granted, Instant UpdatedUtc)>> LatestAsync(Guid customerId, IReadOnlyCollection<Guid> brandIds, CancellationToken ct)
    {
        var rows = await db.ConsentRecords.AsNoTracking()
            .Where(c => c.CustomerId == customerId && c.BrandId != null && brandIds.Contains(c.BrandId.Value) && MarketingKinds.Contains(c.ConsentType))
            .Select(c => new { BrandId = c.BrandId!.Value, c.IsGranted, c.OccurredUtc, c.Id })
            .ToListAsync(ct);
        return rows.GroupBy(r => r.BrandId).ToDictionary(
            g => g.Key,
            g => g.OrderByDescending(r => r.OccurredUtc).ThenByDescending(r => r.Id).Select(r => (r.IsGranted, r.OccurredUtc)).First());
    }

    // Kural: alan-modeli.md § ConsentRecord (yalnız eklenir); sözleşme meUpdateMarketingConsent (aynı değer yeni satır yazmaz)
    public async Task SetAsync(Guid customerId, Guid brandId, bool granted, string? ipAddress, Instant now, CancellationToken ct)
    {
        var records = await db.ConsentRecords.AsNoTracking()
            .Where(c => c.CustomerId == customerId && c.BrandId == brandId)
            .OrderByDescending(c => c.OccurredUtc).ThenByDescending(c => c.Id)
            .ToListAsync(ct);
        // Uygulamada yeni metin onaylanmaz; satır, markada son onaylanan aydınlatma metni sürümünü taşır.
        var version = records.FirstOrDefault(c => c.ConsentType == ConsentKind.PrivacyNoticeAcknowledged)?.DocumentVersion ?? string.Empty;
        foreach (var kind in MarketingKinds)
        {
            var current = records.FirstOrDefault(c => c.ConsentType == kind)?.IsGranted ?? false;
            if (current != granted)
            {
                db.ConsentRecords.Add(ConsentRecord.Create(ids.New(), customerId, brandId, kind, granted, version, ConsentSource.CustomerApp, ipAddress, now));
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
