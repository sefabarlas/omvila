using NodaTime;

namespace Omvila.Modules.Identity.Features.ManageSession;

internal sealed record SessionSubscription(string Status, LocalDate? TrialEndsOn, LocalDate? PastDueSince, LocalDate? ReadOnlyOn);
