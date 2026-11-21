using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Scheduling.Infrastructure.ConflictGuard;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>
/// Teklif motoru (durum-makinesi.md § 3; alan-modeli.md § WaitlistOffer; dilim-05 kabul kriteri 3). Boşalan saat
/// (personel + başlangıç) için şubenin bekleyenleri öncelik ve kayıt sırasıyla (FIFO) taranır; tercihi uyan (personel,
/// gün, saat dilimi), bu saati daha önce almamış ve hizmetleri o personelde o başlangıçta sığan ilk kişiye teklif gider.
/// Sığma, randevu yazma yoluyla aynı kurallarla denetlenir: personel × gün kilidi → anlık görüntü (kilitten sonra) →
/// motor (public kip, kişinin kendi tutması kendisine açık). Teklif saati <c>WaitlistOfferHoldMinutes</c> boyunca tutar:
/// bekleyen teklif müsaitlikte başkalarına meşguliyettir (T-90). İsteğin veya işin transaction'ında ve markanın kiracı
/// bağlamında çalışır; commit etmez.
/// </summary>
internal sealed class WaitlistOfferEngine(
    SchedulingDbContext db,
    AvailabilitySnapshotLoader loader,
    AppointmentLocks locks,
    ICustomerDirectory customers,
    ICreditLedger credits,
    IOutbox outbox,
    IIdGenerator ids,
    IClock clock)
{
    private static readonly Duration Hold = Duration.FromMinutes(PlatformDefaults.WaitlistOfferHoldMinutes);

    /// <summary>
    /// Boşalan saati sıradaki uygun kişiye teklif eder; kimse uymuyorsa, saat artık boş değilse veya markanın kontörü
    /// bitmişse (teklif kimseye ulaşmaz, saat boşuna tutulmaz) null.
    /// </summary>
    // Kural: durum-makinesi.md § 3 (— → Pending), A4–A7 "bekleme listesi tetiklenir"
    public async Task<WaitlistOffer?> OfferFreedSlotAsync(Guid brandId, Guid branchId, Guid staffId, Instant startUtc, CancellationToken ct)
    {
        var waiting = await db.WaitlistEntries
            .Include(e => e.Offers)
            .Where(e => e.BranchId == branchId && e.Status == WaitlistEntryStatus.Waiting)
            .OrderByDescending(e => e.Priority).ThenBy(e => e.JoinedUtc).ThenBy(e => e.Id)
            .ToListAsync(ct);
        waiting = [.. waiting.Where(e => e.PendingOffer is null && !e.WasOffered(staffId, startUtc))];
        if (waiting.Count == 0 || await credits.GetBalanceAsync(brandId, ct) < 1)
        {
            return null;
        }

        var reachable = await customers.GetDisplayInfoAsync(brandId, [.. waiting.Select(e => e.BrandCustomerId).Distinct()], ct);
        waiting = [.. waiting.Where(e => reachable.TryGetValue(e.BrandCustomerId, out var c) && !c.IsBlocked && c.PhoneMasked is not null)];
        var locked = await LockedSnapshotAsync(brandId, branchId, staffId, startUtc, waiting.SelectMany(e => e.ServiceIds), ct);
        if (!locked.IsSuccess)
        {
            return null;
        }

        var snapshot = locked.Value;

        var local = startUtc.InZone(snapshot.Zone).LocalDateTime;
        foreach (var entry in waiting.Where(e => e.Preferences.Accepts(staffId, local)))
        {
            if (Fit(snapshot, entry, staffId, startUtc) is { } slot)
            {
                var offered = await OfferAsync(entry, staffId, slot, userId: null, ct);
                return offered.IsSuccess ? offered.Value : null;
            }
        }

        return null;
    }

    /// <summary>
    /// "Saat öner" (sözleşme manageOfferSlot): seçilen saat ve personel bu kişiye teklif edilir. Saat dolu veya personel
    /// müsait değilse <c>slot_taken</c>; kayıt beklemiyorsa veya bekleyen teklifi varsa <c>invalid_state_transition</c>;
    /// kontör yoksa <c>insufficient_credits</c> (teklif oluşmaz).
    /// </summary>
    public async Task<Result<WaitlistOffer>> OfferManuallyAsync(WaitlistEntry entry, Guid staffId, Instant startUtc, Guid? userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (entry.Status != WaitlistEntryStatus.Waiting || entry.PendingOffer is not null)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        if (await credits.GetBalanceAsync(entry.BrandId, ct) < 1)
        {
            return Error.Conflict(ErrorCodes.InsufficientCredits);
        }

        var locked = await LockedSnapshotAsync(entry.BrandId, entry.BranchId, staffId, startUtc, entry.ServiceIds, ct);
        if (!locked.IsSuccess)
        {
            return locked.Error;
        }

        if (!locked.Value.Staff.ContainsKey(staffId))
        {
            return Error.Validation("staffId", "out_of_range");
        }

        return Fit(locked.Value, entry, staffId, startUtc) is { } slot
            ? await OfferAsync(entry, staffId, slot, userId, ct)
            : Error.Conflict(ErrorCodes.SlotTaken);
    }

    /// <summary>Kişinin hizmetleri bu personelde bu başlangıçta sığıyorsa tutulacak aralık (başlangıç–planın sonu).</summary>
    private static Interval? Fit(AvailabilitySnapshot snapshot, WaitlistEntry entry, Guid staffId, Instant startUtc)
    {
        var request = AvailabilityRequest.Create([.. entry.ServiceIds.Select(s => new RequestedItem(s, staffId))], sameStaffForAll: true, AvailabilityMode.Public);
        if (!request.IsSuccess)
        {
            return null;
        }

        var evaluation = AvailabilityEngine.Evaluate(snapshot, request.Value with { CustomerId = entry.CustomerId }, startUtc);
        if (!evaluation.IsAvailable)
        {
            return null;
        }

        var plan = evaluation.Candidate!.ToPlan();
        return new Interval(startUtc, plan.EndUtc);
    }

    private async Task<Result<WaitlistOffer>> OfferAsync(WaitlistEntry entry, Guid staffId, Interval slot, Guid? userId, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var offered = entry.Offer(ids.New(), staffId, slot, Hold, AccessCodeGenerator.New(), now, userId);
        if (!offered.IsSuccess)
        {
            return offered.Error;
        }

        db.WaitlistOffers.Add(offered.Value);
        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(
            new WaitlistOfferCreated(
                entry.BrandId, offered.Value.Id, entry.BranchId, entry.CustomerId, entry.BrandCustomerId, slot.Start,
                PlatformDefaults.WaitlistOfferHoldMinutes, offered.Value.AccessCode),
            ct);
        return offered.Value;
    }

    /// <summary>
    /// Kural: musaitlik.md § Katman 2. Personel × yerel gün kilidi alınır (yazma yolu ve diğer teklifler bekler), anlık
    /// görüntü kilitten sonra okunur. Kilit zaman aşımında <c>try_again</c>, şube yoksa <c>not_found</c>.
    /// </summary>
    private async Task<Result<AvailabilitySnapshot>> LockedSnapshotAsync(
        Guid brandId, Guid branchId, Guid staffId, Instant startUtc, IEnumerable<Guid> serviceIds, CancellationToken ct)
    {
        var services = serviceIds.Distinct().ToList();
        var utcDate = startUtc.InUtc().Date;
        var probe = await loader.LoadAsync(brandId, branchId, utcDate.PlusDays(-1), utcDate.PlusDays(1), services, [staffId], ct);
        if (probe is null)
        {
            return Error.NotFound();
        }

        var date = startUtc.InZone(probe.Zone).Date;
        var locked = await locks.AcquireAsync(AppointmentLocks.OrderedKeys([staffId], [], [date]), ct);
        if (!locked.IsSuccess)
        {
            return locked.Error;
        }

        var snapshot = await loader.LoadAsync(brandId, branchId, date, date, services, [staffId], ct);
        return snapshot is null ? Error.NotFound() : snapshot;
    }
}
