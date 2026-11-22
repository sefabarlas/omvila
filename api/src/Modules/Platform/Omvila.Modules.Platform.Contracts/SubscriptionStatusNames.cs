namespace Omvila.Modules.Platform.Contracts;

/// <summary>
/// Abonelik durum adları (durum-makinesi.md § 8; sözleşme SubscriptionStatus). Diğer modüller durumu metinle değil bu sabitlerle
/// ve <see cref="SubscriptionBannerInfo.BlocksNewWork"/> ile karşılaştırır.
/// </summary>
public static class SubscriptionStatusNames
{
    /// <summary>Deneme.</summary>
    public const string Trialing = "Trialing";

    /// <summary>Aktif.</summary>
    public const string Active = "Active";

    /// <summary>Ödeme gecikti (her şey çalışır).</summary>
    public const string PastDue = "PastDue";

    /// <summary>Salt okunur.</summary>
    public const string ReadOnly = "ReadOnly";

    /// <summary>İptal edildi (panel kapalı).</summary>
    public const string Cancelled = "Cancelled";

    /// <summary>
    /// Salonun yeni iş başlatamadığı durumlar (durum-makinesi.md § 8 ReadOnly: online rezervasyon, yeni randevu ve salonun başlattığı
    /// SMS kapalı; Cancelled'da panel de kapalı). Var olan randevuların hatırlatmaları bu durumlarda da gider (K5).
    /// </summary>
    public static bool BlocksNewWork(string? status) =>
        string.Equals(status, ReadOnly, StringComparison.Ordinal) || string.Equals(status, Cancelled, StringComparison.Ordinal);
}
