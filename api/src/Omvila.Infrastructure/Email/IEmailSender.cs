namespace Omvila.Infrastructure.Email;

/// <summary>
/// İşletmeye giden OMVILA e-postası (bildirimler.md § E-posta). Gönderen <see cref="EmailOptions.FromAddress"/>. Sağlayıcı T-05
/// seçilene kadar SMTP (Mailpit); boş sağlayıcı kapalı demektir. Adres ve metin günlüğe yazılmaz.
/// </summary>
public interface IEmailSender
{
    /// <summary>E-postayı gönderir; kapalıysa <see cref="EmailSendStatus.Disabled"/>. Geçici hata istisna fırlatır (çağıran yeniden dener).</summary>
    /// <param name="message">İleti.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<EmailSendStatus> SendAsync(EmailMessage message, CancellationToken ct);
}
