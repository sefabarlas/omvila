using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Online randevuda istenen kapora (A1; durum-makinesi.md § Oluşturma kuralı): tutar kalem kaporalarının toplamı, açıklama
/// kodu (<see cref="TransferCodeGenerator"/>) ve havale süresi (TransferWindowMinutes). Kapora ve onay birlikte gerekiyorsa
/// tek tutma süresi budur.
/// </summary>
/// <param name="AmountMinor">İstenen kapora (kuruş, &gt; 0).</param>
/// <param name="TransferCode">Açıklama kodu ("OMV-7K2X").</param>
/// <param name="TransferWindow">Havale süresi.</param>
internal sealed record DepositHold(long AmountMinor, string TransferCode, Duration TransferWindow);
