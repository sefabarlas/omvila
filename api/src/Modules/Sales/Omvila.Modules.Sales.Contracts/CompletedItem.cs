namespace Omvila.Modules.Sales.Contracts;

/// <summary>Tamamlanan randevu kalemi (hizmet priminin girdisi).</summary>
/// <param name="AppointmentItemId">Kalem.</param>
/// <param name="ServiceId">Hizmet.</param>
/// <param name="StaffId">Hizmeti veren; personelsiz kalemde null (prim yok).</param>
/// <param name="AmountMinor">Kalemin tutarı (fiyat − indirim, kuruş).</param>
/// <param name="CustomerPackageItemId">Paketten düşüldüyse müşterinin paket kalemi (seans önceden ayrılmış); tahsilata girmez.</param>
public sealed record CompletedItem(Guid AppointmentItemId, Guid ServiceId, Guid? StaffId, long AmountMinor, Guid? CustomerPackageItemId = null);
