namespace Omvila.Modules.Customers.Domain.Import;

/// <summary>Ayrıştırılmış CSV: eşlenen sütunlar, geçerli satırlar, satır hataları.</summary>
internal sealed record ParsedImport(
    IReadOnlyList<ImportColumn> MappedColumns, IReadOnlyList<ImportRow> Rows, IReadOnlyList<ImportRowError> Errors, int TotalRows, int InvalidRows);
