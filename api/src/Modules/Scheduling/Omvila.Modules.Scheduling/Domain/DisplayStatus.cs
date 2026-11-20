namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Arayüz gösterimi (durum-makinesi.md § Arayüz gösterimi; sözleşme DisplayStatus).</summary>
internal enum DisplayStatus
{
    PendingApproval,
    DepositPending,
    Confirmed,
    CheckedIn,
    Completed,
    Cancelled,
    NoShow,
}
