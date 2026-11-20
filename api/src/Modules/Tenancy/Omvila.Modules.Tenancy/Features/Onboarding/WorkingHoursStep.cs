using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Infrastructure;

namespace Omvila.Modules.Tenancy.Features.Onboarding;

/// <summary>Çalışma saatleri: şubede en az bir gün aralığı var (dilim-01 § Alan kuralları).</summary>
internal sealed class WorkingHoursStep(TenancyDbContext db) : IOnboardingStepProvider
{
    public OnboardingStep Step => OnboardingStep.WorkingHours;

    public Task<bool> IsDoneAsync(Guid brandId, CancellationToken ct) => db.BranchHours.AnyAsync(h => h.BrandId == brandId, ct);
}
