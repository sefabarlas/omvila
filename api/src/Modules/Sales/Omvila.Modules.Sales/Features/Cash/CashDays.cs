using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>
/// Kasa günü (sözleşme manageGetCashDay, manageCloseCashDay). Gün şubenin saat dilimindeki yerel gündür; tahsilatlar o günün
/// UTC aralığından tek sorguyla, müşteri adları (Customers) ve randevu bilgisi (Scheduling) sözleşmeleriyle toplu okunur.
/// "Kapanıştan sonra" işareti okumada hesaplanır: tahsilat anı kapanış anından sonraysa (bütün yazma yolları için geçerli).
/// </summary>
internal sealed class CashDays(
    SalesDbContext db, ITenantQueries tenants, ICustomerDirectory customers, IAppointmentQueries appointments, IIdGenerator ids, IClock clock)
{
    private const Currency CashCurrency = PlatformDefaults.DefaultCurrency;

    // Kural: roller-yetkiler.md payment.collect (Kasa; Ş), alan-modeli.md § CashDayClose, docs/acik-sorular.md W3
    public async Task<Result<CashDayView>> GetAsync(Guid brandId, Guid? branchId, string? date, BranchScope scope, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(scope);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (branchId is null)
        {
            errors["branchId"] = ["required"];
        }

        var parsed = date is null ? null : ParseDate("date", date, errors);
        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var zone = await ZoneAsync(brandId, branchId!.Value, scope, ct);
        if (zone is null)
        {
            return Error.NotFound();
        }

        return await BuildAsync(brandId, branchId.Value, parsed ?? Today(zone), zone, ct);
    }

    // Kural: alan-modeli.md § CashDayClose; dilim-05 KK7; roller-yetkiler.md cash.close (Owner, BranchManager kendi şubesinde)
    public async Task<Result<CashDayView>> CloseAsync(Guid brandId, CashDayCloseRequest body, BranchScope scope, Guid? userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(scope);
        if (ValidateClose(body) is { } invalid)
        {
            return invalid;
        }

        var branchId = body.BranchId!.Value;
        var zone = await ZoneAsync(brandId, branchId, scope, ct);
        if (zone is null)
        {
            return Error.NotFound();
        }

        var day = LocalDatePattern.Iso.Parse(body.Date!).Value;
        if (await db.CashDayCloses.AnyAsync(c => c.BrandId == brandId && c.BranchId == branchId && c.BusinessDate == day, ct))
        {
            return Error.Conflict(ErrorCodes.CashDayAlreadyClosed);
        }

        var (start, end) = Bounds(zone, day);
        var entries = await db.Payments.AsNoTracking()
            .Where(p => p.BrandId == brandId && p.BranchId == branchId && p.PaidUtc >= start && p.PaidUtc < end)
            .Select(p => new CashEntry(p.Method, p.AmountMinor, p.IsDeposit, p.Status))
            .ToListAsync(ct);
        var totals = CashDayTotals.Calculate(entries, await CashRefundedAsync(brandId, branchId, start, end, ct));
        var closed = CashDayClose.Close(
            ids,
            brandId,
            branchId,
            day,
            Today(zone),
            new Money(totals.ExpectedCashMinor, CashCurrency),
            new Money(body.CountedCash!.AmountMinor!.Value, CashCurrency),
            body.DifferenceNote,
            userId,
            clock.GetCurrentInstant());
        if (!closed.IsSuccess)
        {
            return closed.Error;
        }

        db.CashDayCloses.Add(closed.Value);
        await db.SaveChangesAsync(ct);
        return await BuildAsync(brandId, branchId, day, zone, ct);
    }

    private static Error? ValidateClose(CashDayCloseRequest body)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.BranchId is null)
        {
            errors["branchId"] = ["required"];
        }

        if (body.Date is null)
        {
            errors["date"] = ["required"];
        }
        else
        {
            ParseDate("date", body.Date, errors);
        }

        if (body.CountedCash is not { AmountMinor: { } counted, Currency: not null })
        {
            errors["countedCash"] = ["required"];
        }
        else if (!string.Equals(body.CountedCash.Currency, CashCurrency.ToString(), StringComparison.Ordinal))
        {
            errors["countedCash.currency"] = ["invalid_format"];
        }
        else if (counted < 0)
        {
            errors["countedCash"] = ["out_of_range"];
        }

        return errors.Count > 0 ? Error.Validation(errors) : null;

    }

    private async Task<CashDayView> BuildAsync(Guid brandId, Guid branchId, LocalDate date, DateTimeZone zone, CancellationToken ct)
    {
        var (start, end) = Bounds(zone, date);
        var payments = await db.Payments.AsNoTracking().Include(p => p.Allocations)
            .Where(p => p.BrandId == brandId && p.BranchId == branchId && p.PaidUtc >= start && p.PaidUtc < end)
            .OrderByDescending(p => p.PaidUtc).ThenByDescending(p => p.Id)
            .ToListAsync(ct);
        var closing = await db.CashDayCloses.AsNoTracking()
            .SingleOrDefaultAsync(c => c.BrandId == brandId && c.BranchId == branchId && c.BusinessDate == date, ct);
        var totals = CashDayTotals.Calculate(
            payments.Select(p => new CashEntry(p.Method, p.AmountMinor, p.IsDeposit, p.Status)),
            await CashRefundedAsync(brandId, branchId, start, end, ct));

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

        var rows = payments.Select(p => Row(p, closing, withDeposit, contexts, names)).ToList();
        return new CashDayView(
            branchId,
            date,
            zone.Id,
            closing is null ? "Open" : "Closed",
            new CashMethodTotalView(Amount(totals.Total.AmountMinor), totals.Total.Count),
            new CashMethodTotalView(Amount(totals.Cash.AmountMinor), totals.Cash.Count),
            new CashMethodTotalView(Amount(totals.Card.AmountMinor), totals.Card.Count),
            new CashTransferTotalView(Amount(totals.Transfer.AmountMinor), totals.Transfer.Count, Amount(totals.TransferDepositMinor), Amount(totals.TransferOtherMinor)),
            Amount(totals.CashRefundedMinor),
            Amount(totals.ExpectedCashMinor),
            rows,
            closing is null ? null : new CashDayClosingView(
                Amount(closing.ExpectedCashMinor),
                Amount(closing.CountedCashMinor),
                closing.DifferenceMinor,
                closing.Difference.ToString(),
                closing.DifferenceNote,
                null,
                closing.ClosedUtc));
    }

    private static CashPaymentView Row(
        Payment p,
        CashDayClose? closing,
        List<Guid> withDeposit,
        IReadOnlyDictionary<Guid, AppointmentPaymentContext> contexts,
        IReadOnlyDictionary<Guid, CustomerDisplayInfo> names)
    {
        var target = Target(p);
        var currency = Enum.TryParse<Currency>(p.CurrencyCode, out var c) ? c : CashCurrency;
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
            closing?.IsRecordedAfter(p.PaidUtc) ?? false);
    }

    /// <summary>v1'de bir tahsilat tek hedefe tahsis edilir; tahsissiz kayıt hedefsizdir.</summary>
    private static PaymentAllocation? Target(Payment p) => p.Allocations.OrderBy(a => a.Id).FirstOrDefault();

    private async Task<long> CashRefundedAsync(Guid brandId, Guid branchId, Instant start, Instant end, CancellationToken ct) =>
        await db.Payments.AsNoTracking()
            .Where(p => p.BrandId == brandId && p.BranchId == branchId && p.Method == PaymentMethod.Cash
                && p.RefundedUtc != null && p.RefundedUtc >= start && p.RefundedUtc < end)
            .SumAsync(p => p.RefundedAmountMinor, ct);

    private async Task<DateTimeZone?> ZoneAsync(Guid brandId, Guid branchId, BranchScope scope, CancellationToken ct)
    {
        if (!scope.Includes(branchId) || await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct) is not { } zoneId)
        {
            return null;
        }

        return DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
    }

    private LocalDate Today(DateTimeZone zone) => clock.GetCurrentInstant().InZone(zone).Date;

    private static (Instant Start, Instant End) Bounds(DateTimeZone zone, LocalDate date) =>
        (zone.AtStartOfDay(date).ToInstant(), zone.AtStartOfDay(date.PlusDays(1)).ToInstant());

    private static Money Amount(long amountMinor) => new(amountMinor, CashCurrency);

    private static LocalDate? ParseDate(string field, string value, Dictionary<string, string[]> errors)
    {
        var parsed = LocalDatePattern.Iso.Parse(value);
        if (parsed.Success)
        {
            return parsed.Value;
        }

        errors[field] = ["invalid_format"];
        return null;
    }
}
