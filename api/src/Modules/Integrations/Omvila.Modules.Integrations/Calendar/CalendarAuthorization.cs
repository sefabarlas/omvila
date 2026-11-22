using NodaTime;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>Bağlantı sonucu.</summary>
internal sealed record CalendarAuthorization(string AccessToken, string RefreshToken, Instant ExpiresUtc, string CalendarId, string? AccountEmail);
