using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Infrastructure;

internal sealed class SubscriptionProvisioning(
    PlatformDbContext db, ICreditLedger credits, IIdGenerator ids, IClock clock, IOptions<PlatformOptions> options) : ISubscriptionProvisioning
{
    public async Task<StartedTrial> StartTrialAsync(Guid brandId, string timeZoneId, CancellationToken ct)
    {
        var settings = options.Value;
        var plan = await db.Plans.AsNoTracking().SingleOrDefaultAsync(p => p.Code == settings.TrialPlanCode && p.IsActive, ct)
            ?? throw new DomainException($"Deneme planı bulunamadı: {settings.TrialPlanCode} (Platform:TrialPlanCode).");

        var now = clock.GetCurrentInstant();
        var zone = DateTimeZoneProviders.Tzdb[timeZoneId];
        var today = now.InZone(zone).Date;
        var subscription = Subscription.StartTrial(ids.New(), brandId, plan.Id, today, settings.TrialDays, now);
        db.Subscriptions.Add(subscription);
        await db.SaveChangesAsync(ct);

        if (settings.TrialCredits is { } trialCredits)
        {
            await credits.GrantAsync(brandId, trialCredits, nameof(Subscription), subscription.Id, ct);
        }

        return new StartedTrial(subscription.Id, subscription.TrialEndsOn!.Value);
    }
}
