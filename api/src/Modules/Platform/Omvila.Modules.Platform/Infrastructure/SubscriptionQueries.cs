using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure;

internal sealed class SubscriptionQueries(PlatformDbContext db, IOptions<PlatformOptions> options) : ISubscriptionQueries
{
    public async Task<SubscriptionBannerInfo?> GetBannerAsync(Guid brandId, CancellationToken ct)
    {
        var subscription = await db.Subscriptions.AsNoTracking().SingleOrDefaultAsync(s => s.BrandId == brandId, ct);
        if (subscription is null)
        {
            return null;
        }

        var readOnlyOn = subscription.Status == SubscriptionStatus.PastDue && subscription.PastDueSince is { } since
            ? since.PlusDays(options.Value.PastDueGraceDays)
            : (NodaTime.LocalDate?)null;
        return new SubscriptionBannerInfo(subscription.Status.ToString(), subscription.TrialEndsOn, subscription.PastDueSince, readOnlyOn);
    }

    public async Task<PlanLimitInfo?> GetPlanLimitsAsync(Guid brandId, CancellationToken ct) =>
        await (from s in db.Subscriptions.AsNoTracking()
               join p in db.Plans.AsNoTracking() on s.PlanId equals p.Id
               where s.BrandId == brandId
               select new PlanLimitInfo(p.StaffLimit, p.IncludedBranchCount, p.ExtraBranchPriceMinor != null || p.IncludedBranchCount == null))
            .SingleOrDefaultAsync(ct);
}
