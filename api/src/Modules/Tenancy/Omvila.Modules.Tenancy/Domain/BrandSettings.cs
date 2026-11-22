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

    /// <summary>Aydınlatma metni seçimi: Standard (OMVILA metni) veya Custom (salonun metni).</summary>
    public string PrivacyNoticeMode { get; private set; } = PrivacySettingsRules.Standard;

    /// <summary>Salonun kendi aydınlatma metni; Standard'a dönünce saklanır ama gösterilmez.</summary>
    public string? CustomPrivacyNoticeText { get; private set; }

    public NodaTime.Instant? CustomPrivacyNoticeUpdatedUtc { get; private set; }

    /// <summary>İşlem kayıtlarını saklama süresi (yıl; 3, 5, 10). Süre dolunca müşteri bağı anonimleşir (ApplyRetention).</summary>
    public int TransactionRetentionYears { get; private set; } = 3;

    /// <summary>Doğrulanmış değerleri uygular (BrandSettingsRules). Hatırlatmalar büyükten küçüğe, tekrarsız saklanır.</summary>
    public void Apply(BrandSettingsValues v)
    {
        ArgumentNullException.ThrowIfNull(v);
        RequiresApproval = v.RequiresApproval;
        ApprovalTimeoutMinutes = v.ApprovalTimeoutMinutes;
        AllowStaffSelection = v.AllowStaffSelection;
        AllowAnyStaffOption = v.AllowAnyStaffOption;
        CancellationCutoffHours = v.CancellationCutoffHours;
        RescheduleCutoffHours = v.RescheduleCutoffHours;
        NoShowAfterMinutes = v.NoShowAfterMinutes;
        MinNoticeMinutes = v.MinNoticeMinutes;
        MaxAdvanceDays = v.MaxAdvanceDays;
        SlotIntervalMinutes = v.SlotIntervalMinutes;
        DefaultBranchTransferMinutes = v.DefaultBranchTransferMinutes;
        DepositPolicy = v.DepositPolicy;
        RiskyCustomerNoShowThreshold = v.RiskyCustomerNoShowThreshold;
        MaskCustomerPhoneFromStaff = v.MaskCustomerPhoneFromStaff;
        ReminderOffsetsMinutes = [.. v.ReminderOffsetsMinutes.Distinct().OrderDescending()];
        ReviewInviteEnabled = v.ReviewInviteEnabled;
    }

    /// <summary>
    /// Bildirim ayarları (sözleşme manageUpdateNotificationSettings; T-62 (4)): kanonik düzenleyici. Değerler bu satırda
    /// saklanır, BrandSettings yanıtındaki aynı alanlar aynı değeri yansıtır ve iki kaynağın sürümü (xmin) birlikte değişir.
    /// </summary>
    public void ApplyNotificationSettings(IReadOnlyCollection<int> reminderOffsetsMinutes, bool reviewInviteEnabled)
    {
        ArgumentNullException.ThrowIfNull(reminderOffsetsMinutes);
        ReminderOffsetsMinutes = [.. reminderOffsetsMinutes.Distinct().OrderDescending()];
        ReviewInviteEnabled = reviewInviteEnabled;
    }

    /// <summary>
    /// KVKK ve veri (sözleşme manageUpdatePrivacySettings): metin verilmezse (null değil, gönderilmedi) değişmez; açıkça null ise
    /// silinir. Doğrulama <see cref="PrivacySettingsRules"/>'ta.
    /// </summary>
    public void ApplyPrivacy(string mode, bool textProvided, string? customText, int retentionYears, NodaTime.Instant now)
    {
        PrivacyNoticeMode = mode;
        TransactionRetentionYears = retentionYears;
        if (textProvided && !string.Equals(CustomPrivacyNoticeText, customText, StringComparison.Ordinal))
        {
            CustomPrivacyNoticeText = customText;
            CustomPrivacyNoticeUpdatedUtc = now;
        }
    }

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
        PrivacyNoticeMode = PrivacySettingsRules.Standard, // PrivacyNoticeMode
        TransactionRetentionYears = 3,         // TransactionRetentionYears (✔ hukuk)
    };
}
