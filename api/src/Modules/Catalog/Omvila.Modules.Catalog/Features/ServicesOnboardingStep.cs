using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Catalog.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Catalog.Features;

/// <summary>Kurulum adımı Services: en az bir aktif hizmet (dilim-01 § Alan kuralları).</summary>
internal sealed class ServicesOnboardingStep(CatalogDbContext db) : IOnboardingStepProvider
{
    public OnboardingStep Step => OnboardingStep.Services;

    public Task<bool> IsDoneAsync(Guid brandId, CancellationToken ct) => db.Services.AnyAsync(s => s.BrandId == brandId && s.IsActive, ct);
}
