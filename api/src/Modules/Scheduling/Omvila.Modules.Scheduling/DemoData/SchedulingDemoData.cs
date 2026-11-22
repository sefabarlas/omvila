using NodaTime;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Scheduling.Features.Approvals;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-74, T-85) için randevu yazma yolu. Randevu <see cref="AppointmentWriter"/> ile
/// yazılır (kilit, çakışma, müsaitlik, kapora çözümü); durum yalnız aggregate metodlarıyla değişir. Geçmiş geçişler (Geldi,
/// Gelmedi, iptal, tamamlama) randevunun kendi zamanına göre kaydedilir. Kaporalı sonuçlar rezervasyon sayfasındaki gibi online yazılır
/// (aktif hesabın öneki ve havale süresiyle); "Kapora geldi" tahsilatı Sales <see cref="IDepositPayments"/> ile aynı
/// transaction'da. Bekleme kaydı ve kapıda sıra bileti domain metodlarıyla (numara <see cref="BranchQueueNumbers"/>). Entegrasyon
/// olayı yazılmaz: demo kurulumu müşteriye SMS, hatırlatma veya personel bildirimi üretmez.
/// Çağıranın transaction'ında ve markanın kiracı bağlamında çalışır.
/// </summary>
public sealed class SchedulingDemoData
{
    private readonly SchedulingDbContext _db;
    private readonly AppointmentWriter _writer;
    private readonly ITenantQueries _tenants;
    private readonly IBrandBankAccounts _bankAccounts;
    private readonly ICustomerDirectory _customers;
    private readonly IDepositPayments _payments;
    private readonly BranchQueueNumbers _queueNumbers;
    private readonly IIdGenerator _ids;

    internal SchedulingDemoData(
        SchedulingDbContext db, AppointmentWriter writer, ITenantQueries tenants, IBrandBankAccounts bankAccounts, ICustomerDirectory customers,
        IDepositPayments payments, BranchQueueNumbers queueNumbers, IIdGenerator ids)
    {
        _queueNumbers = queueNumbers;
        _ids = ids;
        _db = db;
        _writer = writer;
        _tenants = tenants;
        _bankAccounts = bankAccounts;
        _customers = customers;
        _payments = payments;
    }

    /// <summary>Randevuyu yazar ve istenen duruma getirir; yazılamazsa (ör. slot_taken) <see cref="DomainException"/>.</summary>
    /// <param name="input">Randevu.</param>
    /// <param name="now">Seed'in saati (IClock); iptal ve kapora anı için.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task<DemoAppointmentResult> AddAsync(DemoAppointment input, Instant now, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var settings = await _tenants.GetAppointmentSettingsAsync(input.BrandId, ct)
            ?? throw new DomainException("Markanın randevu ayarları yok.");
        var withDeposit = input.Outcome is DemoAppointmentOutcome.DepositAwaiting or DemoAppointmentOutcome.DepositReported
            or DemoAppointmentOutcome.DepositReceived or DemoAppointmentOutcome.DepositWaived;
        var online = withDeposit || input.Outcome == DemoAppointmentOutcome.PendingApproval;
        var deposit = withDeposit ? await DepositTermsAsync(input, ct) : null;
        var slot = new SlotRequest(
            input.BranchId, input.StartUtc, [new RequestedItem(input.ServiceId, input.StaffId)], SameStaffForAll: false,
            online ? AvailabilityMode.Public : AvailabilityMode.Panel);
        var command = new CreateAppointmentCommand(
            input.BrandId, slot, input.CustomerId, input.BrandCustomerId,
            online ? AppointmentChannel.Online : AppointmentChannel.Phone,
            online ? ClientApp.BookingPage : ClientApp.Panel,
            CustomerNote: null, StaffNote: null, SeriesId: null, CreatedByUserId: null,
            Approval: online ? settings.Policy() : null,
            Deposit: deposit);
        var written = await _writer.CreateAsync(command, ct);
        if (!written.IsSuccess)
        {
            throw new DomainException($"Demo randevusu yazılamadı: {written.Error.Code}");
        }

        var appointment = written.Value.Appointment;
        if (withDeposit && appointment.DepositStatus != DepositStatus.Awaiting)
        {
            throw new DomainException("Demo randevusunda kapora çözülmedi (hizmet kaporası, politika ve müşterinin gelmeme sayısı).");
        }

