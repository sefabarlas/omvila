using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Waitlist;

/// <summary>
/// Boşalan bir saatin bir kişiye teklifi (alan-modeli.md § WaitlistOffer). Durumu yalnız <see cref="WaitlistEntry"/>
/// metotlarıyla değişir. Teklif süresince saat başka müşteriye kapalıdır: <c>Pending</c> ve <see cref="HoldExpiresUtc"/>
/// geçmemiş teklif müsaitlikte teklif sahibine ayrılmış meşguliyettir (musaitlik.md § 4, M18; T-90). Bağlantı kodu
/// (<c>/b/{code}</c>) yalnız bu teklifin okunmasına, kabulüne ve reddine yetki verir (roller-yetkiler.md).
/// </summary>
internal sealed class WaitlistOffer
{
    private WaitlistOffer()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid WaitlistEntryId { get; private set; }

    public Guid BranchId { get; private set; }

    /// <summary>Teklif sahibi (global kişi); müsaitlikte tutma ona açıktır.</summary>
    public Guid CustomerId { get; private set; }

    public Guid OfferedStaffId { get; private set; }

    public Instant OfferedStartUtc { get; private set; }

    /// <summary>Tutulan aralığın sonu: bekleyenin hizmet planının bitişi.</summary>
    public Instant OfferedEndUtc { get; private set; }

    public Instant HoldExpiresUtc { get; private set; }

    public WaitlistOfferStatus Status { get; private set; }

    /// <summary>/b/ bağlantı kodu (10 karakter Base62, platformda tek).</summary>
    public string AccessCode { get; private set; } = string.Empty;

    public Instant OfferedUtc { get; private set; }

    /// <summary>"Saat öner" diyen kullanıcı; teklif motorundan geldiyse null.</summary>
    public Guid? OfferedByUserId { get; private set; }

    public Instant? RespondedUtc { get; private set; }

    public Guid? AppointmentId { get; private set; }

    public uint Version { get; private set; }

    /// <summary>Bekliyor ve süresi geçmemiş: saat tutuluyor.</summary>
    public bool IsLiveAt(Instant now) => Status == WaitlistOfferStatus.Pending && now < HoldExpiresUtc;

    internal static WaitlistOffer Create(
        Guid id, WaitlistEntry entry, Guid staffId, Interval slot, Duration hold, string accessCode, Instant now, Guid? userId) => new()
        {
            Id = id,
            BrandId = entry.BrandId,
            WaitlistEntryId = entry.Id,
            BranchId = entry.BranchId,
            CustomerId = entry.CustomerId,
            OfferedStaffId = staffId,
            OfferedStartUtc = slot.Start,
            OfferedEndUtc = slot.End,
            HoldExpiresUtc = now + hold,
            Status = WaitlistOfferStatus.Pending,
            AccessCode = accessCode,
            OfferedUtc = now,
            OfferedByUserId = userId,
        };

    internal void Close(WaitlistOfferStatus status, Instant now, Guid? appointmentId = null)
    {
        Status = status;
        RespondedUtc = now;
        AppointmentId = appointmentId;
    }
}
