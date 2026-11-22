using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Text;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>
/// Google Takvim istemcisi iskeleti (OAuth 2.0 yetki kodu akışı + Calendar API v3 olayları; K7). İstemci kimliği ve gizlisi
/// yapılandırmadan (<c>Integrations:Calendar:Google</c>, gizli değer ortamdan); yoksa bu sınıf seçilmez, bağlanma kapalıdır.
/// Gerçek istemci mağaza hazırlığıyla (OAuth onay ekranı doğrulaması) açılır; o zamana kadar uçtan uca sınanmamıştır (T-121).
/// </summary>
internal sealed class GoogleCalendarProvider(IHttpClientFactory http, IOptions<CalendarOptions> options, IClock clock) : ICalendarProvider
{
    public const string HttpClientName = "google-calendar";
    private const string AuthorizeEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string RevokeEndpoint = "https://oauth2.googleapis.com/revoke";
    private const string Scope = "https://www.googleapis.com/auth/calendar.events";

    private GoogleCalendarOptions Google => options.Value.Google;

    public bool IsEnabled => Google.IsConfigured;

    public Uri BuildAuthorizationUrl(string state, Uri redirectUri) =>
        new(QueryHelpers.AddQueryString(AuthorizeEndpoint, new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["client_id"] = Google.ClientId,
            ["redirect_uri"] = redirectUri.ToString(),
            ["response_type"] = "code",
            ["scope"] = Scope,
            ["access_type"] = "offline",
            ["prompt"] = "consent",
            ["state"] = state,
        }));

    public async Task<CalendarAuthorization?> ExchangeCodeAsync(string code, Uri redirectUri, CancellationToken ct)
    {
        var token = await PostTokenAsync(
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["code"] = code,
                ["redirect_uri"] = redirectUri.ToString(),
                ["grant_type"] = "authorization_code",
            },
            ct);
        return token is { AccessToken: { } access, RefreshToken: { } refresh }
            ? new CalendarAuthorization(access, refresh, clock.GetCurrentInstant() + Duration.FromSeconds(token.ExpiresIn), "primary", AccountEmail: null)
            : null;
    }

    public async Task<CalendarTokenRefresh> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        using var client = http.CreateClient(HttpClientName);
        using var content = new FormUrlEncodedContent(Credentials(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["refresh_token"] = refreshToken,
            ["grant_type"] = "refresh_token",
        }));
        using var response = await client.PostAsync(new Uri(TokenEndpoint), content, ct);
        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized)
        {
            // invalid_grant: kullanıcı erişimi kaldırdı veya jeton süresiz geçersiz.
            return new CalendarTokenRefresh(CalendarCallStatus.Revoked);
        }

        if (!response.IsSuccessStatusCode)
        {
            return new CalendarTokenRefresh(CalendarCallStatus.Transient);
        }

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>(ct);
        return token?.AccessToken is { } access
            ? new CalendarTokenRefresh(CalendarCallStatus.Ok, access, clock.GetCurrentInstant() + Duration.FromSeconds(token.ExpiresIn))
            : new CalendarTokenRefresh(CalendarCallStatus.Transient);
    }

    public async Task<CalendarWriteResult> UpsertEventAsync(CalendarAccess access, string? externalEventId, CalendarEventData data, CancellationToken ct)
    {
        using var client = Authorized(access);
        var url = EventsUrl(access.CalendarId);
        var body = new
        {
            summary = data.Title,
            description = data.Description,
            start = new { dateTime = InstantPattern.General.Format(data.StartUtc) },
            end = new { dateTime = InstantPattern.General.Format(data.EndUtc) },
        };
        using var response = externalEventId is null
            ? await client.PostAsJsonAsync(new Uri(url), body, ct)
            : await client.PatchAsJsonAsync(new Uri($"{url}/{Uri.EscapeDataString(externalEventId)}"), body, ct);
        if (!response.IsSuccessStatusCode)
        {
            return new CalendarWriteResult(Status(response.StatusCode));
        }

        var created = await response.Content.ReadFromJsonAsync<EventResponse>(ct);
        return new CalendarWriteResult(CalendarCallStatus.Ok, created?.Id ?? externalEventId);
    }

    public async Task<CalendarWriteResult> DeleteEventAsync(CalendarAccess access, string externalEventId, CancellationToken ct)
    {
        using var client = Authorized(access);
        var url = EventsUrl(access.CalendarId);
        using var response = await client.DeleteAsync(new Uri($"{url}/{Uri.EscapeDataString(externalEventId)}"), ct);
        return response.IsSuccessStatusCode || response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Gone
            ? new CalendarWriteResult(CalendarCallStatus.Ok, externalEventId)
            : new CalendarWriteResult(Status(response.StatusCode));
    }

    public async Task RevokeAsync(string token, CancellationToken ct)
    {
        using var client = http.CreateClient(HttpClientName);
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal) { ["token"] = token });
        using var response = await client.PostAsync(new Uri(RevokeEndpoint), content, ct);
    }

    private static string EventsUrl(string calendarId) => $"https://www.googleapis.com/calendar/v3/calendars/{Uri.EscapeDataString(calendarId)}/events";

    private static CalendarCallStatus Status(HttpStatusCode code) =>
        code is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden ? CalendarCallStatus.Revoked : CalendarCallStatus.Transient;

    private HttpClient Authorized(CalendarAccess access)
    {
        var client = http.CreateClient(HttpClientName);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access.AccessToken);
        return client;
    }

    private async Task<TokenResponse?> PostTokenAsync(Dictionary<string, string> values, CancellationToken ct)
    {
        using var client = http.CreateClient(HttpClientName);
        using var content = new FormUrlEncodedContent(Credentials(values));
        using var response = await client.PostAsync(new Uri(TokenEndpoint), content, ct);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<TokenResponse>(ct) : null;
    }

    private Dictionary<string, string> Credentials(Dictionary<string, string> values)
    {
        values["client_id"] = Google.ClientId!;
        values["client_secret"] = Google.ClientSecret!;
        return values;
    }

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string? AccessToken,
        [property: JsonPropertyName("refresh_token")] string? RefreshToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);

    private sealed record EventResponse([property: JsonPropertyName("id")] string? Id);
}
