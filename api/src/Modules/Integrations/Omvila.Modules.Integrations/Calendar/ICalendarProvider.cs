using NodaTime;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>
/// Takvim sağlayıcısı (mimari.md § Dış servisler: ICalendarProvider). Aktarım tek yönlüdür: OMVILA → takvim. Geliştirme ve
/// testte <see cref="FakeCalendarProvider"/>; gerçek Google istemcisi mağaza hazırlığıyla (K7). Jeton ve olay içeriği günlüğe yazılmaz.
/// </summary>
internal interface ICalendarProvider
{
    /// <summary>Bağlanma açık mı (sağlayıcı seçili ve yapılandırılmış).</summary>
    bool IsEnabled { get; }

    /// <summary>Kullanıcının yönlendirileceği izin sayfası.</summary>
    Uri BuildAuthorizationUrl(string state, Uri redirectUri);

    /// <summary>Yetki kodunu jetonlara çevirir; sağlayıcı kodu reddederse null.</summary>
    Task<CalendarAuthorization?> ExchangeCodeAsync(string code, Uri redirectUri, CancellationToken ct);

    /// <summary>Erişim jetonunu yeniler.</summary>
    Task<CalendarTokenRefresh> RefreshAsync(string refreshToken, CancellationToken ct);

    /// <summary>Olayı ekler (<paramref name="externalEventId"/> boşsa) veya günceller.</summary>
    Task<CalendarWriteResult> UpsertEventAsync(CalendarAccess access, string? externalEventId, CalendarEventData data, CancellationToken ct);

    /// <summary>Olayı siler (zaten yoksa başarılı sayılır).</summary>
    Task<CalendarWriteResult> DeleteEventAsync(CalendarAccess access, string externalEventId, CancellationToken ct);

    /// <summary>Sağlayıcıdaki erişimi iptal eder (en iyi çaba).</summary>
    Task RevokeAsync(string token, CancellationToken ct);
}
