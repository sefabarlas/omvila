using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Bir başlangıca bağlanmış dilim; <see cref="During"/> [Start, End) yarı açık.</summary>
internal sealed record PlannedSegment(int SeqNo, SegmentKind Kind, Interval During, bool StaffBusy);
