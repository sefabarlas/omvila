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

/// <summary>Sözleşme PublicReviewInvite eşlemesi (marka bağlamında): salon ve şube, personel, müşterinin görünen adı.</summary>
internal sealed class ReviewInviteViews(IPublicBookingQueries booking, ITenantQueries tenants, ICustomerDirectory customers)
{
    public async Task<PublicReviewInvite?> MapAsync(AppointmentReviewSubject subject, Review? review, CancellationToken ct)
    {
        var card = await booking.GetBranchCardAsync(subject.BrandId, subject.BranchId, ct);
        if (card is null)
        {
            return null;
        }

        PublicReviewStaff? staff = null;
        if (subject.StaffId is { } staffId
            && (await tenants.GetStaffCardsAsync(subject.BrandId, [staffId], ct)).TryGetValue(staffId, out var person))
        {
            staff = new PublicReviewStaff(person.DisplayName ?? person.FullName, person.PhotoUrl);
        }

        var customer = (await customers.GetDisplayInfoAsync(subject.BrandId, [subject.BrandCustomerId], ct)).GetValueOrDefault(subject.BrandCustomerId);
        return new PublicReviewInvite(
            new PublicReviewBrand(card.BrandName, card.BrandSlug, card.LogoUrl),
            card.BranchName,
            subject.StartUtc,
            card.TimeZoneId,
            subject.ServiceNames,
            staff,
            PublicReviewEndpoints.DisplayName(customer?.FullName ?? string.Empty),
            review is not null,
            review is null ? null : new PublicSubmittedReview(review.Rating, review.Comment, review.CreatedUtc));
    }
}
