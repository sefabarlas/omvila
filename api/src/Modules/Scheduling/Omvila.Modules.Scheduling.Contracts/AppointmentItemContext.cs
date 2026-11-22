using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Prim satırında randevu kalemi (Sales: "Son primler" satırı; sözleşme MyCommissionEntry).</summary>
/// <param name="AppointmentItemId">Kalem.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="StartUtc">Randevu başlangıcı.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="ServiceName">Kalemin kopyalanan hizmet adı.</param>
public sealed record AppointmentItemContext(Guid AppointmentItemId, Guid AppointmentId, Instant StartUtc, Guid BrandCustomerId, string ServiceName);
