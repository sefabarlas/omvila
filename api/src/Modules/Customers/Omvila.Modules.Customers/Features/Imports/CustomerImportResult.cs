namespace Omvila.Modules.Customers.Features.Imports;

/// <summary>Sözleşme CustomerImportResult.</summary>
internal sealed record CustomerImportResult(Guid ImportId, string Status, int Created, int Merged, int Skipped);
