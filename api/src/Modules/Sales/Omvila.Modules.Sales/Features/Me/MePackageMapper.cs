using NodaTime;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Features.Packages;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Me;

/// <summary>Müşteri paketi → sözleşme MePackage. Gün şubenin saat diliminde.</summary>
internal static class MePackageMapper
{
    // Kural: sözleşme MePackage.isUsable / isRunningLow / finishedOn; sureler-ve-ayarlar.md PackageLowSessionThreshold, PackageExpiringSoonDays
    public static MePackage Map(
        CustomerPackage package,
        CustomerBrandCard brand,
        IReadOnlyDictionary<Guid, ServiceSummary> names,
        IReadOnlyDictionary<Guid, (int Count, Instant Last)> used,
        DateTimeZone zone,
        Instant now)
    {
        var today = now.InZone(zone).Date;
        var items = package.Items.OrderBy(i => i.SeqNo)
            .Select(i => new CustomerPackageItemView(
                i.Id, i.ServiceId, names.GetValueOrDefault(i.ServiceId)?.Name ?? string.Empty, i.TotalSessions,
                i.Remaining(used.TryGetValue(i.Id, out var u) ? u.Count : 0)))
            .ToList();
        var remaining = items.Sum(i => i.RemainingSessions);
        var usable = package.Status == CustomerPackageStatus.Active && remaining > 0 && today <= package.ExpiresOn;
        var runningLow = usable
            && (remaining <= PlatformDefaults.PackageLowSessionThreshold || package.ExpiresOn <= today.PlusDays(PlatformDefaults.PackageExpiringSoonDays));
        return new MePackage(
            package.Id,
            new MeBrandRef(brand.Id, brand.Name, brand.Slug, brand.LogoUrl),
            package.IsAvailableAllBranches ? null : brand.Branches.FirstOrDefault(b => b.Id == package.BranchId)?.Name,
            package.IsAvailableAllBranches,
            package.PackageNameSnapshot,
            package.Status.ToString(),
            items,
            package.TotalSessions,
            remaining,
            package.ExpiresOn,
            usable,
            runningLow,
            usable ? null : FinishedOn(package, remaining, used, zone));
    }

    // "Eylül'de bitti": iadenin, son kullanım gününün veya son seansın yerel günü.
    private static LocalDate FinishedOn(CustomerPackage package, int remaining, IReadOnlyDictionary<Guid, (int Count, Instant Last)> used, DateTimeZone zone)
    {
        if (package is { Status: CustomerPackageStatus.Refunded, RefundedUtc: { } refunded })
        {
            return refunded.InZone(zone).Date;
        }

        var lastUse = package.Items.Select(i => used.TryGetValue(i.Id, out var u) ? u.Last : (Instant?)null).Max();
        return remaining == 0 && lastUse is { } last && last.InZone(zone).Date <= package.ExpiresOn ? last.InZone(zone).Date : package.ExpiresOn;
    }
}
