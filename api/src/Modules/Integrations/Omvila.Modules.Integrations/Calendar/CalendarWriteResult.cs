using NodaTime;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>Yazma sonucu.</summary>
internal sealed record CalendarWriteResult(CalendarCallStatus Status, string? EventId = null);
