namespace Omvila.Modules.Integrations.Calendar;

/// <summary>
/// "Integrations:Calendar" (K7, T-121). Sağlayıcı: boş → kapalı (bağlanma yok); <c>Fake</c> yalnız Development, Testing ve izinli
/// Staging; <c>Google</c> istemci kimliği ve gizlisi yoksa kapalı. Jeton şifreleme anahtarı ortamdan (kasadan).
/// </summary>
internal sealed class CalendarOptions
{
    public const string Section = "Integrations:Calendar";
    public const string TokenEncryptionKeySetting = Section + ":TokenEncryptionKey";
    public const string FakeProvider = "Fake";
    public const string GoogleProvider = "Google";

    public string Provider { get; set; } = string.Empty;

    /// <summary>OAuth jetonlarını şifreleyen anahtar (AES-256-GCM; base64, 32 bayt). Ortam değişkeni Integrations__Calendar__TokenEncryptionKey.</summary>
    public string? TokenEncryptionKey { get; set; }

    /// <summary>Sağlayıcının döneceği adresler: Web (salon paneli) ve Mobile (işletme uygulaması).</summary>
    public CalendarRedirectUris RedirectUris { get; set; } = new();

    public GoogleCalendarOptions Google { get; set; } = new();

    /// <summary>connect-url state'inin geçerliliği (dakika; sözleşme: 10 dk).</summary>
    public int StateMinutes { get; set; }

    /// <summary>Aktarım penceresi: bugünden kaç gün sonrasına kadar randevu takvime yazılır.</summary>
    public int SyncWindowDays { get; set; }

    /// <summary>RefreshCalendarTokens ve SyncCalendarEvents aralığı (saniye; mimari.md: 5 dk).</summary>
    public int SyncIntervalSeconds { get; set; }

    /// <summary>Erişim jetonu bu kadar dakika içinde dolacaksa yenilenir.</summary>
    public int RefreshAheadMinutes { get; set; }

    internal bool IsValid() => StateMinutes > 0 && SyncWindowDays > 0 && SyncIntervalSeconds > 0 && RefreshAheadMinutes >= 0
        && Provider is "" or FakeProvider or GoogleProvider;
}
