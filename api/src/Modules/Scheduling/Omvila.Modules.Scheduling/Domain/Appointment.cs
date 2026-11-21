using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Randevu kabı (alan-modeli.md § Appointment). Durum yalnız bu sınıfın metotlarıyla değişir; geçiş tablosu
/// durum-makinesi.md § 1. Saat, ayar değerleri ve kimlik üretici parametre olarak gelir; sınıf veritabanı ve saat
/// bilmez. Müsaitlik ve çakışma burada denetlenmez (motor, kilit ve exclusion constraint).
/// </summary>
internal sealed class Appointment
{
    private static readonly Domain.CancellationReason[] CustomerReasons =
        [Domain.CancellationReason.PlanChanged, Domain.CancellationReason.Sick, Domain.CancellationReason.WantsDifferentTime, Domain.CancellationReason.Other];

    // DeclinedBySalon yalnız onay bekleyen talebin reddidir (A4, Dilim 3); sözleşme ManageCancelRequest.reason.
    private static readonly Domain.CancellationReason[] SalonReasons =
        [Domain.CancellationReason.CustomerRequested, Domain.CancellationReason.StaffUnavailable, Domain.CancellationReason.Other];

    private readonly List<AppointmentItem> _items = [];

    private Appointment()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid BrandCustomerId { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public AppointmentChannel Channel { get; private set; }

    public ClientApp? ClientApp { get; private set; }

    /// <summary>Kalemlerden türetilen özet; kalem değişince güncellenir.</summary>
    public Instant StartUtc { get; private set; }

    public Instant EndUtc { get; private set; }

    /// <summary>Kalem fiyatları − indirim.</summary>
    public long TotalAmountMinor { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    /// <summary>"Salona notunuz". Günlüğe yazılmaz.</summary>
    public string? CustomerNote { get; private set; }

    /// <summary>Yalnız salon görür. Günlüğe yazılmaz.</summary>
    public string? StaffNote { get; private set; }

    /// <summary>Yalnız PendingApproval'da dolu (değişmez).</summary>
    public Instant? HoldExpiresUtc { get; private set; }

    public HoldReason? HoldReason { get; private set; }

    public Guid? SeriesId { get; private set; }

    public Instant? ConfirmedUtc { get; private set; }

    public Instant? CheckedInUtc { get; private set; }

    public Instant? CompletedUtc { get; private set; }

    /// <summary>Gelmedi işaretlendiği an. "Geldiyse geri al" sonrasında da olgu olarak kalır (T-47).</summary>
    public Instant? NoShowUtc { get; private set; }

    public Instant? CancelledUtc { get; private set; }

    public CancelledBy? CancelledBy { get; private set; }

    public Guid? CancelledByUserId { get; private set; }

    public CancellationReason? CancellationReason { get; private set; }

    public string? CancellationNote { get; private set; }

    /// <summary>İstenen kapora; <c>null</c> = kapora yok.</summary>
    public long? DepositRequiredMinor { get; private set; }

    /// <summary>Havale açıklama kodu ("OMV-7K2X"); marka içinde benzersiz. Kapora yoksa <c>null</c>.</summary>
    public string? DepositTransferCode { get; private set; }

    /// <summary>Müşteri "Havaleyi yaptım" dedi (Dilim 4).</summary>
    public Instant? DepositCustomerReportedUtc { get; private set; }

    /// <summary>Salon "Kapora geldi" dedi (A3).</summary>
    public Instant? DepositReceivedUtc { get; private set; }

    public Guid? DepositReceivedByUserId { get; private set; }

    /// <summary>"Kapora geldi" denince Sales'te oluşan tahsilat kaydı (Payment).</summary>
    public Guid? DepositPaymentId { get; private set; }

    /// <summary>Salon "Kaporasız onayla" dedi (A2, <c>waiveDeposit</c>): kapora istenmiyor olarak kaydedilir (dilim-04 kabul kriteri 7).</summary>
    public Instant? DepositWaivedUtc { get; private set; }

    public Guid? DepositWaivedByUserId { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Guid? CreatedByUserId { get; private set; }

    public Instant? UpdatedUtc { get; private set; }

    public Guid? UpdatedByUserId { get; private set; }

    public uint Version { get; private set; }

    public IReadOnlyList<AppointmentItem> Items => _items;

    /// <summary>Arayüz gösterimi (durum-makinesi.md § Arayüz gösterimi).</summary>
    public DisplayStatus DisplayStatus => DisplayStatusRule.For(Status, DepositRequiredMinor, DepositReceivedUtc);

    /// <summary>"Havaleyi yaptım" dendi mi (sözleşme depositCustomerReported).</summary>
    public bool DepositCustomerReported => DepositCustomerReportedUtc is not null;

    /// <summary>Kapora durumu (sözleşme DepositStatus); kapora istenmemişse <c>null</c>.</summary>
    public DepositStatus? DepositStatus => DepositRequiredMinor switch
    {
        null => null,
        _ when DepositWaivedUtc is not null => Domain.DepositStatus.Waived,
        _ when DepositReceivedUtc is not null => Domain.DepositStatus.Received,
        _ when DepositCustomerReportedUtc is not null => Domain.DepositStatus.CustomerReported,
        _ => Domain.DepositStatus.Awaiting,
    };

    /// <summary>
    /// "Salonda ödenecek": toplam − alınmış kapora (dilim-04 § Kapsam; kabul kriteri 5). Kapora gelmeden ve kaporasız onayda
    /// toplamdır.
    /// </summary>
    public long AmountDueMinor => DepositReceivedUtc is not null && DepositRequiredMinor is { } deposit
        ? Math.Max(0, TotalAmountMinor - deposit)
        : TotalAmountMinor;

    // Kural: docs/alan/durum-makinesi.md#oluşturma-kuralı (A1)
    public static Appointment Create(NewAppointment input, AppointmentPlan plan, IIdGenerator ids, Instant now)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(ids);

        var appointment = new Appointment
        {
            Id = ids.New(),
            BrandId = input.BrandId,
            BranchId = input.BranchId,
            CustomerId = input.CustomerId,
            BrandCustomerId = input.BrandCustomerId,
            Channel = input.Channel,
            ClientApp = input.ClientApp,
            CurrencyCode = plan.Total.Currency.ToString(),
            CustomerNote = Clean(input.CustomerNote),
            StaffNote = Clean(input.StaffNote),
            SeriesId = input.SeriesId,
            CreatedUtc = now,
            CreatedByUserId = input.CreatedByUserId,
        };

        foreach (var itemPlan in plan.Items)
        {
            appointment._items.Add(AppointmentItem.Create(ids.New(), input.BrandId, appointment.Id, itemPlan, ids.New));
        }

        appointment.Summarize();

        // Kapıda sıradan başlatılan randevu doğrudan Geldi'dir; panel ve telefon onay ve kapora istemez; online kapora veya
        // onay gerekiyorsa bekler. Kapora ve onay birlikte gerekiyorsa tek tutma süresi havale süresidir.
        if (input.Channel == AppointmentChannel.WalkIn)
        {
            appointment.Status = AppointmentStatus.CheckedIn;
            appointment.CheckedInUtc = now;
        }
        else if (input.Channel == AppointmentChannel.Online && input.Deposit is { AmountMinor: > 0 } deposit)
        {
            appointment.Status = AppointmentStatus.PendingApproval;
            appointment.HoldExpiresUtc = now + deposit.TransferWindow;
            appointment.HoldReason = input.ApprovalTimeout is null ? Domain.HoldReason.Deposit : Domain.HoldReason.ApprovalAndDeposit;
            appointment.DepositRequiredMinor = deposit.AmountMinor;
            appointment.DepositTransferCode = deposit.TransferCode;
        }
        else if (input.Channel == AppointmentChannel.Online && input.ApprovalTimeout is { } timeout)
        {
            appointment.Status = AppointmentStatus.PendingApproval;
            appointment.HoldExpiresUtc = now + timeout;
            appointment.HoldReason = Domain.HoldReason.Approval;
        }
        else
        {
            appointment.Status = AppointmentStatus.Confirmed;
            appointment.ConfirmedUtc = now;
        }

        return appointment;
    }

    /// <summary>
    /// Açıklama kodu marka içinde başka bir randevuda kullanılmışsa (benzersiz indeks) yazma yolu yeni kodla yeniden dener.
    /// Yalnız kapora bekleyen, henüz kaydedilmemiş randevuda anlamlıdır.
    /// </summary>
    public void ReplaceTransferCode(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        if (DepositTransferCode is null)
        {
            throw new DomainException("Kapora istenmeyen randevuda açıklama kodu yok.");
        }

        DepositTransferCode = code;
    }

    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A6)
    public Result CancelByCustomer(CancellationReason reason, string? note, Instant now, Duration cancellationCutoff)
    {
        if (!CustomerReasons.Contains(reason))
        {
            return Error.Validation("reason", "invalid_format");
        }

        if (reason == Domain.CancellationReason.Other && Clean(note) is null)
        {
            return Error.Validation("note", "required");
        }

        if (Status is not (AppointmentStatus.PendingApproval or AppointmentStatus.Confirmed))
        {
            return InvalidTransition();
        }

        // Onay bekleyen talepte sınır yoktur; onaylı randevuda başlangıca kalan süre sınırdan az olamaz.
        if (Status == AppointmentStatus.Confirmed && StartUtc - now < cancellationCutoff)
        {
            return Error.Conflict(ErrorCodes.CutoffPassed);
        }

        Cancel(Domain.CancelledBy.Customer, reason, note, userId: null, now);
        return Result.Success();
    }

    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A7). Salon sınıra tabi değildir (S-04).
    public Result CancelBySalon(CancellationReason reason, string? note, Guid? userId, Instant now)
    {
        if (!SalonReasons.Contains(reason))
        {
            return Error.Validation("reason", "invalid_format");
        }

        // Geldi durumundaki iptal yönetici istisnasıdır (A12) ve Dilim 2 kapsamında değildir.
        if (Status is not (AppointmentStatus.PendingApproval or AppointmentStatus.Confirmed))
        {
            return InvalidTransition();
        }

        Cancel(Domain.CancelledBy.Salon, reason, note, userId, now);
        return Result.Success();
    }

