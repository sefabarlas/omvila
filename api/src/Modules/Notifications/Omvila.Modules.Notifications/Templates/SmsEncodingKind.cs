namespace Omvila.Modules.Notifications.Templates;

/// <summary>SMS kodlaması: GSM-7 + Türkçe tekli kaydırma tablosu veya (tabloda olmayan karakter varsa) UCS-2.</summary>
internal enum SmsEncodingKind
{
    Gsm7Turkish,
    Ucs2,
}
