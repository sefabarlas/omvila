using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Contracts;

/// <summary>Deneme bitişine <c>TrialEndingNoticeDays</c> kaldı (bildirimler.md owner.trial_ending e-postası; bir kez).</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="TrialEndsOn">Deneme bitiş günü.</param>
/// <param name="PlanName">Planın ekrandaki adı (Salon).</param>
/// <param name="TrialDays">Deneme süresi (gün).</param>
/// <param name="PastDueGraceDays">Ödeme yapılmazsa salt okunura kadarki hatırlatma süresi (gün).</param>
public sealed record SubscriptionTrialEnding(Guid BrandId, LocalDate TrialEndsOn, string PlanName, int TrialDays, int PastDueGraceDays)
    : IIntegrationEvent;
