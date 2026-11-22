# Süreler, ayarlar ve varsayılanlar

Bu belge, sistemi yöneten her sayının tek kaynağıdır. Kodda süre veya eşik sabit yazılmaz (`magic number` yasak). Değer ya bu tablodaki bir ayardan okunur ya da `PlatformDefaults` sınıfındaki adlandırılmış sabitten. O sabit de bu tabloya atıf yapar.

**Kapsam** sütunu değerin nerede durduğunu söyler. Daha dar kapsam geniş olanı ezer: `Service` > `Branch` > `Brand` > `Platform`.

"Onay" sütununda ✔ olan değerler ürün sahibinin onayını bekliyor (teslim notları "Yer tutucular ve açık kalanlar"). Varsayılan olarak kullanılır, değişirse yalnız bu tablo ve seed verisi değişir.

## Randevu kuralları ("Ayarlar → Randevu kuralları")

| Anahtar | Varsayılan | İzinli değerler | Kapsam | Ekran / not | Onay |
|---|---|---|---|---|---|
| `RequiresApproval` | `false` | bool | Brand, Service | "Randevular onayıma kesinleşsin". Hizmette `null` = marka ayarı | |
| `ApprovalTimeoutMinutes` | 120 | 30, 60, 120, 240 | Brand | "Yanıt süresi: 2 saat (önerilen)". Dolarsa otomatik iptal | |
| `AllowStaffSelection` | `true` | bool | Brand | "Müşteri personel seçebilsin" | |
| `AllowAnyStaffOption` | `true` | bool | Brand | "'Fark etmez' seçeneği" | |
| `CancellationCutoffHours` | 24 | 0–168 | Brand | "Ücretsiz iptal süresi". Müşteri bu süreden az kala iptal edemez | ✔ |
| `RescheduleCutoffHours` | 12 | 0–168 | Brand | "Erteleme süresi" | ✔ |
| `NoShowAfterMinutes` | 15 | 5–120 | Brand | "'Gelmedi' sayılma süresi" | |
| `MinNoticeMinutes` | 120 | 0–10080 | Brand, Service | "En az ön bildirim: 2 saat" | |
| `MaxAdvanceDays` | 60 | 1–365 | Brand, Service | "En ileri tarih: 60 gün" | |
| `SlotIntervalMinutes` | 15 | 5, 10, 15 | Brand, Branch | "Randevu saat aralığı" | |
| `DefaultBranchTransferMinutes` | 30 | 0–240 | Brand (çift bazında `BranchTransferTime`) | "Şubeler arası geçiş süresi" | |
| `CheckInEarliestMinutes` | 60 | sabit | Platform | "Geldi" butonu en erken başlangıçtan 60 dk önce | |
| `UndoNoShowWindow` | aynı yerel gün | sabit | Platform | "Geldiyse geri al" | |
| `RecurringHorizonMonths` | 3 | sabit | Platform | Tekrarlayan randevuda somut üretilen ufuk | |

## Kapora ("Ayarlar → Kapora ve IBAN")

| Anahtar | Varsayılan | İzinli değerler | Kapsam | Ekran / not | Onay |
|---|---|---|---|---|---|
| `DepositPolicy` | `RiskyCustomersOnly` | `None`, `RiskyCustomersOnly`, `Always` | Brand | "Kapora istenecek durumlar: Yalnızca riskli müşterilerde" | |
| `RiskyCustomerNoShowThreshold` | 2 | 1–10 | Brand | "Riskli müşteri tanımı: 2 kez gelmeyen" | |
| `TransferWindowMinutes` | 120 | 60, 120, 240 | Brand (`BrandBankAccount`) | "Havale süresi". Dolarsa otomatik iptal | ✔ |
| `TransferCodePrefix` | `OMV` | 2–5 büyük harf | Brand | "Açıklama kodu öneki" | |
| `Service.DepositMode` | `Inherit` | `Inherit`, `Always`, `Never` | Service | Hizmet ekle: "Kapora: Her zaman iste" | |
| `Service.DepositAmountMinor` / `DepositPercent` | — | XOR | Service | "Kapora türü: Tutar / Yüzde" | |

