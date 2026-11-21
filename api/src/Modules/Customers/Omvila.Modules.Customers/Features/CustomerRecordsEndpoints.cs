using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Features.Customers;
using Omvila.Modules.Customers.Features.Deletion;
using Omvila.Modules.Customers.Features.Export;
using Omvila.Modules.Customers.Features.Imports;
using Omvila.Modules.Customers.Features.Merge;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features;

/// <summary>
/// Müşteri kayıt yönetimi uçları (sözleşme manage-customers; Dilim 5, T-92): birleştirme (customer.merge: Owner, BranchManager),
/// CSV içe aktarma (customer.import) ve dışa aktarma (customer.export), KVKK silme ve silme talepleri (customer.kvkk); son
/// üçü yalnız Owner (roller-yetkiler.md).
/// </summary>
internal static class CustomerRecordsEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var customers = app.MapApiGroup("/api/v1/manage/customers", Audiences.Manage);
        customers.MapPost("/{brandCustomerId:guid}/merge", MergeAsync).WithName("manageMergeCustomers").RequirePermission(Permissions.Customer.Merge);
        customers.MapPost("/imports", PreviewImportAsync).WithName("managePreviewImport")
            .RequirePermission(Permissions.Customer.Import).RequireIdempotencyKey();
        customers.MapPost("/imports/{importId:guid}/commit", CommitImportAsync).WithName("manageCommitImport").RequirePermission(Permissions.Customer.Import);
        customers.MapGet("/export", ExportAsync).WithName("manageExportCustomers").RequirePermission(Permissions.Customer.Export);
        customers.MapGet("/{brandCustomerId:guid}/deletion", DeletionPreviewAsync).WithName("manageGetCustomerDeletionPreview")
            .RequirePermission(Permissions.Customer.Kvkk);
        customers.MapPost("/{brandCustomerId:guid}/deletion", RequestDeletionAsync).WithName("manageRequestCustomerDeletion")
            .RequirePermission(Permissions.Customer.Kvkk);

        var requests = app.MapApiGroup("/api/v1/manage/customer-deletion-requests", Audiences.Manage);
        requests.MapGet(string.Empty, ListDeletionRequestsAsync).WithName("manageListDeletionRequests").RequirePermission(Permissions.Customer.Kvkk);
    }

    private static async Task<IResult> MergeAsync(
        Guid brandCustomerId,
        CustomerMergeRequest body,
        HttpContext http,
        ITenantContext tenant,
        CustomerMerger merger,
        CustomerReader reader,
        ITenantQueries tenants,
        IPermissionResolver permissions,
        IAppointmentQueries appointments,
        IClock clock,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var merged = await merger.MergeAsync(brandId, brandCustomerId, body, AuditActor.From(http), ct);
        if (!merged.IsSuccess)
        {
            return merged.Error.ToProblem();
        }

        var row = await reader.FindAsync(brandId, brandCustomerId, ct) ?? throw new DomainException("Birleştirilen müşteri okunamadı.");
        var view = await CustomerViewContext.LoadAsync(brandId, http.User, tenants, permissions, clock, ct);
        var items = await CustomerEndpoints.WithNextAppointmentAsync(brandId, [row.ToResponse(view)], appointments, clock, ct);
        return TypedResults.Ok(items[0]);
    }

    // Çok parçalı form elle okunur: dosya bağlama (IFormFile parametresi) antiforgery ister; panel uçları jetonla korunur.
    private static async Task<IResult> PreviewImportAsync(
        HttpContext http, ITenantContext tenant, CustomerImporter importer, ITenantQueries tenants, IPermissionResolver permissions, IClock clock, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        if (!http.Request.HasFormContentType)
        {
            return Error.Validation("file", "required").ToProblem();
        }

        var form = await http.Request.ReadFormAsync(ct);
        var view = await CustomerViewContext.LoadAsync(brandId, http.User, tenants, permissions, clock, ct);
        var preview = await importer.PreviewAsync(brandId, form.Files.GetFile("file"), view.Today.Year, AuditActor.From(http), ct);
        return preview.ToHttp(p => TypedResults.Created($"/api/v1/manage/customers/imports/{p.ImportId}", p));
    }

    private static async Task<IResult> CommitImportAsync(
        Guid importId, CustomerImportCommitRequest? body, HttpContext http, ITenantContext tenant, CustomerImporter importer, CancellationToken ct)
    {
        var result = await importer.CommitAsync(tenant.RequireBrandId(), importId, body, AuditActor.From(http), ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }

    private static async Task<IResult> ExportAsync(
        string? filter,
        HttpContext http,
        ITenantContext tenant,
        CustomerExporter exporter,
        ITenantQueries tenants,
        IPermissionResolver permissions,
        IClock clock,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var view = await CustomerViewContext.LoadAsync(brandId, http.User, tenants, permissions, clock, ct);
        var export = await exporter.ExportAsync(brandId, filter, view, AuditActor.From(http), ct);
        return export.ToHttp(e => TypedResults.File(e.Content, "text/csv; charset=utf-8", e.FileName));
    }

    private static async Task<IResult> DeletionPreviewAsync(Guid brandCustomerId, ITenantContext tenant, CustomerEraser eraser, CancellationToken ct)
    {
        var preview = await eraser.PreviewAsync(tenant.RequireBrandId(), brandCustomerId, ct);
        return preview is null ? Error.NotFound().ToProblem() : TypedResults.Ok(preview);
    }

    private static async Task<IResult> RequestDeletionAsync(
        Guid brandCustomerId,
        CustomerDeletionRequest body,
        HttpContext http,
        ITenantContext tenant,
        CustomerEraser eraser,
        ITenantQueries tenants,
        IPermissionResolver permissions,
        IClock clock,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var view = await CustomerViewContext.LoadAsync(brandId, http.User, tenants, permissions, clock, ct);
        var result = await eraser.EraseAsync(brandId, brandCustomerId, body, view.Today, AuditActor.From(http), ct);
        return result.ToHttp(r => TypedResults.Ok(DeletionRequestReader.ToView(r)));
    }

    private static async Task<IResult> ListDeletionRequestsAsync(
        string? status, int? limit, string? cursor, ITenantContext tenant, DeletionRequestReader reader, CancellationToken ct)
    {
        var page = await reader.ListAsync(tenant.RequireBrandId(), status, limit, cursor, ct);
        return page.ToHttp(p => TypedResults.Ok(p));
    }
}
