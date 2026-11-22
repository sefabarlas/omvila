using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Files;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;
using static Omvila.Modules.Platform.Features.AdminApprovals.AdminApprovalViews;

namespace Omvila.Modules.Platform.Features.AdminApprovals;

/// <summary>
/// Platform paneli onay kuyrukları (sözleşme admin-approvals; dilim-06 KK2, KK3). Havale bildirimi işlemleri platform.payments.match
/// (bütün platform rolleri; Destek eşleştirebilir), SMS gönderen adı kararı platform.sms-senders.decide (Yönetici, Destek). Her yazım
/// ve bildirim/dekont açılışı denetime yazılır. Gönderen adı Notifications.Contracts <see cref="IPlatformSmsSenders"/> ile yazılır (K4).
/// </summary>
internal static class AdminApprovalEndpoints
{
    private const string NoticeEntity = nameof(PaymentNotice);
    private const string SenderEntity = "BrandSmsSender";
    private const int MessageMin = 3;
    private const int MessageMax = 300;
    private const int ReasonMax = 200;

    public static void Map(IEndpointRouteBuilder app)
    {
        var admin = app.MapApiGroup("/api/v1/admin", Audiences.Admin);
        admin.MapGet("/approval-queue", QueueAsync).WithName("adminListApprovalQueue").RequireAdminPermission(AdminPermissions.PaymentsMatch);
        admin.MapGet("/payment-notices/{noticeId:guid}", GetAsync).WithName("adminGetPaymentNotice").RequireAdminPermission(AdminPermissions.PaymentsMatch);
        admin.MapGet("/payment-notices/{noticeId:guid}/receipt", ReceiptAsync).WithName("adminGetPaymentNoticeReceipt")
            .RequireAdminPermission(AdminPermissions.PaymentsMatch);
        admin.MapPost("/payment-notices/{noticeId:guid}/match", MatchAsync).WithName("adminMatchPaymentNotice")
            .RequireAdminPermission(AdminPermissions.PaymentsMatch).RequireIfMatch();
        admin.MapPost("/payment-notices/{noticeId:guid}/not-found", NotFoundAsync).WithName("adminMarkPaymentNoticeNotFound")
            .RequireAdminPermission(AdminPermissions.PaymentsMatch).RequireIfMatch();
        admin.MapPost("/payment-notices/{noticeId:guid}/reject", RejectAsync).WithName("adminRejectPaymentNotice")
            .RequireAdminPermission(AdminPermissions.PaymentsMatch).RequireIfMatch();
        admin.MapPost("/sms-senders/{senderId:guid}/approve", ApproveSenderAsync).WithName("adminApproveSmsSender")
            .RequireAdminPermission(AdminPermissions.SmsSendersDecide).RequireIfMatch();
        admin.MapPost("/sms-senders/{senderId:guid}/reject", RejectSenderAsync).WithName("adminRejectSmsSender")
            .RequireAdminPermission(AdminPermissions.SmsSendersDecide).RequireIfMatch();
    }

    // Kural: sözleşme adminListApprovalQueue — SMS başlığı satırları yalnız platform.sms-senders.decide izninde (Finans'ta sayı 0, satır yok).
    private static async Task<IResult> QueueAsync(
        string? type, HttpContext http, PaymentNoticeReview review, IPlatformSmsSenders senders, IPlatformBrandDirectory brands,
        IAdminPermissionResolver permissions, CancellationToken ct)
    {
        if (type is not (null or "PaymentNotice" or "SmsSender"))
        {
            return Error.Validation("type", "invalid_format").ToProblem();
        }

        var role = http.User.FindFirst(OmvilaClaims.PlatformRole)?.Value;
        var canDecide = role is not null && permissions.HasPermission(role, AdminPermissions.SmsSendersDecide);
        var notices = await review.QueueAsync(ct);
        var pendingSenders = canDecide ? await senders.ListPendingAsync(ct) : [];
        var names = pendingSenders.Count == 0 ? [] : (await brands.ListAsync(ct)).ToDictionary(b => b.Id, b => b.Name);
        var senderItems = pendingSenders.Select(s => new AdminApprovalQueueItem(
            "SmsSender", s.SenderId, s.BrandId, names.GetValueOrDefault(s.BrandId) ?? string.Empty, s.SubmittedUtc, null,
            new QueuedSmsSender(s.RequestedName, s.CurrentEffectiveName, ETagVersion.Format(s.Version))));
        var all = notices.Concat(senderItems).ToList();
        var items = all
            .Where(i => type is null || string.Equals(i.Type, type, StringComparison.Ordinal))
            .OrderBy(i => i.SubmittedUtc).ThenBy(i => i.Id)
            .ToList();
        return TypedResults.Ok(new AdminApprovalQueue(
            notices.Count, pendingSenders.Count, all.Count == 0 ? null : all.Min(i => i.SubmittedUtc), items));
    }

    private static async Task<IResult> GetAsync(Guid noticeId, HttpContext http, PaymentNoticeReview review, IAuditLog audit, CancellationToken ct)
    {
        var detail = await review.GetAsync(noticeId, ct);
        if (detail is null)
        {
            return Error.NotFound().ToProblem();
        }

        // Kural: sözleşme adminGetPaymentNotice — açılış denetime yazılır (View, PaymentNotice).
        await audit.WriteAsync(AdminActor.Entry(http, detail.BrandId, AuditAction.View, NoticeEntity, detail.Id, AdminActor.Changes()), ct);
        http.Response.Headers.ETag = detail.Version;
        return TypedResults.Ok(detail);
    }

