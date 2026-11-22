using NodaTime;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Markanın aboneliği (alan-modeli.md § Subscription; marka başına bir). Ödeme yöntemi ve havale kodu ödeme akışıyla
/// (Dilim 6) gelir; denemede kart gerekmez.
/// </summary>
internal sealed class Subscription
{
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

    public int BranchCount { get; private set; }

    public int StaffCount { get; private set; }

    public Instant CreatedUtc { get; private set; }

    /// <summary>
    /// Plan değişikliği (sözleşme adminChangePlan): hemen geçerlidir, yeni fiyat bir sonraki faturadan uygulanır. Sınır denetimi
    /// (personel, şube) çağıranda yapılır; etkin olmayan plan verilmez.
    /// </summary>
    public void ChangePlan(Guid planId, BillingCycle billingCycle)
    {
        PlanId = planId;
        BillingCycle = billingCycle;
    }

    /// <summary>Deneme: bugün başlar, bugün + trialDays'te biter; bir şube, bir personel (sahip).</summary>
    public static Subscription StartTrial(Guid id, Guid brandId, Guid planId, LocalDate today, int trialDays, Instant now)
    {
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
            CreatedUtc = now,
        };
    }
}
