using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Salon "Hatırlat" dedi (V6; randevu başına en fazla bir kez). Notifications <c>deposit.awaiting</c> SMS'ini kalan süreyle
/// yeniden kuyruğa yazar (her zaman SMS, sessiz saate kaydırılmaz).
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
public sealed record AppointmentDepositReminded(Guid BrandId, Guid AppointmentId) : IIntegrationEvent;
