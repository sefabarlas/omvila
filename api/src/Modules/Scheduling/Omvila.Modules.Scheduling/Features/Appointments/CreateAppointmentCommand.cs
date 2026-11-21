using NodaTime;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// A1 girdisi. Müşteri (CustomerId, BrandCustomerId) çağıran uçta <c>ICustomerDirectory.EnsureBrandCustomerAsync</c> ile
/// çözülür; marka kimliği jetondan (manage) veya slug'dan (public) gelir, istekten okunmaz.
/// </summary>
internal sealed record CreateAppointmentCommand(
    Guid BrandId,
    SlotRequest Slot,
    Guid CustomerId,
    Guid BrandCustomerId,
    AppointmentChannel Channel,
    ClientApp? ClientApp,
    string? CustomerNote,
    string? StaffNote,
    Guid? SeriesId,
    Guid? CreatedByUserId,
    Duration? ApprovalTimeout = null);
