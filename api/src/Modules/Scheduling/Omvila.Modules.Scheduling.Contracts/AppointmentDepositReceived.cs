using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Salon "Kapora geldi" dedi (A3 → Confirmed). Notifications müşteriye <c>deposit.received</c> SMS'ini yazar (yalnız SMS, V3)
/// ve hatırlatmaları planlar; personelin bekleyen onay bildirimleri çözülür. Bu onay için <see cref="AppointmentApproved"/>
/// ayrıca yayımlanmaz.
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="AmountMinor">Alınan kapora (kuruş; randevunun para biriminde).</param>
/// <param name="ReceivedByUserId">İşaretleyen kullanıcı.</param>
public sealed record AppointmentDepositReceived(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    Instant StartUtc,
    long AmountMinor,
    Guid? ReceivedByUserId) : IIntegrationEvent;
