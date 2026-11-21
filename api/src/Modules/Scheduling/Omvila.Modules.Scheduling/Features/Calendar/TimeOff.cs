using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Calendar;

/// <summary>Sözleşme TimeOff (takvimde izin bloğu).</summary>
internal sealed record TimeOff(Guid Id, Guid StaffId, string Type, Instant StartUtc, Instant EndUtc, bool IsAllDay, string? Note);
