using NodaTime;

namespace Omvila.Modules.Platform.Contracts;

/// <summary>Başlatılan deneme aboneliği.</summary>
/// <param name="SubscriptionId">Abonelik.</param>
/// <param name="TrialEndsOn">Deneme bitiş günü (şubenin saat diliminde).</param>
public sealed record StartedTrial(Guid SubscriptionId, LocalDate TrialEndsOn);
