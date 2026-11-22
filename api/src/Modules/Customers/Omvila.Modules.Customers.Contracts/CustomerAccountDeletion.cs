using NodaTime;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>Hesap silmenin sonucu (sözleşme MeAccountDeletionResult).</summary>
/// <param name="DeletedUtc">Silme anı.</param>
/// <param name="CancelledAppointmentCount">İptal edilen yaklaşan randevular (bütün markalarda).</param>
public sealed record CustomerAccountDeletion(Instant DeletedUtc, int CancelledAppointmentCount);
