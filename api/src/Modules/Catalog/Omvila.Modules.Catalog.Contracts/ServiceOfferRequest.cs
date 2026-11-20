namespace Omvila.Modules.Catalog.Contracts;

/// <summary>Çözülecek hizmet ve personel.</summary>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="StaffId">Personel; <c>null</c> personelsiz çözüm.</param>
public sealed record ServiceOfferRequest(Guid ServiceId, Guid? StaffId);
