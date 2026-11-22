using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Infrastructure;

internal sealed class SubscriptionProvisioning(
    PlatformDbContext db,
    IDbSession session,
    IBrandBilling brands,
    ICreditLedger credits,
    IIdGenerator ids,
    IClock clock,
    IOptions<PlatformOptions> options) : ISubscriptionProvisioning
{
    private const int CodeAttempts = 20;
    private const int ShortCodeDigits = 4;
    private const int LongCodeDigits = 6;

    public async Task<StartedTrial> StartTrialAsync(Guid brandId, string timeZoneId, CancellationToken ct)
    {
        var settings = options.Value;
        var plan = await db.Plans.AsNoTracking().SingleOrDefaultAsync(p => p.Code == settings.TrialPlanCode && p.IsActive, ct)
            ?? throw new DomainException($"Deneme planı bulunamadı: {settings.TrialPlanCode} (Platform:TrialPlanCode).");

        var now = clock.GetCurrentInstant();
        var zone = DateTimeZoneProviders.Tzdb[timeZoneId];
        var today = now.InZone(zone).Date;
        var profile = await brands.GetProfileAsync(brandId, ct);
        var codes = await NewTransferCodesAsync(profile?.Name, ct);
        var subscription = Subscription.StartTrial(ids.New(), brandId, plan.Id, today, settings.TrialDays, codes, now);
        db.Subscriptions.Add(subscription);
        await db.SaveChangesAsync(ct);

        if (settings.TrialCredits is { } trialCredits)
        {
            await credits.GrantAsync(brandId, trialCredits, nameof(Subscription), subscription.Id, ct);
        }

        return new StartedTrial(subscription.Id, subscription.TrialEndsOn!.Value);
    }

    /// <summary>
    /// Benzersiz ABN-/KNT- çifti. Diğer markaların kodlarını görmek için denetim platform bağlamında yapılır (RLS); kalan küçük
    /// yarış penceresini benzersiz indeks kapatır. Dört rakam tükenirse altı rakama geçilir.
    /// </summary>
    private async Task<TransferCodes> NewTransferCodesAsync(string? brandName, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            for (var attempt = 0; attempt < CodeAttempts; attempt++)
            {
                var codes = TransferCodes.Create(brandName, attempt < CodeAttempts / 2 ? ShortCodeDigits : LongCodeDigits);
                var taken = await db.Subscriptions.AnyAsync(
                    s => s.TransferCode == codes.Subscription || s.CreditTransferCode == codes.Credits, ct);
                if (!taken)
                {
                    return codes;
                }
            }
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }

        throw new DomainException("Benzersiz havale kodu üretilemedi.");
    }
}
