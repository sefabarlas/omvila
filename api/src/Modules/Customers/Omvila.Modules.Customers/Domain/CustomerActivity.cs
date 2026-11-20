using NodaTime;

namespace Omvila.Modules.Customers.Domain;

/// <summary>Bir kartın segment hesabı için gereken sayaçlar.</summary>
internal readonly record struct CustomerActivity(int VisitCount, int NoShowCount, LocalDate? FirstVisitOn, LocalDate? LastVisitOn);
