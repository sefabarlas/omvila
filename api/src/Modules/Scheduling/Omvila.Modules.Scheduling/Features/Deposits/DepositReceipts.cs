using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Deposits;

/// <summary>
/// "Kapora geldi" (A3; dilim-04 kabul kriteri 5; sözleşme manageMarkDepositReceived). Sıra: yükle → kapsam (404) → If-Match
/// (412) → geçiş koruması (409 hold_expired / invalid_state_transition) → Sales'te tahsilat kaydı (<see cref="IDepositPayments"/>)
/// → geçiş → kaydet → outbox. Tahsilat ve randevu aynı bağlantı ve transaction'dadır: randevunun yazımı başarısız olursa
/// (ör. otomatik iptalle yarışta eşzamanlılık hatası, 412) tahsilat da geri alınır. Kilit alınmaz; yarış onaydaki gibi xmin
/// ile çözülür (T-63).
/// </summary>
internal sealed class DepositReceipts(SchedulingDbContext db, IDepositPayments payments, IOutbox outbox, IClock clock)
{
    // Kural: durum-makinesi.md A3
    public async Task<Result<Appointment>> MarkReceivedAsync(Guid appointmentId, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
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

        var now = clock.GetCurrentInstant();
        var allowed = appointment.CanMarkDepositReceived(now);
        if (!allowed.IsSuccess)
        {
            return allowed.Error;
        }

        var amount = appointment.DepositRequiredMinor ?? throw new DomainException("Kapora beklenen randevuda tutar yok.");
        var currency = Enum.TryParse<Currency>(appointment.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        var paymentId = await payments.RecordDepositAsync(
            new DepositPaymentRequest(
                appointment.BrandId,
                appointment.BranchId,
                appointment.BrandCustomerId,
                appointment.Id,
                new Money(amount, currency),
                appointment.DepositTransferCode,
                userId),
            ct);

        var marked = appointment.MarkDepositReceived(paymentId, now, userId);
        if (!marked.IsSuccess)
        {
            return marked.Error;
        }

        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(
            new AppointmentDepositReceived(
                appointment.BrandId, appointment.Id, appointment.BranchId, appointment.BrandCustomerId, appointment.StartUtc, amount, userId),
            ct);
        return appointment;
    }
}
