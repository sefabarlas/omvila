using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;
using static Omvila.Modules.Platform.Features.AdminPlans.AdminPlanViews;

namespace Omvila.Modules.Platform.Features.AdminPlans;

/// <summary>
/// Planlar ve fiyat (sözleşme admin-plans; dilim-06 KK4). Görme platform.plans.view (bütün platform rolleri), kaydetme
/// platform.plans.manage (Yönetici, Finans; Destek 403). Her değişen plan veya paket denetime yazılır (Update, Plan / CreditPackage).
/// </summary>
internal static class AdminPlanEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var admin = app.MapApiGroup("/api/v1/admin", Audiences.Admin);
        admin.MapGet("/plans", ListPlansAsync).WithName("adminListPlans").RequireAdminPermission(AdminPermissions.PlansView);
        admin.MapPut("/plans", UpdatePlansAsync).WithName("adminUpdatePlans").RequireAdminPermission(AdminPermissions.PlansManage).RequireIfMatch();
        admin.MapGet("/credit-packages", ListPackagesAsync).WithName("adminListCreditPackages").RequireAdminPermission(AdminPermissions.PlansView);
        admin.MapPut("/credit-packages", UpdatePackagesAsync).WithName("adminUpdateCreditPackages")
            .RequireAdminPermission(AdminPermissions.PlansManage).RequireIfMatch();
    }

    private static async Task<IResult> ListPlansAsync(HttpContext http, PlanCatalog catalog, CancellationToken ct)
    {
        var (body, version) = await catalog.GetPlansAsync(ct);
        return VersionedResults.Versioned(http, version, body);
    }

    private static async Task<IResult> ListPackagesAsync(HttpContext http, PlanCatalog catalog, CancellationToken ct)
    {
        var (body, version) = await catalog.GetPackagesAsync(ct);
        return VersionedResults.Versioned(http, version, body);
    }

    private static async Task<IResult> UpdatePlansAsync(
        AdminPlanCatalogUpdate body, HttpContext http, PlanCatalog catalog, IAuditLog audit, CancellationToken ct)
    {
        var updated = await catalog.UpdatePlansAsync(body, http.GetIfMatchVersion(), ct);
        if (!updated.IsSuccess)
        {
            return updated.Error.ToProblem();
        }

        await WriteAuditAsync(http, audit, "Plan", updated.Value, ct);
        return await ListPlansAsync(http, catalog, ct);
    }

    private static async Task<IResult> UpdatePackagesAsync(
        AdminCreditPackageCatalogUpdate body, HttpContext http, PlanCatalog catalog, IAuditLog audit, CancellationToken ct)
    {
        var updated = await catalog.UpdatePackagesAsync(body, http.GetIfMatchVersion(), ct);
        if (!updated.IsSuccess)
        {
            return updated.Error.ToProblem();
        }

        await WriteAuditAsync(http, audit, "CreditPackage", updated.Value, ct);
        return await ListPackagesAsync(http, catalog, ct);
    }

    private static async Task WriteAuditAsync(
        HttpContext http, IAuditLog audit, string entity, IReadOnlyList<(Guid Id, Dictionary<string, object?> Changes)> changed, CancellationToken ct)
    {
        foreach (var (id, changes) in changed)
        {
            await audit.WriteAsync(AdminActor.Entry(http, null, AuditAction.Update, entity, id, changes), ct);
        }
    }
}
