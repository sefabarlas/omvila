using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Security;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Identity.IntegrationTests;

// Kural: ADR-0007 (yenileme jetonu: yalnız özet, her kullanımda döndürme, tekrar kullanımda aile iptali),
// sureler-ve-ayarlar.md RefreshTokenLifetimeDays (30 hatırla / 1 hatırlama; mobil her zaman 30, kayan)
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class RefreshTokenTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid User = Guid.Parse("0193a8f0-0000-7000-8000-000000000001");
    private static readonly Guid Membership = Guid.Parse("0193a8f0-0000-7000-8000-000000000002");

    private ServiceProvider _services = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private FakeClock Clock => (FakeClock)_services.GetRequiredService<IClock>();

    public async ValueTask InitializeAsync()
    {
        await using (var connection = await db.OpenMigratorConnectionAsync(Ct))
        {
            await using var command = new NpgsqlCommand("TRUNCATE identity.refresh_tokens", connection);
            await command.ExecuteNonQueryAsync(Ct);
        }

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(db.Settings()).Build();
        _services = db.CreateServices(configure: services => services.AddIdentityModule(configuration));
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task Issue_StoresOnlyHash_WithRememberedLifetime()
    {
        var issued = await IssueAsync(RefreshClientType.Web, remember: true);

        issued.ExpiresUtc.ShouldBe(TestClock.Start + Duration.FromDays(30));
        (await ScalarAsync<long>($"SELECT count(*) FROM identity.refresh_tokens WHERE token_hash = '{RefreshTokenService.Hash(issued.Token)}'")).ShouldBe(1);
        (await ScalarAsync<long>($"SELECT count(*) FROM identity.refresh_tokens WHERE token_hash = '{issued.Token}'")).ShouldBe(0);
    }

    [Fact]
    public async Task Issue_WithoutRemember_OnWeb_UsesSessionLifetime() =>
        (await IssueAsync(RefreshClientType.Web, remember: false)).ExpiresUtc.ShouldBe(TestClock.Start + Duration.FromDays(1));

    [Fact]
    public async Task Rotate_ReturnsNewTokenInSameFamily_AndOldCannotBeUsedAgain()
    {
        var first = await IssueAsync(RefreshClientType.Web, remember: true);

        var second = (await RotateAsync(first.Token, commit: true)).Value;

        string.Equals(second.Token, first.Token, StringComparison.Ordinal).ShouldBeFalse();
        second.UserId.ShouldBe(User);
        second.MembershipId.ShouldBe(Membership);
        (await ScalarAsync<long>("SELECT count(DISTINCT family_id) FROM identity.refresh_tokens")).ShouldBe(1);
    }

    [Fact]
    public async Task Reuse_RevokesWholeFamily_EvenThoughRequestTransactionRollsBack()
    {
        var first = await IssueAsync(RefreshClientType.Web, remember: true);
        var second = (await RotateAsync(first.Token, commit: true)).Value;

        // Çalınmış eski jeton yeniden geliyor: istek 401 döner ve kendi transaction'ı geri alınır.
        var reuse = await RotateAsync(first.Token, commit: false);

        reuse.IsSuccess.ShouldBeFalse();
        reuse.Error.Code.ShouldBe(ErrorCodes.Unauthorized);
        (await RotateAsync(second.Token, commit: true)).IsSuccess.ShouldBeFalse("zincirin geçerli jetonu da iptal edildi");
        (await ScalarAsync<string>($"SELECT revoked_reason FROM identity.refresh_tokens WHERE token_hash = '{RefreshTokenService.Hash(second.Token)}'"))
            .ShouldBe(RefreshTokenService.ReuseReason);
    }

    [Fact]
    public async Task ExpiredToken_IsRejected()
    {
        var session = await IssueAsync(RefreshClientType.Web, remember: false);
        Clock.Advance(Duration.FromDays(1));

        (await RotateAsync(session.Token, commit: true)).IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task MobileToken_LifetimeSlidesOnEveryRotation()
    {
        var token = await IssueAsync(RefreshClientType.Mobile, remember: false);
        Clock.Advance(Duration.FromDays(29));

        var rotated = (await RotateAsync(token.Token, commit: true)).Value;

        rotated.ExpiresUtc.ShouldBe(Clock.GetCurrentInstant() + Duration.FromDays(30));
    }

    [Fact]
    public async Task UnknownToken_IsRejected() =>
        (await RotateAsync("bilinmeyen-jeton", commit: true)).Error!.Code.ShouldBe(ErrorCodes.Unauthorized);

    [Fact]
    public async Task Logout_RevokesFamily()
    {
        var first = await IssueAsync(RefreshClientType.Web, remember: true);
        var second = (await RotateAsync(first.Token, commit: true)).Value;

        await InSessionAsync(service => service.RevokeFamilyAsync(second.Token, Ct), commit: true);

        (await RotateAsync(second.Token, commit: true)).IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task ConcurrentRotationOfSameToken_OneSucceeds_AndFamilyIsRevoked()
    {
        var first = await IssueAsync(RefreshClientType.Web, remember: true);

        var results = await Task.WhenAll(RotateAsync(first.Token, commit: true), RotateAsync(first.Token, commit: true));

        results.Count(r => r.IsSuccess).ShouldBe(1);
        var winner = results.Single(r => r.IsSuccess).Value;
        (await RotateAsync(winner.Token, commit: true)).IsSuccess.ShouldBeFalse("tekrar kullanım zinciri düşürür (ADR-0007)");
    }

    private Task<IssuedRefreshToken> IssueAsync(RefreshClientType client, bool remember) =>
        InSessionAsync(service => service.IssueAsync(User, Membership, client, remember, Ct), commit: true);

    private Task<Result<IssuedRefreshToken>> RotateAsync(string token, bool commit) =>
        InSessionAsync(service => service.RotateAsync(token, Ct), commit);

    private async Task<T> InSessionAsync<T>(Func<RefreshTokenService, Task<T>> work, bool commit)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(Ct);
        var result = await work(scope.ServiceProvider.GetRequiredService<RefreshTokenService>());
        if (commit && result is not Result<IssuedRefreshToken> { IsSuccess: false })
        {
            await session.CommitAsync(Ct);
        }
        else
        {
            await session.RollbackAsync(Ct);
        }

        return result;
    }

    private async Task InSessionAsync(Func<RefreshTokenService, Task> work, bool commit) =>
        await InSessionAsync(async service =>
        {
            await work(service);
            return 0;
        }, commit);

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }
}
