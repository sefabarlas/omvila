using NodaTime;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>Olay yazmak için erişim.</summary>
internal sealed record CalendarAccess(string AccessToken, string CalendarId);
