using NodaTime;

namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Personel izni [StartUtc, EndUtc).</summary>
/// <param name="StaffId">Personel.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="EndUtc">Bitiş.</param>
public sealed record StaffTimeOff(Guid StaffId, Instant StartUtc, Instant EndUtc);