**Bir kalem için kapora çözümü:**

```
eğer aktif BrandBankAccount yok          → kapora yok
eğer randevu kanalı Online değil         → kapora yok (v1)
mode = Service.DepositMode
eğer mode == Never                       → kapora yok
eğer mode == Always                      → hizmetin tutarı/yüzdesi
eğer mode == Inherit:
    Brand.DepositPolicy == None          → kapora yok
    Brand.DepositPolicy == Always        → hizmetin tutarı/yüzdesi (tanımlı değilse kapora yok)
    Brand.DepositPolicy == RiskyCustomersOnly
        ve BrandCustomer.NoShowCount >= RiskyCustomerNoShowThreshold
                                         → hizmetin tutarı/yüzdesi (tanımlı değilse kapora yok)
paketten düşülen kalem                   → kapora yok
```

Yüzde kapora kuruşa yukarı yuvarlanmaz, **liraya** yuvarlanır (en yakın tam lira, `MidpointRounding.AwayFromZero`). Havale tutarı kuruşlu olmasın diye. Randevu kaporası kalem kaporalarının toplamıdır.

## Paketler ("Paketler")

| Anahtar | Varsayılan | Kapsam | Not | Onay |
|---|---|---|---|---|
| `Package.ValidityDays` | — | Package | "Geçerlilik": son kullanım günü = satış günü + gün (şubenin yerel takvimi); bu gün dahil kullanılır | |
| `PackageExpiringSoonDays` | 30 | Platform | "Süresi yaklaşan": 30 gün içinde biten Active paketler | |
| `PackageLowSessionThreshold` | 2 | Platform | Müşteri uygulaması Paketlerim "Az kaldı": kalan seans bu sayıyı geçmiyorsa (veya son kullanım `PackageExpiringSoonDays` içindeyse) (T-101) | |
| `ExpireCustomerPackages` | günlük | Platform | Son kullanım günü yerel takvimde geçen Active paket Expired olur (iş saatlik turda çalışır, idempotent; T-95) | |

## Bekleme listesi ve kapıda sıra

| Anahtar | Varsayılan | Kapsam | Not | Onay |
|---|---|---|---|---|
| `WaitlistOfferHoldMinutes` | 15 | Platform | Boşalan saat 15 dk tutulur | |
| `WaitlistMorningEndHour` | 12 | Platform | "Sabah" 12:00'den önce başlayan saat (şube yerel) | T-90 |
| `WaitlistAfternoonEndHour` | 17 | Platform | "Öğleden sonra" 12:00–17:00, "Akşam" 17:00 ve sonrası | T-90 |
| `WalkInAverageServiceMinutes` | hizmet süresinden | — | "Tahmini bekleme ~35 dk" = öndekilerin kalan süreleri / müsait personel | |

## Bildirim ve iletişim ("Ayarlar → Bildirimler ve SMS")

