using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Takvim aktarımı için personelin randevusu.</summary>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="IsCancelled">İptal edildi (veya personelin kalemleri iptal): olay silinir.</param>
/// <param name="StartUtc">Personelin ilk kaleminin başlangıcı.</param>
/// <param name="EndUtc">Personelin son kaleminin bitişi.</param>
/// <param name="ServiceNames">Personelin kalemlerindeki hizmet adları (sırayla).</param>
public sealed record StaffCalendarAppointment(
    Guid AppointmentId, Guid BrandCustomerId, bool IsCancelled, Instant StartUtc, Instant EndUtc, IReadOnlyList<string> ServiceNames);
