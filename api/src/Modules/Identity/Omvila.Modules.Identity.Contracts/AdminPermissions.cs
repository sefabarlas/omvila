namespace Omvila.Modules.Identity.Contracts;

/// <summary>
/// Platform izin kodları (sözleşme AdminPermission; docs/alan/roller-yetkiler.md § Platform rolleri). Admin uçları rol değil
/// izin ister: <c>.RequireAdminPermission(AdminPermissions.CreditsGrant)</c>. Belge ile bu dosya birebir eşleşir (test doğrular).
/// </summary>
public static class AdminPermissions
{
    /// <summary>Marka listesi, detay, abonelikler, genel bakış.</summary>
    public const string BrandsView = "platform.brands.view";

    /// <summary>Plan değiştirme, marka durumu.</summary>
    public const string BrandsManage = "platform.brands.manage";

    /// <summary>Havale bildirimi eşleştirme, banka hareketlerinde arama.</summary>
    public const string PaymentsMatch = "platform.payments.match";

    /// <summary>SMS gönderen adı onay/ret.</summary>
    public const string SmsSendersDecide = "platform.sms-senders.decide";

    /// <summary>Kontör ekleme (hediye, düzeltme).</summary>
    public const string CreditsGrant = "platform.credits.grant";

    /// <summary>Plan ve fiyat tanımlarını görme.</summary>
    public const string PlansView = "platform.plans.view";

    /// <summary>Plan ve fiyat tanımlarını değiştirme.</summary>
    public const string PlansManage = "platform.plans.manage";

    /// <summary>Fatura işlemleri, banka hareketi içe aktarma ve elle ekleme.</summary>
    public const string BillingManage = "platform.billing.manage";

    /// <summary>Denetim kaydı görüntüleme.</summary>
    public const string AuditView = "platform.audit.view";

    /// <summary>Denetim kaydı dışa aktarma.</summary>
    public const string AuditExport = "platform.audit.export";
}
