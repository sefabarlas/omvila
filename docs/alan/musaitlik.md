# Müsaitlik hesabı ve çakışan randevu engeli

Bu belge projenin en riskli parçasını tanımlar. Buradaki her kural için test vardır (bkz. sondaki test listesi). Kod `Scheduling.Domain/Availability/` altında **saf fonksiyonlar** olarak yazılır: veritabanı, saat (`IClock`) ve HTTP bilgisi içermez. Girdi bir "müsaitlik anlık görüntüsüdür" (snapshot), çıktı aday saatlerdir.

## Kavramlar

- **Aralık:** `[Start, End)` yarı açık `Instant` aralığı. 14:00'te biten ile 14:00'te başlayan çakışmaz; bir dakika örtüşen çakışır. `BETWEEN` kullanılmaz.
- **Aralık cebri:** `Intersect`, `Subtract`, `Union/Merge`, `Split` işlemleri `IntervalSet` tipinde. Bu tipin kendi özellik tabanlı testleri vardır (FsCheck veya CsCheck).
- **Yerel → anlık dönüşüm:** Çalışma saatleri yerel (`LocalTime`). Bir gün için şubenin `TimeZoneId`'siyle `Instant`'a çevrilir (NodaTime `ZonedDateTime`, `Resolvers.LenientResolver`). Türkiye'de yaz saati olmasa da kod saat dilimi kurallarına güvenir, sabit `+03:00` yazmaz.

## Girdi: bir kalemin ihtiyacı

Her kalem için önce **plan** çıkarılır:

```
ItemPlan
  ServiceId, StaffId (veya aday personel listesi)
  Segments: [ (offset, duration, staffBusy, kind) ... ]   // BufferBefore + aşamalar + BufferAfter
  ResourceNeeds: [ (resourceTypeId, quantity, segmentRange) ]
  TotalDuration
```

Süre ve fiyat çözümü alan modelindeki kurallarla yapılır (personel ezmesi > şube ezmesi > hizmet). Aşamalı hizmette personele özel süre farkı son personel-meşgul aşamaya uygulanır.

## Algoritma: bir personel, bir gün

1. **Şube açık aralıkları:** `BranchWorkingHours` (o gün, `EffectiveFrom/To` geçerli olanlar). `WorkingHoursOverride(BranchId, Date)` varsa onu kullan (`IsClosed` → boş küme). `PublicHoliday` tek başına kapatmaz; şube override üretmişse etkilidir.
2. **Personel çalışma aralıkları:** `StaffWorkingHours(StaffBranch, gün)` ∩ adım 1. Personel override'ı varsa onu kullan.
3. **İzinleri çıkar:** personelin **tüm şubelerdeki** `TimeOff` kayıtları.
4. **Meşguliyeti çıkar:** personelin **tüm şubelerdeki** aktif ve `StaffBusy = true` segmentleri. Aktif bekleme teklifinin tuttuğu saatler de buraya girer (teklif sahibi dışındakiler için).
5. **Geçiş tamponu:** personelin **başka şubedeki** her meşguliyetinin önüne ve arkasına `BranchTransferTime(from, to)` kadar ek meşguliyet koy.
6. **Kaynaklar:** kalemin istediği kaynak türleri için şubedeki kaynakların müsaitliği (kapasite, `ResourceDowntime`, aktif `AppointmentItemResource`). Kaynak ihtiyacı segmente bağlıysa yalnız o segmentin zamanına uygulanır.
7. **Pencere:** `şimdi + MinNoticeMinutes` öncesini ve `bugün + MaxAdvanceDays` sonrasını kes. Hizmet değeri yoksa marka değeri.
8. **Izgara:** kalan aralıklarda, `SlotIntervalMinutes` ızgarasına hizalı her başlangıç `t` için planın **her segmentini** kontrol et: `StaffBusy` segmentler personelin boş kümesine, kaynak ihtiyaçları kaynağın boş kümesine sığmalı. `StaffBusy = false` segmentler (bekleme) personel için kısıt koymaz ama şube açık aralığı içinde kalmalıdır.

Izgara şube saatinin başlangıcından değil, yerel gece yarısından itibaren hizalanır: 15 dk ızgarada 10:00, 10:15… Şube 10:10'da açılsa da ilk aday 10:15'tir.

## Çok kalemli randevu

