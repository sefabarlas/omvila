using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NodaTime;
using NodaTime.Text;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.AdminAudit;

/// <summary>
/// Denetim kaydı (sözleşme admin-audit; PDenetim "Tüm platform işlemleri · silinemez"). Görüntüleme platform.audit.view (bütün platform
/// rolleri), CSV dışa aktarma platform.audit.export (yalnız Yönetici); dışa aktarma da denetime yazılır (Export, AuditLog).
/// </summary>
internal static class AdminAuditEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var admin = app.MapApiGroup("/api/v1/admin", Audiences.Admin);
        admin.MapGet("/audit-log", ListAsync).WithName("adminListAuditLog").RequireAdminPermission(AdminPermissions.AuditView);
        admin.MapGet("/audit-log/export", ExportAsync).WithName("adminExportAuditLog").RequireAdminPermission(AdminPermissions.AuditExport);
    }

    private static async Task<IResult> ListAsync(
        string? category, string? q, string? brandId, string? fromUtc, string? toUtc, int? limit, string? cursor, AuditLogReader reader,
        CancellationToken ct) =>
        (await reader.ListAsync(new AuditLogReader.AuditQuery(category, q, brandId, fromUtc, toUtc), limit, cursor, ct))
            .ToHttp(page => TypedResults.Ok(page));

    private static async Task<IResult> ExportAsync(
        string? category, string? q, string? brandId, string? fromUtc, string? toUtc, HttpContext http, AuditLogReader reader, IAuditLog audit,
        IClock clock, CancellationToken ct)
    {
        var exported = await reader.ExportAsync(new AuditLogReader.AuditQuery(category, q, brandId, fromUtc, toUtc), ct);
        if (!exported.IsSuccess)
        {
            return exported.Error.ToProblem();
        }

        var (content, rows, filter) = exported.Value;
        var changes = AdminActor.Changes();
        changes["category"] = filter.Category;
        changes["fromUtc"] = InstantPattern.ExtendedIso.Format(filter.FromUtc);
        changes["toUtc"] = filter.ToUtc is { } to ? InstantPattern.ExtendedIso.Format(to) : null;
        changes["rows"] = rows;
        await audit.WriteAsync(AdminActor.Entry(http, filter.BrandId, AuditAction.Export, "AuditLog", null, changes), ct);
        var today = clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId]).Date;
        http.Response.Headers.CacheControl = "private, no-store";
        return TypedResults.File(content, "text/csv; charset=utf-8", $"denetim-kaydi-{LocalDatePattern.Iso.Format(today)}.csv");
    }
}
