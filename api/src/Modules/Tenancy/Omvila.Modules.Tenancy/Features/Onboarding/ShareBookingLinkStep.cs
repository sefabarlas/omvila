using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Infrastructure;

namespace Omvila.Modules.Tenancy.Features.Onboarding;

/// <summary>Bağlantı paylaşıldı: kopyalandı, QR veya hikâye görseli indirildi (istemci bildirir).</summary>
internal sealed class ShareBookingLinkStep(TenancyDbContext db) : IOnboardingStepProvider
{
    public OnboardingStep Step => OnboardingStep.ShareBookingLink;

    public Task<bool> IsDoneAsync(Guid brandId, CancellationToken ct) =>
        db.Onboarding.AnyAsync(o => o.BrandId == brandId && o.BookingLinkSharedUtc != null, ct);
}
