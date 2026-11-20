using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;

namespace Omvila.Modules.Tenancy.Features.Onboarding;

/// <summary>
/// Kurulum adımları (dilim-01 § Alan kuralları). Her adımı verisinin sahibi olan modülün sağlayıcısı değerlendirir;
/// sağlayıcısı henüz olmayan adım (ör. Services, Catalog kalıcılığı gelene kadar) tamamlanmamış sayılır.
/// Hepsi tamamlanınca marka Onboarding → Active olur; değerlendirme adımlar okunurken ve olay kaydedilince yapılır.
/// </summary>
internal sealed class OnboardingService(TenancyDbContext db, IEnumerable<IOnboardingStepProvider> providers, IClock clock)
{
    public async Task<IReadOnlyList<(OnboardingStep Step, bool Done)>> EvaluateAsync(Guid brandId, CancellationToken ct)
    {
        var byStep = providers.ToLookup(p => p.Step);
        var steps = new List<(OnboardingStep, bool)>();
        foreach (var step in Enum.GetValues<OnboardingStep>())
        {
            var done = false;
            foreach (var provider in byStep[step])
            {
                done |= await provider.IsDoneAsync(brandId, ct);
            }

            steps.Add((step, done));
        }

        if (steps.TrueForAll(s => s.Item2))
        {
            var brand = await db.Brands.SingleAsync(b => b.Id == brandId, ct);
            if (brand.CompleteOnboarding())
            {
                await db.SaveChangesAsync(ct);
            }
        }

        return steps;
    }

    public async Task RecordAsync(Guid brandId, OnboardingEvent onboardingEvent, CancellationToken ct)
    {
        var row = await db.Onboarding.SingleOrDefaultAsync(o => o.BrandId == brandId, ct);
        if (row is null)
        {
            row = BrandOnboarding.Create(brandId);
            db.Onboarding.Add(row);
        }

        row.Record(onboardingEvent, clock.GetCurrentInstant());
        await db.SaveChangesAsync(ct);
        await EvaluateAsync(brandId, ct);
    }
}
