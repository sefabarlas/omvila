using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Reviews.Domain;
using Omvila.Modules.Reviews.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

/// <summary>
/// Panel değerlendirme uçları (sözleşme manage-reviews: manageListReviews, manageReplyReview; manage-customers:
/// manageListCustomerReviews). review.view / review.reply: Owner tam, BranchManager kendi şubesinde, personel 403 (roller-yetkiler.md).
/// Kapsam dışı şube veya değerlendirme 404.
/// </summary>
internal static class ManageReviewEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var reviews = app.MapApiGroup("/api/v1/manage/reviews", Audiences.Manage);
        reviews.MapGet(string.Empty, ListAsync).WithName("manageListReviews").RequirePermission(Permissions.Review.View);
        reviews.MapPost("/{reviewId:guid}/reply", ReplyAsync).WithName("manageReplyReview").RequirePermission(Permissions.Review.Reply);

        var customers = app.MapApiGroup("/api/v1/manage/customers", Audiences.Manage);
        customers.MapGet("/{brandCustomerId:guid}/reviews", ListForCustomerAsync).WithName("manageListCustomerReviews")
            .RequirePermission(Permissions.Review.View);
    }

    private static async Task<IResult> ListAsync(
        Guid? branchId, string? filter, int? limit, string? cursor, ClaimsPrincipal user, ITenantContext tenant, ReviewReader reader, CancellationToken ct)
    {
        var result = await reader.ListAsync(tenant.RequireBrandId(), branchId, filter, limit, cursor, BranchScope.From(user), ct);
        return result.ToHttp(page => TypedResults.Ok(page));
    }

    private static async Task<IResult> ListForCustomerAsync(
        Guid brandCustomerId,
        int? limit,
        string? cursor,
        ClaimsPrincipal user,
        ITenantContext tenant,
        ICustomerDirectory customers,
        ReviewReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        if (!(await customers.GetDisplayInfoAsync(brandId, [brandCustomerId], ct)).ContainsKey(brandCustomerId))
        {
            return Error.NotFound().ToProblem();
        }

        var result = await reader.ListForCustomerAsync(brandId, brandCustomerId, limit, cursor, BranchScope.From(user), ct);
        return result.ToHttp(page => TypedResults.Ok(page));
    }

    // Kural: sözleşme manageReplyReview (yanıt üzerine yazılır, müşteriye bildirim gitmez); roller-yetkiler.md review.reply (Ş)
    private static async Task<IResult> ReplyAsync(
        Guid reviewId,
        ReviewReplyRequest body,
        ClaimsPrincipal user,
        ITenantContext tenant,
        ReviewsDbContext db,
        ReviewReader reader,
        ITenantQueries tenants,
        IClock clock,
        CancellationToken ct)
    {
        // Kayıt ve kapsam önce: başka markanın kimliği gövdeden bağımsız 404.
        var brandId = tenant.RequireBrandId();
        var review = await db.Reviews.SingleOrDefaultAsync(r => r.BrandId == brandId && r.Id == reviewId, ct);
        if (review is null || !BranchScope.From(user).Includes(review.BranchId))
        {
            return Error.NotFound().ToProblem();
        }

        var text = Review.Clean(body?.Text);
        string? invalid = text switch
        {
            null => "required",
            { Length: < Review.ReplyMinLength } => "too_short",
            { Length: > Review.ReplyMaxLength } => "too_long",
            _ => null,
        };
        if (invalid is not null)
        {
            return Error.Validation("text", invalid).ToProblem();
        }

        var userId = Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var u) ? u : (Guid?)null;
        string? name = null;
        if (Guid.TryParse(user.FindFirst(OmvilaClaims.StaffId)?.Value, out var staffId)
            && (await tenants.GetStaffCardsAsync(brandId, [staffId], ct)).TryGetValue(staffId, out var card))
        {
            name = card.FullName;
        }

        review.Reply(text!, userId, name, clock.GetCurrentInstant());
        await db.SaveChangesAsync(ct);
        return TypedResults.Ok((await reader.MapAsync(brandId, [review], ct))[0]);
    }
}
