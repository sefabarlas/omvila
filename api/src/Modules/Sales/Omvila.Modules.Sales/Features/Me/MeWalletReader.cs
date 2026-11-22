using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Features.Packages;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Me;

/// <summary>
/// Müşteri uygulaması "Paketlerim" ve "Ödemelerim" (sözleşme meListPackages, meListPayments; dilim-05 KK6, dilim-04 KK9). Kayıtlar
/// müşterinin bütün markalardaki kartlarına bağlıdır ve marka RLS'indedir: okuma platform bağlamında yalnız müşterinin kartlarıyla
/// süzülerek yapılır (T-103 deseni). Kalan seans her zaman defterden. Arşivlenmiş markanın kayıtları listelenmez.
/// </summary>
internal sealed class MeWalletReader(
    SalesDbContext db,
    IDbSession session,
    ICustomerAccounts accounts,
    ICustomerBrandDirectory brands,
    IServiceDirectory services,
    ITenantQueries tenants,
    IAppointmentQueries appointments,
    IClock clock)
{
    // Kural: sözleşme meListPackages (önce kullanılabilirler son kullanıma göre artan, sonra bitenler bitişe göre azalan)
    public async Task<MePackageList> ListPackagesAsync(Guid customerId, CancellationToken ct)
    {
        var cards = await accounts.ListBrandCardsAsync(customerId, ct);
        return await InPlatformAsync(async () =>
        {
            var cardIds = cards.Select(c => c.BrandCustomerId).ToList();
            var packages = await db.CustomerPackages.AsNoTracking().Include(p => p.Items).Where(p => cardIds.Contains(p.BrandCustomerId)).ToListAsync(ct);
            var itemIds = packages.SelectMany(p => p.Items).Select(i => i.Id).ToList();
            var usages = await db.PackageSessionUsages.AsNoTracking()
                .Where(u => itemIds.Contains(u.CustomerPackageItemId) && u.RevertedUtc == null)
                .Select(u => new { u.CustomerPackageItemId, u.UsedUtc })
                .ToListAsync(ct);
            var used = usages.GroupBy(u => u.CustomerPackageItemId).ToDictionary(g => g.Key, g => (Count: g.Count(), Last: g.Max(u => u.UsedUtc)));
            var brandCards = await brands.GetCardsAsync([.. packages.Select(p => p.BrandId).Distinct()], ct);
            var rows = new List<MePackage>();
            foreach (var brand in packages.Where(p => brandCards.ContainsKey(p.BrandId)).GroupBy(p => p.BrandId))
            {
                var names = await services.GetSummariesAsync(brand.Key, [.. brand.SelectMany(p => p.Items).Select(i => i.ServiceId).Distinct()], ct);
                foreach (var package in brand)
                {
                    var zone = await ZoneAsync(package.BrandId, package.BranchId, ct);
                    rows.Add(MePackageMapper.Map(package, brandCards[brand.Key], names, used, zone, clock.GetCurrentInstant()));
                }
            }

            var usable = rows.Where(r => r.IsUsable).OrderBy(r => r.ExpiresOn).ThenBy(r => r.Id).ToList();
            var finished = rows.Where(r => !r.IsUsable).OrderByDescending(r => r.FinishedOn).ThenByDescending(r => r.Id);
            return new MePackageList(usable.Count, [.. usable, .. finished]);
        }, ct);
    }

    // Kural: sözleşme meListPayments (yeniden eskiye; Voided ve 0 TL paket kaydı yok; AwaitingConfirmation randevudan, id = randevu)
    public async Task<Result<MePaymentPage>> ListPaymentsAsync(Guid customerId, int? limit, string? cursor, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (!PageCursor.TryLimit(limit ?? MeDefaultLimit, out var take))
        {
            errors["limit"] = ["out_of_range"];
        }

        if (!PageCursor.TryDecode(cursor, out var offset))
        {
            errors["cursor"] = ["invalid_format"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var cards = await accounts.ListBrandCardsAsync(customerId, ct);
        var reported = await appointments.ListReportedDepositsAsync(customerId, ct);
        return await InPlatformAsync(() => PaymentPageAsync(cards, reported, take, offset, ct), ct);
    }

    /// <summary>Sözleşme Limit varsayılanı (50).</summary>
    private const int MeDefaultLimit = 50;

    private async Task<MePaymentPage> PaymentPageAsync(
        IReadOnlyList<CustomerBrandCardRef> cards, IReadOnlyList<ReportedDepositInfo> reported, int take, int offset, CancellationToken ct)
    {
        var cardIds = cards.Select(c => c.BrandCustomerId).ToList();
        var payments = await db.Payments.AsNoTracking().Include(p => p.Allocations)
            .Where(p => p.BrandCustomerId != null && cardIds.Contains(p.BrandCustomerId.Value)
                && p.Status != PaymentStatus.Voided && p.Method != PaymentMethod.PackageRedemption)
            .ToListAsync(ct);
        var entries = payments.Select(p => (Occurred: p.PaidUtc, p.Id, Payment: (Payment?)p, Reported: (ReportedDepositInfo?)null))
            .Concat(reported.Select(r => (Occurred: r.ReportedUtc, Id: r.AppointmentId, Payment: (Payment?)null, Reported: (ReportedDepositInfo?)r)))
            .OrderByDescending(e => e.Occurred).ThenByDescending(e => e.Id)
            .Skip(offset).Take(take + 1).ToList();
        var page = entries.Take(take).ToList();
        var pagePayments = page.Where(e => e.Payment is not null).Select(e => e.Payment!).ToList();
        var brandCards = await brands.GetCardsAsync([.. pagePayments.Select(p => p.BrandId).Concat(page.Where(e => e.Reported is not null).Select(e => e.Reported!.BrandId)).Distinct()], ct);
        var contexts = await AppointmentContextsAsync(pagePayments, ct);
        var packageIds = pagePayments.Select(MePaymentMapper.Target).Where(t => t?.TargetType == AllocationTargetType.CustomerPackage).Select(t => t!.TargetId).ToList();
        var packages = await db.CustomerPackages.AsNoTracking().Include(c => c.Items).Where(c => packageIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, ct);
        var rows = new List<MePaymentRow>();
        foreach (var entry in page)
        {
            var brandId = entry.Payment?.BrandId ?? entry.Reported!.BrandId;
            if (!brandCards.TryGetValue(brandId, out var brand))
            {
                continue;
            }

            var zone = await ZoneAsync(brandId, entry.Payment?.BranchId ?? entry.Reported!.BranchId, ct);
            rows.Add(entry.Payment is { } payment
                ? MePaymentMapper.FromPayment(payment, brand, zone.Id, contexts, packages)
                : MePaymentMapper.FromReported(entry.Reported!, brand, zone.Id));
        }

        return new MePaymentPage(rows, entries.Count > take ? PageCursor.Encode(offset + take) : null);
    }

    private async Task<Dictionary<Guid, AppointmentPaymentContext>> AppointmentContextsAsync(List<Payment> payments, CancellationToken ct)
    {
        var contexts = new Dictionary<Guid, AppointmentPaymentContext>();
        foreach (var brand in payments.GroupBy(p => p.BrandId))
        {
            var ids = brand.Select(MePaymentMapper.Target).Where(t => t?.TargetType == AllocationTargetType.Appointment).Select(t => t!.TargetId).Distinct().ToList();
            foreach (var (id, context) in await appointments.GetPaymentContextsAsync(brand.Key, ids, ct))
            {
                contexts[id] = context;
            }
        }

        return contexts;
    }

    private async Task<DateTimeZone> ZoneAsync(Guid brandId, Guid branchId, CancellationToken ct)
    {
        var zoneId = await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct);
        return (zoneId is null ? null : DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId)) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
    }

    private async Task<T> InPlatformAsync<T>(Func<Task<T>> read, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            return await read();
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }
}
