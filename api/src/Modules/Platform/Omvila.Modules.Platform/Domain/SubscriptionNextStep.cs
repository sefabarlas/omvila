using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Abonelikler tablosunun "Sonraki adım" sütunu (sözleşme adminListSubscriptions nextStep; T-124). Türetilmiş gösterimdir:
/// Denemede deneme bitişi; Ödeme gecikti'de salt okunur günü (PastDueSince + PastDueGraceDays: "6. gün · yarın salt okunur");
/// Salt okunurda "Ödeme gelince açılır"; Aktifte kontör bittiyse "SMS gönderilmiyor", yoksa yenileme günü (dönem sonundan sonraki
/// gün); İptalde arşiv günü (CancelledOn + CancelledArchiveDays).
/// </summary>
internal readonly record struct SubscriptionNextStep(string Type, LocalDate? Date)
{
    public const string TrialEnds = "TrialEnds";
    public const string ReadOnlyOn = "ReadOnlyOn";
    public const string Renews = "Renews";
    public const string ReactivatesOnPayment = "ReactivatesOnPayment";
    public const string SmsStopped = "SmsStopped";
    public const string ArchivesOn = "ArchivesOn";

    public static SubscriptionNextStep For(Subscription subscription, int creditBalance, int pastDueGraceDays, int cancelledArchiveDays)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        return subscription.Status switch
        {
            SubscriptionStatus.Trialing => new(TrialEnds, subscription.TrialEndsOn),
            // Kural: durum-makinesi.md § 8 (PastDue → ReadOnly "PastDueGraceDays geçti")
            SubscriptionStatus.PastDue => new(ReadOnlyOn, subscription.ReadOnlyOn(pastDueGraceDays)),
            SubscriptionStatus.ReadOnly => new(ReactivatesOnPayment, null),
            SubscriptionStatus.Cancelled => new(ArchivesOn, subscription.CancelledOn?.PlusDays(cancelledArchiveDays)),
            _ when creditBalance <= 0 => new(SmsStopped, null),
            _ => new(Renews, subscription.CurrentPeriodEnd.PlusDays(1)),
        };
    }

    /// <summary>"Kontörü azalan": bakiye LowCreditThreshold'un altında (sureler-ve-ayarlar.md).</summary>
    public static bool IsCreditsLow(int balance) => balance < PlatformDefaults.LowCreditThreshold;
}
