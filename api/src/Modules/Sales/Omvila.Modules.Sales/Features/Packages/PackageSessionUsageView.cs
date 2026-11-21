using NodaTime;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Sözleşme PackageSessionUsageView.</summary>
internal sealed record PackageSessionUsageView(Guid AppointmentId, string ServiceName, Instant AppointmentStartUtc, Instant UsedUtc, Instant? RevertedUtc);
