namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Randevu durumu (durum-makinesi.md § 1). Yedi durum sabittir (AGENTS.md § 1.6). <see cref="Draft"/> v1'de hiçbir
/// akışta üretilmez. "Kapora bekleniyor" durum değil, <see cref="DisplayStatus.DepositPending"/> gösterimidir.
/// </summary>
internal enum AppointmentStatus
{
    Draft,
    PendingApproval,
    Confirmed,
    CheckedIn,
    Completed,
    Cancelled,
    NoShow,
}
