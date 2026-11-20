using Microsoft.EntityFrameworkCore;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Infrastructure;

/// <summary>
/// Kayıtta marka kurulumu (dilim-01 § Alan kuralları). Bağlantı adı bütün markalar arasında tek olmalıdır: alınmış adlar
/// platform bağlamında okunur ve aynı anda gelen kayıtlar işlem kilidiyle sıraya girer (aynı ad iki markaya verilmez).
/// Satırlar yeni markanın kiracı bağlamında eklenir; RLS bu bağlamın dışına yazmayı engeller.
/// </summary>
internal sealed class TenantProvisioning(TenancyDbContext db, IDbSession session, IIdGenerator ids, IClock clock) : ITenantProvisioning
{
    // S-27: sahip personelinin varsayılan takvim rengi. Takvim rengi personel verisidir (sözleşme calendarColor),
    // tasarım tokenı değildir; sahip personel düzenleme ekranından değiştirebilir.
    internal const string OwnerCalendarColor = "#0F6E5C";

    private const string SlugLockKey = "tenancy:brand-slug";

    public async Task<ProvisionedTenant> ProvisionBrandAsync(
        string businessName, string industryCode, Guid ownerUserId, string? ownerFullName, CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(businessName);
        ArgumentException.ThrowIfNullOrWhiteSpace(industryCode);
        var now = clock.GetCurrentInstant();

        var slug = await ReserveSlugAsync(businessName, now, ct);

        var brandId = ids.New();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);

        var brand = Brand.Register(brandId, businessName, slug, industryCode, PlatformDefaults.DefaultCurrency.ToString(), now);
        var branch = Branch.CreateFirst(
            ids.New(), brandId, PlatformDefaults.FirstBranchName, SlugGenerator.BaseFrom(PlatformDefaults.FirstBranchName),
            PlatformDefaults.DefaultTimeZoneId, now);
        var staff = Staff.CreateOwner(ids.New(), brandId, ownerUserId, OwnerName(ownerFullName, businessName), OwnerCalendarColor, now);

        // Ebeveyn satırlar önce: aynı SaveChanges içinde yabancı anahtar sırası EF'e bırakılmaz, adımlar açık.
        db.Brands.Add(brand);
        await db.SaveChangesAsync(ct);
        db.BrandSettings.Add(Domain.BrandSettings.CreateDefault(brandId));
        db.BookingPageSettings.Add(Domain.BookingPageSettings.CreateDefault(brandId));
        db.Branches.Add(branch);
        db.Staff.Add(staff);
        await db.SaveChangesAsync(ct);
        db.StaffBranches.Add(StaffBranch.Create(ids.New(), brandId, staff.Id, branch.Id, isPrimary: true));
        await db.SaveChangesAsync(ct);

        return new ProvisionedTenant(brandId, brand.Slug, branch.Id, staff.Id);
    }

    private static string OwnerName(string? ownerFullName, string businessName)
    {
        var name = string.IsNullOrWhiteSpace(ownerFullName) ? businessName.Trim() : ownerFullName.Trim();
        return name.Length <= 120 ? name : name[..120].TrimEnd();
    }

    private async Task<Slug> ReserveSlugAsync(string businessName, Instant now, CancellationToken ct)
    {
        await session.SetTenantAsync(TenantScope.Platform, ct);
        await using (var command = new NpgsqlCommand(
            "SELECT pg_advisory_xact_lock(hashtextextended($1, 0))", session.Connection, session.Transaction))
        {
            command.Parameters.Add(new NpgsqlParameter { Value = SlugLockKey });
            await command.ExecuteNonQueryAsync(ct);
        }

        // Kilit transaction sonuna kadar tutulur; ReadCommitted'da sonraki sorgu önceki kaydın commit'ini görür.
        // Yalnız tabanla başlayan adlar okunur. 60 karaktere yakın tabanda "-n" adayı kısaltılmış tabandan üretilir;
        // o uç durumda tek ad dizini (ix_brands_slug) ikinci kaydı reddeder.
        var prefix = SlugGenerator.BaseFrom(businessName);
        var current = await db.Brands.AsNoTracking()
            .Where(b => b.Slug.StartsWith(prefix))
            .Select(b => b.Slug)
            .ToListAsync(ct);
        var redirecting = await db.SlugHistory.AsNoTracking()
            .Where(h => h.Slug.StartsWith(prefix) && h.RedirectExpiresUtc > now)
            .Select(h => h.Slug)
            .ToListAsync(ct);
        var taken = current.Concat(redirecting).ToHashSet(StringComparer.Ordinal);
        return SlugGenerator.FirstAvailable(businessName, taken.Contains);
    }
}
