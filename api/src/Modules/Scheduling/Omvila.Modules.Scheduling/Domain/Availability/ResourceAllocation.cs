using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Kaleme ayrılan kaynak birimleri (yazma yolu her birim için bir AppointmentItemResource yazar).</summary>
internal sealed record ResourceAllocation(Guid ResourceId, int Units, Interval During);
