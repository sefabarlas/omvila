using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Reviews.Domain;
using Omvila.Modules.Reviews.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Features.PublicReviews;

/// <summary>
/// Değerlendirme bağlantısı (sözleşme public-review: publicGetReviewInvite, publicCreateReview; review.invite SMS'indeki
/// <c>/d/{code}</c>). Kimlik erişim kodudur (alan-modeli.md § AppointmentAccessCode, Purpose Review). Yalnız tamamlanan randevu
/// ve bir kez değerlendirilir (dilim-05 kabul kriteri 8).
/// </summary>
internal static class PublicReviewEndpoints
{
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/public/reviews");

        // Üyeliksiz (ürün kuralı 7): yetki 10 karakterlik kriptografik rastgele erişim kodudur (62^10), jeton yoktur.
        group.MapGet("/{accessCode}", GetAsync).WithName("publicGetReviewInvite").AllowAnonymous();

        // Üyeliksiz: erişim kodu randevuya yetki verir; değerlendirme kaynak oluşturduğu için Idempotency-Key ister.
        group.MapPost("/{accessCode}", CreateAsync).WithName("publicCreateReview").AllowAnonymous().RequireIdempotencyKey();
    }

    private static async Task<IResult> GetAsync(
        string accessCode, IAppointmentReviewLinks links, ReviewsDbContext db, ReviewInviteViews views, CancellationToken ct)
    {
        var subject = await links.EnterByReviewCodeAsync(accessCode, ct);
        if (subject is null)
        {
            return InvalidCode();
        }

        var review = await db.Reviews.AsNoTracking().SingleOrDefaultAsync(r => r.BrandId == subject.BrandId && r.AppointmentId == subject.AppointmentId, ct);
        var view = await views.MapAsync(subject, review, ct);
        return view is null ? InvalidCode() : TypedResults.Ok(view);
    }

    // Kural: alan-modeli.md § Review (yalnız Completed; bir kez → already_reviewed); sözleşme publicCreateReview
    private static async Task<IResult> CreateAsync(
        string accessCode,
        PublicReviewCreate body,
        IAppointmentReviewLinks links,
        ReviewsDbContext db,
        ReviewInviteViews views,
        IOutbox outbox,
        IIdGenerator ids,
        IClock clock,
        CancellationToken ct)
    {
        var errors = Validate(body);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var subject = await links.EnterByReviewCodeAsync(accessCode, ct);
        if (subject is null)
        {
            return InvalidCode();
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

        var view = await views.MapAsync(subject, review, ct) ?? throw new DomainException("Değerlendirme formu okunamadı.");
        return TypedResults.Created((string?)null, view);
    }

    private static Dictionary<string, string[]> Validate(PublicReviewCreate? body)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body?.Rating is not { } rating)
        {
            errors["rating"] = ["required"];
        }
        else if (!Review.IsValidRating(rating))
        {
            errors["rating"] = ["out_of_range"];
        }

        if (Review.Clean(body?.Comment) is { Length: > Review.CommentMaxLength })
        {
            errors["comment"] = ["too_long"];
        }

        return errors;
    }

    // Sözleşme: kod geçersiz veya süresi dolmuşsa 404 access_code_invalid (hangi kodun var olduğu sızmaz).
    private static IResult InvalidCode() => Error.NotFound(ErrorCodes.AccessCodeInvalid).ToProblem();

    /// <summary>Yorumda görünecek ad: ad + soyadın baş harfi ("Merve Y."; alan-modeli.md § Review).</summary>
    internal static string DisplayName(string fullName)
    {
        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length switch
        {
            0 => string.Empty,
            1 => parts[0],
            _ => $"{string.Join(' ', parts[..^1])} {char.ToUpper(parts[^1][0], Turkish)}.",
        };
    }
}

