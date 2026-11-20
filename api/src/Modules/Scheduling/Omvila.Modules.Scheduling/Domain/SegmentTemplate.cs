using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Kalemin başlangıca göreli dilimi (musaitlik.md § Girdi: (offset, duration, staffBusy, kind)).</summary>
internal sealed record SegmentTemplate(int SeqNo, SegmentKind Kind, Duration Offset, Duration Length, bool StaffBusy);
