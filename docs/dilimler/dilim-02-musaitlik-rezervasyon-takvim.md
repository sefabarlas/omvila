# Dilim 2: Müsaitlik + rezervasyon sayfası + panel takvimi

## Amaç
Müşteri, salonun bağlantısından üyeliksiz randevu alır (onaysız salon akışı). Salon, randevuları takvimde görür, panelden randevu ekler, sürükleyerek taşır, iptal eder, gelen müşteriyi işaretler. Çakışan randevu hiçbir koşulda yazılamaz. Projenin en riskli parçası bu dilimdedir.

## Kapsam
**İçinde**
- Müsaitlik motoru (`docs/alan/musaitlik.md`): çok kalemli, aşamalı hizmet, fark etmez, şubeler arası geçiş, kaynak (oda/cihaz), min/max pencere, ızgara.
- Çakışma engeli: exclusion constraint + advisory lock + idempotency (C1–C7 testleri).
- Rezervasyon sayfası: Şube → Hizmet → Personel → Saat (dolu gün dahil) → Telefon → Kod → Onay → Başarı; masaüstü iki kolon; güvenilir cihaz.
- SMS bağlantısından randevu yönetimi: görüntüle, saat değiştir (son süre), iptal (neden zorunlu, son süre).
- Panel takvimi: gün (personel kolonları) ve hafta görünümü, bekleme dilimi şeridi, şu anki saat, kapalı gün, randevu yan paneli, yeni randevu (müşteri arama/ekleme, boş saat önerisi, tekrar), sürükle-bırak, iptal (tekrarlayan: yalnız bu / bu ve sonrakiler), Geldi, gelmedi otomatiği ve "Geldiyse geri al".
- Müşteriler: asgari liste, arama (ad / son 4 hane), ekleme (aynı numara uyarısı), kart (randevular sekmesi).
- Bildirim outbox'ı ve sahte gönderim: `appointment.confirmed`, `appointment.rescheduled`, `appointment.cancelled_by_salon`, `appointment.reminder` (yalnız outbox'a yazılır; SMS gönderimi Dilim 3).
- Sistem işleri: `MarkNoShows`, `ExtendRecurringSeries`.

**Dışında:** onay gerektiren salon (D3; ayar `RequiresApproval=false` varsayılır), kapora (D4), bekleme listesi (D5; dolu gün ekranında "Bekleme listesine katılın" butonu bu dilimde gizli), paket kullanımı (D5; `my-packages` boş liste döner), tamamla ve ödeme al (D5).

## Ekranlar
- `rezervasyon-sayfasi/`: Sube, Main, Personel, Saat, DoluGun, Telefon, Kod, Onay, Basari, Randevum, Ertele, Iptal, Masaustu, Durumlar (saat az önce doldu, SMS kodu sınırı, online randevu kapalı, bağlantı geçersiz, bağlantı yok).
- `salon-paneli/`: Main (takvim gün), HaftaTakvim, TakvimBos, TakvimKapali, YeniRandevu, RandevuOnayli (yan panel; onay butonları D3), TakvimSurukle, TekrarIptal, Musteriler, MusterilerBos, MusterilerAramaYok, MusteriEkle, MusteriKarti (randevular sekmesi), TabletTakvim, TabletDikeyTakvim, Durumlar (kayıt çakışması, saat doldu).

## Uçlar (sözleşmede hazır)
`manageGetCalendar`, `manageSearchSlots`, `manageCreateAppointment`, `manageGetAppointment`, `manageRescheduleAppointment`, `manageCancelAppointment`, `manageCheckInAppointment`, `manageUndoNoShow`, `manageListCustomers`, `manageCreateCustomer`, `manageGetCustomer`, `public-booking/*`, `public-appointment-link/*`.

## Alan kuralları (özet; ayrıntı belgelerde)
- Durum geçişleri A1, A6, A7, A8, A9, A10, A14 (`durum-makinesi.md`).
- Online oluşturma: `RequiresApproval=false` ve kapora yok → `Confirmed`. `BrandCustomer` yoksa `Source=Online` ile oluşur; `Customer` yoksa oluşur (`IsVerified=true`). Aydınlatma metni okundu ve kampanya izni `ConsentRecord` olarak yazılır.
- Fiyat ve süre oluşturma anında kaleme kopyalanır (şube fiyatı + personel ek ücreti).
- Erişim kodu (`/r/`) her online ve bildirimli panel randevusunda üretilir.
- Müşteri iptali: `CancellationCutoffHours`; erteleme: `RescheduleCutoffHours`. Salon sınıra tabi değil (S-04).
- Engellenmiş müşteri (`IsBlocked`) online oluşturamaz → `403 customer_blocked`.
- Tekrarlayan randevu: haftalık 1/2/4; ilk 3 ay üretilir; çakışan tarihler `skippedOccurrences`.
- Gelmedi: `NoShowAfterMinutes` (15) sonrası otomatik; aynı yerel gün geri alınabilir.
- ReadOnly abonelik bu dilimde test için elle ayarlanabilir: public uçlar `403 booking_disabled`.

