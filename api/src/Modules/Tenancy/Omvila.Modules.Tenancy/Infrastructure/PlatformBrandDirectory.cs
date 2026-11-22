using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Infrastructure;

/// <summary>
/// <see cref="IPlatformBrandDirectory"/>: platform okuma servisi (ADR-0006). Admin isteği platform bağlamındadır (RLS bütün
/// markaları gösterir); başka bağlamda çağrı programlama hatasıdır.
/// </summary>
internal sealed class PlatformBrandDirectory(TenancyDbContext db, ITenantContext tenant, PublicBookingQueries booking) : IPlatformBrandDirectory
{
    public async Task<IReadOnlyList<PlatformBrandRow>> ListAsync(CancellationToken ct)
    {
        RequirePlatformScope();
        var brands = await db.Brands.AsNoTracking().Where(b => b.Status != BrandStatus.Archived).ToListAsync(ct);
        return await RowsAsync(brands, ct);
    }

    public async Task<PlatformBrandProfile?> GetAsync(Guid brandId, CancellationToken ct)
    {
        RequirePlatformScope();
        var brand = await db.Brands.AsNoTracking().SingleOrDefaultAsync(b => b.Id == brandId, ct);
        if (brand is null)
        {
            return null;
        }

        var row = (await RowsAsync([brand], ct))[0];
        var branches = await db.Branches.AsNoTracking()
            .Where(b => b.BrandId == brandId && b.IsActive)
            .OrderBy(b => b.CreatedUtc)
            .Select(b => new BranchSummary(b.Id, b.Name))
            .ToListAsync(ct);
        return new PlatformBrandProfile(row, branches, booking.BookingPageUrl(brand.Slug));
    }

    public async Task<Result> ChangeStatusAsync(Guid brandId, string status, CancellationToken ct)
    {
        RequirePlatformScope();
        var brand = await db.Brands.SingleOrDefaultAsync(b => b.Id == brandId, ct);
        if (brand is null)
        {
            return Error.NotFound();
        }

        if (!Enum.TryParse<BrandStatus>(status, ignoreCase: false, out var target))
        {
            return Error.Validation("status", "invalid_format");
        }

        var changed = brand.ChangeStatusByPlatform(target);
        if (!changed.IsSuccess)
        {
            return changed;
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<List<PlatformBrandRow>> RowsAsync(List<Brand> brands, CancellationToken ct)
    {
        var ids = brands.Select(b => b.Id).ToList();
        var branches = (await db.Branches.AsNoTracking()
                .Where(b => ids.Contains(b.BrandId) && b.IsActive)
                .OrderBy(b => b.CreatedUtc)
                .Select(b => new { b.BrandId, b.City })
                .ToListAsync(ct))
            .ToLookup(b => b.BrandId);
        var staff = await db.Staff.AsNoTracking()
            .Where(s => ids.Contains(s.BrandId) && s.IsActive)
            .GroupBy(s => s.BrandId)
            .Select(g => new { BrandId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.BrandId, g => g.Count, ct);
        var deposit = (await db.BankAccounts.AsNoTracking()
                .Where(a => ids.Contains(a.BrandId) && a.IsActive)
                .Select(a => a.BrandId)
                .ToListAsync(ct))
            .ToHashSet();
        var hours = (await db.BranchHours.AsNoTracking()
                .Where(h => ids.Contains(h.BrandId))
                .Select(h => h.BrandId)
                .Distinct()
                .ToListAsync(ct))
            .ToHashSet();
        return [.. brands.Select(b => new PlatformBrandRow(
            b.Id,
            b.Name,
            b.Slug,
            b.IndustryCode,
            b.Status.ToString(),
            branches[b.Id].Select(x => x.City).FirstOrDefault(c => !string.IsNullOrWhiteSpace(c)),
            b.CreatedUtc,
            branches[b.Id].Count(),
            staff.GetValueOrDefault(b.Id),
            deposit.Contains(b.Id),
            hours.Contains(b.Id),
            b.Version))];
    }

    private void RequirePlatformScope()
    {
        if (!tenant.Scope.IsPlatform)
        {
            throw new DomainException("Platform okuma servisi yalnız platform bağlamında çağrılır (ADR-0006).");
        }
    }
}
