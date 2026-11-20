using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Features.Brand;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>
/// Personel uçları (sözleşme manage-staff; Personel, PersonelEkle; kabul kriteri 10). Okuma şube kapsamıyla süzülür;
/// yazmak staff.manage ister ve şube yöneticisi yalnız kendi şubelerinde, yalnız Staff rolüyle çalışır.
/// </summary>
internal static class StaffEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/staff", Audiences.Manage);
        group.MapGet(string.Empty, ListAsync).WithName("manageListStaff").RequireAuthenticated(Audiences.Manage);
        group.MapPost(string.Empty, (StaffCreateRequest body, HttpContext http, StaffWriter writer, CancellationToken ct) => writer.CreateAsync(body, http, ct))
            .WithName("manageCreateStaff").RequirePermission(Permissions.Staff.Manage).RequireIdempotencyKey();
        group.MapGet("/{staffId:guid}", GetAsync).WithName("manageGetStaff").RequireAuthenticated(Audiences.Manage);
        group.MapPatch("/{staffId:guid}", (Guid staffId, JsonElement body, HttpContext http, StaffWriter writer, CancellationToken ct) => writer.UpdateAsync(staffId, new JsonBody(body), http, ct))
            .WithName("manageUpdateStaff").RequirePermission(Permissions.Staff.Manage).RequireIfMatch();
    }

    private static async Task<IResult> ListAsync(
        Guid? branchId, bool? includeInactive, ClaimsPrincipal user, ITenantContext tenant, StaffReader reader, TenancyDbContext db, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var scope = BranchScope.From(user);
        if (branchId is { } b && (!scope.Includes(b) || !await db.Branches.AnyAsync(x => x.Id == b && x.BrandId == brandId, ct)))
        {
            return Error.NotFound().ToProblem();
        }

        return TypedResults.Ok(new StaffList(await reader.ListAsync(brandId, scope, branchId, includeInactive is true, ct)));
    }

    private static async Task<IResult> GetAsync(Guid staffId, HttpContext http, ITenantContext tenant, StaffReader reader, CancellationToken ct)
    {
        var staff = await reader.FindAsync(tenant.RequireBrandId(), staffId, http.User, track: false, ct);
        return staff is null ? Error.NotFound().ToProblem() : VersionedResults.Versioned(http, staff.Version, await reader.DetailAsync(staff, ct));
    }
}