## Kabul kriterleri
1. **Online randevu (mutlu yol):** `kuafor-elif` → Kadıköy → Saç kesimi + Fön → Elif → Sal 25 Kas 14:30 → telefon → kod → Onay → `Confirmed`; yanıtta `manageUrl`; panel takviminde Elif kolonunda 14:30–15:45 "Onaylandı" bloğu; outbox'ta `appointment.confirmed` (SMS planı) ve 24 sa / 2 sa hatırlatmaları.
2. **Fiyat:** Elif'in Balyaj ek ücreti (+300 ₺) ve Nişantaşı şube fiyatı yanıttaki kalem fiyatlarına yansır. Sonradan hizmet fiyatı değişince var olan randevunun tutarı değişmez.
3. **Aşamalı hizmet:** Elif'e 10:30 Boya (45+40+35) yazılır; aynı gün 11:15–11:45 (bekleme dilimi) için Elif'e 30 dk'lık Fön yazılabilir; 10:45 Fön yazılamaz (`slot_taken`). Takvimde Boya bloğunda taralı "Bekleme · müsait" şeridi görünür.
4. **Paralel istek:** Aynı slota 20 paralel online istek → tam 1 `201`, 19 `409 slot_taken`; her birinde `meta.alternatives`.
5. **İki şube:** Mert Kadıköy'de 12:00'ye kadar dolu; Nişantaşı'nda ilk aday 12:30 (30 dk geçiş).
6. **Fark etmez:** Saç kesimi, fark etmez, 14:00 için iki personel boş → deterministik kurala göre biri atanır ve onay ekranında adı görünür.
7. **Dolu gün:** Perşembe Elif dolu → gün şeridinde Perşembe noktasız; seçilince "Perşembe dolu · en yakın uygun saat Cuma 10:30".
8. **Pencere:** Saat 10:05'te `MinNoticeMinutes=120` → bugünün ilk adayı 12:15; 61. gün seçilemez.
9. **Güvenilir cihaz:** Aynı tarayıcıda ikinci randevuda telefon girilince kod adımı atlanır (`AlreadyVerified`), "Tekrar hoş geldiniz, Ayşe" görünür.
10. **SMS bağlantısı:** `/r/{code}` randevuyu gösterir; 24 saatten fazla varken iptal (neden "Planım değişti") → `Cancelled`, salon takviminde blok kalkar ve saat yeniden alınabilir. 12 saatten az kala "Saati değiştir" → `cutoff_passed` ve "Salonu ara" deseni. Geçersiz kod → "Bu randevu bulunamadı".
11. **Panel yeni randevu:** Telefonla gelen "Derya Şahin · Saç kesimi · Elif · Çar 14:30", "Müşteriye SMS gönder" açık → `Confirmed`, `Channel=Phone`; outbox'ta onay mesajı. Çalışma saati dışı 20:00 → önce `outside_working_hours` uyarısı, onayla eklenir.
12. **Sürükle-bırak:** Derya'nın randevusu Zeynep'in 14:00 kolonuna sürüklenir → "Zeynep Aksoy'a taşı · 14:00–14:45 · müşteriye SMS gider" ipucu → bırakınca kaydedilir. Dolu bir yere bırakılırsa blok geri döner, "Saat dolu" uyarısı.
13. **Tekrarlayan:** "Her 4 haftada bir" randevu oluşur, 3 aylık tekrarlar görünür; bir tekrar "Yalnızca bu randevu" iptal edilince diğerleri durur; "Bu ve sonraki randevular" iptal edilince sonrakiler iptal, öncekiler durur.
14. **Gelmedi:** 12:30 randevusu 12:45'te check-in yoksa (saat ileri alınarak) `NoShow` olur, sayaç artar; panelde "Geldiyse geri al" → `CheckedIn`, sayaç azalır.
15. **Müşteri ekleme:** Kayıtlı numarayla müşteri ekleme → `phone_already_exists` + "Mevcut kartı aç". Arama "4418" son 4 haneyle bulur. Personel rolünde liste maskeli telefon döner.
16. **Kapalı gün:** Pazar kapalı; takvimde "Kapalı gün" boş hâli; rezervasyon sayfasında Pazar seçilemez. "Yalnızca bu gün için aç" override'ı eklenince o Pazar açılır.
17. **Salt okunur:** Abonelik `ReadOnly` → rezervasyon sayfası "Online randevu kapalı · Salonu ara"; panelde yeni randevu butonu pasif, kırmızı şerit.
18. **Saat dilimi:** Test şubesi `Europe/Berlin` ile kurulduğunda (yaz saati geçiş günü) panel ve rezervasyon saatleri yerel olarak doğru gösterilir (M20).
19. **Performans:** 12 personelli şubede 7 günlük gün listesi p95 < 300 ms, tek gün saat ızgarası p95 < 200 ms (entegrasyon performans testi, seed verisiyle).
20. **Tasarım:** Listelenen ekranlar açık/koyu, masaüstü/mobil, tablet kırılımlarında tasarımla karşılaştırılır; rezervasyon sayfası Lighthouse bütçesini geçer. Kayıt: [dilim-02-tasarim-karsilastirma.md](dilim-02-tasarim-karsilastirma.md).

## Testler
- Alan: `musaitlik.md` M1–M20; durum geçişleri A1, A6–A10, A14; fiyat/süre çözümü; tekrar üretimi.
- Eşzamanlılık: C1–C7.
- Entegrasyon: kabul kriterleri 1–18.
- Uçtan uca: kritik akış 2, 6, 7 (`test-stratejisi.md`).
