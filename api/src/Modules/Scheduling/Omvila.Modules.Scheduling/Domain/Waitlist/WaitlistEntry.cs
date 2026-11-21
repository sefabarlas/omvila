using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain.Waitlist;

/// <summary>
/// Bekleme kaydı ve teklifleri (alan-modeli.md § WaitlistEntry, WaitlistOffer; durum-makinesi.md § 3). Kayıt:
/// <c>Waiting</c> → <c>Converted</c> | <c>Cancelled</c> | <c>Expired</c>. Teklif: <c>Pending</c> → <c>Accepted</c> |
/// <c>Declined</c> | <c>Expired</c> | <c>Withdrawn</c>. Kayıtta aynı anda en çok bir bekleyen teklif olur. Teklifteki her
/// değişiklik kaydın sürümünü de değiştirir (panelin If-Match'i kaydın sürümüdür).
/// </summary>
internal sealed class WaitlistEntry
{
    /// <summary>Sözleşme ManageWaitlistCreate.note maxLength.</summary>
    public const int NoteMaxLength = 300;

    private readonly List<WaitlistOffer> _offers = [];

    private WaitlistEntry()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid BrandCustomerId { get; private set; }

    public List<Guid> ServiceIds { get; private set; } = [];

    public Guid? PreferredStaffId { get; private set; }

    public bool AcceptOtherStaff { get; private set; }

    public List<LocalDate> Dates { get; private set; } = [];

    public List<WaitlistTimeWindow> TimeWindows { get; private set; } = [];

    /// <summary>Yüksek olan önce; eşitlikte kayıt sırası (FIFO). v1'de arayüz değiştirmez, hep 0.</summary>
    public int Priority { get; private set; }

    /// <summary>Panel notu. Günlüğe yazılmaz.</summary>
    public string? Note { get; private set; }

    public WaitlistSource Source { get; private set; }

    public WaitlistEntryStatus Status { get; private set; }

    /// <summary>"Listeden çık" bağlantı kodu (10 karakter Base62, platformda tek).</summary>
    public string AccessCode { get; private set; } = string.Empty;

    public Instant JoinedUtc { get; private set; }

    public Instant UpdatedUtc { get; private set; }

    /// <summary>Converted, Cancelled veya Expired olduğu an.</summary>
    public Instant? ClosedUtc { get; private set; }

    public Guid? CreatedByUserId { get; private set; }

    public uint Version { get; private set; }

    public IReadOnlyList<WaitlistOffer> Offers => _offers;

    public WaitlistOffer? PendingOffer => _offers.SingleOrDefault(o => o.Status == WaitlistOfferStatus.Pending);

    public WaitlistPreferences Preferences => new(ServiceIds, PreferredStaffId, AcceptOtherStaff, Dates, TimeWindows);

    public static WaitlistEntry Create(
        Guid id, Guid brandId, Guid branchId, Guid customerId, Guid brandCustomerId, WaitlistPreferences preferences,
        string? note, WaitlistSource source, string accessCode, Instant now, Guid? userId = null)
    {
        ArgumentNullException.ThrowIfNull(preferences);
        if (!AccessCodeGenerator.IsWellFormed(accessCode))
        {
            throw new DomainException("Erişim kodu 10 karakter Base62 olmalı.");
        }

        var entry = new WaitlistEntry
        {
            Id = id,
            BrandId = brandId,
            BranchId = branchId,
            CustomerId = customerId,
            BrandCustomerId = brandCustomerId,
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
            Source = source,
            Status = WaitlistEntryStatus.Waiting,
            AccessCode = accessCode,
            JoinedUtc = now,
            UpdatedUtc = now,
            CreatedByUserId = userId,
        };
        entry.Apply(preferences);
        return entry;
    }

