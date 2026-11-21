using NodaTime;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>Şubenin (verilmezse markanın "bugün" saat diliminin) yerel takvimi; cihazın veya sunucunun saat dilimi kullanılmaz.</summary>
internal static class BranchCalendar
{
    public static async Task<DateTimeZone> ZoneAsync(ITenantQueries tenants, Guid brandId, Guid? branchId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(tenants);
        var zoneId = branchId is { } branch
            ? await tenants.GetBranchTimeZoneAsync(brandId, branch, ct)
            : (await tenants.GetCustomerSettingsAsync(brandId, ct))?.TimeZoneId;
        return DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId ?? PlatformDefaults.DefaultTimeZoneId)
            ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
    }
}
