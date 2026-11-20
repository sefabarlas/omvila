namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Randevu kuralları ve kapora ayarları (sözleşme BrandSettings; "Değişiklikleri kaydet" bütün değerleri gönderir).</summary>
internal sealed record BrandSettingsValues(
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
    bool ReviewInviteEnabled);
