using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;
using static Omvila.Modules.Platform.Features.AdminFinance.AdminFinanceViews;

namespace Omvila.Modules.Platform.Features.AdminFinance;

/// <summary>
/// Platform paneli finans uçları (sözleşme admin-finance): banka hareketleri listesi (platform.payments.match), elle ekleme ve CSV içe
/// aktarma (platform.billing.manage: Yönetici, Finans; Destek 403) ve abonelikler (platform.brands.view). Yazımlar denetime yazılır.
/// </summary>
internal static class AdminFinanceEndpoints
{
    private const string Entity = "PlatformBankTransaction";

    public static void Map(IEndpointRouteBuilder app)
    {
        var admin = app.MapApiGroup("/api/v1/admin", Audiences.Admin);
        admin.MapGet("/bank-transactions", ListAsync).WithName("adminListBankTransactions").RequireAdminPermission(AdminPermissions.PaymentsMatch);
        admin.MapPost("/bank-transactions", CreateAsync).WithName("adminCreateBankTransaction")
            .RequireAdminPermission(AdminPermissions.BillingManage).RequireIdempotencyKey();
        admin.MapPost("/bank-transactions/import", ImportAsync).WithName("adminImportBankTransactions")
            .RequireAdminPermission(AdminPermissions.BillingManage);
        admin.MapGet("/subscriptions", ListSubscriptionsAsync).WithName("adminListSubscriptions").RequireAdminPermission(AdminPermissions.BrandsView);
    }

    private static async Task<IResult> ListAsync(
        string? q, string? from, string? to, bool? unmatchedOnly, int? limit, string? cursor, BankTransactionBook book, CancellationToken ct) =>
        (await book.ListAsync(q, from, to, unmatchedOnly ?? false, limit, cursor, ct)).ToHttp(page => TypedResults.Ok(page));

    private static async Task<IResult> CreateAsync(
        BankTransactionCreate body, HttpContext http, BankTransactionBook book, IAuditLog audit, CancellationToken ct)
    {
        var created = await book.CreateAsync(body, AdminActor.UserId(http), ct);
        if (!created.IsSuccess)
        {
            return created.Error.ToProblem();
        }

        var view = created.Value;
        var changes = AdminActor.Changes();
        changes["source"] = view.Source.ToString();
        changes["amountMinor"] = view.Amount.AmountMinor;
        changes["valueDate"] = view.ValueDate.ToString("uuuu-MM-dd", null);
        await audit.WriteAsync(AdminActor.Entry(http, null, AuditAction.Create, Entity, view.Id, changes), ct);
        return TypedResults.Json(view, statusCode: StatusCodes.Status201Created);
    }

    // Çok parçalı form elle okunur (dosya bağlama antiforgery ister; admin uçları jetonla korunur).
    // Kural: sözleşme adminImportBankTransactions — önizleme kayıt ve denetim yazmaz; uygulama satır sayılarıyla denetime yazılır.
    private static async Task<IResult> ImportAsync(HttpContext http, BankTransactionBook book, IAuditLog audit, CancellationToken ct)
    {
        if (!http.Request.HasFormContentType)
        {
            return Error.Validation("file", "required").ToProblem();
        }

        IFormCollection form;
        try
        {
            form = await http.Request.ReadFormAsync(ct);
        }
        catch (InvalidDataException)
        {
            return Error.Validation("file", "invalid_format").ToProblem();
        }

        var result = await book.ImportAsync(form, AdminActor.UserId(http), ct);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        var r = result.Value;
        if (!r.DryRun)
        {
            var changes = AdminActor.Changes();
            changes["source"] = "Import";
            changes["totalRows"] = r.TotalRows;
            changes["imported"] = r.Imported;
            changes["duplicates"] = r.Duplicates;
            changes["skippedOutgoing"] = r.SkippedOutgoing;
            changes["invalidRows"] = r.InvalidRows;
            await audit.WriteAsync(AdminActor.Entry(http, null, AuditAction.Create, Entity, null, changes), ct);
        }

        return TypedResults.Ok(r);
    }

    private static async Task<IResult> ListSubscriptionsAsync(
        string? filter, string? q, int? limit, string? cursor, AdminSubscriptionReader reader, CancellationToken ct) =>
        (await reader.ListAsync(filter, q, limit, cursor, ct)).ToHttp(page => TypedResults.Ok(page));
}
