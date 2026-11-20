namespace Omvila.Modules.Identity.Contracts;

/// <summary>
/// İzin kodları (docs/alan/roller-yetkiler.md § İzin kataloğu). Uç nokta rol değil izin ister:
/// <c>.RequirePermission(Permissions.Settings.Manage)</c>. Belge ile bu dosya birebir eşleşir (test doğrular).
/// </summary>
public static class Permissions
{
    /// <summary>Takvim.</summary>
    public static class Calendar
    {
        /// <summary>Takvimi görme.</summary>
        public const string View = "calendar.view";

        /// <summary>Diğer personelin kolonlarını görme.</summary>
        public const string ViewAllStaff = "calendar.view.all-staff";
    }

    /// <summary>Randevu.</summary>
    public static class Appointment
    {
        /// <summary>Randevu ekleme.</summary>
        public const string Create = "appointment.create";

        /// <summary>Saat, personel, hizmet değiştirme.</summary>
        public const string Update = "appointment.update";

        /// <summary>İptal.</summary>
        public const string Cancel = "appointment.cancel";

        /// <summary>Gelmiş randevuyu iptal.</summary>
        public const string CancelCheckedIn = "appointment.cancel.checkedin";

        /// <summary>Onay bekleyen talebi onaylama veya reddetme.</summary>
        public const string Approve = "appointment.approve";

        /// <summary>"Kapora geldi" işaretleme.</summary>
        public const string DepositMark = "appointment.deposit.mark";

        /// <summary>"Geldi" ve "Geldiyse geri al".</summary>
        public const string CheckIn = "appointment.checkin";

        /// <summary>"Tamamla ve ödeme al".</summary>
        public const string Complete = "appointment.complete";
    }

    /// <summary>Müşteri.</summary>
    public static class Customer
    {
        /// <summary>Müşteri listesi ve kartı.</summary>
        public const string View = "customer.view";

        /// <summary>Tam telefon numarası.</summary>
        public const string PhoneView = "customer.phone.view";

        /// <summary>Müşteri ekleme, düzenleme, not.</summary>
        public const string Edit = "customer.edit";

        /// <summary>Kayıt birleştirme.</summary>
        public const string Merge = "customer.merge";

        /// <summary>CSV içe aktarma.</summary>
        public const string Import = "customer.import";

        /// <summary>CSV dışa aktarma.</summary>
        public const string Export = "customer.export";

        /// <summary>KVKK silme talebi işleme.</summary>
        public const string Kvkk = "customer.kvkk";
    }

    /// <summary>Kapıda sıra.</summary>
    public static class WalkIn
    {
        /// <summary>Kapıda sıra yönetimi.</summary>
        public const string Manage = "walkin.manage";
    }

    /// <summary>Bekleme listesi.</summary>
    public static class Waitlist
    {
        /// <summary>Bekleme listesi yönetimi.</summary>
        public const string Manage = "waitlist.manage";
    }

    /// <summary>Ödeme.</summary>
    public static class Payment
    {
        /// <summary>Ödeme alma, kasa.</summary>
        public const string Collect = "payment.collect";
    }

    /// <summary>Kasa.</summary>
    public static class Cash
    {
        /// <summary>Günü kapatma.</summary>
        public const string Close = "cash.close";
    }

    /// <summary>Paket.</summary>
    public static class Package
    {
        /// <summary>Müşteriye paket satma.</summary>
        public const string Sell = "package.sell";

        /// <summary>Paket tanımları.</summary>
        public const string Manage = "package.manage";
    }

    /// <summary>Rapor.</summary>
    public static class Report
    {
        /// <summary>Raporlar.</summary>
        public const string View = "report.view";
    }

    /// <summary>Prim.</summary>
    public static class Commission
    {
        /// <summary>Prim görme.</summary>
        public const string View = "commission.view";
    }

    /// <summary>Değerlendirme.</summary>
    public static class Review
    {
        /// <summary>Değerlendirmeleri görme.</summary>
        public const string View = "review.view";

        /// <summary>Değerlendirmeye yanıt.</summary>
        public const string Reply = "review.reply";
    }

    /// <summary>Hizmet.</summary>
    public static class Service
    {
        /// <summary>Hizmet, kategori, fiyat düzenleme.</summary>
        public const string Manage = "service.manage";
    }

    /// <summary>Personel.</summary>
    public static class Staff
    {
        /// <summary>Personel, çalışma saati, izin.</summary>
        public const string Manage = "staff.manage";
    }

    /// <summary>İzin (tatil).</summary>
    public static class TimeOff
    {
        /// <summary>Kendi iznini girme.</summary>
        public const string Request = "timeoff.request";
    }

    /// <summary>Ayarlar.</summary>
    public static class Settings
    {
        /// <summary>İşletme, şube, kurallar, rezervasyon sayfası, bildirim.</summary>
        public const string Manage = "settings.manage";

        /// <summary>Kapora ve IBAN.</summary>
        public const string Payment = "settings.payment";

        /// <summary>Kullanıcı ve rol yönetimi.</summary>
        public const string Roles = "settings.roles";
    }

    /// <summary>Abonelik.</summary>
    public static class Subscription
    {
        /// <summary>Abonelik, kart, havale bildirimi, kontör satın alma.</summary>
        public const string Manage = "subscription.manage";
    }
}
