using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Rapor için randevu (Raporlar ve prim; sözleşme ReportOverview, CommissionReport).</summary>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="Status">PendingApproval, Confirmed, CheckedIn, Completed veya NoShow (Draft ve Cancelled dönmez).</param>
/// <param name="Channel">Online, Panel, Phone veya WalkIn.</param>
/// <param name="PayableAmountMinor">Tahsil edilecek tutar: toplam − paketten düşülen kalemler.</param>
/// <param name="RetainedDepositMinor">Salonda kalan kapora (alınan − iade; alınmadıysa 0).</param>
/// <param name="Items">İptal edilmemiş kalemler.</param>
public sealed record ReportAppointment(
    Guid AppointmentId,
    Guid BranchId,
    Instant StartUtc,
    string Status,
    string Channel,
    long PayableAmountMinor,
    long RetainedDepositMinor,
    IReadOnlyList<ReportAppointmentItem> Items);
