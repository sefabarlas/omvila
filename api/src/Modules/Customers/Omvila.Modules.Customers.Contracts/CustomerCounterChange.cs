using NodaTime;

namespace Omvila.Modules.Customers.Contracts;

/// <summary>
/// Müşteri sayaçlarına uygulanacak fark (alan-modeli.md § BrandCustomer: türetilmiş sayaçlar olaylarla güncellenir).
/// Geri alma (ör. "Geldiyse geri al") negatif farkla yapılır; sayaçlar sıfırın altına inmez.
/// </summary>
/// <param name="Visits">Tamamlanan ziyaret farkı.</param>
/// <param name="Cancels">İptal farkı.</param>
/// <param name="NoShows">Gelmeme farkı.</param>
/// <param name="SpentMinor">Tamamlanan randevu tutarı farkı (kuruş, markanın para biriminde).</param>
/// <param name="VisitedOn">Ziyaret günü (şubenin yerel tarihi); verilirse ilk ve son ziyaret günü güncellenir.</param>
public sealed record CustomerCounterChange(int Visits = 0, int Cancels = 0, int NoShows = 0, long SpentMinor = 0, LocalDate? VisitedOn = null);
