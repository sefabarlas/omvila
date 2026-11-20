namespace Omvila.Modules.Identity.Security;

/// <summary>
/// "Legal" bölümü: yürürlükteki kullanım koşulları ve aydınlatma metni sürümü (sureler-ve-ayarlar.md PrivacyNoticeVersion).
/// Kayıtta istemci okuduğu aydınlatma metni sürümünü gönderir; yürürlüktekiyle aynı olmalıdır.
/// </summary>
public sealed class LegalOptions
{
    public const string Section = "Legal";

    public string TermsOfUseVersion { get; set; } = string.Empty;

    public string PrivacyNoticeVersion { get; set; } = string.Empty;

    internal bool IsValid() => !string.IsNullOrWhiteSpace(TermsOfUseVersion) && !string.IsNullOrWhiteSpace(PrivacyNoticeVersion);
}