    // Kural: sözleşme adminGetPaymentNoticeReceipt — Cache-Control: private, no-store; indirme denetime yazılır (View, section Receipt).
    private static async Task<IResult> ReceiptAsync(
        Guid noticeId, HttpContext http, PaymentNoticeReview review, IFileStorage files, IAuditLog audit, CancellationToken ct)
    {
        var notice = await review.FindAsync(noticeId, ct);
        if (notice?.ReceiptFileKey is not { } key || await files.OpenReadAsync(key, ct) is not { } content)
        {
            return Error.NotFound().ToProblem();
        }

        var changes = AdminActor.Changes();
        changes["section"] = "Receipt";
        await audit.WriteAsync(AdminActor.Entry(http, notice.BrandId, AuditAction.View, NoticeEntity, notice.Id, changes), ct);
        http.Response.Headers.CacheControl = "private, no-store";
        return TypedResults.Stream(
            content, notice.ReceiptContentType ?? "application/octet-stream", notice.ReceiptFileName ?? $"dekont{Path.GetExtension(key)}");
    }

    private static async Task<IResult> MatchAsync(
        Guid noticeId, PaymentNoticeMatchRequest body, HttpContext http, PaymentNoticeReview review, CancellationToken ct)
    {
        if (body.BankTransactionId is not { } transactionId)
        {
            return Error.Validation("bankTransactionId", "required").ToProblem();
        }

        var result = await review.MatchAsync(noticeId, transactionId, http.GetIfMatchVersion(), AdminActor.UserId(http), ct);
        return Detail(http, result);
    }

    private static async Task<IResult> NotFoundAsync(
        Guid noticeId, PaymentNoticeResolutionRequest? body, HttpContext http, PaymentNoticeReview review, CancellationToken ct)
    {
        var message = body?.Message?.Trim();
        if (message is { Length: < MessageMin or > MessageMax })
        {
            return Error.Validation("message", "out_of_range").ToProblem();
        }

        return await ResolveAsync(noticeId, PaymentNoticeStatus.NotFound, message, http, review, ct);
    }

    // Kural: sözleşme adminRejectPaymentNotice — message zorunlu (errors.message = [required]).
    private static async Task<IResult> RejectAsync(
        Guid noticeId, PaymentNoticeResolutionRequest? body, HttpContext http, PaymentNoticeReview review, CancellationToken ct)
    {
        var message = body?.Message?.Trim();
        if (string.IsNullOrEmpty(message))
        {
            return Error.Validation("message", "required").ToProblem();
        }

        if (message.Length is < MessageMin or > MessageMax)
        {
            return Error.Validation("message", "out_of_range").ToProblem();
        }

        return await ResolveAsync(noticeId, PaymentNoticeStatus.Rejected, message, http, review, ct);
    }

    private static async Task<IResult> ResolveAsync(
        Guid noticeId, PaymentNoticeStatus outcome, string? message, HttpContext http, PaymentNoticeReview review, CancellationToken ct)
    {
        var template = AdminActor.Entry(http, null, AuditAction.Update, NoticeEntity, noticeId, AdminActor.Changes());
        return Detail(http, await review.ResolveAsync(noticeId, outcome, message, http.GetIfMatchVersion(), template, ct));
    }

    private static Task<IResult> ApproveSenderAsync(
        Guid senderId, HttpContext http, IPlatformSmsSenders senders, IAuditLog audit, CancellationToken ct) =>
        DecideAsync(senderId, true, null, http, senders, audit, ct);

    private static async Task<IResult> RejectSenderAsync(
        Guid senderId, SmsSenderRejectionRequest? body, HttpContext http, IPlatformSmsSenders senders, IAuditLog audit, CancellationToken ct)
    {
        var reason = body?.Reason?.Trim();
        if (string.IsNullOrEmpty(reason))
        {
            return Error.Validation("reason", "required").ToProblem();
        }

        if (reason.Length is < MessageMin or > ReasonMax)
        {
            return Error.Validation("reason", "out_of_range").ToProblem();
        }

        return await DecideAsync(senderId, false, reason, http, senders, audit, ct);
    }

    // Kural: sözleşme adminApproveSmsSender (Approve, BrandSmsSender) ve adminRejectSmsSender (Update, BrandSmsSender).
    private static async Task<IResult> DecideAsync(
        Guid senderId, bool approve, string? reason, HttpContext http, IPlatformSmsSenders senders, IAuditLog audit, CancellationToken ct)
    {
        var decided = await senders.DecideAsync(senderId, approve, reason, http.GetIfMatchVersion(), ct);
        if (!decided.IsSuccess)
        {
            return decided.Error.ToProblem();
        }

        var d = decided.Value;
        var changes = AdminActor.Changes();
        changes["status"] = d.Status;
        changes["smsHeader"] = d.Name;
        await audit.WriteAsync(
            AdminActor.Entry(http, d.BrandId, approve ? AuditAction.Approve : AuditAction.Update, SenderEntity, d.SenderId, changes), ct);
        return TypedResults.Ok(new AdminSmsSenderDecision(d.SenderId, d.BrandId, d.Name, d.Status, d.DecidedUtc, d.RejectionReason));
    }

    private static IResult Detail(HttpContext http, Result<AdminPaymentNoticeDetail> result)
    {
        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        http.Response.Headers.ETag = result.Value.Version;
        return TypedResults.Ok(result.Value);
    }
}
