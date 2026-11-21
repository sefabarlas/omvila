namespace Omvila.Modules.Sales.Contracts;

/// <summary>Paketten düşülecek bir randevu kalemi.</summary>
/// <param name="AppointmentItemId">Randevu kalemi.</param>
/// <param name="ServiceId">Kalemin hizmeti (paket kalemindeki hizmetle aynı olmalı).</param>
/// <param name="CustomerPackageItemId">Müşterinin paket kalemi.</param>
public sealed record PackageSessionClaim(Guid AppointmentItemId, Guid ServiceId, Guid CustomerPackageItemId);
