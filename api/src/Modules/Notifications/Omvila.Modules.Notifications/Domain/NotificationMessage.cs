using NodaTime;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>
/// notifications.notification_outbox satırı (alan-modeli.md § NotificationOutbox). Asgari kapsam (D1): telefona SMS.
/// Push, kanal planı geçişi, kontör ve sessiz saat D3'te.
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

    /// <summary>Telefona SMS satırı; zamanlanmış gönderim verilmezse hemen.</summary>
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
        string? templateVariant = null) => new()
        {
            Id = id,
            BrandId = brandId,
            EventCode = eventCode,
            RecipientPhone = phoneE164,
            Payload = payload,
            ScheduledUtc = scheduledUtc ?? now,
            CreatedUtc = now,
            Status = NotificationStatus.Pending,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            TemplateVariant = templateVariant,
        };

    public void MarkSent(string body, string? providerMessageId)
    {
        AttemptCount++;
        RenderedBody = body;
        ChannelUsed = "Sms";
        ProviderMessageId = providerMessageId;
        Status = NotificationStatus.Sent;
    }

    /// <summary>Geçici hata: en fazla deneme sayısına kadar Pending kalır ve <paramref name="retryAtUtc"/>'de yeniden denenir.</summary>
    public void MarkAttemptFailed(string reason, Instant retryAtUtc, int maxAttempts)
    {
        AttemptCount++;
        FailureReason = reason;
        if (AttemptCount >= maxAttempts)
        {
            Status = NotificationStatus.Failed;
            return;
        }

        ScheduledUtc = retryAtUtc;
    }
}
