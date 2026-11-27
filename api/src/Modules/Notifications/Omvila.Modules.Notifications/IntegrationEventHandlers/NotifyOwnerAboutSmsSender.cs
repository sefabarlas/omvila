using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Email;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// SMS gönderen adı kararı markaya (bildirimler.md owner.sms_sender_approved / _rejected; T-124 (4), T-132): ayarları
/// yönetebilen kullanıcılara (settings.manage: Owner) bildirim paneli kaydı (hedef Ayarlar → Bildirimler ve SMS; ret nedeni
/// orada ve e-postada, panel metninde değil) ve işletmenin e-postası varsa e-posta. İşletme push'u gönderilmez. E-posta
/// hatası işleyiciyi yeniden denetir.
/// </summary>
internal sealed class NotifyOwnerAboutSmsSender(
    IDbSession session,
    IBrandUserDirectory users,
    IBrandBilling brands,
    InAppNotifier inApp,
    IEmailSender email,
    IOptions<EmailOptions> options)
    : IIntegrationEventHandler<SmsSenderDecided>
{
    public async Task HandleAsync(SmsSenderDecided integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var brandId = integrationEvent.BrandId;
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var recipients = await users.GetUserIdsWithPermissionAsync(brandId, Permissions.Settings.Manage, null, ct);
        if (recipients.Count > 0)
        {
            var eventCode = integrationEvent.Approved ? NotificationEvents.OwnerSmsSenderApproved : NotificationEvents.OwnerSmsSenderRejected;
            var values = new Dictionary<string, string>(StringComparer.Ordinal) { ["ad"] = integrationEvent.Name };
            await inApp.AddAsync(new InAppEntry(brandId, eventCode, values, null, null, InAppNotifier.SmsCreditsTarget, null), recipients, ct);
        }

        var brand = await brands.GetProfileAsync(brandId, ct);
        if (string.IsNullOrWhiteSpace(brand?.Email))
        {
            return;
        }

        await email.SendAsync(
            EmailTemplates.SmsSenderDecided(
                brand.Email, brand.Name, integrationEvent.Name, integrationEvent.Approved, integrationEvent.Reason,
                new Uri(options.Value.PanelBaseUrl, UriKind.Absolute)),
            ct);
    }
}
