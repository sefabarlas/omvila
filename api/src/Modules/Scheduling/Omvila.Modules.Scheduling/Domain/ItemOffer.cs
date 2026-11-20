using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Kalemin çözülmüş teklifi (Catalog IServiceOffers'tan eşlenir; alan katmanı Catalog'a bağlı değildir).
/// <see cref="Price"/> personel ek ücreti dahil fiyattır; <see cref="StaffSurcharge"/> onun içindeki ek ücrettir.
/// Personel süre farkı aşamalara Catalog'da uygulanmıştır (son personel-meşgul aşama).
/// </summary>
internal sealed record ItemOffer(
    Guid ServiceId,
    Guid? StaffId,
    string ServiceName,
    Money Price,
    Money StaffSurcharge,
    int BufferBeforeMinutes,
    int BufferAfterMinutes,
    IReadOnlyList<PlannedStage> Stages);
