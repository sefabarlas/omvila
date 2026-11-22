using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Reviews.Domain;
using Omvila.Modules.Reviews.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Features.MeReviews;

/// <summary>
/// Müşteri uygulamasından değerlendirme (sözleşme meCreateReview; Degerlendir; dilim-05 KK8). publicCreateReview ile aynı kurallar:
/// yalnız tamamlanan randevu ve bir kez (409 already_reviewed, Completed değilse 409 invalid_state_transition); kimlik erişim kodu
/// yerine me jetonunun sub'ıdır, başka müşterinin randevusu 404.
/// </summary>
internal static class MeReviewEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/me/reviews", Audiences.Me);
        group.MapPost(string.Empty, CreateAsync).WithName("meCreateReview").RequireAuthenticated(Audiences.Me).RequireIdempotencyKey();
    }

    // Kural: alan-modeli.md § Review (yalnız Completed; bir kez → already_reviewed); sözleşme meCreateReview
    private static async Task<IResult> CreateAsync(
        MeReviewCreate body,
        ITenantContext tenant,
        IAppointmentReviewLinks links,
        ReviewsDbContext db,
        IOutbox outbox,
        IIdGenerator ids,
        IClock clock,
        CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.AppointmentId is null)
        {
            errors["appointmentId"] = ["required"];
        }

        if (body.Rating is not { } rating)
        {
            errors["rating"] = ["required"];
        }
        else if (!Review.IsValidRating(rating))
        {
            errors["rating"] = ["out_of_range"];
        }

        if (Review.Clean(body.Comment) is { Length: > Review.CommentMaxLength })
        {
            errors["comment"] = ["too_long"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var customerId = tenant.Scope.CustomerId ?? throw new DomainException("me isteğinde müşteri bağlamı yok.");
        var subject = await links.EnterForCustomerAsync(customerId, body.AppointmentId!.Value, ct);
        if (subject is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (await db.Reviews.AnyAsync(r => r.BrandId == subject.BrandId && r.AppointmentId == subject.AppointmentId, ct))
        {
            return Error.Conflict(ErrorCodes.AlreadyReviewed).ToProblem();
        }

        if (!subject.IsCompleted)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition).ToProblem();
        }

        var review = Review.Submit(
            ids.New(), subject.BrandId, subject.BranchId, subject.AppointmentId, subject.BrandCustomerId, subject.StaffId, subject.ServiceNames,
            body.Rating!.Value, body.Comment, clock.GetCurrentInstant());
        db.Reviews.Add(review);
        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(
            new ReviewReceived(review.BrandId, review.Id, review.AppointmentId, review.BranchId, review.BrandCustomerId, review.Rating), ct);
        return TypedResults.Created((string?)null, new MeReview(review.Id, review.AppointmentId, review.Rating, review.Comment, review.CreatedUtc));
    }
}
