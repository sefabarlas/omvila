# OMVILA — Teknik Detaylar, Akış Kuralları ve Kısıtlar

**Amaç:** Frontend, mobil ve entegrasyon çalışırken "sistem bunu yapar mı, hangi koşulda yapar" sorularının tek yerde cevabı.
**Kaynaklar:** `omvila-veri-mimarisi.md` (v1.0, tasarım), `omvila-backend-plani.md` (Faz 0–11), Faz 12/13 dokümanları, `OmvilaDbContextModelSnapshot.cs` (gerçek kod), Swagger dosyaları.

**Güven etiketleri**
- 🟢 **Kodda doğrulandı:** snapshot'taki CHECK kısıtı, kolon veya indeksten görülüyor.
- 🟡 **Tasarım kuralı:** dokümanda yazıyor ama ilgili endpoint/davranış kodda ayrıca doğrulanmadı.
- 🔴 **Açık:** karar yok veya backend'e sorulmalı.

---

## 1. Platform ve mimari kısıtlar

| Konu | Kural |
|---|---|
| Yığın | .NET 10, ASP.NET Core Minimal API, EF Core code-first, SQL Server 2022, Hangfire, Serilog |
| Mimari | Modüler monolit, dikey dilim; MediatR ve AutoMapper yok |
| API yüzeyleri | `/api/v1/manage` (salon), `/api/v1/public` (anonim + OTP), `/api/v1/me` (üye müşteri), `/api/v1/admin` (OMVILA ekibi). Her biri ayrı yetkilendirme, hız sınırı ve OpenAPI dokümanı |
| Kimlik | Üç farklı oturum türü: marka kullanıcısı (`BrandId`), platform (`platform:true` claim, `User.BrandId` NULL), müşteri (`CustomerId`, `BrandId` yok) |
| Hata kuralı | Başka markanın kaydı istendiğinde **404** (403 değil; varlığı sızdırılmaz). Yetkisiz işlemde 403 |
| Zaman | Tüm zamanlar UTC (`...Utc`, `datetime2(0)`). Yerel saat yalnızca çalışma saatlerinde (`time(0)`). Gösterim `Branch.TimeZoneId` ile çevrilir 🟢 |
| Para | `int`, kuruş (`...Minor`) + `CurrencyCode char(3)`. `money` tipi yok 🟢 |
| ID | Guid (`NEWSEQUENTIALID()`) |
| Silme | Fiziksel silme yok; `IsActive`/`Status` kullanılır. Tek istisna KVKK anonimleştirme (`Customer.AnonymizedUtc`) |
| Eşzamanlılık | Düzenlenebilir tablolarda `RowVersion`; ikinci kaydeden hata alır (iyimser kilit) 🟢 |
| Telefon | Tek bir E.164 normalizasyon sınıfı; OTP, panel, CSV ve public rezervasyon hepsi oradan geçer. "0532 123 45 67", "5321234567", "+90 532 123 4567" aynı kişi |

---

## 2. Kiracı izolasyonu (multi-tenancy)

1. Her tabloda `BrandId` vardır (başka tablodan türetilebilse bile).
2. İki katman birlikte çalışır: **EF global query filter** (ergonomi) + **SQL Server Row-Level Security** (güvenlik garantisi). `IgnoreQueryFilters()` kullanımı yasak.
3. RLS tek predicate fonksiyonunda üç dalı **OR** ile birleştirir (SQL Server birden çok filter predicate'i AND'ler, bu yüzden ayrı satır yazılmaz):
   - marka: `BrandId = SESSION_CONTEXT('BrandId')`
   - platform: `SESSION_CONTEXT('IsPlatform') = 1`
   - müşteri: `CustomerId = SESSION_CONTEXT('CustomerId')`
4. Müşteri erişimi iki desen:
   - Doğrudan `CustomerId` taşıyan tablolar: `Appointment`, `BrandCustomer`, `Review`, `ConsentRecord`.
   - `BrandCustomerId` üzerinden dolaylı: `CustomerPackage`, `Payment` → `PaymentAllocation` (RLS içinde join).
