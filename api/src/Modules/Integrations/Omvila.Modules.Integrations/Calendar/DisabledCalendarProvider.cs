namespace Omvila.Modules.Integrations.Calendar;

/// <summary>Sağlayıcı seçili değil veya Google istemcisi yapılandırılmamış: bağlanma kapalı, aktarım yapılmaz.</summary>
internal sealed class DisabledCalendarProvider : ICalendarProvider
{
    public bool IsEnabled => false;

    public Uri BuildAuthorizationUrl(string state, Uri redirectUri) => throw new InvalidOperationException("Takvim sağlayıcısı kapalı.");

    public Task<CalendarAuthorization?> ExchangeCodeAsync(string code, Uri redirectUri, CancellationToken ct) => Task.FromResult<CalendarAuthorization?>(null);

    public Task<CalendarTokenRefresh> RefreshAsync(string refreshToken, CancellationToken ct) => Task.FromResult(new CalendarTokenRefresh(CalendarCallStatus.Transient));

    public Task<CalendarWriteResult> UpsertEventAsync(CalendarAccess access, string? externalEventId, CalendarEventData data, CancellationToken ct) =>
        Task.FromResult(new CalendarWriteResult(CalendarCallStatus.Transient));

    public Task<CalendarWriteResult> DeleteEventAsync(CalendarAccess access, string externalEventId, CancellationToken ct) =>
        Task.FromResult(new CalendarWriteResult(CalendarCallStatus.Transient));

    public Task RevokeAsync(string token, CancellationToken ct) => Task.CompletedTask;
}
