using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Yenileme jetonları (ADR-0007): 256 bit rastgele, yalnız SHA-256 özeti saklanır, her kullanımda döndürülür ve
/// ömür baştan başlar. Kullanılmış jeton yeniden gelirse aile iptal edilir; bu iptal ayrı transaction'da commit
/// edilir (istek 401 döndüğü için kendi transaction'ı geri alınır). İstek transaction'ı içinde çağrılır.
/// </summary>
internal sealed class RefreshTokenService(
    IdentityDbContext db,
    IClock clock,
    IIdGenerator ids,
    IOptions<AuthOptions> options,
    IServiceScopeFactory scopes)
{
    public const string ReuseReason = "reuse";
    public const string LogoutReason = "logout";

    public async Task<IssuedRefreshToken> IssueAsync(
        Guid userId, Guid? membershipId, RefreshClientType clientType, bool rememberDevice, CancellationToken ct)
    {
        var issued = Add(userId, membershipId, ids.New(), clientType, rememberDevice);
        await db.SaveChangesAsync(ct);
        return issued;
    }

    /// <summary>Jetonu kullanır ve aynı ailede yenisini verir. Geçersiz, süresi dolmuş veya iptal edilmişse 401.</summary>
    public async Task<Result<IssuedRefreshToken>> RotateAsync(string token, CancellationToken ct)
    {
        var hash = Hash(token);
        var current = await db.RefreshTokens
            .FromSql($"SELECT * FROM identity.refresh_tokens WHERE token_hash = {hash} FOR UPDATE")
            .SingleOrDefaultAsync(ct);
        if (current is null)
        {
            return Error.Unauthorized();
        }

        var now = clock.GetCurrentInstant();
        if (current.UsedUtc is not null)
        {
            // Tekrar kullanım: jeton çalınmış olabilir; zincirin bütün geçerli jetonları düşer.
            await RevokeFamilyIndependentlyAsync(current.FamilyId, exceptId: current.Id, ct);
            return Error.Unauthorized();
        }

        if (!current.IsUsable(now))
        {
            return Error.Unauthorized();
        }

        current.MarkUsed(now);
        var next = Add(current.UserId, current.MembershipId, current.FamilyId, current.ClientType, current.RememberDevice);
        await db.SaveChangesAsync(ct);
        return next;
    }

    /// <summary>Çıkış: jetonun ailesini iptal eder. Bilinmeyen jeton sessizce yok sayılır.</summary>
    public async Task RevokeFamilyAsync(string token, CancellationToken ct)
    {
        var hash = Hash(token);
        var familyId = await db.RefreshTokens.Where(t => t.TokenHash == hash).Select(t => (Guid?)t.FamilyId).SingleOrDefaultAsync(ct);
        if (familyId is null)
        {
            return;
        }

        var now = clock.GetCurrentInstant();
        await db.RefreshTokens
            .Where(t => t.FamilyId == familyId && t.RevokedUtc == null)
            .ExecuteUpdateAsync(set => set.SetProperty(t => t.RevokedUtc, now).SetProperty(t => t.RevokedReason, LogoutReason), ct);
    }

    internal static string Hash(string token) =>
        Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private IssuedRefreshToken Add(Guid userId, Guid? membershipId, Guid familyId, RefreshClientType clientType, bool rememberDevice)
    {
        var settings = options.Value;
        var days = clientType == RefreshClientType.Mobile || rememberDevice
            ? settings.RefreshTokenDaysRemembered
            : settings.RefreshTokenDaysSession;
        var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        var entity = RefreshToken.Create(
            ids.New(), userId, membershipId, familyId, Hash(token), clientType, rememberDevice, clock.GetCurrentInstant(), Duration.FromDays(days));
        db.RefreshTokens.Add(entity);
        return new IssuedRefreshToken(token, entity.ExpiresUtc, userId, membershipId, rememberDevice);
    }

    // İstek transaction'ı tekrar kullanılan satırı kilitli tutar; o satır dışarıda bırakılır (zaten kullanılmış,
    // yeniden geçemez). Diğerleri ayrı transaction'da iptal edilir ve istek geri alınsa da kalır.
    private async Task RevokeFamilyIndependentlyAsync(Guid familyId, Guid exceptId, CancellationToken ct)
    {
        await using var scope = scopes.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(ct);
        var now = clock.GetCurrentInstant();
        await scope.ServiceProvider.GetRequiredService<IdentityDbContext>().RefreshTokens
            .Where(t => t.FamilyId == familyId && t.Id != exceptId && t.RevokedUtc == null)
            .ExecuteUpdateAsync(set => set.SetProperty(t => t.RevokedUtc, now).SetProperty(t => t.RevokedReason, ReuseReason), ct);
        await session.CommitAsync(ct);
    }
}
