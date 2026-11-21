namespace Omvila.Modules.Customers.Features.Merge;

/// <summary>Sözleşme CustomerMergeRequest.</summary>
internal sealed record CustomerMergeRequest(Guid SourceBrandCustomerId, bool? MergeNotes);
