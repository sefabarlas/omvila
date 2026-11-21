using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>
/// notifications.notification_outbox satırı (alan-modeli.md § NotificationOutbox). Alıcı: telefon (SMS), müşteri
/// (kanal planı [Push, Sms]: aktif cihaz yoksa veya push başarısızsa SMS) veya işletme kullanıcısı (yalnız push).
/// </summary>
internal sealed class NotificationMessage
{
    private NotificationMessage()
    {
    }

    public Guid Id { get; private set; }

    public Guid? BrandId { get; private set; }

    public string EventCode { get; private set; } = string.Empty;

    public string Category { get; private set; } = "Transactional";

    public string RecipientType { get; private set; } = "Phone";

    public string? RecipientPhone { get; private set; }

    /// <summary>Müşteri veya kullanıcı alıcının kimliği (cihazları bununla bulunur); telefon alıcısında boş.</summary>
    public Guid? RecipientId { get; private set; }

    public string[] ChannelPlan { get; private set; } = ["Sms"];

    public string? ChannelUsed { get; private set; }

    public string Payload { get; private set; } = "{}";

    public string? RenderedBody { get; private set; }

    public Instant ScheduledUtc { get; private set; }

    public NotificationStatus Status { get; private set; }

    public int AttemptCount { get; private set; }

    public int CreditsConsumed { get; private set; }

    public string? ProviderMessageId { get; private set; }

    public string? FailureReason { get; private set; }

    /// <summary>Ait olduğu kayıt türü (ör. Appointment); bekleyen hatırlatmalar bununla düşürülür.</summary>
    public string? ReferenceType { get; private set; }

    public Guid? ReferenceId { get; private set; }

    /// <summary>Aynı olay kodunun şablon çeşidi (ör. aynı gün hatırlatması); boşsa temel metin (alan modeline ek, T-53).</summary>
    public string? TemplateVariant { get; private set; }

    public Instant CreatedUtc { get; private set; }

    /// <summary>Sağlayıcı teslim raporunun "teslim edildi" anı (alan modeline ek, T-64); Sent satırda bir kez yazılır.</summary>
    public Instant? DeliveredUtc { get; private set; }

    public bool PushFirst => ChannelPlan is [PushChannel, ..];

    public bool HasSmsFallback => ChannelPlan.Contains(SmsChannel, StringComparer.Ordinal);

    public const string PushChannel = "Push";

    public const string SmsChannel = "Sms";

    /// <summary>
    /// Telefona SMS satırı; zamanlanmış gönderim verilmezse hemen. <paramref name="customerId"/> verilirse kanal planı
    /// [Push, Sms] (bildirimler.md § Kanal planı).
    /// </summary>
    public static NotificationMessage ForPhone(
        Guid id,
        Guid? brandId,
        string eventCode,
        string phoneE164,
        string payload,
        Instant now,
        Instant? scheduledUtc = null,
        string? referenceType = null,
        Guid? referenceId = null,
        string? templateVariant = null,
        Guid? customerId = null) => new()
        {
            Id = id,
            BrandId = brandId,
            EventCode = eventCode,
            RecipientPhone = phoneE164,
            RecipientType = customerId is null ? "Phone" : "Customer",
            RecipientId = customerId,
            ChannelPlan = customerId is null ? [SmsChannel] : [PushChannel, SmsChannel],
            Payload = payload,
            ScheduledUtc = scheduledUtc ?? now,
            CreatedUtc = now,
            Status = NotificationStatus.Pending,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            TemplateVariant = templateVariant,
        };

    /// <summary>İşletme kullanıcısına push satırı (bildirimler.md § İşletme olayları); SMS'e düşmez.</summary>
    public static NotificationMessage ForUser(
        Guid id, Guid brandId, string eventCode, Guid userId, string payload, Instant now, Instant scheduledUtc, string? referenceType, Guid? referenceId, string? templateVariant) => new()
        {
            Id = id,
            BrandId = brandId,
            EventCode = eventCode,
            RecipientType = Device.UserOwner,
            RecipientId = userId,
            ChannelPlan = [PushChannel],
            Payload = payload,
            ScheduledUtc = scheduledUtc,
            CreatedUtc = now,
            Status = NotificationStatus.Pending,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            TemplateVariant = templateVariant,
        };

