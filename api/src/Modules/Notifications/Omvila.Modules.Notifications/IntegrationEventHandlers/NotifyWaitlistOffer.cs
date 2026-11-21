using NodaTime;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// Bekleme teklifi (dilim-05 kabul kriteri 3; bildirimler.md <c>waitlist.offer</c>): müşteriye "Beklediğiniz {tarih} {saat}
/// boşaldı. 15 dk içinde onaylarsanız sizin: {b bağlantısı}". Kanal planı push → SMS (müşteri uygulamasında cihaz yoksa
/// SMS); sessiz saate kaydırılmaz (teklif 15 dk'lıktır). Bekleme listesine katılan kişi bu mesajı istemiştir: "Randevu
/// bilgilendirme SMS'leri" tercihine bağlı değildir; numara anonimleştirilmişse yazılmaz. Telefon günlüğe yazılmaz.
/// </summary>
internal sealed class NotifyWaitlistOffer(
    IDbSession session,
    IPublicBookingQueries booking,
    ICustomerDirectory customers,
    INotificationOutbox outbox) : IIntegrationEventHandler<WaitlistOfferCreated>
{
    // Kural: bildirimler.md waitlist.offer
    public async Task HandleAsync(WaitlistOfferCreated integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var brandId = integrationEvent.BrandId;
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var branch = await booking.GetBranchCardAsync(brandId, integrationEvent.BranchId, ct);
        var customer = (await customers.GetDisplayInfoAsync(brandId, [integrationEvent.BrandCustomerId], ct)).GetValueOrDefault(integrationEvent.BrandCustomerId);
        if (branch is null || customer is not { PhoneMasked: not null } || !PhoneNumber.TryParse(customer.PhoneE164, out _))
        {
            return;
        }

        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(branch.TimeZoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var start = integrationEvent.StartUtc.InZone(zone);
        var values = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["salon"] = branch.BrandName,
            ["tarih"] = SmsFormat.Date(start.Date),
            ["saat"] = SmsFormat.Time(start.TimeOfDay),
            ["dakika"] = integrationEvent.HoldMinutes.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["b bağlantısı"] = SmsFormat.Link(booking.WaitlistOfferPageUrl(integrationEvent.AccessCode)),
        };
        await outbox.EnqueueSmsAsync(
            new SmsNotification(
                NotificationEvents.WaitlistOffer,
                customer.PhoneE164,
                values,
                brandId,
                NotificationReference.ForWaitlistOffer(integrationEvent.OfferId),
                CustomerId: integrationEvent.CustomerId),
            ct);
    }
}