- Kalemler **ardışık** planlanır: kalem *n+1*'in en erken başlangıcı, kalem *n*'in son personel-meşgul segmentinin bitişidir. Kalemler farklı personelde olabilir ("İki hizmet de aynı kişiden" kapalıysa).
- "İki hizmet de aynı kişiden" açıksa tek personel bütün kalemleri yapabilmelidir. Bu personel her hizmetin `ServiceStaff` listesinde olmalıdır.
- Arayüzdeki tek "saat" ilk kalemin başlangıcıdır. Bir başlangıç ancak bütün kalemler sığıyorsa adaydır.
- v1 sınırı: aynı randevuda en fazla 5 kalem. Kalemler arası boşluk en fazla 0 dk (kesintisiz zincir). Farklı personelle paralel kalem (iki uzman aynı anda) v2'dedir.

## "Fark etmez"

- Hizmeti veren, o şubede çalışan ve `IsBookableOnline` olan her personel için hesap tekrarlanır. Sonuç, her başlangıç için **bir** personel atamasıyla birleşir.
- Aynı başlangıcı birden çok personel verebiliyorsa seçim kuralı sırasıyla: (1) o gün en az doluluk oranı, (2) müşterinin `PreferredStaffId`'si, (3) `Staff.Id` (deterministik).
- Atanan personel müşteriye özet ekranında gösterilir.

## Gün listesi (takvim şeridi)

`GET …/availability/days?from&to` her gün için yalnız `hasAvailability` ve isteğe bağlı `firstSlot` döner. Gün şeridindeki noktalar ve "Perşembe dolu · en yakın uygun saat Cuma 10:30" buradan gelir. Hesap aynı motordur. Performans için gün başına ilk aday bulununca durur.

## Performans hedefi

- 12 personelli şubede, tek hizmet, 7 günlük gün listesi: **p95 < 300 ms**. Tek günün saat ızgarası (fark etmez): **p95 < 200 ms**.
- Veri tek sorgu turunda yüklenir. Hedef gün aralığı için şube saatleri, personel saatleri, override'lar, izinler, segmentler ve kaynaklar ayrı sorgularla toplu çekilir; döngü içinde sorgu yasaktır (N+1 testi var).
- v1'de önbellek yok. Gerekirse şube + gün anahtarlı, randevu değişikliğinde geçersizlenen kısa önbellek eklenir (ADR gerekir).

## Çakışma engeli: üç katman

Müsaitlik hesabı yalnız **öneridir**. Asıl garanti yazma anında verilir.

### Katman 1: Exclusion constraint (son garanti)

```sql
CREATE EXTENSION IF NOT EXISTS btree_gist;

ALTER TABLE scheduling.appointment_item_segments
  ADD CONSTRAINT ex_segment_staff_overlap
  EXCLUDE USING gist (staff_id WITH =, during WITH &&)
  WHERE (staff_busy AND is_active AND staff_id IS NOT NULL);

ALTER TABLE scheduling.appointment_item_resources
  ADD CONSTRAINT ex_item_resource_overlap
  EXCLUDE USING gist (resource_id WITH =, during WITH &&)
  WHERE (is_active AND is_exclusive);
```

- Kısıt **şubeyle filtrelenmez**: aynı personel aynı anda iki şubede olamaz.
- İhlal PostgreSQL `23P01 exclusion_violation` üretir. Altyapı bunu `SlotTakenException` → `409 slot_taken` olarak çevirir.
- Randevu iptal veya gelmedi olunca segmentlerin `is_active` alanı aynı transaction'da `false` yapılır.

### Katman 2: Advisory lock (kısıtın ifade edemedikleri)

Kısıt şunları ifade edemez: şubeler arası geçiş tamponu, kapasitesi birden fazla olan kaynak, çalışma saati ve izin. Bunlar için yazma transaction'ında:

