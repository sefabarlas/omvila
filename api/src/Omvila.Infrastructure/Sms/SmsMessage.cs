namespace Omvila.Infrastructure.Sms;

/// <param name="PhoneE164">Alıcı, E.164.</param>
/// <param name="Sender">Gönderen başlığı (OMVILA veya salonun onaylı başlığı).</param>
/// <param name="Body">Gönderilecek metin (bildirimler.md: ≤ 155 karakter = 1 kontör).</param>
public sealed record SmsMessage(string PhoneE164, string Sender, string Body);
