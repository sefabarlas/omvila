using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Features.ManageSubscription;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.AdminBrands;

/// <summary>
/// Platform paneli Genel bakış, Markalar ve marka detayı (sözleşme admin-overview, admin-brands; dilim-06 KK4, KK5). Görüntüleme
/// platform.brands.view (bütün platform rolleri); marka detayının ve her sekmenin açılışı AuditLog(View) yazar. Yazma uçları yalnız
/// platform kavramlarına dokunur (K4): kontör Notifications.Contracts, marka durumu Tenancy.Contracts, plan Platform'un kendi
/// aboneliği; her yazım denetime yazılır. Kontör yalnız PlatformAdmin (platform.credits.grant), durum ve plan yalnız PlatformAdmin
/// (platform.brands.manage); diğer rollere 403.
/// </summary>
internal static class AdminBrandEndpoints
{
    private const string BrandEntity = "Brand";
    private const string SubscriptionEntity = "Subscription";
    private const string CreditTransactionEntity = "CreditTransaction";

    public static void Map(IEndpointRouteBuilder app)
    {
        var admin = app.MapApiGroup("/api/v1/admin", Audiences.Admin);
        admin.MapGet("/overview", GetOverviewAsync).WithName("adminGetOverview").RequireAdminPermission(AdminPermissions.BrandsView);
        admin.MapGet("/brands", ListAsync).WithName("adminListBrands").RequireAdminPermission(AdminPermissions.BrandsView);
        admin.MapGet("/brands/{brandId:guid}", GetAsync).WithName("adminGetBrand").RequireAdminPermission(AdminPermissions.BrandsView);
        admin.MapGet("/brands/{brandId:guid}/invoices", ListInvoicesAsync).WithName("adminListBrandInvoices")
            .RequireAdminPermission(AdminPermissions.BrandsView);
        admin.MapGet("/brands/{brandId:guid}/credit-transactions", ListCreditsAsync).WithName("adminListBrandCreditTransactions")
            .RequireAdminPermission(AdminPermissions.BrandsView);
        admin.MapGet("/brands/{brandId:guid}/payment-notices", ListPaymentNoticesAsync).WithName("adminListBrandPaymentNotices")
            .RequireAdminPermission(AdminPermissions.BrandsView);
        admin.MapPost("/brands/{brandId:guid}/credits", GrantCreditsAsync).WithName("adminGrantCredits")
            .RequireAdminPermission(AdminPermissions.CreditsGrant).RequireIdempotencyKey();
        admin.MapPost("/brands/{brandId:guid}/status", ChangeStatusAsync).WithName("adminChangeBrandStatus")
            .RequireAdminPermission(AdminPermissions.BrandsManage).RequireIfMatch();
        admin.MapPost("/brands/{brandId:guid}/plan", ChangePlanAsync).WithName("adminChangePlan")
            .RequireAdminPermission(AdminPermissions.BrandsManage).RequireIfMatch();
    }

    private static async Task<IResult> GetOverviewAsync(AdminBrandReader reader, CancellationToken ct) =>
        TypedResults.Ok(await reader.GetOverviewAsync(ct));

    private static async Task<IResult> ListAsync(
        string? filter, string? q, int? limit, string? cursor, AdminBrandReader reader, CancellationToken ct) =>
        (await reader.ListAsync(filter, q, limit, cursor, ct)).ToHttp(page => TypedResults.Ok(page));

    private static async Task<IResult> GetAsync(Guid brandId, HttpContext http, AdminBrandReader reader, IAuditLog audit, CancellationToken ct)
    {
        var found = await reader.GetDetailAsync(brandId, ct);
        if (found is not { } detail)
        {
            return Error.NotFound().ToProblem();
        }

        // Kural: dilim-06 KK5 — her açılış denetime yazılır ("Bu görüntüleme denetim kaydına yazıldı").
        await WriteViewAsync(http, audit, brandId, "Summary", ct);
        return VersionedResults.Versioned(http, detail.Version, detail.Detail);
    }

    // Faturalar sekmesi: salonun manageListInvoices ile aynı sayfa (SubscriptionInvoicePage; T-118).
    private static Task<IResult> ListInvoicesAsync(
        Guid brandId, int? limit, string? cursor, HttpContext http, IPlatformBrandDirectory brands, SubscriptionAccount account, IAuditLog audit,
        CancellationToken ct) =>
        TabAsync(brandId, "Invoices", http, brands, audit, async () =>
        {
            var page = await account.ListInvoicesAsync(brandId, limit, cursor, ct);
            return page.IsSuccess ? TypedResults.Ok(page.Value) : page.Error.ToProblem();
        }, ct);

