namespace Omvila.Modules.Customers.Domain;

/// <summary>İçe aktarma durumu (alan-modeli.md § CustomerImport.Status).</summary>
internal enum CustomerImportStatus
{
    Previewed,
    Committed,
    Failed,
}
