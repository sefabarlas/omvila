using Microsoft.EntityFrameworkCore;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure;

/// <summary>
/// Bağlantı adlarının markalar arası görünümü (T-11). Okuma platform bağlamında yapılır ve sonunda oturumun önceki
/// bağlamı geri yüklenir. Ad seçen işlemler (kayıt, ad değişikliği) aynı işlem kilidiyle sıraya girer.
/// </summary>
internal sealed class SlugRegistry(TenancyDbContext db, IDbSession session)
{
    private const string LockKey = "tenancy:brand-slug";

    /// <summary>İşlem sonuna kadar tutulan kilit; ReadCommitted'da sonraki okuma önceki işlemin commit'ini görür.</summary>
    public async Task LockAsync(CancellationToken ct)
    {
        await using var command = new NpgsqlCommand("SELECT pg_advisory_xact_lock(hashtextextended($1, 0))", session.Connection, session.Transaction);
        command.Parameters.Add(new NpgsqlParameter { Value = LockKey });
        await command.ExecuteNonQueryAsync(ct);
    }

    /// <summary>Tabanla başlayan, şu an alınmış adlar (güncel veya yönlendirmesi süren).</summary>
    public Task<HashSet<string>> TakenWithPrefixAsync(string prefix, Instant now, CancellationToken ct) =>
        AsPlatformAsync(async () =>
        {
            var current = await db.Brands.AsNoTracking().Where(b => b.Slug.StartsWith(prefix)).Select(b => b.Slug).ToListAsync(ct);
            var redirecting = await db.SlugHistory.AsNoTracking()
                .Where(h => h.Slug.StartsWith(prefix) && h.RedirectExpiresUtc > now)
                .Select(h => h.Slug)
                .ToListAsync(ct);
            return current.Concat(redirecting).ToHashSet(StringComparer.Ordinal);
        }, ct);

    /// <summary>Bir adın bütün kullanımları (SlugAvailability girdisi).</summary>
    public Task<List<SlugUse>> UsesAsync(string slug, CancellationToken ct) =>
        AsPlatformAsync(async () =>
        {
            var uses = await db.Brands.AsNoTracking().Where(b => b.Slug == slug)
                .Select(b => new SlugUse(b.Id, true, null)).ToListAsync(ct);
            uses.AddRange(await db.SlugHistory.AsNoTracking().Where(h => h.Slug == slug)
                .Select(h => new SlugUse(h.BrandId, false, h.RedirectExpiresUtc)).ToListAsync(ct));
            return uses;
        }, ct);

    private async Task<T> AsPlatformAsync<T>(Func<Task<T>> read, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            return await read();
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }
}
