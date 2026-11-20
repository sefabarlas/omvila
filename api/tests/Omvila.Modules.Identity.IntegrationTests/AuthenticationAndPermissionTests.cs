using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using NodaTime.Testing;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Security;
using Omvila.TestKit;

namespace Omvila.Modules.Identity.IntegrationTests;

// Kural: ADR-0007 (aud, ES256, 15 dk), roller-yetkiler.md (izin tabanlı yetki, 403), ADR-0006 (kiracı jetondan),
// api-kurallari.md (401 unauthorized / token_expired)
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class AuthenticationAndPermissionTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid User = Guid.Parse("0193a8f0-0000-7000-8000-000000000001");
    private static readonly Guid Membership = Guid.Parse("0193a8f0-0000-7000-8000-000000000002");

    private WebApplication _app = null!;
    private HttpClient _client = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        _app = await TestApiHost.StartAsync(
            db,
            MapEndpoints,
            services => services.AddIdentityModule(new ConfigurationBuilder().AddInMemoryCollection(db.Settings()).Build()),
            app =>
            {
                app.UseAuthentication();
                app.UseAuthorization();
            },
            audience: Audiences.Manage);
        _client = _app.GetTestClient();
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
    }

    [Fact]
    public async Task NoToken_Returns401Unauthorized()
    {
        var (status, body) = await GetAsync("/api/test/settings", token: null);

        status.ShouldBe(401);
        body.GetProperty("code").GetString().ShouldBe("unauthorized");
    }

    [Fact]
    public async Task OwnerToken_IsAuthorized_AndTenantComesFromToken()
    {
        var (status, body) = await GetAsync("/api/test/settings", Issue(Roles.Owner));

        status.ShouldBe(200);
        body.GetProperty("brandId").GetGuid().ShouldBe(Brand);
    }

    [Fact]
    public async Task StaffToken_WithoutPermission_Returns403Forbidden_ButWithPermissionPasses()
    {
        var staff = Issue(Roles.Staff);

        var (forbidden, body) = await GetAsync("/api/test/settings", staff);
        var (allowed, _) = await GetAsync("/api/test/calendar", staff);

        forbidden.ShouldBe(403);
        body.GetProperty("code").GetString().ShouldBe("forbidden");
        allowed.ShouldBe(200);
    }

    [Fact]
    public async Task ExpiredToken_Returns401TokenExpired()
    {
        var token = Issue(Roles.Owner);
        ((FakeClock)_app.Services.GetRequiredService<IClock>()).Advance(Duration.FromMinutes(15));

        var (status, body) = await GetAsync("/api/test/settings", token);

        status.ShouldBe(401);
        body.GetProperty("code").GetString().ShouldBe("token_expired");
    }

    [Fact]
    public async Task TamperedToken_Returns401()
    {
        var token = Issue(Roles.Staff);
        var parts = token.Split('.');
        var payload = Base64UrlEncoder.Decode(parts[1]).Replace("\"Staff\"", "\"Owner\"", StringComparison.Ordinal);
        var tampered = $"{parts[0]}.{Base64UrlEncoder.Encode(payload)}.{parts[2]}";

        (await GetAsync("/api/test/settings", tampered)).Status.ShouldBe(401);
    }

    [Fact]
    public async Task TokenSignedWithUnknownKey_Returns401()
    {
        using var other = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        (await GetAsync("/api/test/settings", Craft(Audiences.Manage, new ECDsaSecurityKey(other) { KeyId = TestSigningKey.Kid })))
            .Status.ShouldBe(401);
    }

    [Fact]
    public async Task TokenForAnotherSurface_Returns401()
    {
        using var key = ECDsa.Create();
        key.ImportFromPem(TestSigningKey.PrivateKeyPem);

        (await GetAsync("/api/test/settings", Craft(Audiences.Admin, new ECDsaSecurityKey(key) { KeyId = TestSigningKey.Kid })))
            .Status.ShouldBe(401);
    }

    [Fact]
    public async Task AnonymousEndpoint_NeedsNoToken() =>
        (await GetAsync("/api/test/open", token: null)).Status.ShouldBe(200);

    [Fact]
    public void EndpointAudit_FindsEndpointsWithoutPermissionOrAnonymousMarker() =>
        EndpointAudit.FindUnmarkedApiEndpoints(_app.Services).ShouldBe(["/api/test/unmarked"]);

    private string Issue(string role) =>
        _app.Services.GetRequiredService<AccessTokenIssuer>()
            .IssueManage(new ManageTokenClaims(User, Brand, Membership, role, null, null)).Token;

    private string Craft(string audience, SecurityKey key)
    {
        var now = _app.Services.GetRequiredService<IClock>().GetCurrentInstant();
        return new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false }.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = "omvila",
            Audience = audience,
            Subject = new ClaimsIdentity([new Claim(OmvilaClaims.Subject, User.ToString()), new Claim(OmvilaClaims.BrandId, Brand.ToString()), new Claim(OmvilaClaims.Role, Roles.Owner)]),
            IssuedAt = now.ToDateTimeUtc(),
            NotBefore = now.ToDateTimeUtc(),
            Expires = (now + Duration.FromMinutes(15)).ToDateTimeUtc(),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.EcdsaSha256),
        });
    }

    private async Task<(int Status, JsonElement Body)> GetAsync(string path, string? token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        using var response = await _client.SendAsync(request, Ct);
        var text = await response.Content.ReadAsStringAsync(Ct);
        return ((int)response.StatusCode, text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone());
    }

    private static void MapEndpoints(RouteGroupBuilder api)
    {
        api.MapGet("/settings", (IDbSession session) => TypedResults.Ok(new { brandId = session.Scope.BrandId }))
            .RequirePermission(Permissions.Settings.Manage);
        api.MapGet("/calendar", () => TypedResults.Ok()).RequirePermission(Permissions.Calendar.View);
        // Deneme: açık uç örneği.
        api.MapGet("/open", () => TypedResults.Ok()).AllowAnonymous();
        api.MapGet("/unmarked", () => TypedResults.Ok());
    }
}
