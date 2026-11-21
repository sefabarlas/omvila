namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Bir hizmetin müsaitliği etkileyen kuralları ve personel bazında çözülmüş teklifleri.</summary>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="IsOnlineBookable">Online randevuya açık mı; panel kipinde dikkate alınmaz.</param>
/// <param name="MinNoticeMinutes">Hizmet ezmesi; null = marka değeri.</param>
/// <param name="MaxAdvanceDays">Hizmet ezmesi; null = marka değeri.</param>
/// <param name="OffersByStaff">Hizmeti veren (ServiceStaff) ve şubede çalışan personelin teklifleri (personel süresi uygulanmış; M15, M16).</param>
/// <param name="ResourceNeeds">Kaynak ihtiyaçları (ServiceResourceRule).</param>
internal sealed record ServiceAvailability(
    Guid ServiceId,
    bool IsOnlineBookable,
    int? MinNoticeMinutes,
    int? MaxAdvanceDays,
    IReadOnlyDictionary<Guid, ItemOffer> OffersByStaff,
    IReadOnlyList<ResourceNeed> ResourceNeeds);
