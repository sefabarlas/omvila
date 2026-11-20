using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// A1 girdisi. <see cref="ApprovalTimeout"/> doluysa online randevu onay ister ve <c>PendingApproval</c> başlar
/// (şimdi + ApprovalTimeoutMinutes). Dilim 2'de onay hiç gerekmez (RequiresApproval=false), çağıranlar <c>null</c> verir.
/// Kapora Dilim 4'tedir.
/// </summary>
internal sealed record NewAppointment(
    Guid BrandId,
    Guid BranchId,
    Guid CustomerId,
    Guid BrandCustomerId,
    AppointmentChannel Channel,
    ClientApp? ClientApp,
    string? CustomerNote,
    string? StaffNote,
    Guid? SeriesId,
    Guid? CreatedByUserId,
    Duration? ApprovalTimeout = null);
