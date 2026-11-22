using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Email;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.IntegrationEventHandlers;

/// <summary>
/// SMS gönderen adı kararı markaya e-postayla (bildirimler.md § E-posta owner.sms_sender_approved / _rejected; T-124). Bildirim
/// paneli kaydı yazılmaz: sözleşmenin işletme olay kodları (StaffNotificationEvent) bu olayı içermez; salon kararı Ayarlar →
/// Bildirimler ve SMS'te görür. İşletme e-postası yoksa bir şey gönderilmez.
/// </summary>
internal sealed class EmailOwnerAboutSmsSender(IDbSession session, IBrandBilling brands, IEmailSender email, IOptions<EmailOptions> options)
    : IIntegrationEventHandler<SmsSenderDecided>
{
    public async Task HandleAsync(SmsSenderDecided integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        await session.SetTenantAsync(TenantScope.ForBrand(integrationEvent.BrandId), ct);
        var brand = await brands.GetProfileAsync(integrationEvent.BrandId, ct);
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
