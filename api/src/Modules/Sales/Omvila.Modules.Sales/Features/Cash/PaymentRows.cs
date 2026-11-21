using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>
/// Tahsilat kayıtlarını sözleşme PaymentView satırına çevirir (Kasa tablosu ve müşteri kartı Ödemeler sekmesi). Müşteri adı
/// Customers, randevu hizmetleri / başlangıcı / ilk personel Scheduling sözleşmesinden toplu okunur; döngüde sorgu yoktur.
/// Kasada ve kartta tam telefon dönmez, yalnız maskeli.
/// </summary>
internal sealed class PaymentRows(SalesDbContext db, ICustomerDirectory customers, IAppointmentQueries appointments)
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
        return [.. payments.Select(p => Row(p, recordedAfterDayClose(p), withDeposit, contexts, names))];
    }

    /// <summary>v1'de bir tahsilat tek hedefe tahsis edilir; tahsissiz kayıt hedefsizdir.</summary>
    private static PaymentAllocation? Target(Payment p) => p.Allocations.OrderBy(a => a.Id).FirstOrDefault();

    private static CashPaymentView Row(
        Payment p,
        bool afterClose,
        List<Guid> withDeposit,
        IReadOnlyDictionary<Guid, AppointmentPaymentContext> contexts,
        IReadOnlyDictionary<Guid, CustomerDisplayInfo> names)
    {
        var target = Target(p);
        var currency = Enum.TryParse<Currency>(p.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        // Paket satışı (T-95) ve ileride eklenen hedefler: randevu dışındaki her tahsis paket satırı olarak gösterilir.
        var isPackage = target is { TargetType: not AllocationTargetType.Appointment };
        var appointmentId = target is { TargetType: AllocationTargetType.Appointment } ? target.TargetId : (Guid?)null;
        var context = appointmentId is { } id && contexts.TryGetValue(id, out var found) ? found : null;
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
            appointmentId is { } a && !p.IsDeposit && withDeposit.Contains(a),
            context?.StaffName,
            isPackage ? target!.TargetId : null,
            null,
            null,
            afterClose);
    }
}