        var noShowAfter = Duration.FromMinutes(settings.NoShowAfterMinutes);
        var collected = 0L;
        var retained = 0L;
        var changed = input.Outcome switch
        {
            DemoAppointmentOutcome.Confirmed => Expect(appointment, AppointmentStatus.Confirmed),
            DemoAppointmentOutcome.PendingApproval or DemoAppointmentOutcome.DepositAwaiting => Expect(appointment, AppointmentStatus.PendingApproval),
            DemoAppointmentOutcome.CheckedIn => appointment.CheckIn(input.StartUtc, userId: null),
            DemoAppointmentOutcome.NoShow => appointment.MarkNoShow(input.StartUtc + noShowAfter, noShowAfter),
            DemoAppointmentOutcome.CancelledBySalon => appointment.CancelBySalon(
                CancellationReason.CustomerRequested, note: null, userId: null, input.StartUtc > now ? now : input.StartUtc - Duration.FromHours(2)),
            DemoAppointmentOutcome.CancelledByCustomer => appointment.CancelByCustomer(
                CancellationReason.PlanChanged, note: null, now, Duration.FromHours(settings.CancellationCutoffHours)),
            DemoAppointmentOutcome.DepositReported => Reported(appointment.ReportDepositTransfer(now)),
            DemoAppointmentOutcome.DepositReceived => await MarkReceivedAsync(appointment, now, ct),
            DemoAppointmentOutcome.DepositWaived => appointment.Approve(now, userId: null, waiveDeposit: true),
            DemoAppointmentOutcome.Completed => Complete(appointment, input, out collected, out retained),
            _ => throw new ArgumentOutOfRangeException(nameof(input), input.Outcome, null),
        };
        if (!changed.IsSuccess)
        {
            throw new DomainException($"Demo randevusu {input.Outcome} durumuna geçemedi: {changed.Error.Code}");
        }

