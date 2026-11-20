using System.Globalization;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Features.Brand;

/// <summary>Sözleşme BrandSettings (okuma).</summary>
internal sealed record BrandSettingsResponse(
    bool RequiresApproval,
    int ApprovalTimeoutMinutes,
    bool AllowStaffSelection,
    bool AllowAnyStaffOption,
    int CancellationCutoffHours,
    int RescheduleCutoffHours,
    int NoShowAfterMinutes,
    int MinNoticeMinutes,
    int MaxAdvanceDays,
    int SlotIntervalMinutes,
    int DefaultBranchTransferMinutes,
    string DepositPolicy,
    int RiskyCustomerNoShowThreshold,
    bool MaskCustomerPhoneFromStaff,
    IReadOnlyList<int> ReminderOffsetsMinutes,
    bool ReviewInviteEnabled,
    bool DepositAvailable,
    string Version)
{
    public static BrandSettingsResponse From(BrandSettings s, bool depositAvailable) => new(
        s.RequiresApproval, s.ApprovalTimeoutMinutes, s.AllowStaffSelection, s.AllowAnyStaffOption, s.CancellationCutoffHours,
        s.RescheduleCutoffHours, s.NoShowAfterMinutes, s.MinNoticeMinutes, s.MaxAdvanceDays, s.SlotIntervalMinutes,
        s.DefaultBranchTransferMinutes, s.DepositPolicy, s.RiskyCustomerNoShowThreshold, s.MaskCustomerPhoneFromStaff,
        s.ReminderOffsetsMinutes, s.ReviewInviteEnabled, depositAvailable, s.Version.ToString(CultureInfo.InvariantCulture));
}