    // Ödemeler sekmesi: markanın havale bildirimleri (AdminPaymentNoticePage; T-118).
    private static Task<IResult> ListPaymentNoticesAsync(
        Guid brandId, int? limit, string? cursor, HttpContext http, IPlatformBrandDirectory brands, SubscriptionAccount account, IAuditLog audit,
        CancellationToken ct) =>
        TabAsync(brandId, "Payments", http, brands, audit, async () =>
        {
            var page = await account.ListPaymentNoticesAsync(brandId, limit, cursor, ct);
            return page.IsSuccess
                ? TypedResults.Ok(new AdminPage<PaymentNoticeSummary>(page.Value.Items, page.Value.NextCursor))
                : page.Error.ToProblem();
        }, ct);

    private static async Task<IResult> ListCreditsAsync(
        Guid brandId, int? limit, string? cursor, HttpContext http, IPlatformBrandDirectory brands, AdminBrandReader reader, IAuditLog audit,
        CancellationToken ct)
    {
        if (await brands.GetAsync(brandId, ct) is null)
        {
            return Error.NotFound().ToProblem();
        }

        var page = await reader.CreditPageAsync(brandId, limit, cursor, ct);
        if (!page.IsSuccess)
        {
            return page.Error.ToProblem();
        }

        await WriteViewAsync(http, audit, brandId, "Credits", ct);
        return TypedResults.Ok(page.Value);
    }

