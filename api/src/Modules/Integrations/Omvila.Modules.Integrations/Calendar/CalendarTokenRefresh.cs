using NodaTime;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>Yenileme sonucu: Ok (yeni jeton), Revoked (kalıcı: bağlantı koptu), Transient (sonra yeniden denenir).</summary>
internal sealed record CalendarTokenRefresh(CalendarCallStatus Status, string? AccessToken = null, Instant? ExpiresUtc = null);