5. Müşteri birden çok markanın müşterisi olabilir ama her markada yalnızca kendi kaydını görür.
6. Platform ekibi tüm markaları görür; rol bazlı (Support yalnızca görüntüleme + onay kuyrukları, Finance faturalama).

---

## 3. Kimlik, giriş ve oturum

| Konu | Kural |
|---|---|
| Salon girişi | Telefon + OTP → JWT (marka, roller, şube kapsamı) + refresh token |
| Müşteri girişi | Ayrı akış: `POST /me/auth/otp/request` → `verify` → `refresh` → `logout`. Müşteri JWT'si personel JWT'sinden ayrı 🟢 (Swagger) |
| OTP | Hız sınırı vardır (aynı numaraya dakikada N kod) 🟡. İlk doğrulamadan sonra cihaza token bırakılır; aynı cihazdan tekrar randevuda OTP istenmez 🟡 |
| Yetki | `RequirePermission("appointment.create")` benzeri izin tabanlı kontrol. Şube müdürü yalnızca kendi şubesinde işlem yapar |
| Personel telefon maskesi | `BrandSettings.MaskCustomerPhoneFromStaff = 1` ise `Staff` rolü maskeli telefon görür, yönetici tam görür |
| Prim gizliliği | Personel yalnızca kendi prim kayıtlarını görür |
| Geliştirme | Sahte SMS sağlayıcısı konsola yazar (Faz 1); gerçek sağlayıcı Faz 9'da |

---

## 4. Müşteri modeli

- `Customer` **global** (telefon E.164 UNIQUE), `BrandCustomer` **markaya özel** (not, etiket, sayaçlar). Aynı telefon iki markada = tek `Customer`, iki `BrandCustomer`.
- **Misafir ayrı varlık değil:** `Customer` + `HasAccount = 0`. `IsVerified` = OTP yapıldı; `HasAccount` = şifre/hesap belirledi.
- Not yalnızca `BrandCustomer` üzerindedir; A markasının notu B'de görünmez.
- Birleştirme: `Customer.MergedIntoId`. Birleştirme sonrası eski Id ile sorgu hedef kayda yönlenir.
- Doğum **yılı** tutulur (tam tarih yok). Sağlık verisi tutulmaz.
- Telefon CSV içe aktarmada yinelenenler birleştirilir, hatalı satırlar raporlanır.
- KVKK silme talebi: ad/telefon/e-posta maskelenir (`AnonymizedUtc`), işlem kayıtları korunur.
- `ConsentRecord`: rıza izi (pazarlama izni açma/kapama müşteri `/me` üzerinden yapabilir).

---

## 5. Randevu yaşam döngüsü

### 5.1 Durumlar 🟢 (CHECK kısıtlarından)

| Tablo | Değerler |
|---|---|
| `Appointment.Status` | `Draft`, `PendingApproval`, `Confirmed`, `CheckedIn`, `Completed`, `Cancelled`, `NoShow` |
| `AppointmentItem.Status` | `Confirmed`, `Completed`, `Cancelled` (kalemde NoShow yok) |
| `Appointment.Channel` | `Online`, `Panel`, `Phone`, `WalkIn` |

### 5.2 Geçişler 🟡

```
Draft → PendingApproval → Confirmed → CheckedIn → Completed
              ↓               ↓           ↓
          Cancelled       Cancelled   Cancelled
                          NoShow
```

