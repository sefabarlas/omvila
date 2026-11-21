namespace Omvila.Modules.Scheduling.Domain.Waitlist;

/// <summary>Bekleme kaydı durumu (durum-makinesi.md § 3).</summary>
internal enum WaitlistEntryStatus
{
    Waiting,
    Converted,
    Cancelled,
    Expired,
}
