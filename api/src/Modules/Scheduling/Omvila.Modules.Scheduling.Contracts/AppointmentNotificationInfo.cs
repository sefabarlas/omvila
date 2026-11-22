using NodaTime;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Bildirim metni için randevu bilgisi (Notifications: appointment.* SMS ve hatırlatmalar). Not ve kişisel metin taşımaz.
/// </summary>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="Status">AppointmentStatus metni (güncel durum).</param>
/// <param name="StartUtc">Güncel başlangıç.</param>
/// <param name="ServiceNames">Kalemlerin kopyalanan hizmet adları, sırayla.</param>
/// <param name="StaffNames">Kalemlerin personelinin müşteriye görünen adları (görünen ad, yoksa ad soyad; tekrarsız, sırayla).</param>
/// <param name="ManageAccessCode">SMS bağlantısının (/r/) geçerli erişim kodu; yoksa null.</param>
/// <param name="Deposit">Kapora bekleniyorsa (Kapora bekleniyor gösterimi) tutar, açıklama kodu ve son an; değilse null.</param>
/// <param name="StaffIds">Kalemlerin personel kayıtları, sırayla (işletme bildiriminde "İlgili personel"); bilinmiyorsa null.</param>
public sealed record AppointmentNotificationInfo(
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    string Status,
    Instant StartUtc,
    IReadOnlyList<string> ServiceNames,
    IReadOnlyList<string> StaffNames,
    string? ManageAccessCode,
    AwaitedDepositInfo? Deposit = null,
    IReadOnlyList<Guid>? StaffIds = null);
