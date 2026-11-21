namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Müsaitlik ayarları (sureler-ve-ayarlar.md).</summary>
/// <param name="MinNoticeMinutes">Marka "En az ön bildirim".</param>
/// <param name="MaxAdvanceDays">Marka "En ileri tarih".</param>
/// <param name="SlotIntervalMinutes">Şube değeri yoksa marka "Randevu saat aralığı".</param>
/// <param name="DefaultBranchTransferMinutes">"Şubeler arası geçiş süresi".</param>
public sealed record BookingRules(int MinNoticeMinutes, int MaxAdvanceDays, int SlotIntervalMinutes, int DefaultBranchTransferMinutes);
