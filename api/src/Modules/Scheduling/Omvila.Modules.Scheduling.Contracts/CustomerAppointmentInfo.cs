using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Müşteri kartındaki randevu satırı (sözleşme CustomerAppointmentRow).</summary>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="Status">AppointmentStatus metni.</param>
/// <param name="DisplayStatus">DisplayStatus metni.</param>
/// <param name="ServiceNames">Kalemlerin kopyalanan hizmet adları, sırayla.</param>
/// <param name="StaffNames">Kalemlerin personel adları (tekrarsız, sırayla).</param>
/// <param name="TotalAmountMinor">Kopyalanan kalem fiyatlarının toplamı (kuruş).</param>
/// <param name="CurrencyCode">Para birimi.</param>
public sealed record CustomerAppointmentInfo(
    Guid AppointmentId,
    Guid BranchId,
    Instant StartUtc,
    string Status,
    string DisplayStatus,
    IReadOnlyList<string> ServiceNames,
    IReadOnlyList<string> StaffNames,
    long TotalAmountMinor,
    string CurrencyCode);
