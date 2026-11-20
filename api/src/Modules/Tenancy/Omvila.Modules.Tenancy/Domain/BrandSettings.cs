namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Markanın davranış ayarları (alan-modeli.md § BrandSettings; tek satır, BrandId PK). Varsayılanlar
/// sureler-ve-ayarlar.md'deki "Varsayılan" sütunudur; her biri o tablodaki anahtarla aynı adı taşır.
/// </summary>
internal sealed class BrandSettings
{
    private BrandSettings()
    {
    }

    public Guid BrandId { get; private set; }

    public bool RequiresApproval { get; private set; }

    public int ApprovalTimeoutMinutes { get; private set; }

    public bool AllowStaffSelection { get; private set; }

    public bool AllowAnyStaffOption { get; private set; }

    public int CancellationCutoffHours { get; private set; }

    public int RescheduleCutoffHours { get; private set; }

    public int NoShowAfterMinutes { get; private set; }

    public int MinNoticeMinutes { get; private set; }

    public int MaxAdvanceDays { get; private set; }

    public int SlotIntervalMinutes { get; private set; }

    public int DefaultBranchTransferMinutes { get; private set; }

    public string DepositPolicy { get; private set; } = string.Empty;

    public int RiskyCustomerNoShowThreshold { get; private set; }

    public bool MaskCustomerPhoneFromStaff { get; private set; }

    public int[] ReminderOffsetsMinutes { get; private set; } = [];

    public bool ReviewInviteEnabled { get; private set; }

    /// <summary>Kayıtta oluşan ayarlar: sureler-ve-ayarlar.md varsayılanları birebir.</summary>
    /// <summary>Eşzamanlı düzenleme sürümü (PostgreSQL xmin; If-Match/ETag).</summary>
    public uint Version { get; private set; }

    public static BrandSettings CreateDefault(Guid brandId) => new()
    {
        BrandId = brandId,
        RequiresApproval = false,              // RequiresApproval
        ApprovalTimeoutMinutes = 120,          // ApprovalTimeoutMinutes
        AllowStaffSelection = true,            // AllowStaffSelection
        AllowAnyStaffOption = true,            // AllowAnyStaffOption
        CancellationCutoffHours = 24,          // CancellationCutoffHours (✔ onay bekliyor)
        RescheduleCutoffHours = 12,            // RescheduleCutoffHours (✔ onay bekliyor)
        NoShowAfterMinutes = 15,               // NoShowAfterMinutes
        MinNoticeMinutes = 120,                // MinNoticeMinutes
        MaxAdvanceDays = 60,                   // MaxAdvanceDays
        SlotIntervalMinutes = 15,              // SlotIntervalMinutes
        DefaultBranchTransferMinutes = 30,     // DefaultBranchTransferMinutes
        DepositPolicy = "RiskyCustomersOnly",  // DepositPolicy
        RiskyCustomerNoShowThreshold = 2,      // RiskyCustomerNoShowThreshold
        MaskCustomerPhoneFromStaff = true,     // MaskCustomerPhoneFromStaff
        ReminderOffsetsMinutes = [1440, 120],  // ReminderOffsetsMinutes (✔ onay bekliyor)
        ReviewInviteEnabled = true,            // ReviewInviteEnabled
    };
}
