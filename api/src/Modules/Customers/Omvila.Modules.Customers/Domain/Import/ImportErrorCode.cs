namespace Omvila.Modules.Customers.Domain.Import;

/// <summary>Satır hata kodları (sözleşme CustomerImportRowError.code).</summary>
internal static class ImportErrorCode
{
    public const string InvalidPhone = "invalid_phone";
    public const string MissingName = "missing_name";
    public const string DuplicateInFile = "duplicate_in_file";
    public const string TooLong = "too_long";
}
