namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>Sözleşme Money (istek).</summary>
internal sealed record MoneyInput(long? AmountMinor, string? Currency);
