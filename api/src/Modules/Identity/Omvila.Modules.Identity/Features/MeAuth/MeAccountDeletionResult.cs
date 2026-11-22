using NodaTime;

namespace Omvila.Modules.Identity.Features.MeAuth;

/// <summary>Sözleşme MeAccountDeletionResult (HesapSilindi).</summary>
internal sealed record MeAccountDeletionResult(Instant DeletedUtc, int CancelledAppointmentCount);
