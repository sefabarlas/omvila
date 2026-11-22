using NodaTime;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>Takvim olayı içeriği (UTC anlar).</summary>
internal sealed record CalendarEventData(string Title, string Description, Instant StartUtc, Instant EndUtc);
