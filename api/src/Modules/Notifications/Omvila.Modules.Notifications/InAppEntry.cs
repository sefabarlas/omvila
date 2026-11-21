using NodaTime;

namespace Omvila.Modules.Notifications;

/// <summary>Bildirim paneli kaydı.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="EventCode">Olay kodu (push şablonuyla aynı).</param>
/// <param name="Values">Şablon değişkenleri; müşteri adı "Ad Soyad".</param>
/// <param name="Variant">Şablon çeşidi.</param>
/// <param name="BranchId">Olayın şubesi; marka düzeyinde null.</param>
/// <param name="TargetType">Açılacak ekran.</param>
/// <param name="TargetId">Hedef kayıt.</param>
/// <param name="HoldExpiresUtc">Yalnız onay bekleyen talepte.</param>
internal sealed record InAppEntry(
    Guid BrandId,
    string EventCode,
    IReadOnlyDictionary<string, string> Values,
    string? Variant,
    Guid? BranchId,
    string TargetType,
    Guid? TargetId,
    Instant? HoldExpiresUtc = null);
