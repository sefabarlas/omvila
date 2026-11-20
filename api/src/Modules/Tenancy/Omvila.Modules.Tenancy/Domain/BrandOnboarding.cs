using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// İstemcinin bildirdiği kurulum olayları (sözleşme manageRecordOnboardingEvent; tek satır, BrandId PK).
/// "Tek başıma çalışıyorum" (onboarding.soloConfirmed) ve bağlantı paylaşımı (kopyalama, QR, hikâye görseli).
/// İlk zaman korunur.
/// </summary>
internal sealed class BrandOnboarding
{
    private BrandOnboarding()
    {
    }

    public Guid BrandId { get; private set; }

    public Instant? SoloConfirmedUtc { get; private set; }

    public Instant? BookingLinkSharedUtc { get; private set; }

    public static BrandOnboarding Create(Guid brandId) => new() { BrandId = brandId };

    public void Record(OnboardingEvent onboardingEvent, Instant now)
    {
        if (onboardingEvent == OnboardingEvent.SoloConfirmed)
        {
            SoloConfirmedUtc ??= now;
        }
        else
        {
            BookingLinkSharedUtc ??= now;
        }
    }
}
