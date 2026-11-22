using NodaTime;

namespace Omvila.Modules.Sales.Features.Commissions;

/// <summary>Sözleşme MyCommissionEntry ("Son primler" satırı).</summary>
internal sealed record MyCommissionEntry(
    Guid Id,
    string Type,
    long AmountMinor,
    string Status,
    Instant OccurredUtc,
    string CustomerName,
    string Description,
    Guid? AppointmentId,
    Guid? CustomerPackageId);
