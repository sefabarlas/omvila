namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>Sözleşme ManageApproveRequest (isteğe bağlı gövde). <see cref="WaiveDeposit"/> Dilim 4'e kadar yok sayılır (T-62 (1)).</summary>
internal sealed record ManageApproveRequest(bool? WaiveDeposit);
