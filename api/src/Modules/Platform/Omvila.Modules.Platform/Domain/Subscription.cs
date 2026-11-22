using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Markanın aboneliği (alan-modeli.md § Subscription; marka başına bir). Durumlar durum-makinesi.md § 8; her geçiş bir metottur.
/// Dönem günleri OMVILA'nın faturalama takvimindedir (<see cref="BillingCalendar"/>). Kartla ödeme (T-06) yazılmadı: ödeme yolu
/// şimdilik yalnız havale.
/// </summary>
internal sealed class Subscription
{
    public const int TransferCodeMaxLength = 20;

    private Subscription()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid PlanId { get; private set; }

    public BillingCycle BillingCycle { get; private set; }

    public SubscriptionStatus Status { get; private set; }

    public LocalDate? TrialEndsOn { get; private set; }

    public LocalDate CurrentPeriodStart { get; private set; }

    public LocalDate CurrentPeriodEnd { get; private set; }

    public LocalDate? PastDueSince { get; private set; }

    public LocalDate? ReadOnlySince { get; private set; }

    public bool CancelAtPeriodEnd { get; private set; }

    public Instant? CancelledUtc { get; private set; }

    public LocalDate? CancelledOn { get; private set; }

    /// <summary>Son kullanılan ödeme yolu; henüz ödeme yoksa boş.</summary>
    public SubscriptionPaymentMethod? PaymentMethod { get; private set; }

    /// <summary>Sabit abonelik havale kodu (ABN-KE-1125); her dönem aynı.</summary>
    public string TransferCode { get; private set; } = string.Empty;

    /// <summary>Sabit kontör havale kodu (KNT-KE-1125; T-116).</summary>
    public string CreditTransferCode { get; private set; } = string.Empty;

    public int BranchCount { get; private set; }

    public int StaffCount { get; private set; }

    /// <summary>owner.trial_ending e-postası yazıldı (bir kez).</summary>
    public Instant? TrialEndingNotifiedUtc { get; private set; }

    /// <summary>İptalden CancelledArchiveDays sonra marka arşive alındı (iş bir kez yapar).</summary>
    public Instant? BrandArchivedUtc { get; private set; }

    public Instant CreatedUtc { get; private set; }

    /// <summary>
    /// Deneme: bugün başlar, TrialEndsOn = bugün + trialDays; o gün deneme biter ve ilk ücretli dönem başlar (T-118).
    /// Bir şube, bir personel (sahip). Havale kodları markaya sabittir.
    /// </summary>
    public static Subscription StartTrial(
        Guid id, Guid brandId, Guid planId, LocalDate today, int trialDays, TransferCodes codes, Instant now)
    {
        ArgumentNullException.ThrowIfNull(codes);
        var endsOn = today.PlusDays(trialDays);
        return new Subscription
        {
            Id = id,
            BrandId = brandId,
            PlanId = planId,
            BillingCycle = BillingCycle.Monthly,
            Status = SubscriptionStatus.Trialing,
            TrialEndsOn = endsOn,
            CurrentPeriodStart = today,
            CurrentPeriodEnd = endsOn,
            BranchCount = 1,
            StaffCount = 1,
            TransferCode = codes.Subscription,
            CreditTransferCode = codes.Credits,
            CreatedUtc = now,
        };
    }

    /// <summary>Deneme bitti mi (TrialEndsOn günü dahil biter).</summary>
    public bool IsTrialOver(LocalDate today) => Status == SubscriptionStatus.Trialing && TrialEndsOn is { } end && today >= end;

    /// <summary>Ödenmiş dönem bitti mi (bir sonraki dönem başladı, ödeme gelmedi).</summary>
    public bool IsPeriodOver(LocalDate today) => Status == SubscriptionStatus.Active && today > CurrentPeriodEnd;

    /// <summary>Sonraki dönem: deneme bitişinden veya geçerli dönem sonundan başlar.</summary>
    public (LocalDate Start, LocalDate End) NextPeriod()
    {
        var start = Status == SubscriptionStatus.Trialing && TrialEndsOn is { } trialEnd ? trialEnd : CurrentPeriodEnd.PlusDays(1);
        return (start, BillingCalendar.PeriodEnd(start, BillingCycle));
    }

    // Kural: durum-makinesi.md § 8 (Trialing → PastDue "deneme bitti, ödeme yok"; Active → PastDue "havale gelmedi")
    public Result MarkPastDue(LocalDate today)
    {
        if (!IsTrialOver(today) && !IsPeriodOver(today))
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        // Gecikme ödenmeyen dönemin başından (vade) sayılır; iş gecikse de hatırlatma süresi uzamaz.
        var (start, end) = NextPeriod();
        CurrentPeriodStart = start;
        CurrentPeriodEnd = end;
        Status = SubscriptionStatus.PastDue;
        PastDueSince = start;
        return Result.Success();
    }

