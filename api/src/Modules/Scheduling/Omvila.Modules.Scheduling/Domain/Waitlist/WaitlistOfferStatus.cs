namespace Omvila.Modules.Scheduling.Domain.Waitlist;

/// <summary>Bekleme teklifi durumu (durum-makinesi.md § 3).</summary>
internal enum WaitlistOfferStatus
{
    Pending,
    Accepted,
    Declined,
    Expired,
    Withdrawn,
}
