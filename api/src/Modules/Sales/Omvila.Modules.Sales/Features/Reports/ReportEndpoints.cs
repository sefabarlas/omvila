using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>
/// Raporlar ve prim (sözleşme manageGetReportOverview: report.view, Owner ve BranchManager kendi şubeleri; manageGetCommissionReport:
/// commission.view, personel yalnız kendi satırı). Kapsam dışı veya başka markanın şubesi 404.
/// </summary>
internal static class ReportEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/reports", Audiences.Manage);
        group.MapGet("/overview", GetOverviewAsync)
            .WithName("manageGetReportOverview")
            .RequirePermission(Permissions.Report.View);
        group.MapGet("/commissions", GetCommissionsAsync)
            .WithName("manageGetCommissionReport")
            .RequirePermission(Permissions.Commission.View);
    }

    private static async Task<IResult> GetOverviewAsync(
        Guid? branchId, string? from, string? to, ClaimsPrincipal user, ITenantContext tenant, ReportReader reader, CancellationToken ct)
    {
        var result = await reader.GetAsync(tenant.RequireBrandId(), branchId, BranchScope.From(user), from, to, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }

    private static async Task<IResult> GetCommissionsAsync(
        Guid? branchId,
        string? from,
        string? to,
        ClaimsPrincipal user,
        ITenantContext tenant,
        IPermissionResolver permissions,
        CommissionReportReader reader,
        CancellationToken ct)
    {
        var role = user.FindFirst(OmvilaClaims.Role)?.Value;
        Guid? ownStaffId = null;
        if (role is null || permissions.Resolve(role, Permissions.Commission.View) == PermissionScope.Own)
        {
            ownStaffId = Guid.TryParse(user.FindFirst(OmvilaClaims.StaffId)?.Value, out var id) ? id : Guid.Empty;
        }

        var result = await reader.GetAsync(tenant.RequireBrandId(), branchId, BranchScope.From(user), ownStaffId, from, to, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }
}
