using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Infrastructure;

namespace Omvila.Modules.Tenancy.Features.Onboarding;

/// <summary>Personel: sahip dışında en az bir etkin personel veya "tek başıma çalışıyorum" (dilim-01 § Alan kuralları).</summary>
internal sealed class StaffStep(TenancyDbContext db) : IOnboardingStepProvider
{
    public OnboardingStep Step => OnboardingStep.Staff;

    public async Task<bool> IsDoneAsync(Guid brandId, CancellationToken ct) =>
        await db.Onboarding.AnyAsync(o => o.BrandId == brandId && o.SoloConfirmedUtc != null, ct)
        || await db.Staff.CountAsync(s => s.BrandId == brandId && s.IsActive, ct) > 1;
}
