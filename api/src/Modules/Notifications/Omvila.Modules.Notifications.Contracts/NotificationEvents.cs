namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Bildirim olay kodları (docs/standartlar/bildirimler.md § Olay kataloğu).</summary>
public static class NotificationEvents
{
    /// <summary>OTP isteği: "OMVILA giriş kodunuz: {kod}…" (her zaman SMS, gönderen OMVILA).</summary>
    public const string OtpCode = "otp.code";

    /// <summary>Personel daveti: "{salon}: Ekibimize eklendiniz…" (SMS; metin taslak, T-32).</summary>
    public const string StaffInvite = "staff.invite";
}
