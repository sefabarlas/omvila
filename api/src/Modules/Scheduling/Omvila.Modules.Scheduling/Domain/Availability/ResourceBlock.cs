using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Kaynağın bir birimini kaplayan aralık: aktif AppointmentItemResource veya ResourceDowntime.</summary>
internal sealed record ResourceBlock(Guid ResourceId, Interval During, Guid? AppointmentId = null);
