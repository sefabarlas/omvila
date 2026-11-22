# mobile/isletme/: OMVILA İşletme ajan kuralları

Salon çalışanının uygulaması: "İşim cebimde." Gün içinde ayakta, hızlı, durum odaklı. Önce kök [AGENTS.md](../../AGENTS.md)'yi okuyun.

Zorunlu okuma: [kodlama-flutter.md](../../docs/standartlar/kodlama-flutter.md), [design/tasarim-sistemleri/mobil/README.md](../../design/tasarim-sistemleri/mobil/README.md) ve `flutter.md`, [roller-yetkiler.md](../../docs/alan/roller-yetkiler.md).
Ekranlar: `design/ekranlar/isletme-uygulamasi/` (Acilis, Giris, GirisKod, BildirimIzni → Main(Bugün), MainBos, MainBildirimKapali, MainSaltOkunur → Takvim, TakvimBos, TakvimKapali → RandevuDetay, RandevuIslem, KaporaOnay, OdemeAl, YeniRandevu → Sira, SiraEkle, SiraBos → Musteriler, MusterilerAramaYok, MusterilerBos, MusteriKarti, MusteriKartiYeni → Bildirimler, BildirimlerBos, Profil, BildirimAyar).

## Yapı ve yığın
Flutter 3.27+, Riverpod (elle sağlayıcı; kod üretimi neden yok: T-107), go_router, `omvila_ui` (tema + ortak widget'lar), `omvila_api` (`packages/api-client-dart`), `flutter_secure_storage`, `lucide_icons_flutter`, `flutter_svg`, `share_plus`, `url_launcher` ("Ara"/"SMS": tel:/sms:). `firebase_messaging` mağaza/Firebase hesapları açılınca (K4; o zamana kadar `PushService` + `FakePushService`), `sms_autofill` imzalı Android derlemesiyle eklenir.

Yapı: `lib/app/` (uygulama, router, sağlayıcılar: `apiProvider`, `clockProvider`, `keyValueStoreProvider`, `pushServiceProvider`), `lib/features/<özellik>/{data,presentation}`, `lib/shared/`. Testler `test/helpers/fake_api.dart` (`Harness`: sözleşme uçlarını taklit eden HTTP bağdaştırıcısı, bellekte depo, sahte push, sahte `ExternalLauncher`, sabit saat; takvim/randevu verisi `appointment_fixtures.dart`) ve `test/helpers/screen_golden.dart` (390×844, açık/koyu/1.3×; `test/goldens/ci/`). Uygulama kimliği `com.omvila.isletme`; ad "OMVILA İşletme"; ikon koyu petrol zemin (`packages/design-tokens/assets/app-icon-isletme.svg`).

## Özel kurallar
- API yüzeyi `/api/v1/manage` (salon paneliyle aynı uçlar ve izinler). Menü ve eylemler `session.permissions` ile.
- Sekmeler: Bugün, Takvim, Müşteriler, Bildirimler, Profil. Alt sekme çubuğu yüzen koyu hap (64 pt, kenarlardan 16, alttan 24); içerik altında 104 boşluk.
- **Bugün:** bento (sıradaki randevu `blok-marka`, onay bekleyen `blok-vurgu`, sıra `blok-kum`), gün akışı listesi, kaydırma eylemleri (Geldi, Ara) ve her birinin buton karşılığı.
- **Personel rolü:** yalnız kendi randevuları ve kendi primi; müşteri telefonu maskeli (arama butonu yine çalışır); onay butonları yok.
- **Bildirim izni:** kendi ekranımızla istenir. "Şimdi değil" → Bugün'de kapatılabilir şerit ("onay bekleyen talepler kaçabilir").
- **Kapora geldi mi?** alt sayfası: tutar, açıklama kodu, müşteri havale bildirdiyse saati, kalan süre; "Kapora geldi" birincil, "Henüz gelmedi, hatırlat" ikincil (Dilim 4).
- **Tamamla ve ödeme al:** yöntemler Nakit / Kart (salonun POS'u) / Havale / Paketten; "İkinci ödeme ekle"; uygulamada ödeme alınmaz, yalnız kaydedilir (Dilim 5).
- **Kapıda sıra:** numara her gün 1'den; "Başlat" randevuya çevirir.
- Salt okunur abonelikte kırmızı şerit; yeni randevu pasif.
- Sözleşmede `format: date` olan alanlara ve sorgulara `ApiDate` verilir (üretilmiş istemci `DateTime`'ı tam an olarak yazar; T-109).
- Push bildirimine dokununca ilgili ekran açılır (randevu detayı, kapora, değerlendirme).

## Komutlar
```bash
cd mobile/isletme
fvm flutter run --dart-define=API_URL=http://10.0.2.2:4010   # Prism mock (Android emülatör)
fvm flutter test
fvm flutter test --update-goldens   # yalnız bilinçli görsel değişiklikte
fvm flutter analyze --fatal-infos
```
