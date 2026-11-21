using NodaTime;

namespace Omvila.Modules.Customers.Features.Imports;

/// <summary>Sözleşme CustomerImportPreview.</summary>
internal sealed record CustomerImportPreview(
    Guid ImportId,
    string FileName,
    int TotalRows,
    int ToCreate,
    int ToMerge,
    int InvalidRows,
    IReadOnlyList<string> MappedColumns,
    IReadOnlyList<Domain.Import.ImportRowError> Errors,
    string Status,
    Instant? ExpiresUtc);