1. İlgili her personel için `pg_advisory_xact_lock(hashtext('staff:' || staff_id || ':' || local_date))`, sonra her kaynak için `'resource:' || resource_id || ':' || local_date`.
2. Kilitler **her zaman sıralı** alınır: önce personeller, sonra kaynaklar, her grup kendi içinde `Id` sırasıyla. Böylece deadlock önlenir.
3. Kilit altında: aynı personelin süresi dolmuş tutmalarını iptal et → müsaitliği **yeniden hesapla** (snapshot'ı kilitten sonra oku) → uygunsa randevu + kalem + segment + kaynak + outbox satırlarını yaz → commit.
4. Kilit zaman aşımı `lock_timeout = 5s`. Aşılırsa `503 try_again`.

### Katman 3: İdempotency
Oluşturma uçları `Idempotency-Key` başlığı ister. Aynı anahtarla gelen tekrar istek aynı yanıtı döner, ikinci randevu üretmez (`infra.idempotency_keys`, 24 saat).

## Saat değişikliği
Aynı transaction'da eski segmentler `is_active = false`, yeni segmentler `is_active = true`. Kısıt yalnız aktifleri kapsadığı için randevu kendi eski saatiyle çakışmaz. Kilitler eski ve yeni günlerin ikisi için alınır.

## Panelden elle ekleme
Salon personeli çalışma saati dışına randevu ekleyebilir ("Elif için boş saatler" önerir ama zorlamaz). İzin ve çalışma saati ihlali panelde **uyarı**dır, ikinci onayla geçilir (`allowOutsideWorkingHours: true`). Çakışma ise **asla** geçilemez: kısıt panel için de geçerlidir.

## Zorunlu testler

Alan testleri (`Scheduling.Domain.Tests/Availability`):

| # | Senaryo | Beklenen |
|---|---|---|
| M1 | 14:00'te biten randevu, 14:00 başlangıç | Aday |
| M2 | 14:00–14:45 dolu, 13:30'dan 45 dk hizmet | 13:30 aday değil (14:15'e taşar) |
| M3 | Boya: 45 uygulama + 40 bekleme + 35 fön | Bekleme diliminde aynı personele 30 dk'lık başka hizmet sığar |
| M4 | Personel Kadıköy'de 12:00'ye kadar dolu, Nişantaşı için 30 dk geçiş | Nişantaşı'nda ilk aday 12:30 |
| M5 | Personel izinli (tüm gün) | Hiç aday yok, şube açık olsa bile |
| M6 | Şube override `IsClosed` | Hiç aday yok |
| M7 | "Yalnızca bu gün için aç" override (pazar) | Override saatleri içinde adaylar |
| M8 | Kaynak kapasitesi 1, aynı saatte başka kalem kullanıyor | Aday değil |
| M9 | Kaynak downtime | Aday değil |
| M10 | `MinNoticeMinutes = 120`, şimdi 10:05 | İlk aday 12:15 (15 dk ızgara) |
| M11 | `MaxAdvanceDays = 60` | 61. gün boş |
| M12 | İki kalem, farklı personel | Kalem 2, kalem 1'in personel-meşgul bitişinde başlar |
| M13 | İki kalem, "aynı kişiden" | Yalnız iki hizmeti de veren personel |
| M14 | Fark etmez, iki personel aynı saatte boş | Deterministik seçim kuralı |
| M15 | Personel özel süresi (75 dk) | Plan 75 dk |
| M16 | Şubeye özel süre ezmesi | Plan ezilmiş süre |
| M17 | Süresi dolmuş `PendingApproval` tutması | Yazma sırasında iptal edilir, saat alınır |
| M18 | Aktif bekleme teklifi | Saat teklif sahibine açık, başkasına kapalı |
| M19 | Gün ortasında öğle arası (iki çalışma aralığı) | Aradaki saat aday değil |
| M20 | Saat dilimi: `Europe/Istanbul` dışındaki şube (test için `Europe/Berlin`, yaz saati geçişi) | Yerel 10:00 doğru `Instant`'a döner |

Bütünleşik testler (`Scheduling.IntegrationTests`, Testcontainers + gerçek PostgreSQL):

| # | Senaryo | Beklenen |
|---|---|---|
| C1 | Aynı slota **paralel** 20 istek (`Task.WhenAll`, ayrı bağlantılar) | Tam 1 başarı, 19 × `409 slot_taken` |
| C2 | Aynı personel, iki şube, aynı saat, paralel | Tam 1 başarı |
| C3 | Tek kapasiteli kaynak, iki personel, paralel | Tam 1 başarı |
| C4 | İptal edilen randevunun saati | Hemen tekrar alınabilir |
| C5 | Saat değişikliği kendi eski saatiyle örtüşen yeni saat | Başarılı |
| C6 | Aynı `Idempotency-Key` ile iki istek | Tek randevu, aynı yanıt |
| C7 | Kilit sırası: A→B ve B→A personelli iki çok kalemli randevu, paralel | Deadlock yok, ikisi de sonuçlanır |

Sıralı (paralel olmayan) test çakışma engelini kanıtlamaz. C1–C3 her CI çalışmasında koşar.
