using System.Globalization;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Features.NotificationSettings;

/// <summary>
/// Sözleşme NotificationSettings. <c>templates</c> salonun ezebildiği SMS şablonlarıdır; şablon ezmesi gelene kadar boş
/// (T-71): eleman şeması NotificationTemplateSetting o işle eklenir.
/// </summary>
internal sealed record NotificationSettingsResponse(
    IReadOnlyList<int> ReminderOffsetsMinutes,
    bool ReviewInviteEnabled,
    IReadOnlyList<object> Templates,
    string Version)
{
    public static NotificationSettingsResponse From(BrandSettings s) =>
        new(s.ReminderOffsetsMinutes, s.ReviewInviteEnabled, [], s.Version.ToString(CultureInfo.InvariantCulture));
}
