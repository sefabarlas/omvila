namespace Omvila.Modules.Scheduling.Features.Completion;

/// <summary>Sözleşme PackageRedemptionRequest ("Paketten").</summary>
/// <param name="AppointmentItemId">Randevu kalemi.</param>
/// <param name="CustomerPackageItemId">Müşterinin paket kalemi.</param>
internal sealed record PackageRedemptionRequest(Guid? AppointmentItemId, Guid? CustomerPackageItemId);
