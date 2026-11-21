using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Completion;

/// <summary>
/// "Tamamla ve ödeme al" (A11; dilim-05 KK5; sözleşme manageCompleteAppointment). Sıra: yükle → kapsam (404) → If-Match (412)
/// → biçim (400) → durum (409 invalid_state_transition) → paket (409 package_session_unavailable; paketler henüz yok, T-88) →
/// toplam (409 payment_total_mismatch) → geçiş → Sales'te tahsilatlar ve hizmet primi (<see cref="IAppointmentPayments"/>) →
/// müşteri sayaçları → kaydet → <see cref="AppointmentCompleted"/>. Hepsi aynı bağlantı ve transaction'dadır: randevunun yazımı
/// düşerse (412) tahsilat ve prim de geri alınır.
/// </summary>
internal sealed class AppointmentCompletions(
    SchedulingDbContext db, IAppointmentPayments payments, ICustomerDirectory customers, ITenantQueries tenants, IOutbox outbox, IClock clock)
{
    public const string CustomerPackageItemMeta = "customerPackageItemId";

    // Kural: durum-makinesi.md A11
    public async Task<Result<Appointment>> CompleteAsync(
        Guid brandId, Guid appointmentId, ManageCompleteRequest body, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(access);
        var appointment = await db.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments).SingleOrDefaultAsync(a => a.Id == appointmentId, ct);
        if (appointment is null || !access.Covers(appointment))
        {
            return Error.NotFound();
        }

        if (!db.MatchesVersion(appointment, appointment.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var currency = Enum.TryParse<Currency>(appointment.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        var checkedBody = Precheck(body, appointment, currency);
        if (!checkedBody.IsSuccess)
        {
            return checkedBody.Error;
        }

        var lines = body.Lines(currency);
        var retainedDeposit = appointment.RetainedDepositMinor;
        var now = clock.GetCurrentInstant();
        var completed = appointment.Complete(lines.Sum(l => l.Amount.AmountMinor), now, userId);
        if (!completed.IsSuccess)
        {
            return completed.Error;
        }

        var recorded = await RecordAsync(brandId, appointment, currency, lines, retainedDeposit, now, userId, ct);
        if (!recorded.IsSuccess)
        {
            return recorded.Error;
        }

        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(
            new AppointmentCompleted(
                brandId,
                appointment.Id,
                appointment.BranchId,
                appointment.BrandCustomerId,
                now,
                [.. appointment.Items.Where(i => i.Status == AppointmentItemStatus.Completed && i.StaffId is not null)
                    .OrderBy(i => i.SeqNo).Select(i => i.StaffId!.Value).Distinct()],
                userId),
            ct);
        return appointment;
    }

    /// <summary>Biçim (400), durum (409) ve paket (409) denetimi; toplam geçişte denetlenir.</summary>
    private static Result Precheck(ManageCompleteRequest body, Appointment appointment, Currency currency)
    {
        var errors = body.Validate(currency);
        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var allowed = appointment.CanComplete();
        if (!allowed.IsSuccess)
        {
            return allowed;
        }

        // Paket seansı (W1) paket PR'ıyla gelir; o zamana kadar düşülecek paket kalemi hiçbir müşteride bulunmaz.
        return body.PackageRedemptions is [var redemption, ..]
            ? Error.Conflict(
                ErrorCodes.PackageSessionUnavailable,
                new Dictionary<string, object?>(StringComparer.Ordinal) { [CustomerPackageItemMeta] = redemption.CustomerPackageItemId })
            : Result.Success();
    }

    /// <summary>Sales'te tahsilatlar ve hizmet primi, Customers'ta sayaçlar (aynı transaction).</summary>
    private async Task<Result> RecordAsync(
        Guid brandId,
        Appointment appointment,
        Currency currency,
        IReadOnlyList<CompletionPaymentLine> lines,
        long retainedDeposit,
        Instant now,
        Guid? userId,
        CancellationToken ct)
    {
        var zoneId = await tenants.GetBranchTimeZoneAsync(brandId, appointment.BranchId, ct) ?? PlatformDefaults.DefaultTimeZoneId;
        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var items = appointment.Items.Where(i => i.Status == AppointmentItemStatus.Completed).OrderBy(i => i.SeqNo).ToList();
        await payments.RecordCompletionAsync(
            new AppointmentCompletionRequest(
                brandId,
                appointment.BranchId,
                appointment.BrandCustomerId,
                appointment.Id,
                currency,
                lines,
                [.. items.Select(i => new CompletedItem(i.Id, i.ServiceId, i.StaffId, i.NetAmountMinor))],
                retainedDeposit,
                now.InZone(zone).Date,
                userId),
            ct);

        return await customers.ApplyCountersAsync(
            brandId,
            appointment.BrandCustomerId,
            new CustomerCounterChange(Visits: 1, SpentMinor: appointment.TotalAmountMinor, VisitedOn: appointment.StartUtc.InZone(zone).Date),
            ct);
    }
}
