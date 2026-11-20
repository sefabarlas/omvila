using NodaTime;
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
internal sealed class TenantProvisioning(TenancyDbContext db, IDbSession session, SlugRegistry slugs, IIdGenerator ids, IClock clock) : ITenantProvisioning
{
    // S-27: sahip personelinin varsayılan takvim rengi. Takvim rengi personel verisidir (sözleşme calendarColor),
    // tasarım tokenı değildir; sahip personel düzenleme ekranından değiştirebilir.
    internal const string OwnerCalendarColor = "#0F6E5C";

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
        await slugs.LockAsync(ct);

        // Yalnız tabanla başlayan adlar okunur. 60 karaktere yakın tabanda "-n" adayı kısaltılmış tabandan üretilir;
        // o uç durumda tek ad dizini (ix_brands_slug) ikinci kaydı reddeder.
        var taken = await slugs.TakenWithPrefixAsync(SlugGenerator.BaseFrom(businessName), now, ct);
        return SlugGenerator.FirstAvailable(businessName, taken.Contains);
    }
}
