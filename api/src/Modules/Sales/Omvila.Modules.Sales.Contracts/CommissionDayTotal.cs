using NodaTime;

namespace Omvila.Modules.Sales.Contracts;

/// <summary>Bir günün prim toplamı (sözleşme CommissionDayAmount).</summary>
/// <param name="Date">Yerel gün.</param>
/// <param name="AmountMinor">Toplam (kuruş; ters kayıt yüzünden eksi olabilir).</param>
/// <param name="CurrencyCode">Para birimi.</param>
public sealed record CommissionDayTotal(LocalDate Date, long AmountMinor, string CurrencyCode);