| Anahtar | Varsayılan | Kapsam | Not | Onay |
|---|---|---|---|---|
| `ReminderOffsetsMinutes` | `[1440, 120]` | Brand | "Randevu hatırlatma zamanları: 1 gün önce, 2 saat önce" | ✔ |
| `ReviewInviteEnabled` | `true` | Brand | "Değerlendirme daveti" | |
| `ReviewInviteDelayMinutes` | 120 | Platform | Randevu tamamlandıktan 2 saat sonra | ✔ |
| `QuietHoursLocal` | 22:00–08:00 | Platform (işletme kullanıcısı kendi tercihini değiştirebilir) | İşletme push'ları sessiz saatte gönderilmez. Müşteri hatırlatması sessiz saate denk gelirse 08:00'e kaydırılır. Doğrulama kodu ve kapora mesajı kaydırılmaz | ✔ |
| `DailySummaryLocalTime` | 20:00 | Platform | İşletme uygulaması "Gün sonu özeti · Her akşam 20:00" (şube yerel); kullanıcı yalnız açıp kapatır (varsayılan kapalı, NotificationPreference; T-105) | |
| `LowCreditThreshold` | 200 | Platform | "Bakiye 200'ün altına düşünce" bildirimi | |
| `SmsUsageWindowDays` | 30 | Platform | "Bu tempoyla ~18 gün yeter": son 30 günün net kontör tüketimi (düşüm − iade) ortalamasıyla; tüketim yoksa tahmin yok (T-71) | |
| `SmsMaxCharsSingleCredit` | 155 | Platform | Türkçe karakterle 1 kontör sınırı. Şablon testleri bunu doğrular | |
| `PushTitleMaxChars` / `PushBodyMaxChars` | 40 / 90 | Platform | | |
| `NotificationMaxAttempts` | 5 | Platform | Üstel geri çekilme: 30 sn, 2 dk, 10 dk, 30 dk, 2 sa | |
| `DefaultSmsSender` | `OMVILA` | Platform | Salona özel ad onaylanana kadar | |

## Kimlik ve güvenlik

| Anahtar | Varsayılan | Kapsam | Not | Onay |
|---|---|---|---|---|
| `OtpLength` | 6 | Platform | "6 haneli kod" | |
| `OtpTtlSeconds` | 180 | Platform | | |
| `OtpResendCooldownSeconds` | 60 | Platform | "Kod gelmedi mi? 0:42 sonra tekrar gönderebilirsiniz" | |
| `OtpMaxAttempts` | 5 | Platform | Beşinci yanlışta kod geçersiz | |
| `OtpLockoutMinutes` | 5 | Platform | "5 dakika sonra yeni kod isteyebilirsiniz" | |
| `OtpMaxPerPhonePerHour` | 5 | Platform | Hız sınırı; ayrıca IP başına sınır | |
| `AccessTokenLifetimeMinutes` | 15 | Platform | Tüm audience'lar | |
| `RefreshTokenLifetimeDays` | 30 (hatırla) / 1 (hatırlama) | Platform | Panel "Bu bilgisayarda 30 gün hatırla". Mobil uygulamalar her zaman 30 gün, kullanıldıkça uzar | |
| `PublicBookingTokenMinutes` | 30 | Platform | Rezervasyon sayfasında OTP sonrası jeton | |
| `TrustedDeviceDays` | 30 | Platform | Aynı tarayıcıdan tekrar randevuda kod sorulmaz | |
| `AccessCodeValidityAfterEndDays` | 7 | Platform | SMS bağlantısı randevu bitişinden 7 gün sonra geçersiz | |
| `MaskCustomerPhoneFromStaff` | `true` | Brand | "Telefon numarasını personelden gizle". Personel arama/SMS butonuyla yine ulaşır | |

## KVKK ve saklama ("Ayarlar → KVKK ve veri")

| Anahtar | Varsayılan | Kapsam | Not | Onay |
|---|---|---|---|---|
| `TransactionRetentionYears` | 3 | Brand (izinli: 3, 5, 10) | "İşlem kayıtlarını saklama süresi". Süre dolunca müşteri bağı anonimleşir | ✔ hukuk |
| `DeletionRequestDueDays` | 30 | Platform | Silme talepleri 30 gün içinde tamamlanır | ✔ hukuk |
| `AuditLogRetentionYears` | 2 | Platform | "Kayıtlar 2 yıl saklanır" | ✔ hukuk |
| `OutboxRetentionDays` | 180 | Platform | Gönderilmiş bildirim gövdeleri sonra silinir, sayaçlar kalır | ✔ hukuk |
| `PrivacyNoticeVersion` | `2026-11-01` | Platform / Brand | "OMVILA standart aydınlatma metni" veya salonun kendi metni | |
| `PrivacyNoticeMode` | `Standard` | Brand (`Standard`, `Custom`) | "Kendi metnimi kullan"; `Custom` iken salonun metni (`CustomPrivacyNoticeText`, en fazla 20.000 karakter) gösterilir (`manageUpdatePrivacySettings`) | |
| `DataExportLinkValidityDays` | 7 | Platform | "Tüm verileri dışa aktar" e-postasındaki indirme bağlantısının geçerliliği (T-116) | |