    /// <summary>PastDue ise salt okunura geçeceği gün (PastDueSince + PastDueGraceDays).</summary>
    public LocalDate? ReadOnlyOn(int graceDays) =>
        Status == SubscriptionStatus.PastDue && PastDueSince is { } since ? since.PlusDays(graceDays) : null;

    // Kural: durum-makinesi.md § 8 (PastDue → ReadOnly "7 gün geçti")
    public Result MarkReadOnly(LocalDate today, int graceDays)
    {
        if (ReadOnlyOn(graceDays) is not { } due || today < due)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        Status = SubscriptionStatus.ReadOnly;
        ReadOnlySince = today;
        return Result.Success();
    }

    // Kural: durum-makinesi.md § 8 (Trialing → Active "ödeme alındı"; PastDue → Active "havale eşleşti")
    public Result Activate(LocalDate periodStart, SubscriptionPaymentMethod method)
    {
        if (Status is not (SubscriptionStatus.Trialing or SubscriptionStatus.PastDue))
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        BecomeActive(periodStart, method);
        return Result.Success();
    }

    // Kural: durum-makinesi.md § 8 (ReadOnly → Active "ödeme alındı"; salt okunur hemen kalkar)
    public Result Reactivate(LocalDate periodStart, SubscriptionPaymentMethod method)
    {
        if (Status != SubscriptionStatus.ReadOnly)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        BecomeActive(periodStart, method);
        return Result.Success();
    }

    /// <summary>Ödemenin durumu ne olursa olsun doğru geçişi seçer (Active iken yalnız ödeme yolu yazılır).</summary>
    public Result RecordPayment(LocalDate periodStart, SubscriptionPaymentMethod method)
    {
        if (Status == SubscriptionStatus.Active)
        {
            PaymentMethod = method;
            return Result.Success();
        }

        return Status == SubscriptionStatus.ReadOnly ? Reactivate(periodStart, method) : Activate(periodStart, method);
    }

    // Kural: durum-makinesi.md § 8 (ReadOnly → Cancelled "salon iptal / 30 gün"; Active → Cancelled "dönem sonunda iptal")
    public Result Cancel(LocalDate today, Instant now)
    {
        if (Status is not (SubscriptionStatus.ReadOnly or SubscriptionStatus.Active))
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        Status = SubscriptionStatus.Cancelled;
        CancelledUtc = now;
        CancelledOn = today;
        CancelAtPeriodEnd = false;
        return Result.Success();
    }

    /// <summary>Salt okunurda ReadOnlyToCancelledDays doldu mu.</summary>
    public bool IsReadOnlyExpired(LocalDate today, int readOnlyToCancelledDays) =>
        Status == SubscriptionStatus.ReadOnly && ReadOnlySince is { } since && today >= since.PlusDays(readOnlyToCancelledDays);

    /// <summary>İptalden CancelledArchiveDays geçti ve marka henüz arşive alınmadı mı.</summary>
    public bool IsDueForArchive(LocalDate today, int cancelledArchiveDays) =>
        Status == SubscriptionStatus.Cancelled && BrandArchivedUtc is null && CancelledOn is { } on && today >= on.PlusDays(cancelledArchiveDays);

    public void MarkBrandArchived(Instant now)
    {
        if (Status != SubscriptionStatus.Cancelled)
        {
            throw new DomainException("Yalnız iptal edilen aboneliğin markası arşive alınır.");
        }

        BrandArchivedUtc ??= now;
    }

    /// <summary>Deneme bitişine noticeDays veya daha az kaldı ve uyarı henüz yazılmadı mı (bildirimler.md owner.trial_ending).</summary>
    public bool IsDueForTrialEndingNotice(LocalDate today, int noticeDays) =>
        Status == SubscriptionStatus.Trialing && TrialEndingNotifiedUtc is null && TrialEndsOn is { } end
        && today >= end.PlusDays(-noticeDays) && today < end;

    public void MarkTrialEndingNotified(Instant now) => TrialEndingNotifiedUtc ??= now;

    /// <summary>Faturalanan şube ve personel sayısı (fatura kesilirken güncellenir).</summary>
    public void UpdateCounts(int branchCount, int staffCount)
    {
        BranchCount = Math.Max(0, branchCount);
        StaffCount = Math.Max(0, staffCount);
    }

    private void BecomeActive(LocalDate periodStart, SubscriptionPaymentMethod method)
    {
        Status = SubscriptionStatus.Active;
        CurrentPeriodStart = periodStart;
        CurrentPeriodEnd = BillingCalendar.PeriodEnd(periodStart, BillingCycle);
        TrialEndsOn = null;
        PastDueSince = null;
        ReadOnlySince = null;
        PaymentMethod = method;
    }
}
