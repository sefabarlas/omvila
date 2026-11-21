namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Ayar aralıkları (sözleşme BrandSettings; sureler-ve-ayarlar.md). Alan yolu → hata kodu döner; boşsa geçerlidir.
/// </summary>
internal static class BrandSettingsRules
{
    // Sözleşme reminderOffsetsMinutes: en çok 3 değer, 30–4320 dakika.
    private const int MaxReminderOffsets = 3;
    private const int MinReminderOffsetMinutes = 30;
    private const int MaxReminderOffsetMinutes = 4320;

    private static readonly int[] ApprovalTimeouts = [30, 60, 120, 240];
    private static readonly int[] SlotIntervals = [5, 10, 15];
    private static readonly string[] DepositPolicies = ["None", "RiskyCustomersOnly", "Always"];

    public static Dictionary<string, string[]> Validate(BrandSettingsValues v)
    {
        ArgumentNullException.ThrowIfNull(v);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        void Range(string field, int value, int min, int max)
        {
            if (value < min || value > max)
            {
                errors[field] = ["out_of_range"];
            }
        }

        if (!ApprovalTimeouts.Contains(v.ApprovalTimeoutMinutes))
        {
            errors["approvalTimeoutMinutes"] = ["out_of_range"];
        }

        Range("cancellationCutoffHours", v.CancellationCutoffHours, 0, 168);
        Range("rescheduleCutoffHours", v.RescheduleCutoffHours, 0, 168);
        Range("noShowAfterMinutes", v.NoShowAfterMinutes, 5, 120);
        Range("minNoticeMinutes", v.MinNoticeMinutes, 0, 10080);
        Range("maxAdvanceDays", v.MaxAdvanceDays, 1, 365);
        if (!SlotIntervals.Contains(v.SlotIntervalMinutes))
        {
            errors["slotIntervalMinutes"] = ["out_of_range"];
        }

        Range("defaultBranchTransferMinutes", v.DefaultBranchTransferMinutes, 0, 240);
        if (!DepositPolicies.Contains(v.DepositPolicy, StringComparer.Ordinal))
        {
            errors["depositPolicy"] = ["invalid_format"];
        }

        Range("riskyCustomerNoShowThreshold", v.RiskyCustomerNoShowThreshold, 1, 10);
        if (ReminderOffsetsError(v.ReminderOffsetsMinutes) is { } code)
        {
            errors["reminderOffsetsMinutes"] = [code];
        }

        return errors;
    }

    /// <summary>
    /// Bildirim ayarları (sözleşme NotificationSettingsUpdate): BrandSettings'teki kurallara ek olarak tekrar
    /// <c>invalid_format</c>'tır (manageUpdateNotificationSettings; BrandSettings PUT'u tekrarı sessizce birleştirir).
    /// </summary>
    public static string? ValidateNotificationOffsets(IReadOnlyCollection<int> offsets)
    {
        ArgumentNullException.ThrowIfNull(offsets);
        return ReminderOffsetsError(offsets) ?? (offsets.Distinct().Count() != offsets.Count ? "invalid_format" : null);
    }

    private static string? ReminderOffsetsError(IReadOnlyCollection<int> offsets)
    {
        if (offsets.Count > MaxReminderOffsets)
        {
            return "too_long";
        }

        return offsets.Any(m => m is < MinReminderOffsetMinutes or > MaxReminderOffsetMinutes) ? "out_of_range" : null;
    }
}
