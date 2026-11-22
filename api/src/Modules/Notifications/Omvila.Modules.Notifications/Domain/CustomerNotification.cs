using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>
/// Müşteri uygulamasının bildirim listesi satırı (alan-modeli.md § InAppNotification, müşteri karşılığı; sözleşme MeNotification).
/// Push metni olan müşteri mesajı gönderime alındığında, cihaz ve teslimden bağımsız yazılır (T-103). Başlık ve metin o anki
/// push metnidir; kod, IBAN ve not içeriği yazılmaz. Kimlik, kaynağı olan outbox satırının kimliğidir (tekrar denemede ikinci
/// satır yazılmaz).
/// </summary>
internal sealed class CustomerNotification
{
    public const int TitleMaxLength = 80;

    public const int BodyMaxLength = 200;

    public const string AppointmentTarget = "Appointment";

    public const string WaitlistOfferTarget = "WaitlistOffer";

    public const string ReviewTarget = "Review";

    public const string BookingTarget = "Booking";

    private CustomerNotification()
    {
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid? BrandId { get; private set; }

    /// <summary>Sözleşme MeNotificationEvent.</summary>
    public string EventCode { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Body { get; private set; } = string.Empty;

    /// <summary>Sözleşme MeNotificationTarget.type.</summary>
    public string TargetType { get; private set; } = string.Empty;

    public Guid? TargetAppointmentId { get; private set; }

    /// <summary>Bekleme teklifi kodu (/b/{kod}).</summary>
    public string? TargetAccessCode { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Instant? ReadUtc { get; private set; }

    public static CustomerNotification Create(
        Guid id, Guid customerId, Guid? brandId, string eventCode, string title, string body, string targetType,
        Guid? targetAppointmentId, string? targetAccessCode, Instant now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(targetType);
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Bildirim başlığı boş olamaz.");
        }

        return new CustomerNotification
        {
            Id = id,
            CustomerId = customerId,
            BrandId = brandId,
            EventCode = eventCode,
            Title = Fit(title, TitleMaxLength),
            Body = Fit(body ?? string.Empty, BodyMaxLength),
            TargetType = targetType,
            TargetAppointmentId = targetAppointmentId,
            TargetAccessCode = targetAccessCode,
            CreatedUtc = now,
        };
    }

    private static string Fit(string text, int max) => text.Length <= max ? text : string.Concat(text.AsSpan(0, max - 1).TrimEnd(), "…");
}
