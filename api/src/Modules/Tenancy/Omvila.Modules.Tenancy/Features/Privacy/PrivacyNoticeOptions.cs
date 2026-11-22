using NodaTime;
using NodaTime.Text;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Privacy;

/// <summary>
/// Yürürlükteki OMVILA standart aydınlatma metninin sürümü ("Legal:PrivacyNoticeVersion", sureler-ve-ayarlar.md
/// PrivacyNoticeVersion). Sürüm yyyy-MM-dd biçimindedir ve "Son güncelleme" günü olarak gösterilir.
/// </summary>
internal sealed class PrivacyNoticeOptions
{
    public const string Section = "Legal";

    public string PrivacyNoticeVersion { get; set; } = string.Empty;

    internal LocalDate? UpdatedOn => LocalDatePattern.Iso.Parse(PrivacyNoticeVersion) is { Success: true } r ? r.Value : null;

    internal bool IsValid() => UpdatedOn is not null;
}
