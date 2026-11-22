using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Email;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// Abonelik e-postaları (bildirimler.md § E-posta): owner.trial_ending ve owner.subscription_payment_failed. Alıcı işletmenin
/// e-postasıdır (AyarlarIsletme); yoksa gönderilmez. Gönderim hatası istisna olarak işleyiciyi yeniden denetir (outbox geri çekilmesi).
/// Günler OMVILA'nın faturalama takviminde (Europe/Istanbul).
/// </summary>
internal sealed class EmailOwnerAboutSubscription(
    IDbSession session, IBrandBilling brands, IEmailSender email, IOptions<EmailOptions> options, IClock clock)
    : IIntegrationEventHandler<SubscriptionTrialEnding>, IIntegrationEventHandler<SubscriptionPastDue>
{
    public async Task HandleAsync(SubscriptionTrialEnding integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        if (await RecipientAsync(integrationEvent.BrandId, ct) is not { } brand)
        {
            return;
        }

        var daysLeft = Math.Max(1, Period.DaysBetween(Today(), integrationEvent.TrialEndsOn));
        await email.SendAsync(
            EmailTemplates.TrialEnding(
                brand.Email!,
                brand.Name,
                integrationEvent.PlanName,
                integrationEvent.TrialDays,
                integrationEvent.TrialEndsOn,
                daysLeft,
                integrationEvent.PastDueGraceDays,
                Panel()),
            ct);
    }

    public async Task HandleAsync(SubscriptionPastDue integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        if (await RecipientAsync(integrationEvent.BrandId, ct) is not { } brand)
        {
            return;
        }

        var today = Today();
        var graceDay = Math.Max(1, Period.DaysBetween(integrationEvent.PastDueSince, today) + 1);
        await email.SendAsync(
            EmailTemplates.PaymentFailed(
                brand.Email!, brand.Name, integrationEvent.PlanName, integrationEvent.PeriodStart, today, integrationEvent.ReadOnlyOn, graceDay, Panel()),
            ct);
    }

    private async Task<BrandBillingProfile?> RecipientAsync(Guid brandId, CancellationToken ct)
    {
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var brand = await brands.GetProfileAsync(brandId, ct);
        return string.IsNullOrWhiteSpace(brand?.Email) ? null : brand;
    }

    private LocalDate Today() =>
        clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId]).Date;

    private Uri Panel() => new(options.Value.PanelBaseUrl, UriKind.Absolute);
}
