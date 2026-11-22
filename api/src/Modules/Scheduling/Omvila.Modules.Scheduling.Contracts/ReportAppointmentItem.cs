namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Rapor için randevu kalemi.</summary>
/// <param name="AppointmentItemId">Kalem.</param>
/// <param name="StaffId">Personel; atanmamışsa null.</param>
/// <param name="NetAmountMinor">Kalem tutarı (fiyat − indirim).</param>
/// <param name="CustomerPackageItemId">Paketten düşülüyorsa müşteri paketi kalemi.</param>
public sealed record ReportAppointmentItem(Guid AppointmentItemId, Guid? StaffId, long NetAmountMinor, Guid? CustomerPackageItemId);