    /// <summary>
    /// A2: salon onay bekleyen talebi onaylar. Tutma süresi geçmişse (otomatik iptal işi henüz çalışmamış olsa da) 409
    /// <c>hold_expired</c> (T-62 (1)); sınır tam eşitlikte geçmiş sayılır (A5 <c>HoldExpiresUtc &lt;= şimdi</c>). Kapora
    /// bekleniyorsa (Awaiting, CustomerReported) <paramref name="waiveDeposit"/> zorunludur, yoksa 409 <c>deposit_required</c>;
    /// verilirse kapora istenmiyor olarak kaydedilir (Waived), tahsilat oluşmaz. Kapora yoksa <paramref name="waiveDeposit"/>
    /// yok sayılır.
    /// </summary>
    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A2)
    public Result Approve(Instant now, Guid? userId, bool waiveDeposit = false)
    {
        var pending = EnsureLivePending(now);
        if (!pending.IsSuccess)
        {
            return pending;
        }

        if (IsDepositAwaited)
        {
            if (!waiveDeposit)
            {
                return Error.Conflict(ErrorCodes.DepositRequired);
            }

            DepositWaivedUtc = now;
            DepositWaivedByUserId = userId;
        }

        Confirm(now, userId);
        return Result.Success();
    }

    /// <summary>
    /// A3 koruması: onay bekleyen, tutma süresi geçmemiş ve kaporası beklenen randevu. Tahsilat kaydı yazılmadan önce
    /// denetlenir; <see cref="MarkDepositReceived"/> aynı kuralı yeniden uygular.
    /// </summary>
    public Result CanMarkDepositReceived(Instant now)
    {
        var pending = EnsureLivePending(now);
        if (!pending.IsSuccess)
        {
            return pending;
        }

        return IsDepositAwaited ? Result.Success() : InvalidTransition();
    }

    /// <summary>
    /// A3: salon "Kapora geldi" der. Salonun işaretlemesi onay sayılır (onay da gerekiyorsa ayrıca "Onayla" gerekmez):
    /// randevu Confirmed, tutma temizlenir, <paramref name="paymentId"/> Sales'teki tahsilat kaydıdır. Kapora beklenmiyorsa
    /// (yok, Received, Waived) veya durum PendingApproval değilse 409 <c>invalid_state_transition</c>; süre geçmişse
    /// <c>hold_expired</c>.
    /// </summary>
    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A3)
    public Result MarkDepositReceived(Guid paymentId, Instant now, Guid? userId)
    {
        if (paymentId == Guid.Empty)
        {
            throw new DomainException("Kapora tahsilat kimliği boş olamaz.");
        }

        var allowed = CanMarkDepositReceived(now);
        if (!allowed.IsSuccess)
        {
            return allowed;
        }

        DepositReceivedUtc = now;
        DepositReceivedByUserId = userId;
        DepositPaymentId = paymentId;
        Confirm(now, userId);
        return Result.Success();
    }

    private void Confirm(Instant now, Guid? userId)
    {
        Status = AppointmentStatus.Confirmed;
        ConfirmedUtc = now;
        HoldExpiresUtc = null;
        HoldReason = null;
        Touch(now, userId);
    }

    /// <summary>
    /// A4: salon talebi reddeder. Neden sabit <c>DeclinedBySalon</c>, iptal eden Salon, not yok (sözleşme gövdesiz; T-62 (1)).
    /// Segmentler pasifleşir, saat boşalır. Müşteriye "iptal" denir; "reddedildi" yalnız iç neden adıdır.
    /// </summary>
    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A4)
    public Result Decline(Instant now, Guid? userId)
    {
        var pending = EnsureLivePending(now);
        if (!pending.IsSuccess)
        {
            return pending;
        }

        Cancel(Domain.CancelledBy.Salon, Domain.CancellationReason.DeclinedBySalon, note: null, userId, now);
        return Result.Success();
    }

    /// <summary>
    /// A5: süresi dolan tutma sistemce iptal edilir. Kapora istenmişse neden <c>DepositNotReceived</c>, değilse
    /// <c>ApprovalExpired</c>. <c>ExpireAppointmentHolds</c> işi ve yazma yolu (çakışma denetiminden önce aynı personelin
    /// süresi dolmuş tutmaları satır içinde; durum-makinesi.md § Sistem işleri) bunu çağırır ve
    /// <c>AppointmentApprovalExpired</c> olayını yayımlar. Bekleme listesi yan etkisi Dilim 5'tedir.
    /// </summary>
    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A5)
    public Result ExpireHold(Instant now)
    {
        if (Status != AppointmentStatus.PendingApproval || HoldExpiresUtc is not { } expires || expires > now)
        {
            return InvalidTransition();
        }

        var reason = HoldReason is Domain.HoldReason.Deposit or Domain.HoldReason.ApprovalAndDeposit
            ? Domain.CancellationReason.DepositNotReceived
            : Domain.CancellationReason.ApprovalExpired;
        Cancel(Domain.CancelledBy.System, reason, note: null, userId: null, now);
        return Result.Success();
    }

    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A8)
    public Result MarkNoShow(Instant now, Duration noShowAfter)
    {
        if (Status != AppointmentStatus.Confirmed || CheckedInUtc is not null || StartUtc + noShowAfter > now)
        {
            return InvalidTransition();
        }

        Status = AppointmentStatus.NoShow;
        NoShowUtc = now;
        UpdatedUtc = now;
        UpdatedByUserId = null;
        foreach (var item in _items)
        {
            item.DeactivateSegments();
        }

        return Result.Success();
    }

    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A9). "Aynı yerel gün" randevunun başladığı şube günüdür (T-47).
    public Result UndoNoShow(Instant now, DateTimeZone branchZone, Guid? userId)
    {
        ArgumentNullException.ThrowIfNull(branchZone);
        if (Status != AppointmentStatus.NoShow
            || StartUtc.InZone(branchZone).Date != now.InZone(branchZone).Date)
        {
            return InvalidTransition();
        }

        Status = AppointmentStatus.CheckedIn;
        CheckedInUtc = now;
        Touch(now, userId);
        // Dilimler yeniden aktif; arada saat başkasına verildiyse yazma sırasında exclusion constraint 409 döner.
        foreach (var item in _items)
        {
            item.ReactivateSegments();
        }

        return Result.Success();
    }

    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A10)
    public Result CheckIn(Instant now, Guid? userId)
    {
        if (Status != AppointmentStatus.Confirmed
            || now < StartUtc - Duration.FromMinutes(PlatformDefaults.CheckInEarliestMinutes))
        {
            return InvalidTransition();
        }

        Status = AppointmentStatus.CheckedIn;
        CheckedInUtc = now;
        Touch(now, userId);
        return Result.Success();
    }

    /// <summary>
    /// A14: saat (ve gerekirse personel) değişikliği. <paramref name="plan"/> kalemleri mevcut kalemlerle aynı sırada ve
    /// aynı hizmetlerdedir. Müşteri <paramref name="rescheduleCutoff"/> sınırına yalnız onaylı randevuda tabidir (onay
    /// bekleyen talepte sınır yok; T-52 (6)), salon hiç tabi değildir (S-04). Durum aynı kalır; yalnız müşteri değiştirdiğinde
    /// ve yeni plan onay istiyorsa (<paramref name="approvalHold"/> dolu) A13: randevu yeniden <c>PendingApproval</c> olur,
    /// yeni tutma şimdi + süre (onay bekleyen talepte süre yeniden başlar; S-11).
    /// </summary>
    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A13, A14)
    public Result Reschedule(
        AppointmentPlan plan, ChangeActor actor, Instant now, Duration rescheduleCutoff, IIdGenerator ids, Guid? userId, Duration? approvalHold = null)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(ids);
        if (Status is not (AppointmentStatus.PendingApproval or AppointmentStatus.Confirmed))
        {
            return InvalidTransition();
        }

        if (actor == ChangeActor.Customer && Status == AppointmentStatus.Confirmed && StartUtc - now < rescheduleCutoff)
        {
            return Error.Conflict(ErrorCodes.CutoffPassed);
        }

        if (plan.Items.Count != _items.Count
            || !plan.Items.Select(p => p.Offer.ServiceId).SequenceEqual(_items.OrderBy(i => i.SeqNo).Select(i => i.ServiceId)))
        {
            throw new DomainException("Saat değişikliği planı randevunun kalemleriyle eşleşmiyor.");
        }

        foreach (var (item, itemPlan) in _items.OrderBy(i => i.SeqNo).Zip(plan.Items))
        {
            item.Reschedule(itemPlan, ids.New, now);
        }

        StartUtc = _items.Min(i => i.StartUtc);
        EndUtc = _items.Max(i => i.EndUtc);
        if (actor == ChangeActor.Customer && approvalHold is { } hold)
        {
            Status = AppointmentStatus.PendingApproval;
            HoldExpiresUtc = now + hold;
            // Kapora hâlâ bekleniyorsa süre dolumu DepositNotReceived kalır (A5); kapora yeni saate aktarılır (A14).
            HoldReason = DepositStatus is Domain.DepositStatus.Awaiting or Domain.DepositStatus.CustomerReported
                ? Domain.HoldReason.ApprovalAndDeposit
                : Domain.HoldReason.Approval;
        }

        Touch(now, userId);
        return Result.Success();
    }

    /// <summary>
    /// Randevuyu seriye bağlar. Tekrarlayan oluşturmada seri şablonu ilk randevunun planından çıkarıldığı için ilk randevu
    /// önce yazılır, sonra bağlanır; "bu ve sonrakiler" saat değişikliğinde sonraki tekrarlar bölünen yeni seriye geçer.
    /// </summary>
    // Kural: alan-modeli.md § AppointmentSeries ("bu ve sonrakiler" seriyi o tarihten böler)
    public void AssignSeries(Guid seriesId, Instant now)
    {
        if (seriesId == Guid.Empty)
        {
            throw new DomainException("Seri kimliği boş olamaz.");
        }

        SeriesId = seriesId;
        UpdatedUtc = now;
    }

    private void Cancel(CancelledBy by, CancellationReason reason, string? note, Guid? userId, Instant now)
    {
        Status = AppointmentStatus.Cancelled;
        CancelledUtc = now;
        CancelledBy = by;
        CancelledByUserId = userId;
        CancellationReason = reason;
        CancellationNote = Clean(note);
        HoldExpiresUtc = null;
        HoldReason = null;
        Touch(now, userId);
        foreach (var item in _items)
        {
            item.Cancel();
        }
    }

    private void Summarize()
    {
        StartUtc = _items.Min(i => i.StartUtc);
        EndUtc = _items.Max(i => i.EndUtc);
        TotalAmountMinor = _items.Sum(i => i.NetAmountMinor);
    }

    private void Touch(Instant now, Guid? userId)
    {
        UpdatedUtc = now;
        UpdatedByUserId = userId;
    }

    /// <summary>A2/A4 koruması: yalnız onay bekleyen ve tutma süresi geçmemiş talep yanıtlanır.</summary>
    private Result EnsureLivePending(Instant now)
    {
        if (Status != AppointmentStatus.PendingApproval)
        {
            return InvalidTransition();
        }

        return HoldExpiresUtc is { } expires && expires <= now ? Error.Conflict(ErrorCodes.HoldExpired) : Result.Success();
    }

    /// <summary>Kapora istenmiş, ne alınmış ne de kaporasız onaylanmış (Awaiting veya CustomerReported).</summary>
    private bool IsDepositAwaited => DepositStatus is Domain.DepositStatus.Awaiting or Domain.DepositStatus.CustomerReported;

    private static Error InvalidTransition() => Error.Conflict(ErrorCodes.InvalidStateTransition);

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
