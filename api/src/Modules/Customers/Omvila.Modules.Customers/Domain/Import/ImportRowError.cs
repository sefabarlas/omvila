namespace Omvila.Modules.Customers.Domain.Import;

/// <summary>Hatalı satır nedeni (kişisel veri yok).</summary>
internal sealed record ImportRowError(int RowNumber, string? Column, string Code);
