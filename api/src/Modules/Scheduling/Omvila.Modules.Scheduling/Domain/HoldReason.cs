namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Tutma nedeni (alan-modeli.md § Appointment.HoldReason); yalnız PendingApproval'da dolu.</summary>
internal enum HoldReason
{
    Approval,
    Deposit,
    ApprovalAndDeposit,
}