    /// <summary>Aynı müşteri aynı şubede aynı hizmetler için yeniden katılınca tercihler güncellenir (sözleşme publicJoinWaitlist).</summary>
    public Result UpdatePreferences(WaitlistPreferences preferences, Instant now)
    {
        ArgumentNullException.ThrowIfNull(preferences);
        if (Status != WaitlistEntryStatus.Waiting)
        {
            return InvalidTransition();
        }

        Apply(preferences);
        UpdatedUtc = now;
        return Result.Success();
    }

    /// <summary>Bu saat (personel + başlangıç) bu kayda daha önce teklif edildi mi; reddedilen saat aynı kişiye yeniden gitmez.</summary>
    public bool WasOffered(Guid staffId, Instant start) => _offers.Exists(o => o.OfferedStaffId == staffId && o.OfferedStartUtc == start);

    /// <summary>
    /// Teklif (motor veya "Saat öner"): <c>HoldExpiresUtc = şimdi + WaitlistOfferHoldMinutes</c>. Kayıt beklemiyorsa veya
    /// bekleyen teklifi varsa <c>invalid_state_transition</c>.
    /// </summary>
    // Kural: durum-makinesi.md § 3 (— → Pending)
    public Result<WaitlistOffer> Offer(Guid offerId, Guid staffId, Interval slot, Duration hold, string accessCode, Instant now, Guid? userId)
    {
        if (Status != WaitlistEntryStatus.Waiting || PendingOffer is not null)
        {
            return InvalidTransition();
        }

        if (!AccessCodeGenerator.IsWellFormed(accessCode))
        {
            throw new DomainException("Erişim kodu 10 karakter Base62 olmalı.");
        }

        var offer = WaitlistOffer.Create(offerId, this, staffId, slot, hold, accessCode, now, userId);
        _offers.Add(offer);
        UpdatedUtc = now;
        return offer;
    }

    /// <summary>
    /// Kabul edilebilir mi (randevu yazılmadan önce): bekleyen ve süresi geçmemiş teklif. Süre geçmişse
    /// <c>hold_expired</c>; reddedilmiş, geri alınmış veya kabul edilmişse <c>invalid_state_transition</c>.
    /// </summary>
    public Result CanAccept(WaitlistOffer offer, Instant now)
    {
        var owned = Own(offer);
        if (offer.Status == WaitlistOfferStatus.Expired || (offer.Status == WaitlistOfferStatus.Pending && now >= offer.HoldExpiresUtc))
        {
            return Error.Conflict(ErrorCodes.HoldExpired);
        }

        return owned.Status == WaitlistOfferStatus.Pending && Status == WaitlistEntryStatus.Waiting ? Result.Success() : InvalidTransition();
    }

    /// <summary>Teklif kabul edildi, randevu oluştu: teklif Accepted, kayıt Converted.</summary>
    // Kural: durum-makinesi.md § 3 (Pending → Accepted)
    public Result Accept(WaitlistOffer offer, Guid appointmentId, Instant now)
    {
        var can = CanAccept(offer, now);
        if (!can.IsSuccess)
        {
            return can;
        }

        offer.Close(WaitlistOfferStatus.Accepted, now, appointmentId);
        Status = WaitlistEntryStatus.Converted;
        ClosedUtc = now;
        UpdatedUtc = now;
        return Result.Success();
    }

    /// <summary>
    /// "Bu saat olmaz": teklif Declined, kişi listede kalır. Zaten reddedilmişse değişiklik yok (false). Süresi geçmişse
    /// <c>hold_expired</c>; kabul edilmiş veya geri alınmışsa <c>invalid_state_transition</c>.
    /// </summary>
    // Kural: durum-makinesi.md § 3 (Pending → Declined)
    public Result<bool> Decline(WaitlistOffer offer, Instant now)
    {
        Own(offer);
        if (offer.Status == WaitlistOfferStatus.Declined)
        {
            return false;
        }

        if (offer.Status == WaitlistOfferStatus.Expired || (offer.Status == WaitlistOfferStatus.Pending && now >= offer.HoldExpiresUtc))
        {
            return Error.Conflict(ErrorCodes.HoldExpired);
        }

        if (offer.Status != WaitlistOfferStatus.Pending)
        {
            return InvalidTransition();
        }

        offer.Close(WaitlistOfferStatus.Declined, now);
        UpdatedUtc = now;
        return true;
    }