## Google Takvim (K7, T-121)

| Anahtar | Varsayılan | Not | Onay |
|---|---|---|---|
| `Integrations:Calendar:StateMinutes` | 10 | connect-url state geçerliliği (sözleşme) | |
| `Integrations:Calendar:SyncWindowDays` | 60 | Bugünden bu kadar gün sonrasına kadar randevular takvime yazılır (dünden itibaren) | |
| `Integrations:Calendar:SyncIntervalSeconds` | 300 | RefreshCalendarTokens ve SyncCalendarEvents aralığı (mimari.md: 5 dk) | |
| `Integrations:Calendar:RefreshAheadMinutes` | 10 | Erişim jetonu bu süre içinde dolacaksa yenilenir | |

## Abonelik ve platform ("Planlar ve fiyat → Ortak kurallar")

| Anahtar | Varsayılan | Not | Onay |
|---|---|---|---|
| `TrialDays` | 14 | Deneme süresi; kart gerekmez | |
| `PastDueGraceDays` | 7 | 7 gün hatırlatma, sonra salt okunur | |
| `ReadOnlyToCancelledDays` | 30 | Salt okunurdan 30 gün sonra iptal (durum-makinesi.md § 8 "ReadOnly → Cancelled: 30 gün"; `CancelledArchiveDays`'ten ayrı anahtar, T-118) | |
| `CancelledArchiveDays` | 30 | İptalden 30 gün sonra arşiv | |
| `TrialEndingNoticeDays` | 3 | owner.trial_ending e-postası deneme bitmeden 3 gün önce (bildirimler.md § E-posta) | |
| `VatRate` | `[KDV ORANI]` | Fatura KDV oranı (`Platform:VatRatePercent`); tanımlı değilse fatura vergisi 0 yazılır (T-118) | ✔ |
| `TrialCredits` | `[DENEME KONTÖRÜ]` | Deneme hediyesi SMS | ✔ |
| `Plan.*Price*`, `CreditPackage.PriceMinor` | `[AYLIK FİYAT]`, `[FİYAT]` | Veride; kodda yok | ✔ |
| `Plan.StaffLimit`, `IncludedBranchCount` | `[PLAN SINIRI]` | | ✔ |
| `VatNote` | `[KDV NOTU]` | Fiyat gösteriminde | ✔ |
| `OmvilaBankAccount` | `[OMVILA ŞİRKET UNVANI]`, `[OMVILA IBAN]` | Abonelik havalesinde gösterilir; yapılandırmadan okunur | ✔ |
| `ReviewPublicThreshold` | 5 | Rezervasyon sayfasında puan için en az 5 yayınlanmış değerlendirme | ✔ |
| `ReviewOverviewWindowDays` | 90 | Değerlendirmeler "Ortalama · son 90 gün" ve "En çok övülen" penceresi (T-89); işletme uygulaması Primim "Puan" da aynı pencere (personelin değerlendirmeleri, T-105) | |
| `BookingDomain` | `omvila.com` (örnek) | Rezervasyon ve kısa bağlantı alanı; yapılandırmadan | ✔ |
| `SlugRedirectDays` | 30 | Rezervasyon bağlantı adı değişince eski adres bu kadar gün yeni adrese yönlenir (alan-modeli.md § Brand, `BrandSlugHistory`) | |

## Yer tutucu kuralı
Köşeli parantezli değerler kodda **sabit metin olarak geçmez**. Fiyat ve plan değerleri veritabanındaki `Plan` ve `CreditPackage` satırlarından, banka bilgileri ve alan adı yapılandırmadan (`appsettings` / ortam değişkeni) okunur. Seed verisi geliştirme ortamında yer tutucu metni gösterir. Böylece değer geldiğinde tek yerde değişir.
