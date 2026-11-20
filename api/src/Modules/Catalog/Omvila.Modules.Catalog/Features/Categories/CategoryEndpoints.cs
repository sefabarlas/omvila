using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Catalog.Domain;
using Omvila.Modules.Catalog.Infrastructure;
using Omvila.Modules.Identity.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features.Categories;

/// <summary>
/// Hizmet kategorileri (sözleşme manage-catalog; KategoriEkle). Yeni kategori ve düzenleme yalnız marka genelinde
/// yetkili (Owner) içindir: şube yöneticisi service.manage'i yalnız kendi şubesinin fiyatı için kullanır (roller-yetkiler.md ***).
/// </summary>
internal static class CategoryEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/service-categories", Audiences.Manage);
        group.MapGet(string.Empty, ListAsync).WithName("manageListServiceCategories").RequireAuthenticated(Audiences.Manage);
        group.MapPost(string.Empty, CreateAsync).WithName("manageCreateServiceCategory").RequirePermission(Permissions.Service.Manage);
        group.MapPatch("/{categoryId:guid}", UpdateAsync).WithName("manageUpdateServiceCategory").RequirePermission(Permissions.Service.Manage);
    }

    private static async Task<IResult> ListAsync(ITenantContext tenant, CatalogDbContext db, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var counts = await db.Services.AsNoTracking().Where(s => s.BrandId == brandId && s.IsActive)
            .GroupBy(s => s.CategoryId).Select(g => new { g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.Key, x => x.Count, ct);
        var items = await db.Categories.AsNoTracking().Where(c => c.BrandId == brandId)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name).ToListAsync(ct);
        return TypedResults.Ok(new CategoryList([.. items.Select(c => CategoryResponse.From(c, counts.GetValueOrDefault(c.Id)))]));
    }

    private static async Task<IResult> CreateAsync(
        CategoryWriteRequest body, HttpContext http, ITenantContext tenant, CatalogDbContext db, IIdGenerator ids, IClock clock, CancellationToken ct)
    {
        if (!BranchScope.From(http.User).All)
        {
            return Error.Forbidden().ToProblem();
        }

        var brandId = tenant.RequireBrandId();
        var errors = await ValidateAsync(body, brandId, exceptId: null, db, ct);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var sortOrder = body.SortOrder ?? (await db.Categories.Where(c => c.BrandId == brandId).MaxAsync(c => (int?)c.SortOrder, ct) ?? 0) + 1;
        var category = ServiceCategory.Create(ids.New(), brandId, body.Name!, sortOrder, clock.GetCurrentInstant());
        if (body.IsActive is false)
        {
            category.Update(category.Name, sortOrder, isActive: false);
        }

        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);
        return TypedResults.Created($"/api/v1/manage/service-categories/{category.Id}", CategoryResponse.From(category, 0));
    }

    private static async Task<IResult> UpdateAsync(
        Guid categoryId, CategoryWriteRequest body, HttpContext http, ITenantContext tenant, CatalogDbContext db, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var category = await db.Categories.SingleOrDefaultAsync(c => c.Id == categoryId && c.BrandId == brandId, ct);
        if (category is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (!BranchScope.From(http.User).All)
        {
            return Error.Forbidden().ToProblem();
        }

        var errors = await ValidateAsync(body, brandId, categoryId, db, ct);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        category.Update(body.Name!, body.SortOrder ?? category.SortOrder, body.IsActive ?? category.IsActive);
        await db.SaveChangesAsync(ct);
        var count = await db.Services.CountAsync(s => s.CategoryId == categoryId && s.IsActive, ct);
        return TypedResults.Ok(CategoryResponse.From(category, count));
    }

    // Ad marka içinde tektir; çakışma alan hatası olarak döner (T-30).
    private static async Task<Dictionary<string, string[]>> ValidateAsync(
        CategoryWriteRequest body, Guid brandId, Guid? exceptId, CatalogDbContext db, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var name = body.Name?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            errors["name"] = ["required"];
        }
        else if (name.Length > 60)
        {
            errors["name"] = ["too_long"];
        }
        else if (await db.Categories.AnyAsync(c => c.BrandId == brandId && c.Name == name && c.Id != exceptId, ct))
        {
            errors["name"] = ["already_exists"];
        }

        return errors;
    }
}