    /// <summary>Süresi dolan teklif (ExpireWaitlistOffers): Expired, kişi listede kalır.</summary>
    // Kural: durum-makinesi.md § 3 (Pending → Expired)
    public Result ExpireOffer(WaitlistOffer offer, Instant now)
    {
        Own(offer);
        if (offer.Status != WaitlistOfferStatus.Pending || offer.HoldExpiresUtc > now)
        {
            return InvalidTransition();
        }

        offer.Close(WaitlistOfferStatus.Expired, now);
        UpdatedUtc = now;
        return Result.Success();
    }

    /// <summary>"Teklifi geri al": bekleyen teklif Withdrawn; saat sıradakine gitmez, kişi listede kalır.</summary>
    // Kural: durum-makinesi.md § 3 (Pending → Withdrawn)
    public Result<WaitlistOffer> WithdrawOffer(Instant now)
    {
        if (PendingOffer is not { } offer)
        {
            return InvalidTransition();
        }

        offer.Close(WaitlistOfferStatus.Withdrawn, now);
        UpdatedUtc = now;
        return offer;
    }

    /// <summary>
    /// "Listeden çık": kayıt Cancelled; bekleyen teklif varsa Declined olur ve döner (saat sıradakine geçer). Zaten çıkmışsa
    /// değişiklik yok. Converted veya Expired ise <c>invalid_state_transition</c>.
    /// </summary>
    // Kural: durum-makinesi.md § 3 (Waiting → Cancelled)
    public Result<WaitlistOffer?> Leave(Instant now)
    {
        if (Status == WaitlistEntryStatus.Cancelled)
        {
            return (WaitlistOffer?)null;
        }

        if (Status != WaitlistEntryStatus.Waiting)
        {
            return InvalidTransition();
        }

        var released = PendingOffer is { } pending && pending.IsLiveAt(now) ? pending : null;
        PendingOffer?.Close(released is null ? WaitlistOfferStatus.Expired : WaitlistOfferStatus.Declined, now);
        Status = WaitlistEntryStatus.Cancelled;
        ClosedUtc = now;
        UpdatedUtc = now;
        return released;
    }

    /// <summary>
    /// Kural: T-90 (durum-makinesi.md "istenen son tarih geçti"). İstenen günlerin hepsi şubenin bugününden önceyse kayıt
    /// Expired olur; gün listesi boşsa ("Herhangi bir gün") veya bekleyen teklifi varsa olmaz.
    /// </summary>
    public Result Expire(LocalDate today, Instant now)
    {
        if (Status != WaitlistEntryStatus.Waiting || PendingOffer is not null || !Preferences.IsPastAllDates(today))
        {
            return InvalidTransition();
        }

        Status = WaitlistEntryStatus.Expired;
        ClosedUtc = now;
        UpdatedUtc = now;
        return Result.Success();
    }

    private void Apply(WaitlistPreferences preferences)
    {
        ServiceIds = [.. preferences.ServiceIds.Distinct()];
        PreferredStaffId = preferences.PreferredStaffId;
        AcceptOtherStaff = preferences.AcceptOtherStaff;
        Dates = [.. preferences.Dates.Distinct().Order()];
        TimeWindows = [.. preferences.TimeWindows.Distinct().Order()];
    }

    private WaitlistOffer Own(WaitlistOffer offer)
    {
        ArgumentNullException.ThrowIfNull(offer);
        return offer.WaitlistEntryId == Id && _offers.Contains(offer) ? offer : throw new DomainException("Teklif bu kayda ait değil.");
    }

    private static Error InvalidTransition() => Error.Conflict(ErrorCodes.InvalidStateTransition);
}
