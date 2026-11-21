namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>
/// Sözleşme ManageApproveRequest (isteğe bağlı gövde). <see cref="WaiveDeposit"/> "Kaporasız onayla": kapora bekleniyorsa
/// zorunlu (yoksa 409 deposit_required), kapora yoksa veya alınmışsa yok sayılır (A2).
/// </summary>
internal sealed record ManageApproveRequest(bool? WaiveDeposit);