- Otomatik onaylı salonda `PendingApproval` atlanır.
- `CheckedIn` atlanmaz (no-show'u otomatik işaretleyebilmek için gerekli).
- `Appointment.StartUtc/EndUtc` kalemlerden türetilen **özet**tir; kalem değişince güncellenir.

### 5.3 Onay (`RequiresApproval`)

- `BrandSettings.RequiresApproval` (varsayılan 0) + `Service.RequiresApproval` (NULL = marka ayarını ezme).
- Onay bekleyen randevu slotu geçici tutar: `HoldExpiresUtc = şimdi + ApprovalTimeoutMinutes` (varsayılan 120 dk).
- Süre dolunca Hangfire işi randevuyu otomatik `Cancelled` yapar, slot serbest kalır, müşteriye bildirim gider.
- `HoldExpiresUtc` indeksi yalnızca `PendingApproval` satırları için tanımlı 🟢. Yani tutma süresi `Confirmed` randevuda kullanılmaz.

### 5.4 İptal ve erteleme

- `CancellationCutoffHours`: randevuya bu süreden az kala **müşteri** iptal edemez. `RescheduleCutoffHours`: erteleme için ayrı eşik. 🟡 Salon eşiğe tabi değil (🔴 doğrulanmalı).
- İptalde `CancelledUtc`, `CancelledByUserId`, `CancelledByCustomer` (bit) ve `CancellationReason` yazılır. `/me` iptalinde sebep **zorunlu** alan 🟢 (Swagger: `cancellationReason` required).
- İptalde seans geri döner (bkz. 8), prim ters kaydı açılır, Google Takvim olayı güncellenir.

### 5.5 No-show

- `BrandSettings.NoShowAfterMinutes` sonrasında arka plan işi `Confirmed` randevuyu `NoShow` yapar. `Customer/BrandCustomer.NoShowCount` kapora politikasının girdisidir.

### 5.6 Fiyat dondurma

- Fiyat randevu oluşurken `AppointmentItem`'a kopyalanır (`PriceMinor`, `DiscountMinor`, `StaffSurchargeMinor`). Sonradan hizmet fiyatı değişse geçmiş randevu tutarı değişmez.
- Fiyat önceliği: `BranchServicePrice` (şube istisnası) > `Service.BasePriceMinor`; personel farkı `ServiceStaff` ek ücretiyle.

### 5.7 Çok hizmetli randevu

- Yapı: `Appointment` (kap) → `AppointmentItem` (planlama birimi, kendi başlangıcı ve fiyatı) → `AppointmentItemSegment` (gerçek meşgul süre) → `AppointmentItemResource` (oda/cihaz).
- `AppointmentItem.StaffId` kalem bazında ve nullable 🟢; `Appointment` üzerinde personel yok. Hizmet başına farklı personel **şema olarak mümkün**; online akışta bunun açık olup olmadığı 🔴 (backend'e sorulacak).
- `ServiceStage` → segment açılımı: boya gibi bekleme dilimli hizmetlerde bekleme süresince personel `StaffBusy=0` olur ve başka müşteriye müsait görünür.
- Paketten düşülen kalem `CustomerPackageItemId` taşır.

---

## 6. Müsaitlik (slot) hesabı

Hesap C# içinde saf fonksiyonlarla yapılır (aralık cebri: `Intersect`, `Subtract`, `Merge`, `Split`), SQL'de değil. Sekiz adım:

1. Şube çalışma saatleri (günde birden çok aralık); `WorkingHoursOverride` ve `PublicHoliday` ezer.
2. Personelin o şubedeki çalışma saatleriyle kesiştir.
3. Personelin **tüm şubelerdeki** izinlerini (`TimeOff`) çıkar.
4. Personelin **tüm şubelerdeki** meşgul segmentlerini çıkar.
5. Başka şubedeki randevu çevresine geçiş tamponu (`BranchTransferTime`) uygula.
6. Hizmetin gerektirdiği kaynak müsaitliğiyle kesiştir (`ServiceResourceRule`, `ResourceDowntime`).
7. `Service.MinNoticeMinutes` ve `MaxAdvanceDays` ile kırp.
8. Kalan aralığa `SlotIntervalMinutes` ızgarasında hizmet süresini (buffer dahil) yerleştir.

**Kısıtlar**
- Meşguliyet sorgusu **asla** şubeyle filtrelenmez (aynı personel aynı saatte iki şubede olamaz).
- Aralıklar yarı açık: `[Start, End)`. 14:00'te biten ile 14:00'te başlayan çakışmaz; 1 dakika örtüşen çakışır. `BETWEEN` kullanılmaz.
- "Personel farketmez": uygun her personel için hesap tekrarlanır (`AllowAnyStaffOption` açıksa sunulur). v1'de önbellek yok; hedef 12 personelli haftalık sorgu < 500 ms.
- Müşteri personel seçimi `BrandSettings.AllowStaffSelection` ile kapatılabilir.
- Slot ızgarası öncelik: `Branch.SlotIntervalMinutes` (nullable) > `BrandSettings.SlotIntervalMinutes` (5/10/15).
- Public uçlar: `GET /public/branches/{branchId}/availability` (staffId, serviceId, date) ve `.../availability/any-staff` (serviceId, fromDate, toDate) 🟢.
- **Not:** Yüklenen public Swagger'da rezervasyon **oluşturma** ucu yok (yalnızca müsaitlik, Google callback, ping). Online rezervasyon akışının yazma tarafı 🔴 doğrulanmalı.

---

## 7. Çift rezervasyon engeli ve eşzamanlılık

SQL Server'da zaman aralığı EXCLUDE kısıtı yoktur; engel uygulama katmanındadır:

- `sp_getapplock` ile kilit, anahtar **personel + gün** (`staff:{id}:{yyyy-MM-dd}`), transaction kapsamlı, timeout 5 sn.
- Kaynak (oda/cihaz) kilitleri de alınır; **daima Id sırasıyla** (deadlock önleme).
- Kilit altında: çakışma kontrolü → randevu/kalem/segment/kaynak ekleme → outbox satırı, **tek transaction**.
- Aynı slota paralel iki istekten tam biri başarılı olur (testle doğrulanır, sıralı test yetmez).

---

## 8. Paket ve seans kuralları

- Kalan seans **sayaç değil, kullanım defteri**: `Kalan = TotalSessions − (RevertedUtc'i boş kullanım sayısı)` (`PackageSessionUsage`).
- Randevu iptal → `RevertedUtc` dolar, seans geri döner (geri dönüş tarihli görünür).
- Süresi dolmuş paketin seansı geri dönse bile **kullanılamaz** (`ExpiresOn`).
- `CustomerPackage.Status`: `Active`, `Expired`, `Refunded`, `Transferred`. Paket markaya bağlıdır, şubeler arası kullanılabilir (kod: evet).
- Paket birden çok hizmet içerebilir; kalan seans **hizmet düzeyinde** (`CustomerPackageItem`).
- Raporlamada **tahsilat** ile **hak edilen gelir** ayrıdır (peşin satılan paket henüz kazanılmış gelir değil).
- 🔴 Kısmi kullanılmış paketin iade hesabı henüz karara bağlanmadı.

---

## 9. Ödeme, kapora ve prim

### 9.1 Ödeme
- `Payment.Method`: `Cash`, `Card`, `Transfer`, `Online`, `PackageRedemption` 🟢
- `Payment.Status`: `Pending`, `Succeeded`, `Failed`, `Refunded`, `PartiallyRefunded` 🟢
- `ProviderName`: `Iyzico`, `PayTR` 🟢; `ProviderReference` iade/mutabakat için zorunlu referans.
- Bir ödeme `PaymentAllocation` ile birden çok hedefe (`Appointment`, `CustomerPackage`) bölünebilir; bir randevu birden çok ödemeyle kapanabilir (kapora + salonda nakit). `TargetType/TargetId` FK'siz (polimorfik), bütünlük uygulamada.
- Kapsam: **basit tahsilat kaydı**. Ürün satışı, stok, tam POS ve Z raporu yok (v2). Gün sonu kasa özeti var (nakit/kart/havale).

### 9.2 Kapora
- `BrandSettings.DepositPolicy`: `None`, `Always`, `RiskyCustomersOnly`, `ByService` 🟢; önerilen `RiskyCustomersOnly`, eşik `DepositThresholdNoShowCount`.
- Hizmette `DepositMinor` **veya** `DepositPercent` (ikisinden biri).
- Kapora **doğrudan salonun** alt üye işyerine gider; OMVILA tahsilata girmez.
- Randevuda `DepositRequiredMinor` istenen tutarı tutar; kapora ödemesi `Payment.IsDeposit = 1`.
- "Kapora bekleniyor" için ayrı randevu durumu **yoktur** 🟢. Büyük olasılıkla `PendingApproval` + `HoldExpiresUtc` + `Pending` ödeme, ancak 🔴 kodda doğrulanmalı.

### 9.3 Ödeme hesabı (alt üye işyeri)
- `BrandPaymentAccount.Status`: `NotStarted`, `Pending`, `Approved`, `Rejected` 🟢. `Provider` başına bir kayıt (UNIQUE `BrandId+Provider`).
- Kayıt akışında zorunlu değil: salon önce değeri görür, evrak sonra gelir; `NotStarted` geçerli durumdur.
- `Approved` olmadan kapora istenemez (🟡: panel tarafında kapora ayarı onaya bağlı).
- `SubMerchantKey` ve `Iban` şifreli/ maskeli tutulmalıdır. Onay/ret `/admin` onay kuyruğundan yapılır, markaya bildirim gider.

### 9.4 Prim (`CommissionEntry`)
- `Type`: `Service`, `Sale` 🟢. `Status`: `Accrued`, `Approved`, `Paid`, `Reversed` 🟢.
- Hizmet primi `AppointmentItem.StaffId`'den, **randevu `Completed` olunca** ve seans başına doğar. Satış primi `CustomerPackage.SoldByStaffId`'den.
- `Percent` hesap anındaki **kopyadır**; sonradan oran değişse geçmiş hakediş değişmez.
- İptal/iade: kayıt silinmez, `Reversed` ters kayıt açılır.
- Dönem kapanışı `PeriodYearMonth` (ör. 202607).

---

## 10. Bildirim ve iletişim

- **Outbox deseni:** `NotificationOutbox` satırı randevu işlemiyle **aynı transaction'da** yazılır. Randevu başarısızsa satır da yok.
- İşleyici Hangfire, kuyruğu `READPAST` ile çeker (çoklu worker çakışmaz).
- `Channel`: `Sms`, `Push`, `Email`. Kanal-bağımsız, **geri düşüşlü**: push dene → token yoksa/başarısızsa SMS. WhatsApp v1.1.
- `Category`: `Transactional` | `Marketing`. `Marketing` göndermeden önce rıza (+ İYS) kontrolü zorunlu; rızasız gönderim engellenir. Randevu hatırlatması işlem bildirimi sayılır (🔴 avukat görüşü bekleniyor).
- Durumlar: `Pending`, `Sending`, `Sent`, `Failed`, `Cancelled`. `RetryCount` kadar üstel geri çekilmeyle tekrar, sonra `Failed`.
- Hatırlatma zamanları `BrandSettings.ReminderOffsets` (JSON dizi, dakika; ör. `[1440,120]`, CHECK `ISJSON`).
- SMS **kontör** harcar: `CreditTransaction` defteri; bakiye negatife düşmez, yetersizse SMS engellenir ve uyarı üretilir.
- Gönderen başlığı `BrandSmsSender` (operatör onayı gerekir; CRM'de takip edilir).
- Şablon: `NotificationTemplate` (`BrandId` NULL = sistem şablonu; salon kendi metniyle ezebilir), dil seçimli.
- Push için `CustomerDevice` var 🟢 (`Platform`: `iOS`/`Android`, `PushToken` aktif kayıtlar arasında UNIQUE). `/me/devices` ucu henüz yok (mobil faza ertelendi).

---

## 11. Bekleme listesi, walk-in, tekrarlayan randevu

- **Bekleme listesi:** slot boşalınca ilk kişiye bildirim gider, slot kısa süre (öneri 15 dk) `HoldExpiresUtc` ile tutulur.
- **Walk-in kuyruğu:** `BranchQueueCounter` ile şube + gün bazlı sıra numarası (1'den başlar, `UPDATE ... OUTPUT` ile atomik; iki şubenin numaraları bağımsız). Kuyruktan randevuya dönüştürülebilir.
- **Tekrarlayan randevu:** RRULE tabanlı; ilk 3 ay somut satır üretilir, job yeni dönemleri üretmeye devam eder. Erteleme/iptal "yalnızca bu" veya "bundan sonrakiler" ayrımı; geçmiş randevular etkilenmez.

---

## 12. Abonelik ve platform kuralları

- `Subscription.Status` akışı: `Trialing → Active → PastDue → ReadOnly → Cancelled`.

| Durum | Davranış |
|---|---|
| `Trialing` | Deneme süresi; bitince `Active` (ödeme varsa) veya `PastDue` |
| `PastDue` | 7 gün nazik hatırlatma; her şey çalışır |
| `ReadOnly` | **Online rezervasyon kapanır** (public link 403), panel takvimi görünür, salon manuel randevu ekleyebilir |
| `Cancelled` | 30 gün sonra arşiv, veri silinmez |

- Faturalama şube/personel sayısına göre (dahil + ek şube). Fatura numarası SQL `SEQUENCE` ile boşluksuz. Aylık/yıllık (`BillingCycle`).
- Temporal (geçmişi tutulan) tablolar: `Service`, `Package`, `BranchServicePrice`, `Plan`, `Subscription` 🟢.
- CRM: aktivasyon hunisi (kaydoldu → hizmet ekledi → ilk randevu), onay kuyrukları (`BrandPaymentAccount`, `BrandSmsSender`), denetim (`AuditLog`).

---

## 13. Değerlendirme ve takvim

- Randevu `Completed` olunca otomatik değerlendirme daveti gider; **aynı randevu ikinci kez değerlendirilemez**. Müşteri yalnızca tamamlanan randevuyu `POST /me/reviews` ile değerlendirir (rating 1 baytlık sayı; `staffId`, `comment` boş olabilir).
- Salon yoruma yanıt verebilir.
- Google Takvim: **tek yönlü** (OMVILA → Google, salt okunur olay). OAuth token şifreli saklanır, yenileme job'ı; yenilenemezse `TokenExpired`. İptal/erteleme Google'a yansır. İki yönlü senkron v2.

---

## 14. Panel ve müşteri API yetenek sınırları

- `/me` (12 işlem): randevu listesi (`scope`, varsayılan `Upcoming`) ve iptal, paketler, ödemeler (salt okunur), profil okuma/güncelleme (`fullName`, `email`, `languageCode`), markalar, yorum gönderme, OTP/refresh/logout, ping.
- `/me` müşterinin randevu **oluşturmasını** kapsamaz (o public akışında).
- `/admin`: platform ekibi; `/manage`: salon paneli (73 işlem); `/public`: 4 işlem.

---

## 15. Bilinçli olarak yapılmayanlar (v1 kapsam dışı)

Ürün satışı/stok, tam POS ve Z raporu, diyetisyen ölçüm/program dosyası, müşteri fotoğrafı, WhatsApp (v1.1), iki yönlü takvim, pazaryeri keşif ekranı, sadakat puanı/hediye çeki, e-fatura, çoklu dil arayüz (altyapı hazır, çeviri yok), müşteri güven skoru.

---

## 16. Açık kalan konular (🔴)

1. Online rezervasyonda çok hizmetli randevuda personel hizmet başına mı seçiliyor, tek personel mi? Public rezervasyon **oluşturma** ucu Swagger'da yok.
2. Kapora bekleyen randevunun durumu ve ödeme gelince `Confirmed` geçişi.
3. Salonun müşteri adına iptal/erteleme yaparken cutoff kuralına tabi olup olmadığı.
4. KVKK veri sorumluluğu ve İYS kapsamı (hukuki görüş).
5. Kısmi kullanılmış paketin iade hesabı.
6. Veri saklama süreleri (`AuditLog`, temporal geçmiş, `NotificationOutbox` arşivi).
7. Abonelik fiyat kademeleri.

> Bu doküman tasarım dokümanı v1.0 + gerçek şema snapshot'ına dayanır. 🟡 ve 🔴 işaretli maddeler için son söz kodundadır.
