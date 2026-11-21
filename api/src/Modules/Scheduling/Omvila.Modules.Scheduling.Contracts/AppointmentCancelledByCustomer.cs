using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Müşteri randevuyu SMS bağlantısından iptal etti (A6). Müşteriye mesaj gitmez; Notifications ilgili personele ve
/// yöneticilere <c>staff.appointment_changed</c> ("İptal", neden) gönderir ve planlanmış hatırlatmaları düşürür
/// (bildirimler.md § İşletme olayları). İptal notu taşınmaz (günlüğe ve olay yüküne kişisel metin yazılmaz).
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="BrandCustomerId">Müşteri kartı.</param>
/// <param name="StartUtc">İptal edilen randevunun başlangıcı.</param>
/// <param name="Reason">CancellationReason metni (PlanChanged, Sick, WantsDifferentTime, Other).</param>
public sealed record AppointmentCancelledByCustomer(
    Guid BrandId,
    Guid AppointmentId,
    Guid BranchId,
    Guid BrandCustomerId,
    Instant StartUtc,
    string Reason) : IIntegrationEvent;
