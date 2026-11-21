namespace Omvila.Modules.Sales.Domain;

/// <summary>Bir yöntemin toplamı ve ödeme sayısı.</summary>
internal readonly record struct MethodTotal(long AmountMinor, int Count);
