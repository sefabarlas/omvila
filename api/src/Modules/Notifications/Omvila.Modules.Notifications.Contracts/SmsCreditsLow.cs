using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>
/// Bakiye düşümle <c>LowCreditThreshold</c>'un altına indi (bildirimler.md <c>owner.credits_low</c>). Eşik yukarıdan
/// aşağı geçildiğinde bir kez yazılır; satın alma bakiyeyi eşiğin üstüne çıkarırsa sonraki iniş yine yazar.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="Balance">Düşümden sonraki bakiye.</param>
/// <param name="Threshold">Eşik.</param>
public sealed record SmsCreditsLow(Guid BrandId, int Balance, int Threshold) : IIntegrationEvent;
