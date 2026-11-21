namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>Online oluşturmada paketten düşülecek hizmet ("Fön paketinizden düşülsün"; sözleşme usePackageItems[]).</summary>
/// <param name="ServiceId">Kalemin hizmeti.</param>
/// <param name="CustomerPackageItemId">Müşterinin paket kalemi.</param>
internal sealed record PackageUse(Guid ServiceId, Guid CustomerPackageItemId);
