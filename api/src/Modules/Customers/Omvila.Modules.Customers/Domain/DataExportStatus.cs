namespace Omvila.Modules.Customers.Domain;

/// <summary>Dışa aktarma isteğinin durumu (sözleşmedeki Queued, Processing, Sent, Failed ve iç durum Ready).</summary>
internal enum DataExportStatus
{
    Queued,
    Processing,
    Ready,
    Sent,
    Failed,
}
