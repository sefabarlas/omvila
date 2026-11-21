using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>
/// Bildirim paneli (zil) satırı (sözleşme InAppNotification; alan modeline ek, T-71). Kullanıcı başına bir satır: okundu
/// bilgisi kişiseldir. Push tercihlerinden bağımsız yazılır. Başlık ve metin olay anındaki push metnidir, müşteri adı
/// "Ad Soyad"; not ve sağlık içeriği yazılmaz.
/// </summary>
internal sealed class InAppNotification
{
    /// <summary>Sözleşme InAppNotification.title en çok 80 karakter.</summary>
    public const int TitleMaxLength = 80;

    /// <summary>Sözleşme InAppNotification.body en çok 200 karakter.</summary>
    public const int BodyMaxLength = 200;

    private InAppNotification()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid UserId { get; private set; }

    public string EventCode { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Body { get; private set; } = string.Empty;

    public Guid? BranchId { get; private set; }

    /// <summary>Sözleşme NotificationTarget.type (Appointment, SmsCredits…).</summary>
    public string TargetType { get; private set; } = string.Empty;

    public Guid? TargetId { get; private set; }

    /// <summary>Yalnız staff.approval_needed; talep yanıtlanınca (onay, red, süre dolumu) null olur.</summary>
    public Instant? HoldExpiresUtc { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Instant? ReadUtc { get; private set; }

    public static InAppNotification Create(
        Guid id, Guid brandId, Guid userId, string eventCode, string title, string body, Guid? branchId, string targetType, Guid? targetId,
        Instant? holdExpiresUtc, Instant now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetType);
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Bildirim başlığı boş olamaz.");
        }

        return new InAppNotification
        {
            Id = id,
            BrandId = brandId,
            UserId = userId,
            EventCode = eventCode,
            Title = Fit(title, TitleMaxLength),
            Body = Fit(body ?? string.Empty, BodyMaxLength),
            BranchId = branchId,
            TargetType = targetType,
            TargetId = targetId,
            HoldExpiresUtc = holdExpiresUtc,
            CreatedUtc = now,
        };
    }

    /// <summary>Okundu say; okunmuş bildirimin okunma anı değişmez (sözleşme: idempotent).</summary>
    public void MarkRead(Instant now) => ReadUtc ??= now;

    private static string Fit(string text, int max) => text.Length <= max ? text : string.Concat(text.AsSpan(0, max - 1).TrimEnd(), "…");
}
