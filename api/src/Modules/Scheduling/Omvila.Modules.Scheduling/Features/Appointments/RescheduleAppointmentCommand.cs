using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// A14 girdisi. Şube değişmez. <see cref="StaffChanges"/> verilmezse kalemlerin personeli korunur. Müşteri
/// <see cref="RescheduleCutoff"/> sınırına tabidir (Public kip), salon değildir (Panel kip). <see cref="ExpectedVersion"/>
/// If-Match sürümüdür; uymazsa 412. <see cref="Approval"/> müşteri değişikliğinde verilir: yeni plan onay istiyorsa A13
/// (yeniden <c>PendingApproval</c>). <see cref="DepositWindow"/> kapora hâlâ bekleniyorsa A13 tutmasının süresidir (aktif
/// hesabın havale süresi; kapora ve onay birlikteyken tek tutma süresi).
/// </summary>
internal sealed record RescheduleAppointmentCommand(
    Guid AppointmentId,
    Instant StartUtc,
    ChangeActor Actor,
    Duration RescheduleCutoff,
    Guid? UserId,
    IReadOnlyList<ItemStaffChange>? StaffChanges = null,
    bool AllowOutsideWorkingHours = false,
    uint? ExpectedVersion = null,
    ApprovalPolicy? Approval = null,
    Duration? DepositWindow = null)
{
    /// <summary>Müşteri rezervasyon sayfası kurallarıyla (pencere, ızgara, online personel), salon panel kurallarıyla değiştirir.</summary>
    public AvailabilityMode Mode => Actor == ChangeActor.Customer ? AvailabilityMode.Public : AvailabilityMode.Panel;
}