    private static async Task<IResult> GrantCreditsAsync(
        Guid brandId, AdminBrandRequests.CreditGrant body, HttpContext http, IPlatformBrandDirectory brands, IPlatformCredits credits,
        IAuditLog audit, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        // Kural: sözleşme AdminCreditGrant — -100000…100000, 0 olamaz; neden 3–200 karakter.
        if (body.Credits is not { } amount || amount == 0 || amount is < -100_000 or > 100_000)
        {
            errors["credits"] = [body.Credits is null ? "required" : "out_of_range"];
        }

        var reason = body.Reason?.Trim();
        if (reason is null || reason.Length is < 3 or > 200)
        {
            errors["reason"] = [reason is null ? "required" : "out_of_range"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        if (await brands.GetAsync(brandId, ct) is null)
        {
            return Error.NotFound().ToProblem();
        }

        var granted = await credits.GrantAsync(brandId, body.Credits!.Value, ct);
        if (!granted.IsSuccess)
        {
            return granted.Error.ToProblem();
        }

        var entry = granted.Value;
        await audit.WriteAsync(
            Entry(http, brandId, AuditAction.Create, CreditTransactionEntity, entry.Id, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["credits"] = entry.Credits,
                ["balanceAfter"] = entry.BalanceAfter,
                ["reason"] = reason,
            }),
            ct);
        return TypedResults.Json(
            new AdminBrandRequests.CreditGrantResult(AdminBrandReader.GrantRow(entry, reason!), entry.BalanceAfter), statusCode: StatusCodes.Status201Created);
    }

    private static async Task<IResult> ChangeStatusAsync(
        Guid brandId, AdminBrandRequests.StatusChange body, HttpContext http, AdminBrandReader reader, IPlatformBrandDirectory brands,
        IAuditLog audit, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.Status is not ("Active" or "Suspended"))
        {
            errors["status"] = [body.Status is null ? "required" : "invalid_format"];
        }

        var reason = body.Reason?.Trim();
        if (reason is null || reason.Length is < 3 or > 300)
        {
            errors["reason"] = [reason is null ? "required" : "out_of_range"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var current = await reader.GetDetailAsync(brandId, ct);
        if (current is not { } before)
        {
            return Error.NotFound().ToProblem();
        }

        if (before.Version != http.GetIfMatchVersion())
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var changed = await brands.ChangeStatusAsync(brandId, body.Status!, ct);
        if (!changed.IsSuccess)
        {
            return changed.Error.ToProblem();
        }

        await audit.WriteAsync(
            Entry(http, brandId, AuditAction.Update, BrandEntity, brandId, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["status"] = new[] { before.Detail.BrandStatus, body.Status },
                ["reason"] = reason,
            }),
            ct);
        return await DetailAsync(http, reader, brandId, ct);
    }

    private static async Task<IResult> ChangePlanAsync(
        Guid brandId, AdminBrandRequests.PlanChange body, HttpContext http, AdminBrandReader reader, IPlatformBrandDirectory brands,
        PlatformDbContext db, IAuditLog audit, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        BillingCycle? cycle = Enum.TryParse<BillingCycle>(body.BillingCycle, ignoreCase: false, out var parsed) && Enum.IsDefined(parsed) ? parsed : null;
        if (body.BillingCycle is not null && cycle is null)
        {
            errors["billingCycle"] = ["invalid_format"];
        }

        var reason = body.Reason?.Trim();
        if (reason is null || reason.Length is < 3 or > 300)
        {
            errors["reason"] = [reason is null ? "required" : "out_of_range"];
        }

        // Kural: sözleşme adminChangePlan — etkin olmayan (veya bilinmeyen) plan 400 errors.planCode = [invalid_format].
        var plan = string.IsNullOrWhiteSpace(body.PlanCode)
            ? null
            : await db.Plans.AsNoTracking().SingleOrDefaultAsync(p => p.Code == body.PlanCode && p.IsActive, ct);
        if (plan is null)
        {
            errors["planCode"] = [string.IsNullOrWhiteSpace(body.PlanCode) ? "required" : "invalid_format"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var current = await reader.GetDetailAsync(brandId, ct);
        var profile = await brands.GetAsync(brandId, ct);
        var subscription = await db.Subscriptions.SingleOrDefaultAsync(s => s.BrandId == brandId, ct);
        if (current is not { } before || profile is null || subscription is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (before.Version != http.GetIfMatchVersion())
        {
            return Error.PreconditionFailed().ToProblem();
        }

        if (PlanLimitError(plan!, profile.Brand) is { } limitError)
        {
            return limitError.ToProblem();
        }

        var previousPlan = before.Detail.Subscription.PlanCode;
        var previousCycle = subscription.BillingCycle;
        subscription.ChangePlan(plan!.Id, cycle ?? subscription.BillingCycle);
        await db.SaveChangesAsync(ct);
        await audit.WriteAsync(
            Entry(http, brandId, AuditAction.Update, SubscriptionEntity, subscription.Id, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["planCode"] = new[] { previousPlan, plan.Code },
                ["billingCycle"] = new[] { previousCycle.ToString(), subscription.BillingCycle.ToString() },
                ["reason"] = reason,
            }),
            ct);
        return await DetailAsync(http, reader, brandId, ct);
    }

    // Kural: sözleşme adminChangePlan — personel veya şube sayısı yeni planın sınırını aşıyorsa 409; ek şube ücretli planda şube sınırı yok.
    private static Error? PlanLimitError(Plan plan, PlatformBrandRow brand)
    {
        var staffOver = plan.StaffLimit is { } staffLimit && brand.ActiveStaffCount > staffLimit;
        var branchOver = plan.IncludedBranchCount is { } included && plan.ExtraBranchPriceMinor is null && brand.BranchCount > included;
        return staffOver || branchOver
            ? Error.Conflict(ErrorCodes.PlanLimitExceeded, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["staffCount"] = brand.ActiveStaffCount,
                ["staffLimit"] = plan.StaffLimit,
                ["branchCount"] = brand.BranchCount,
                ["includedBranchCount"] = plan.IncludedBranchCount,
            })
            : null;
    }

    // Marka yoksa 404; sayfa hatası (400) denetime yazılmaz; başarılı açılış AuditLog(View, section).
    private static async Task<IResult> TabAsync(
        Guid brandId, string section, HttpContext http, IPlatformBrandDirectory brands, IAuditLog audit, Func<Task<IResult>> read, CancellationToken ct)
    {
        if (await brands.GetAsync(brandId, ct) is null)
        {
            return Error.NotFound().ToProblem();
        }

        var result = await read();
        if (result is IStatusCodeHttpResult { StatusCode: >= 400 })
        {
            return result;
        }

        await WriteViewAsync(http, audit, brandId, section, ct);
        return result;
    }

    private static async Task<IResult> DetailAsync(HttpContext http, AdminBrandReader reader, Guid brandId, CancellationToken ct)
    {
        var after = await reader.GetDetailAsync(brandId, ct);
        return after is { } updated ? VersionedResults.Versioned(http, updated.Version, updated.Detail) : Error.NotFound().ToProblem();
    }

    // Kural: sözleşme adminGetBrand — AuditLog Action = View, EntityType = Brand, changes.section (Summary, Invoices, Credits, Payments).
    private static Task WriteViewAsync(HttpContext http, IAuditLog audit, Guid brandId, string section, CancellationToken ct) =>
        audit.WriteAsync(
            Entry(http, brandId, AuditAction.View, BrandEntity, brandId, new Dictionary<string, object?>(StringComparer.Ordinal) { ["section"] = section }),
            ct);

    private static AuditEntry Entry(
        HttpContext http, Guid brandId, AuditAction action, string entityType, Guid entityId, IReadOnlyDictionary<string, object?> changes) =>
        new(
            brandId,
            AuditActorType.PlatformUser,
            Guid.TryParse(http.User.FindFirstValue(OmvilaClaims.Subject), out var actor) ? actor : null,
            action,
            entityType,
            entityId,
            changes,
            http.Connection.RemoteIpAddress?.ToString(),
            http.Request.Headers.UserAgent.ToString());
}
