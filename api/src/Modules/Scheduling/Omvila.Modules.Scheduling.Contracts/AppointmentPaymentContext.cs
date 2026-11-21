using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Tahsilat satırında randevu bilgisi (Sales: Kasa tablosu "Kalem" ve "Personel"; sözleşme PaymentView). İptal edilen kalemler
/// hariç, sırayla.
/// </summary>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="ServiceNames">Kalemlerin kopyalanan hizmet adları.</param>
/// <param name="StaffName">İlk personelli kalemin personelinin ad soyadı; yoksa null.</param>
public sealed record AppointmentPaymentContext(Guid AppointmentId, Instant StartUtc, IReadOnlyList<string> ServiceNames, string? StaffName);
