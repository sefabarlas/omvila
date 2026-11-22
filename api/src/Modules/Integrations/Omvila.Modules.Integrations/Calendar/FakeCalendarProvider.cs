using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using NodaTime;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>
/// Sahte takvim sağlayıcısı (Development, Testing ve izinli Staging; K7). İzin sayfası yoktur: adres doğrudan geri dönüş
/// sayfasını kod ve state ile gösterir. Kod <c>rejected</c> ise reddedilir. Olaylar <see cref="FakeCalendarStore"/>'a yazılır.
/// </summary>
internal sealed class FakeCalendarProvider(FakeCalendarStore store, IClock clock) : ICalendarProvider
{
    public const string RejectedCode = "rejected";
    private static readonly Duration TokenLifetime = Duration.FromHours(1);

    public bool IsEnabled => true;

    public Uri BuildAuthorizationUrl(string state, Uri redirectUri) =>
        new(QueryHelpers.AddQueryString(redirectUri.ToString(), new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["code"] = "fake-" + Token(),
            ["state"] = state,
        }));

    public Task<CalendarAuthorization?> ExchangeCodeAsync(string code, Uri redirectUri, CancellationToken ct) =>
        Task.FromResult(string.Equals(code, RejectedCode, StringComparison.Ordinal)
            ? null
            : new CalendarAuthorization("fake-access-" + Token(), "fake-refresh-" + Token(), clock.GetCurrentInstant() + TokenLifetime, "primary", "personel@ornek.com"));

    public Task<CalendarTokenRefresh> RefreshAsync(string refreshToken, CancellationToken ct) =>
        Task.FromResult(store.IsRevoked(refreshToken)
            ? new CalendarTokenRefresh(CalendarCallStatus.Revoked)
            : new CalendarTokenRefresh(CalendarCallStatus.Ok, "fake-access-" + Token(), clock.GetCurrentInstant() + TokenLifetime));

    public Task<CalendarWriteResult> UpsertEventAsync(CalendarAccess access, string? externalEventId, CalendarEventData data, CancellationToken ct)
    {
        if (store.IsRevoked(access.AccessToken))
        {
            return Task.FromResult(new CalendarWriteResult(CalendarCallStatus.Revoked));
        }

        var id = externalEventId ?? "fake-event-" + Token();
        store.Put(id, data);
        return Task.FromResult(new CalendarWriteResult(CalendarCallStatus.Ok, id));
    }

    public Task<CalendarWriteResult> DeleteEventAsync(CalendarAccess access, string externalEventId, CancellationToken ct)
    {
        if (store.IsRevoked(access.AccessToken))
        {
            return Task.FromResult(new CalendarWriteResult(CalendarCallStatus.Revoked));
        }

        store.Remove(externalEventId);
        return Task.FromResult(new CalendarWriteResult(CalendarCallStatus.Ok, externalEventId));
    }

    public Task RevokeAsync(string token, CancellationToken ct)
    {
        store.Revoke(token);
        return Task.CompletedTask;
    }

    private static string Token() => WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(12));
}
