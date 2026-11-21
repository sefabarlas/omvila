using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// A1 girdisi. <see cref="ApprovalTimeout"/> doluysa online randevu onay ister ve <c>PendingApproval</c> başlar
/// (şimdi + ApprovalTimeoutMinutes); değeri <see cref="ApprovalPolicy.HoldFor"/> çözer. Panel, telefon ve kapıda sıra için
/// yok sayılır (T-47 (3)). Kapora Dilim 4'tedir.
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
