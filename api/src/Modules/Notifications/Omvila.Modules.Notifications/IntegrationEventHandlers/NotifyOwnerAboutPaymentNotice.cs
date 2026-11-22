using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Email;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// Havale bildirimi sonuçları markaya (bildirimler.md owner.payment_notice_matched / _not_found / _rejected; dilim-06 KK2, KK3;
/// T-124): aboneliği yönetebilen kullanıcılara (subscription.manage: Owner) bildirim paneli kaydı ve işletme push'u, işletmenin
/// e-postası varsa e-posta (platform ekibinin notu yalnız e-postada; panel metni kısa). E-posta hatası işleyiciyi yeniden denetir.
/// </summary>
internal sealed class NotifyOwnerAboutPaymentNotice(
    IDbSession session,
    ITenantQueries tenants,
    IBrandUserDirectory users,
    IBrandBilling brands,
    InAppNotifier inApp,
    INotificationOutbox outbox,
    IEmailSender email,
    IOptions<EmailOptions> options)
    : IIntegrationEventHandler<PaymentNoticeMatched>, IIntegrationEventHandler<PaymentNoticeNotFound>, IIntegrationEventHandler<PaymentNoticeRejected>
{
    public async Task HandleAsync(PaymentNoticeMatched integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var credits = integrationEvent.Credits;
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        if (credits is { } added)
        {
            values["kontör"] = StaffText.Count(added);
        }

        var variant = credits is null ? PushTemplates.SubscriptionVariant : PushTemplates.CreditsVariant;
        await NotifyAsync(integrationEvent.BrandId, NotificationEvents.OwnerPaymentNoticeMatched, values, variant, ct);
        await EmailAsync(integrationEvent.BrandId, (to, name, panel) => EmailTemplates.PaymentNoticeMatched(to, name, credits, panel), ct);
    }

    public async Task HandleAsync(PaymentNoticeNotFound integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        await NotifyAsync(integrationEvent.BrandId, NotificationEvents.OwnerPaymentNoticeNotFound, Empty(), null, ct);
        await EmailAsync(
            integrationEvent.BrandId, (to, name, panel) => EmailTemplates.PaymentNoticeNotFound(to, name, integrationEvent.Message, panel), ct);
    }

    public async Task HandleAsync(PaymentNoticeRejected integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        await NotifyAsync(integrationEvent.BrandId, NotificationEvents.OwnerPaymentNoticeRejected, Empty(), null, ct);
        await EmailAsync(
            integrationEvent.BrandId, (to, name, panel) => EmailTemplates.PaymentNoticeRejected(to, name, integrationEvent.Message, panel), ct);
    }

    private static Dictionary<string, string> Empty() => new(StringComparer.Ordinal);

    private async Task NotifyAsync(
        Guid brandId, string eventCode, IReadOnlyDictionary<string, string> values, string? variant, CancellationToken ct)
    {
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var recipients = await users.GetUserIdsWithPermissionAsync(brandId, Permissions.Subscription.Manage, null, ct);
        var settings = await tenants.GetNotificationSettingsAsync(brandId, ct);
        if (recipients.Count == 0 || settings is null)
        {
            return;
        }

        await inApp.AddAsync(new InAppEntry(brandId, eventCode, values, variant, null, InAppNotifier.SubscriptionTarget, null), recipients, ct);
        await outbox.EnqueueStaffPushAsync(
            new StaffPushNotification(eventCode, brandId, recipients, values, settings.TimeZoneId, Variant: variant), ct);
    }

    private async Task EmailAsync(Guid brandId, Func<string, string, Uri, EmailMessage> build, CancellationToken ct)
    {
        var brand = await brands.GetProfileAsync(brandId, ct);
        if (string.IsNullOrWhiteSpace(brand?.Email))
        {
            return;
        }

        await email.SendAsync(build(brand.Email, brand.Name, new Uri(options.Value.PanelBaseUrl, UriKind.Absolute)), ct);
    }
}
