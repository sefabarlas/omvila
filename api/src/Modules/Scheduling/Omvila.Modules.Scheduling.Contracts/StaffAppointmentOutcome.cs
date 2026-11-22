using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Personelin sonuçlanmış veya gelmiş randevusu (kişisel prim özeti).</summary>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="Status">Completed, NoShow veya CheckedIn.</param>
public sealed record StaffAppointmentOutcome(Guid AppointmentId, Guid BrandCustomerId, Instant StartUtc, string Status);
