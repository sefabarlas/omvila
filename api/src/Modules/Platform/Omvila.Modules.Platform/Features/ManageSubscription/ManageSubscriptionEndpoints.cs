using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>
/// Ayarlar → Abonelik ve fatura uçları (sözleşme manage-subscription). Hepsi subscription.manage ister (yalnız Owner; diğer roller
/// 403). Salt okunur ve gecikmede de açıktır: salon ödemeyi buradan yapar. Kart akışı (T-06) yoktur.
/// </summary>
internal static class ManageSubscriptionEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var subscription = app.MapApiGroup("/api/v1/manage/subscription", Audiences.Manage);
        subscription.MapGet(string.Empty, GetAsync).WithName("manageGetSubscription").RequirePermission(Permissions.Subscription.Manage);
        subscription.MapGet("/invoices", ListInvoicesAsync).WithName("manageListInvoices").RequirePermission(Permissions.Subscription.Manage);
        subscription.MapGet("/invoices/{invoiceId:guid}/pdf", GetInvoicePdfAsync).WithName("manageGetInvoicePdf")
            .RequirePermission(Permissions.Subscription.Manage);
        subscription.MapPost("/payment-notices", NotifyTransferAsync).WithName("manageNotifyTransfer")
            .RequirePermission(Permissions.Subscription.Manage).RequireIdempotencyKey();

        var credits = app.MapApiGroup("/api/v1/manage", Audiences.Manage);
        credits.MapGet("/credit-packages", ListCreditPackagesAsync).WithName("manageListCreditPackages")
            .RequirePermission(Permissions.Subscription.Manage);
        credits.MapPost("/credits/purchase", PurchaseCreditsAsync).WithName("managePurchaseCredits")
            .RequirePermission(Permissions.Subscription.Manage);
    }

    private static async Task<IResult> GetAsync(ITenantContext tenant, SubscriptionAccount account, CancellationToken ct) =>
        await account.GetDetailAsync(tenant.RequireBrandId(), ct) is { } detail ? TypedResults.Ok(detail) : Error.NotFound().ToProblem();

    private static async Task<IResult> ListInvoicesAsync(
        int? limit, string? cursor, ITenantContext tenant, SubscriptionAccount account, CancellationToken ct)
    {
        var page = await account.ListInvoicesAsync(tenant.RequireBrandId(), limit, cursor, ct);
        return page.ToHttp(p => TypedResults.Ok(p));
    }

    private static async Task<IResult> GetInvoicePdfAsync(
        Guid invoiceId, HttpContext http, ITenantContext tenant, SubscriptionAccount account, CancellationToken ct)
    {
        if (await account.GetInvoicePdfAsync(tenant.RequireBrandId(), invoiceId, ct) is not { } pdf)
        {
            return Error.NotFound().ToProblem();
        }

        http.Response.Headers.CacheControl = "private, no-store";
        return TypedResults.File(pdf.Content, "application/pdf", pdf.FileName);
    }

    // Çok parçalı form elle okunur: dosya bağlama (IFormFile parametresi) antiforgery ister; panel uçları jetonla korunur.
    private static async Task<IResult> NotifyTransferAsync(
        HttpContext http, ITenantContext tenant, PaymentNoticeIntake intake, CancellationToken ct)
    {
        if (!http.Request.HasFormContentType)
        {
            return Error.Validation("purpose", "required").ToProblem();
        }

        IFormCollection form;
        try
        {
            form = await http.Request.ReadFormAsync(ct);
        }
        catch (InvalidDataException)
        {
            // Boş veya bozuk çok parçalı gövde: zorunlu alan yok.
            return Error.Validation("purpose", "required").ToProblem();
        }

        var channel = string.Equals(http.Request.Headers["X-Client-Type"], "web", StringComparison.OrdinalIgnoreCase)
            ? PaymentNoticeChannel.Panel
            : PaymentNoticeChannel.App;
        var result = await intake.NotifyAsync(tenant.RequireBrandId(), form, channel, UserId(http.User), ct);
        return result.ToHttp(n => TypedResults.Created((string?)null, n));
    }

    private static async Task<IResult> ListCreditPackagesAsync(SubscriptionAccount account, CancellationToken ct) =>
        TypedResults.Ok(await account.ListCreditPackagesAsync(ct));

    private static async Task<IResult> PurchaseCreditsAsync(
        CreditPurchaseRequest? body, ITenantContext tenant, SubscriptionAccount account, CancellationToken ct)
    {
        var result = await account.PurchaseInstructionsAsync(tenant.RequireBrandId(), body, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }

    private static Guid? UserId(ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var id) ? id : null;
}
