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
/// → biçim (400) → durum (409 invalid_state_transition) → "Paketten" (kalem 400; seans Sales'te ayrılır, uygun değilse 409
/// package_session_unavailable) → toplam (409 payment_total_mismatch) → geçiş → Sales'te tahsilatlar, 0 TL'lik paket kayıtları ve
/// hizmet primi (<see cref="IAppointmentPayments"/>) →
/// müşteri sayaçları → kaydet → <see cref="AppointmentCompleted"/>. Hepsi aynı bağlantı ve transaction'dadır: randevunun yazımı
/// düşerse (412) tahsilat ve prim de geri alınır.
/// </summary>
internal sealed class AppointmentCompletions(
    SchedulingDbContext db,
    IAppointmentPayments payments,
    IPackageSessions packageSessions,
    ICustomerDirectory customers,
    ITenantQueries tenants,
    IOutbox outbox,
    IClock clock)
{

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

        var zone = await ZoneAsync(brandId, appointment.BranchId, ct);
        var redeemed = await RedeemAsync(brandId, appointment, body, zone, ct);
        if (!redeemed.IsSuccess)
        {
            return redeemed.Error;
        }

        var lines = body.Lines(currency);
        var retainedDeposit = appointment.RetainedDepositMinor;
        var now = clock.GetCurrentInstant();
        var completed = appointment.Complete(lines.Sum(l => l.Amount.AmountMinor), now, userId);
        if (!completed.IsSuccess)
        {
            return completed.Error;
        }

        var recorded = await RecordAsync(brandId, appointment, currency, lines, retainedDeposit, zone, now, userId, ct);
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

    /// <summary>Biçim (400) ve durum (409) denetimi; paket ve toplam sonra denetlenir.</summary>
    private static Result Precheck(ManageCompleteRequest body, Appointment appointment, Currency currency)
    {
        var errors = body.Validate(currency);
        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        return appointment.CanComplete();
    }

    /// <summary>
    /// "Paketten" (W1): kalemler randevuda paketten düşülür, seanslar Sales'te ayrılır (aynı transaction; toplam tutmazsa birlikte
    /// geri alınır). Kalem yoksa, etkin değilse veya zaten paketten düşülüyorsa 400; seans uygun değilse 409
    /// package_session_unavailable.
    /// </summary>
    // Kural: sözleşme manageCompleteAppointment (packageRedemptions); durum-makinesi.md A11
    private async Task<Result> RedeemAsync(Guid brandId, Appointment appointment, ManageCompleteRequest body, DateTimeZone zone, CancellationToken ct)
    {
        var redemptions = body.PackageRedemptions ?? [];
        var claims = new List<PackageSessionClaim>();
        for (var i = 0; i < redemptions.Count; i++)
        {
            var (itemId, packageItemId) = (redemptions[i].AppointmentItemId!.Value, redemptions[i].CustomerPackageItemId!.Value);
            var redeemed = appointment.RedeemFromPackage(itemId, packageItemId, $"packageRedemptions[{i}].appointmentItemId");
            if (!redeemed.IsSuccess)
            {
                return redeemed;
            }

            claims.Add(new PackageSessionClaim(itemId, appointment.Items.Single(x => x.Id == itemId).ServiceId, packageItemId));
        }

        return claims.Count == 0
            ? Result.Success()
            : await packageSessions.ReserveAsync(
                new PackageSessionReservation(
                    brandId, appointment.BranchId, appointment.BrandCustomerId, appointment.Id, appointment.StartUtc.InZone(zone).Date, claims),
                ct);
    }

    private async Task<DateTimeZone> ZoneAsync(Guid brandId, Guid branchId, CancellationToken ct)
    {
        var zoneId = await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct) ?? PlatformDefaults.DefaultTimeZoneId;
        return DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
    }

    /// <summary>Sales'te tahsilatlar ve hizmet primi, Customers'ta sayaçlar (aynı transaction).</summary>
    private async Task<Result> RecordAsync(
        Guid brandId,
        Appointment appointment,
        Currency currency,
        IReadOnlyList<CompletionPaymentLine> lines,
        long retainedDeposit,
        DateTimeZone zone,
        Instant now,
        Guid? userId,
        CancellationToken ct)
    {
        var items = appointment.Items.Where(i => i.Status == AppointmentItemStatus.Completed).OrderBy(i => i.SeqNo).ToList();
        await payments.RecordCompletionAsync(
            new AppointmentCompletionRequest(
                brandId,
                appointment.BranchId,
                appointment.BrandCustomerId,
                appointment.Id,
                currency,
                lines,
                [.. items.Select(i => new CompletedItem(i.Id, i.ServiceId, i.StaffId, i.NetAmountMinor, i.CustomerPackageItemId))],
                retainedDeposit,
                now.InZone(zone).Date,
                userId),
            ct);

        return await customers.ApplyCountersAsync(
            brandId,
            appointment.BrandCustomerId,
            new CustomerCounterChange(Visits: 1, SpentMinor: appointment.PayableAmountMinor, VisitedOn: appointment.StartUtc.InZone(zone).Date),
            ct);
    }
}
