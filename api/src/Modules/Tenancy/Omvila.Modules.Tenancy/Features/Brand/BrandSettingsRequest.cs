using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Features.Brand;

/// <summary>Sözleşme BrandSettings (yazma). Bütün alanlar zorunludur; eksikler "required" olarak döner.</summary>
internal sealed record BrandSettingsRequest(
    bool? RequiresApproval,
    int? ApprovalTimeoutMinutes,
    bool? AllowStaffSelection,
    bool? AllowAnyStaffOption,
    int? CancellationCutoffHours,
    int? RescheduleCutoffHours,
    int? NoShowAfterMinutes,
    int? MinNoticeMinutes,
    int? MaxAdvanceDays,
    int? SlotIntervalMinutes,
    int? DefaultBranchTransferMinutes,
    string? DepositPolicy,
    int? RiskyCustomerNoShowThreshold,
    bool? MaskCustomerPhoneFromStaff,
    IReadOnlyList<int>? ReminderOffsetsMinutes,
    bool? ReviewInviteEnabled)
{
    public BrandSettingsValues? ToValues(out Dictionary<string, string[]> missing)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        missing = errors;
        void Need(string field, object? value)
        {
            if (value is null)
            {
                errors[field] = ["required"];
            }
        }

        Need("requiresApproval", RequiresApproval);
        Need("approvalTimeoutMinutes", ApprovalTimeoutMinutes);
        Need("allowStaffSelection", AllowStaffSelection);
        Need("allowAnyStaffOption", AllowAnyStaffOption);
        Need("cancellationCutoffHours", CancellationCutoffHours);
        Need("rescheduleCutoffHours", RescheduleCutoffHours);
        Need("noShowAfterMinutes", NoShowAfterMinutes);
        Need("minNoticeMinutes", MinNoticeMinutes);
        Need("maxAdvanceDays", MaxAdvanceDays);
        Need("slotIntervalMinutes", SlotIntervalMinutes);
        Need("defaultBranchTransferMinutes", DefaultBranchTransferMinutes);
        Need("depositPolicy", DepositPolicy);
        Need("riskyCustomerNoShowThreshold", RiskyCustomerNoShowThreshold);
        Need("maskCustomerPhoneFromStaff", MaskCustomerPhoneFromStaff);
        Need("reminderOffsetsMinutes", ReminderOffsetsMinutes);
        Need("reviewInviteEnabled", ReviewInviteEnabled);
        if (missing.Count > 0)
        {
            return null;
        }

        return new BrandSettingsValues(
            RequiresApproval!.Value, ApprovalTimeoutMinutes!.Value, AllowStaffSelection!.Value, AllowAnyStaffOption!.Value,
            CancellationCutoffHours!.Value, RescheduleCutoffHours!.Value, NoShowAfterMinutes!.Value, MinNoticeMinutes!.Value,
            MaxAdvanceDays!.Value, SlotIntervalMinutes!.Value, DefaultBranchTransferMinutes!.Value, DepositPolicy!,
            RiskyCustomerNoShowThreshold!.Value, MaskCustomerPhoneFromStaff!.Value, ReminderOffsetsMinutes!, ReviewInviteEnabled!.Value);
    }
}
