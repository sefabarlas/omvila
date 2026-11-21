using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>
/// Tahsilat kayıtlarını sözleşme PaymentView satırına çevirir (Kasa tablosu ve müşteri kartı Ödemeler sekmesi). Müşteri adı
/// Customers, randevu hizmetleri / başlangıcı / ilk personel Scheduling sözleşmesinden, paket satışında paket adı, toplam seans ve
/// satan personel (Tenancy personel kartı) toplu okunur; döngüde sorgu yoktur.
/// Kasada ve kartta tam telefon dönmez, yalnız maskeli.
/// </summary>
internal sealed class PaymentRows(SalesDbContext db, ICustomerDirectory customers, IAppointmentQueries appointments, ITenantQueries tenants)
{
    /// <param name="brandId">Marka.</param>
    /// <param name="payments">Tahsis kayıtlarıyla yüklenmiş tahsilatlar (sıra korunur).</param>
    /// <param name="recordedAfterDayClose">Tahsilat kendi gününün kapanışından sonra mı yazıldı.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task<IReadOnlyList<CashPaymentView>> MapAsync(
        Guid brandId, IReadOnlyList<Payment> payments, Func<Payment, bool> recordedAfterDayClose, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(payments);
        ArgumentNullException.ThrowIfNull(recordedAfterDayClose);
        if (payments.Count == 0)
        {
            return [];
        }

        var appointmentIds = payments.Select(Target).Where(t => t?.TargetType == AllocationTargetType.Appointment)
            .Select(t => t!.TargetId).Distinct().ToList();
        var withDeposit = await (from a in db.PaymentAllocations.AsNoTracking()
                                 join p in db.Payments.AsNoTracking() on a.PaymentId equals p.Id
                                 where a.BrandId == brandId && a.TargetType == AllocationTargetType.Appointment && appointmentIds.Contains(a.TargetId)
                                       && p.IsDeposit
                                 select a.TargetId).Distinct().ToListAsync(ct);
        var contexts = await appointments.GetPaymentContextsAsync(brandId, appointmentIds, ct);
        var customerIds = payments.Where(p => p.BrandCustomerId is not null).Select(p => p.BrandCustomerId!.Value).Distinct().ToList();
        var names = await customers.GetDisplayInfoAsync(brandId, customerIds, ct);
        var packageIds = payments.Select(Target).Where(t => t?.TargetType == AllocationTargetType.CustomerPackage)
            .Select(t => t!.TargetId).Distinct().ToList();
        var packages = await db.CustomerPackages.AsNoTracking()
            .Where(c => c.BrandId == brandId && packageIds.Contains(c.Id))
            .Select(c => new SoldPackage(c.Id, c.PackageNameSnapshot, c.Items.Sum(i => i.TotalSessions), c.SoldByStaffId, null))
            .ToListAsync(ct);
        var sellers = await tenants.GetStaffCardsAsync(
            brandId, [.. packages.Where(c => c.SoldByStaffId is not null).Select(c => c.SoldByStaffId!.Value).Distinct()], ct);
        var sold = packages.ToDictionary(
            c => c.Id, c => c with { SellerName = c.SoldByStaffId is { } s && sellers.TryGetValue(s, out var card) ? card.FullName : null });
        return [.. payments.Select(p => Row(p, recordedAfterDayClose(p), withDeposit, contexts, names, sold))];
    }

    /// <summary>v1'de bir tahsilat tek hedefe tahsis edilir; tahsissiz kayıt hedefsizdir.</summary>
    private static PaymentAllocation? Target(Payment p) => p.Allocations.OrderBy(a => a.Id).FirstOrDefault();

    private static CashPaymentView Row(
        Payment p,
        bool afterClose,
        List<Guid> withDeposit,
        IReadOnlyDictionary<Guid, AppointmentPaymentContext> contexts,
        IReadOnlyDictionary<Guid, CustomerDisplayInfo> names,
        Dictionary<Guid, SoldPackage> sold)
    {
        var target = Target(p);
        var currency = Enum.TryParse<Currency>(p.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        // Paket satışı (T-95) ve ileride eklenen hedefler: randevu dışındaki her tahsis paket satırı olarak gösterilir.
        var isPackage = target is { TargetType: not AllocationTargetType.Appointment };
        var appointmentId = target is { TargetType: AllocationTargetType.Appointment } ? target.TargetId : (Guid?)null;
        var context = appointmentId is { } id && contexts.TryGetValue(id, out var found) ? found : null;
        var package = isPackage && sold.TryGetValue(target!.TargetId, out var pkg) ? pkg : null;
        var customer = p.BrandCustomerId is { } cid && names.TryGetValue(cid, out var info)
            ? new CashCustomerRef(info.BrandCustomerId, info.FullName, info.PhoneMasked)
            : null;
        return new CashPaymentView(
            p.Id,
            p.BranchId,
            p.Method.ToString(),
            new Money(p.AmountMinor, currency),
            p.IsDeposit,
            p.Status.ToString(),
            p.RefundedAmountMinor > 0 ? new Money(p.RefundedAmountMinor, currency) : null,
            p.PaidUtc,
            null,
            p.Reference,
            isPackage ? nameof(AllocationTargetType.CustomerPackage) : nameof(AllocationTargetType.Appointment),
            customer,
            appointmentId,
            context?.StartUtc,
            context?.ServiceNames ?? [],
            // "kalan": kaporadan sonra salonda alınan tutar; 0 TL'lik paketten düşüm kalan sayılmaz.
            appointmentId is { } a && !p.IsDeposit && p.Method != PaymentMethod.PackageRedemption && withDeposit.Contains(a),
            isPackage ? package?.SellerName : context?.StaffName,
            isPackage ? target!.TargetId : null,
            package?.Name,
            package?.TotalSessions,
            afterClose);
    }

    /// <summary>Paket satışı satırı: paket adı (satıştaki kopya), toplam seans, satan personel.</summary>
    private sealed record SoldPackage(Guid Id, string Name, int TotalSessions, Guid? SoldByStaffId, string? SellerName);
}
