namespace Omvila.Modules.Notifications.Templates;

/// <summary>Metnin kodlaması, birimi ve parça (kontör) sayısı.</summary>
/// <param name="Encoding">Kodlama.</param>
/// <param name="Units">GSM-7'de septet (kaydırma karakteri 2), UCS-2'de UTF-16 kod birimi.</param>
/// <param name="Segments">Parça sayısı = düşülecek kontör (bildirimler.md: ≤ 155 = 1 kontör).</param>
internal sealed record SmsMeasure(SmsEncodingKind Encoding, int Units, int Segments);
