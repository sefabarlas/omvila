namespace Omvila.Modules.Customers.Domain;

/// <summary>Silme talebi durumu (alan-modeli.md § DataDeletionRequest.Status).</summary>
internal enum DeletionRequestStatus
{
    Pending,
    Completed,
    Rejected,
}
