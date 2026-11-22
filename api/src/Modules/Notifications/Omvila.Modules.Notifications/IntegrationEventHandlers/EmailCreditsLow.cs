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
/// owner.credits_low e-postası (bildirimler.md § E-posta "Kontör eşiğin altına inince (push ile birlikte)"): yalnız eşik altına
/// inişte (<see cref="SmsCreditsLow"/>; yetersiz kontörle atlanan her SMS'te değil). İşlenene kadar bakiye eşiğin üstüne
/// çıktıysa veya işletme e-postası yoksa gönderilmez. Push ve bildirim paneli <see cref="NotifyCreditsLow"/>'dadır.
/// </summary>
internal sealed class EmailCreditsLow(
    IDbSession session, IBrandBilling brands, ICreditLedger ledger, SmsUsage usage, IEmailSender email, IOptions<EmailOptions> options)
    : IIntegrationEventHandler<SmsCreditsLow>
{
    public async Task HandleAsync(SmsCreditsLow integrationEvent, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);
        var brandId = integrationEvent.BrandId;
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var balance = Math.Max(0, await ledger.GetBalanceAsync(brandId, ct));
        var brand = await brands.GetProfileAsync(brandId, ct);
        if (balance >= PlatformDefaults.LowCreditThreshold || string.IsNullOrWhiteSpace(brand?.Email))
        {
            return;
        }

        var days = await usage.EstimateDaysLeftAsync(brandId, balance, ct);
        await email.SendAsync(
            EmailTemplates.CreditsLow(brand.Email, brand.Name, balance, days, new Uri(options.Value.PanelBaseUrl, UriKind.Absolute)), ct);
    }
}
