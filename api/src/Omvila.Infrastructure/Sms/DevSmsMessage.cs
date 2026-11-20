using NodaTime;

namespace Omvila.Infrastructure.Sms;

/// <summary>infra.dev_sms_log: sahte sağlayıcının "gönderdiği" mesajlar (geliştirme ve test; AGENTS.md).</summary>
internal sealed class DevSmsMessage
{
    public Guid Id { get; init; }

    public required string PhoneE164 { get; init; }

    public required string Sender { get; init; }

    public required string Body { get; init; }

    public Instant SentUtc { get; init; }
}