    /// <summary>En az bir cihaza push kabul edildi; kontör düşmez.</summary>
    public void MarkPushed(string body, string? providerMessageId)
    {
        EnsurePending();
        AttemptCount++;
        RenderedBody = body;
        ChannelUsed = PushChannel;
        ProviderMessageId = providerMessageId;
        FailureReason = null;
        Status = NotificationStatus.Sent;
    }

    /// <summary>Sağlayıcı mesajı kabul etti; <paramref name="credits"/> bu gönderim için düşülen kontör (platform mesajında 0).</summary>
    public void MarkSent(string body, string? providerMessageId, int credits = 0)
    {
        EnsurePending();
        AttemptCount++;
        RenderedBody = body;
        ChannelUsed = SmsChannel;
        ProviderMessageId = providerMessageId;
        CreditsConsumed = credits;
        FailureReason = null;
        Status = NotificationStatus.Sent;
    }

    /// <summary>Gönderilmedi ve gönderilmeyecek (ör. <c>insufficient_credits</c>; alan-modeli.md § CreditTransaction).</summary>
    public void MarkSkipped(string reason)
    {
        EnsurePending();
        FailureReason = reason;
        Status = NotificationStatus.Skipped;
    }

    /// <summary>
    /// Gönderim denemesi başarısız: <paramref name="maxAttempts"/>'e ulaşıldıysa veya hata kalıcıysa Failed; değilse Pending
    /// kalır ve <paramref name="backoff"/> sonra yeniden denenir (dilim-03 kriter 10).
    /// </summary>
    public void MarkAttemptFailed(string reason, Instant now, Func<int, Duration> backoff, int maxAttempts, bool permanent = false)
    {
        ArgumentNullException.ThrowIfNull(backoff);
        EnsurePending();
        AttemptCount++;
        ScheduleRetryOrFail(reason, now, backoff, maxAttempts, permanent);
    }

    /// <summary>Teslim raporu "teslim edildi": Sent satırda ilk raporda true; tekrar gelen veya geç kalan rapor false.</summary>
    public bool MarkDelivered(Instant now)
    {
        if (Status != NotificationStatus.Sent || DeliveredUtc is not null)
        {
            return false;
        }

        DeliveredUtc = now;
        return true;
    }

    /// <summary>
    /// Teslim raporu "teslim edilemedi": yalnız teslim edilmemiş Sent satırda geçerlidir. Deneme sayısı gönderimde
    /// artmıştı; sınırdaysa Failed, değilse geri çekilmeyle Pending. Düşülen kontör iade edilmek üzere döner (yoksa 0).
    /// </summary>
    public int? MarkDeliveryFailed(string reason, Instant now, Func<int, Duration> backoff, int maxAttempts)
    {
        ArgumentNullException.ThrowIfNull(backoff);
        if (Status != NotificationStatus.Sent || DeliveredUtc is not null)
        {
            return null;
        }

        var refund = CreditsConsumed;
        CreditsConsumed = 0;
        Status = NotificationStatus.Pending;
        ScheduleRetryOrFail(reason, now, backoff, maxAttempts, permanent: false);
        return refund;
    }

    private void ScheduleRetryOrFail(string reason, Instant now, Func<int, Duration> backoff, int maxAttempts, bool permanent)
    {
        FailureReason = reason;
        if (permanent || AttemptCount >= maxAttempts)
        {
            Status = NotificationStatus.Failed;
            return;
        }

        // Tekrar zamanı ScheduledUtc'dir: dağıtıcının taradığı kısmi indeks (status = 'Pending') aynen kullanılır.
        ScheduledUtc = now + backoff(AttemptCount);
    }

    private void EnsurePending()
    {
        if (Status != NotificationStatus.Pending)
        {
            throw new DomainException($"Bildirim {Status} durumunda; yalnız bekleyen bildirim gönderilir.");
        }
    }
}
