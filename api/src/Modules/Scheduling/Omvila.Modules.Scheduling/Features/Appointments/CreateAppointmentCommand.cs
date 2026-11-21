using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// A1 girdisi. Müşteri (CustomerId, BrandCustomerId) çağıran uçta <c>ICustomerDirectory.EnsureBrandCustomerAsync</c> ile
/// çözülür; marka kimliği jetondan (manage) veya slug'dan (public) gelir, istekten okunmaz. <see cref="Approval"/> yalnız
/// online oluşturmada verilir: plan onay istiyorsa randevu <c>PendingApproval</c> başlar (panel ve telefon her zaman onaylı).
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
    ApprovalPolicy? Approval = null);
