namespace Omvila.Modules.Customers.Features.Export;

/// <summary>Dışa aktarılan dosya.</summary>
internal sealed record CustomerExport(string FileName, byte[] Content);
