using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Scheduling.Features.Me;

/// <summary>Randevu satırlarının toplu okunan bilgisi: personelin görünen adı, değerlendirme özeti, "Aynısını tekrar al" uygunluğu.</summary>
internal sealed record MeAppointmentContext(
    IReadOnlyDictionary<Guid, StaffCard> Staff,
    IReadOnlyDictionary<Guid, AppointmentReviewSummary> Reviews,
    IReadOnlySet<Guid> Rebookable);
