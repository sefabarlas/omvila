namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Randevu uçlarının ihtiyaç duyduğu marka ayarları (sureler-ve-ayarlar.md § Randevu kuralları).</summary>
/// <param name="NoShowAfterMinutes">"'Gelmedi' sayılma süresi".</param>
/// <param name="CancellationCutoffHours">Müşterinin iptal son süresi.</param>
/// <param name="RescheduleCutoffHours">Müşterinin saat değiştirme son süresi.</param>
/// <param name="MaskCustomerPhoneFromStaff">Personel müşteri telefonunu maskeli görür (roller-yetkiler.md **).</param>
/// <param name="RequiresApproval">"Randevular onayıma kesinleşsin" (hizmet ayarı <c>null</c> ise geçerli).</param>
/// <param name="ApprovalTimeoutMinutes">Onay yanıt süresi; dolarsa otomatik iptal.</param>
public sealed record BrandAppointmentSettings(
    int NoShowAfterMinutes,
    int CancellationCutoffHours,
    int RescheduleCutoffHours,
    bool MaskCustomerPhoneFromStaff,
    bool RequiresApproval,
    int ApprovalTimeoutMinutes);