        await _db.SaveChangesAsync(ct);
        return await FinishAsync(input, appointment, collected, retained, ct);
    }

    /// <summary>Müşteri sayaçları ve sonuç (tamamlamada tahsilatın girdisi).</summary>
    private async Task<DemoAppointmentResult> FinishAsync(DemoAppointment input, Appointment appointment, long collected, long retained, CancellationToken ct)
    {
        var item = appointment.Items.Single();
        var zoneId = await _tenants.GetBranchTimeZoneAsync(input.BrandId, input.BranchId, ct) ?? PlatformDefaults.DefaultTimeZoneId;
        var localDate = input.StartUtc.InZone(DateTimeZoneProviders.Tzdb[zoneId]).Date;
        await ApplyCountersAsync(input, appointment.PayableAmountMinor, localDate, ct);
        var currency = Enum.TryParse<Currency>(appointment.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        return new DemoAppointmentResult(
            appointment.Id, item.Id, appointment.EndUtc, localDate, currency, item.NetAmountMinor, collected, retained, item.CustomerPackageItemId);
    }

    /// <summary>
    /// Demo bekleme kaydı (PublicWaitlist/WaitlistManagement gibi, tercih denetimi uçta olduğundan burada yapılmaz).
    /// </summary>
    /// <param name="input">Kayıt.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task<Guid> AddWaitlistEntryAsync(DemoWaitlistEntry input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var preferences = new WaitlistPreferences(
            input.ServiceIds, input.PreferredStaffId, input.AcceptOtherStaff, input.Dates, [.. input.TimeWindows.Select(Enum.Parse<WaitlistTimeWindow>)]);
        var entry = WaitlistEntry.Create(
            _ids.New(), input.BrandId, input.BranchId, input.CustomerId, input.BrandCustomerId, preferences, input.Note,
            input.Online ? WaitlistSource.Online : WaitlistSource.Panel, AccessCodeGenerator.New(), input.JoinedUtc);
        _db.WaitlistEntries.Add(entry);
        await _db.SaveChangesAsync(ct);
        return entry.Id;
    }

    /// <summary>Demo kapıda sıra bileti (WalkInTickets.CreateAsync gibi; olay ve bildirim yok). Dönüş: sıra numarası.</summary>
    /// <param name="input">Bilet.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task<int> AddWalkInTicketAsync(DemoWalkInTicket input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var zoneId = await _tenants.GetBranchTimeZoneAsync(input.BrandId, input.BranchId, ct)
            ?? throw new DomainException("Demo biletinin şubesi yok.");
        PhoneNumber? phone = null;
        if (input.BrandCustomerId is { } cardId
            && (await _customers.GetDisplayInfoAsync(input.BrandId, [cardId], ct)).TryGetValue(cardId, out var card)
            && card.AppointmentSmsEnabled && PhoneNumber.TryParse(card.PhoneE164, out var cardPhone))
        {
            phone = cardPhone;
        }

        var date = input.CreatedUtc.InZone(DateTimeZoneProviders.Tzdb[zoneId]).Date;
        var number = await _queueNumbers.NextAsync(input.BrandId, input.BranchId, date, ct);
        _db.WalkInTickets.Add(WalkInTicket.Create(
            new NewWalkInTicket(
                input.BrandId, input.BranchId, input.CustomerName, phone, input.BrandCustomerId, input.ServiceId, input.ServiceName, input.ServiceMinutes,
                input.PreferredStaffId, CreatedByUserId: null),
            _ids.New(),
            date,
            number,
            input.CreatedUtc));
        await _db.SaveChangesAsync(ct);
        return number;
    }

    /// <summary>
    /// Geldi (başlangıçta) → paket kalemi verilmişse paketten düşüm → bitişte A11; salonda alınan "Salonda ödenecek"in tamamı.
    /// Seans kullanımı ve tahsilat Sales demo verisindedir.
    /// </summary>
    private static Result Complete(Appointment appointment, DemoAppointment input, out long collected, out long retained)
    {
        collected = 0;
        retained = 0;
        var checkedIn = appointment.CheckIn(input.StartUtc, userId: null);
        if (!checkedIn.IsSuccess)
        {
            return checkedIn;
        }

        if (input.CustomerPackageItemId is { } packageItemId)
        {
            var redeemed = appointment.RedeemFromPackage(appointment.Items.Single().Id, packageItemId, "customerPackageItemId");
            if (!redeemed.IsSuccess)
            {
                return redeemed;
            }
        }

        collected = appointment.AmountDueMinor;
        retained = appointment.RetainedDepositMinor;
        return appointment.Complete(collected, appointment.EndUtc, userId: null);
    }

    private async Task ApplyCountersAsync(DemoAppointment input, long payableMinor, LocalDate localDate, CancellationToken ct)
    {
        var counters = input.Outcome switch
        {
            DemoAppointmentOutcome.NoShow => new CustomerCounterChange(NoShows: 1),
            DemoAppointmentOutcome.CancelledByCustomer => new CustomerCounterChange(Cancels: 1),
            DemoAppointmentOutcome.Completed => new CustomerCounterChange(Visits: 1, SpentMinor: payableMinor, VisitedOn: localDate),
            _ => null,
        };
        if (counters is not null && !(await _customers.ApplyCountersAsync(input.BrandId, input.BrandCustomerId, counters, ct)).IsSuccess)
        {
            throw new DomainException("Demo müşterisinin sayacı güncellenemedi.");
        }
    }

    private async Task<DepositTerms> DepositTermsAsync(DemoAppointment input, CancellationToken ct)
    {
        var account = await _bankAccounts.GetActiveAsync(input.BrandId, ct)
            ?? throw new DomainException("Kaporalı demo randevusu için markanın aktif kapora hesabı yok.");
        return new DepositTerms(input.CustomerNoShowCount, account.TransferCodePrefix, Duration.FromMinutes(account.TransferWindowMinutes));
    }

    /// <summary>"Kapora geldi" (DepositReceipts gibi): önce Sales'te tahsilat, sonra A3.</summary>
    private async Task<Result> MarkReceivedAsync(Appointment appointment, Instant now, CancellationToken ct)
    {
        var allowed = appointment.CanMarkDepositReceived(now);
        if (!allowed.IsSuccess)
        {
            return allowed;
        }

        var currency = Enum.TryParse<Currency>(appointment.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;
        var paymentId = await _payments.RecordDepositAsync(
            new DepositPaymentRequest(
                appointment.BrandId, appointment.BranchId, appointment.BrandCustomerId, appointment.Id,
                new Money(appointment.DepositRequiredMinor!.Value, currency), appointment.DepositTransferCode, CollectedByUserId: null),
            ct);
        return appointment.MarkDepositReceived(paymentId, now, userId: null);
    }

    private static Result Reported(Result<bool> reported) => reported.IsSuccess ? Result.Success() : reported.Error;

    private static Result Expect(Appointment appointment, AppointmentStatus status) =>
        appointment.Status == status ? Result.Success() : Error.Conflict(ErrorCodes.InvalidStateTransition);
}
