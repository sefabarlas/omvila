using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary>
/// <see cref="IAppointmentPayments"/>: tamamlamanın tahsilatlarını ve hizmet primini istek transaction'ında yazar; commit
/// veya geri alma çağıranın oturumundadır. Oranlar Tenancy (personel) ve Catalog (personele özel hizmet oranı) sözleşmelerinden
/// toplu okunur.
/// </summary>
internal sealed class AppointmentPayments(
    SalesDbContext db, ITenantQueries tenants, IStaffServiceAssignments assignments, IIdGenerator ids, IClock clock) : IAppointmentPayments
{
    // Kural: durum-makinesi.md A11 (tahsilatlar kaydedilir; prim doğar); alan-modeli.md § CommissionEntry
    public async Task RecordCompletionAsync(AppointmentCompletionRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        var now = clock.GetCurrentInstant();
        foreach (var line in request.Payments)
        {
            db.Payments.Add(Payment.RecordCollected(
                ids, request.BrandId, request.BranchId, request.BrandCustomerId, request.AppointmentId, ToDomain(line.Method), line.Amount,
                request.CollectedByUserId, now));
        }

        // Paketten düşülen kalemler (W1): 0 TL'lik kayıt; prim tabanı paket fiyatının seans payı (W2).
        var packaged = request.Items.Where(i => i.CustomerPackageItemId is not null).ToList();
        var shares = await PackageSessionShares.LoadAsync(db, [.. packaged.Select(i => i.CustomerPackageItemId!.Value)], ct);
        for (var i = 0; i < packaged.Count; i++)
        {
            db.Payments.Add(Payment.RecordPackageRedemption(
                ids, request.BrandId, request.BranchId, request.BrandCustomerId, request.AppointmentId, request.Currency, request.CollectedByUserId, now));
        }

        var paid = request.Items.Where(i => i.CustomerPackageItemId is null).ToList();
        var collected = request.Payments.Sum(p => p.Amount.AmountMinor) + request.RetainedDepositMinor;
        var paidBases = CommissionBasis.Distribute([.. paid.Select(i => i.AmountMinor)], collected);
        var basisByItem = paid.Zip(paidBases).ToDictionary(x => x.First.AppointmentItemId, x => x.Second);
        foreach (var item in packaged)
        {
            basisByItem[item.AppointmentItemId] = shares.GetValueOrDefault(item.CustomerPackageItemId!.Value);
        }

        var staffIds = request.Items.Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct().ToList();
        var staffPercents = await tenants.GetServiceCommissionPercentsAsync(request.BrandId, staffIds, ct);
        var overrides = (await assignments.GetCommissionOverridesAsync(request.BrandId, staffIds, ct))
            .ToDictionary(o => (o.StaffId, o.ServiceId), o => o.Percent);
        foreach (var item in request.Items)
        {
            var basis = basisByItem[item.AppointmentItemId];
            if (item.StaffId is not { } staffId)
            {
                continue;
            }

            var percent = CommissionEntry.ServicePercent(
                overrides.TryGetValue((staffId, item.ServiceId), out var o) ? o : null,
                staffPercents.TryGetValue(staffId, out var s) ? s : null);
            if (percent > 0)
            {
                db.CommissionEntries.Add(CommissionEntry.AccrueService(
                    ids, request.BrandId, request.BranchId, staffId, item.AppointmentItemId, new Money(basis, request.Currency), percent, request.CompletedOn, now));
            }
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<AppointmentPaymentRecord>> ListAsync(Guid brandId, Guid appointmentId, CancellationToken ct)
    {
        var rows = await (from a in db.PaymentAllocations.AsNoTracking()
                          join p in db.Payments.AsNoTracking() on a.PaymentId equals p.Id
                          where a.BrandId == brandId && a.TargetType == AllocationTargetType.Appointment && a.TargetId == appointmentId
                          orderby p.PaidUtc, p.Id
                          select new { p, a.AmountMinor }).ToListAsync(ct);
        return [.. rows.Select(r => new AppointmentPaymentRecord(
            r.p.Id,
            r.p.BranchId,
            r.p.Method.ToString(),
            new Money(r.AmountMinor, Enum.Parse<Currency>(r.p.CurrencyCode)),
            r.p.IsDeposit,
            r.p.Status.ToString(),
            r.p.RefundedAmountMinor,
            r.p.PaidUtc,
            r.p.CollectedByUserId,
            r.p.Reference))];
    }

    internal static PaymentMethod ToDomain(CollectedPaymentMethod method) => method switch
    {
        CollectedPaymentMethod.Cash => PaymentMethod.Cash,
        CollectedPaymentMethod.Card => PaymentMethod.Card,
        CollectedPaymentMethod.Transfer => PaymentMethod.Transfer,
        _ => throw new DomainException("Bilinmeyen tahsilat yöntemi."),
    };
}
