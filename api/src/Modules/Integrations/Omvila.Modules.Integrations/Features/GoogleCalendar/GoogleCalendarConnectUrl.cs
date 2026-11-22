using NodaTime;

namespace Omvila.Modules.Integrations.Features.GoogleCalendar;

/// <summary>Sözleşme GoogleCalendarConnectUrl.</summary>
internal sealed record GoogleCalendarConnectUrl(Uri Url, string State, Instant ExpiresUtc);
