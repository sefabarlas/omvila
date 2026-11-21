namespace Omvila.Modules.Customers.Features.Deletion;

/// <summary>Sözleşme DataDeletionRequestPage.</summary>
internal sealed record DataDeletionRequestPage(IReadOnlyList<DataDeletionRequestView> Items, string? NextCursor);
