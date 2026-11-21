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

    /// <summary>İstenen kapora; <c>null</c> = kapora yok (Dilim 4'te yazılır).</summary>
    public long? DepositRequiredMinor { get; private set; }

    /// <summary>Müşteri "Havaleyi yaptım" dedi (Dilim 4).</summary>
    public Instant? DepositCustomerReportedUtc { get; private set; }

    /// <summary>Salon "Kapora geldi" dedi (Dilim 4).</summary>
    public Instant? DepositReceivedUtc { get; private set; }

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

        // Kapıda sıradan başlatılan randevu doğrudan Geldi'dir; panel ve telefon onay istemez; online yalnız onay
        // gerekiyorsa bekler (kapora Dilim 4).
        if (input.Channel == AppointmentChannel.WalkIn)
        {
            appointment.Status = AppointmentStatus.CheckedIn;
            appointment.CheckedInUtc = now;
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
    /// A5: süresi dolan tutma sistemce iptal edilir. Kapora istenmişse neden <c>DepositNotReceived</c>, değilse
    /// <c>ApprovalExpired</c>. Yazma yolu, çakışma denetiminden önce aynı personelin süresi dolmuş tutmalarını bununla
    /// satır içinde iptal eder (durum-makinesi.md § Sistem işleri); bildirim ve bekleme listesi yan etkileri Dilim 3–4'tedir.
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
    /// aynı hizmetlerdedir. Müşteri <paramref name="rescheduleCutoff"/> sınırına tabidir, salon değildir (S-04). Durum
    /// aynı kalır; onaylı salonda müşteri değişikliğinin onaya düşmesi (A13) Dilim 3'tedir.
    /// </summary>
    // Kural: docs/alan/durum-makinesi.md#geçiş-tablosu (A14)
    public Result Reschedule(AppointmentPlan plan, ChangeActor actor, Instant now, Duration rescheduleCutoff, IIdGenerator ids, Guid? userId)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(ids);
        if (Status is not (AppointmentStatus.PendingApproval or AppointmentStatus.Confirmed))
        {
            return InvalidTransition();
        }

        if (actor == ChangeActor.Customer && StartUtc - now < rescheduleCutoff)
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

    private static Error InvalidTransition() => Error.Conflict(ErrorCodes.InvalidStateTransition);

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
