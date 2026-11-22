using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Messaging;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Deposits;

/// <summary>
/// "Kapora geldi" dışındaki kapora işlemleri (dilim-04 kabul kriteri 4; V6, V7): müşterinin "Havaleyi yaptım" bildirimi
/// (publicReportDepositTransfer), salonun "Hatırlat"ı (manageRemindDeposit) ve "Salon iade etti" (manageMarkDepositRefunded).
/// Randevu durumu hiçbirinde değişmez; yan etkiler outbox'a aynı transaction'da yazılır. İadede tahsilat kaydı Sales'te
/// <see cref="IDepositPayments.MarkDepositRefundedAsync"/> ile aynı transaction'da güncellenir.
/// </summary>
internal sealed class DepositFollowUps(
    SchedulingDbContext db, IDepositPayments payments, ICreditLedger credits, ISubscriptionQueries subscriptions, IOutbox outbox, IClock clock)
{
    /// <summary>
    /// "Havaleyi yaptım, salona bildir". Randevu <see cref="Features.AppointmentLink.AppointmentLinkResolver"/> ile yüklenmiş
    /// olmalı. İlk bildirimde <see cref="AppointmentDepositReported"/> yazılır; tekrar çağrı aynı sonucu döner, olay yazmaz.
    /// </summary>
    // Kural: dilim-04 kabul kriteri 4
    public async Task<Result> ReportTransferAsync(Appointment appointment, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(appointment);
        var reported = appointment.ReportDepositTransfer(clock.GetCurrentInstant());
        if (!reported.IsSuccess)
        {
            return reported.Error;
        }

        if (!reported.Value)
        {
            return Result.Success();
        }

        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(
            new AppointmentDepositReported(
                appointment.BrandId, appointment.Id, appointment.BranchId, appointment.BrandCustomerId, appointment.DepositRequiredMinor ?? 0),
            ct);
        return Result.Success();
    }

    /// <summary>
    /// "Hatırlat": salt okunur abonelik (403 subscription_read_only; salonun başlattığı SMS, K5) → durum korumaları (409 invalid_state_transition / hold_expired / deposit_reminder_already_sent) → kontör
    /// (yoksa 409 insufficient_credits; hak harcanmaz, hiçbir şey yazılmaz) → işaret → <see cref="AppointmentDepositReminded"/>.
    /// Kilit alınmaz; eşzamanlı iki hatırlatmadan biri xmin ile 412 alır.
    /// </summary>
    // Kural: V6 (randevu başına en fazla bir kez)
    public async Task<Result> RemindAsync(Guid appointmentId, ManageAccess access, Guid? userId, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(access);
        var appointment = await LoadAsync(appointmentId, access, ct);
        if (appointment is null)
        {
            return Error.NotFound();
        }

        // Kural: durum-makinesi.md § 8 ReadOnly ("SMS kapalı"); K5: salonun yeni başlattığı SMS kapalı, var olan hatırlatmalar sürer.
        if (await subscriptions.GetBannerAsync(appointment.BrandId, ct) is { BlocksNewWork: true })
        {
            return Error.Forbidden(ErrorCodes.SubscriptionReadOnly);
        }

        var now = clock.GetCurrentInstant();
        var allowed = appointment.CanRemindDeposit(now);
        if (!allowed.IsSuccess)
        {
            return allowed;
        }

        // Kapora SMS'i tek parçadır (V2 kırpma); bakiye bir kontörün altındaysa gönderilemez.
        if (await credits.GetBalanceAsync(appointment.BrandId, ct) < 1)
        {
            return Error.Conflict(ErrorCodes.InsufficientCredits);
        }

        var reminded = appointment.RemindDeposit(now, userId);
        if (!reminded.IsSuccess)
        {
            return reminded;
        }

        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(new AppointmentDepositReminded(appointment.BrandId, appointment.Id), ct);
        return Result.Success();
    }

    /// <summary>
    /// "Salon iade etti": yükle → kapsam (404) → If-Match (412) → gövde biçimi (400) → randevu kuralı (409 / 400 out_of_range)
    /// → Sales tahsilatı Refunded / PartiallyRefunded → kaydet. Müşteriye bildirim yazılmaz.
    /// </summary>
    // Kural: V7
    public async Task<Result<Appointment>> MarkRefundedAsync(
        Guid appointmentId, DepositRefundRequest body, ManageAccess access, Guid? userId, uint expectedVersion, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(access);
        var appointment = await LoadAsync(appointmentId, access, ct);
        if (appointment is null)
        {
            return Error.NotFound();
        }

        if (!db.MatchesVersion(appointment, appointment.Version, expectedVersion))
        {
            return Error.PreconditionFailed();
        }

        var currency = Enum.TryParse<Currency>(appointment.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        var errors = body.Validate(currency);
        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var amount = new Money(body.Amount!.AmountMinor!.Value, currency);
        var marked = appointment.MarkDepositRefunded(amount.AmountMinor, body.Note, clock.GetCurrentInstant(), userId);
        if (!marked.IsSuccess)
        {
            return marked.Error;
        }

        var paymentId = appointment.DepositPaymentId ?? throw new DomainException("Alınmış kaporanın tahsilat kaydı yok.");
        await payments.MarkDepositRefundedAsync(paymentId, amount, ct);
        await db.SaveChangesAsync(ct);
        return appointment;
    }

    private async Task<Appointment?> LoadAsync(Guid appointmentId, ManageAccess access, CancellationToken ct)
    {
        var appointment = await db.Appointments.Include(a => a.Items).ThenInclude(i => i.Segments).SingleOrDefaultAsync(a => a.Id == appointmentId, ct);
        return appointment is not null && access.Covers(appointment) ? appointment : null;
    }
}
