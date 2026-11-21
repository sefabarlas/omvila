namespace Omvila.Modules.Notifications.Domain;

/// <summary>Gönderen adı durumu (sözleşme SmsSenderStatus). None = hiç başvuru yok (satır yok).</summary>
internal enum SmsSenderStatus
{
    None,
    Pending,
    Approved,
    Rejected,
}
