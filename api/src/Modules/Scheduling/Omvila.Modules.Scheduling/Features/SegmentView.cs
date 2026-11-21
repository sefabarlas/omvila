using NodaTime;

namespace Omvila.Modules.Scheduling.Features;

/// <summary>Sözleşme SegmentView: staffBusy = false olan dilim taralı "Bekleme · müsait" şerididir.</summary>
internal sealed record SegmentView(Instant StartUtc, Instant EndUtc, bool StaffBusy);
