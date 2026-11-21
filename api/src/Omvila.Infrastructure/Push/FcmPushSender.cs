using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Omvila.Infrastructure.Push;

/// <summary>
/// FCM HTTP v1 adaptörü. Hizmet hesabı anahtarıyla RS256 imzalı JWT → OAuth erişim jetonu (süresinden önce yenilenir) →
/// <c>projects/{id}/messages:send</c>. 404 / UNREGISTERED ve jetona ait INVALID_ARGUMENT → <see cref="PushSendStatus.InvalidToken"/>;
/// 429, 5xx ve ağ hatası geçici. Jeton, gövde ve anahtar günlüğe yazılmaz.
/// </summary>
internal sealed partial class FcmPushSender(
    IHttpClientFactory httpFactory, IOptions<PushOptions> options, IClock clock, ILogger<FcmPushSender> logger) : IPushSender
{
    public const string HttpClientName = "fcm";

    private const string Scope = "https://www.googleapis.com/auth/firebase.messaging";

    private static readonly Duration TokenLifetime = Duration.FromMinutes(60);

    private static readonly Duration TokenRefreshMargin = Duration.FromMinutes(5);

    private static readonly SemaphoreSlim TokenLock = new(1, 1);

    private static (string Token, Instant ExpiresUtc)? _accessToken;

    public async Task<PushSendResult> SendAsync(PushMessage message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);
        var fcm = options.Value.Fcm;
        try
        {
            var http = httpFactory.CreateClient(HttpClientName);
            using var request = new HttpRequestMessage(HttpMethod.Post, $"https://fcm.googleapis.com/v1/projects/{fcm.ProjectId}/messages:send")
            {
                Content = JsonContent.Create(new
                {
                    message = new { token = message.Token, notification = new { title = message.Title, body = message.Body }, data = message.Data },
                }),
            };
            request.Headers.Authorization = new("Bearer", await AccessTokenAsync(http, fcm, ct));
            using var response = await http.SendAsync(request, ct);
            var text = await response.Content.ReadAsStringAsync(ct);
            if (response.IsSuccessStatusCode)
            {
                return new PushSendResult(PushSendStatus.Sent, JsonElement.Parse(text).GetProperty("name").GetString());
            }

            var invalid = response.StatusCode == HttpStatusCode.NotFound
                || text.Contains("UNREGISTERED", StringComparison.Ordinal)
                || (response.StatusCode == HttpStatusCode.BadRequest && text.Contains("registration token", StringComparison.OrdinalIgnoreCase));
            LogRejected(logger, (int)response.StatusCode);
            return new PushSendResult(invalid ? PushSendStatus.InvalidToken : PushSendStatus.Failed, FailureReason: $"fcm_{(int)response.StatusCode}");
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or JsonException && !ct.IsCancellationRequested)
        {
            LogRejected(logger, 0);
            return new PushSendResult(PushSendStatus.Failed, FailureReason: "fcm_unreachable");
        }
    }

    private async Task<string> AccessTokenAsync(HttpClient http, FcmOptions fcm, CancellationToken ct)
    {
        await TokenLock.WaitAsync(ct);
        try
        {
            var now = clock.GetCurrentInstant();
            if (_accessToken is { } cached && cached.ExpiresUtc - TokenRefreshMargin > now)
            {
                return cached.Token;
            }

            using var credentials = JsonDocument.Parse(fcm.CredentialsJson);
            var account = credentials.RootElement;
            var tokenUri = account.GetProperty("token_uri").GetString()!;
            var assertion = SignJwt(account.GetProperty("client_email").GetString()!, account.GetProperty("private_key").GetString()!, tokenUri, now);
            using var response = await http.PostAsync(tokenUri, new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                ["assertion"] = assertion,
            }), ct);
            response.EnsureSuccessStatusCode();
            using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
            var token = body.RootElement.GetProperty("access_token").GetString()!;
            _accessToken = (token, now + Duration.FromSeconds(body.RootElement.GetProperty("expires_in").GetInt32()));
            return token;
        }
        finally
        {
            TokenLock.Release();
        }
    }

    private static string SignJwt(string clientEmail, string privateKeyPem, string audience, Instant now)
    {
        var header = Base64Url("""{"alg":"RS256","typ":"JWT"}"""u8.ToArray());
        var claims = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            ["iss"] = clientEmail,
            ["scope"] = Scope,
            ["aud"] = audience,
            ["iat"] = now.ToUnixTimeSeconds(),
            ["exp"] = (now + TokenLifetime).ToUnixTimeSeconds(),
        }));
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);
        var signature = rsa.SignData(Encoding.ASCII.GetBytes($"{header}.{claims}"), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return $"{header}.{claims}.{Base64Url(signature)}";
    }

    private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    [LoggerMessage(Level = LogLevel.Warning, Message = "FCM rejected push (HTTP {StatusCode})")]
    private static partial void LogRejected(ILogger logger, int statusCode);
}
