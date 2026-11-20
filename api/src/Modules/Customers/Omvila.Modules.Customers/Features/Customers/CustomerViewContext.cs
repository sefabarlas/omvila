using System.Security.Claims;
using NodaTime;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>Bir isteğin müşteri görünümü: marka ayarları, markanın "bugün"ü ve kullanıcının tam numarayı görüp göremeyeceği.</summary>
internal sealed record CustomerViewContext(BrandCustomerSettings Settings, LocalDate Today, Currency Currency, bool CanSeeFullPhone)
{
    public static async Task<CustomerViewContext> LoadAsync(
        Guid brandId, ClaimsPrincipal user, ITenantQueries tenants, IPermissionResolver permissions, IClock clock, CancellationToken ct)
    {
        var settings = await tenants.GetCustomerSettingsAsync(brandId, ct)
            ?? throw new DomainException("Marka ayarları bulunamadı.");
        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(settings.TimeZoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var today = clock.GetCurrentInstant().InZone(zone).Date;
        var role = user.FindFirst(OmvilaClaims.Role)?.Value;
        var hasPhoneView = role is not null && permissions.Resolve(role, Permissions.Customer.PhoneView) is not null;
        var currency = Enum.TryParse<Currency>(settings.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        return new CustomerViewContext(settings, today, currency, Domain.CustomerPhoneDisplay.CanSeeFullPhone(hasPhoneView, settings.MaskCustomerPhoneFromStaff));
    }
}
