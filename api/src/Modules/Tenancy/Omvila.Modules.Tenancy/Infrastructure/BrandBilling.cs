using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Tenancy.Infrastructure;

internal sealed class BrandBilling(TenancyDbContext db) : IBrandBilling
{
    public async Task<BrandBillingProfile?> GetProfileAsync(Guid brandId, CancellationToken ct)
    {
        var brand = await db.Brands.AsNoTracking()
            .Where(b => b.Id == brandId)
            .Select(b => new { b.Name, b.LegalName, b.TaxOffice, b.TaxNumber, b.Email })
            .SingleOrDefaultAsync(ct);
        if (brand is null)
        {
            return null;
        }

        var branches = await db.Branches.CountAsync(b => b.BrandId == brandId && b.IsActive, ct);
        var staff = await db.Staff.CountAsync(s => s.BrandId == brandId && s.IsActive, ct);
        return new BrandBillingProfile(brandId, brand.Name, brand.LegalName, brand.TaxOffice, brand.TaxNumber, brand.Email, branches, staff);
    }

    // Kural: durum-makinesi.md § 8 Cancelled ("30 gün sonra arşiv (Brand.Status = Archived)")
    public async Task<bool> ArchiveAsync(Guid brandId, CancellationToken ct)
    {
        var brand = await db.Brands.SingleOrDefaultAsync(b => b.Id == brandId, ct);
        if (brand is null || !brand.Archive())
        {
            return false;
        }

        await db.SaveChangesAsync(ct);
        return true;
    }
}
