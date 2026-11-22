namespace Omvila.Modules.Scheduling.Features.Today;

/// <summary>Sözleşme TodayCommission ("KASIM PRİMİ 6.840 ₺" kartı ve çizgi grafiği).</summary>
internal sealed record TodayCommission(string Month, long TotalMinor, string Currency, IReadOnlyList<TodayCommissionDay> Daily);

