# Kodlama standartları: Flutter (mobile/, packages/omvila_ui, packages/api-client-dart)

## Genel
- Flutter 3.27+ (FVM ile sabit; `.fvmrc`). Dart `strict-casts`, `strict-inference`, `strict-raw-types` açık. Lint: `very_good_analysis`; uyarı = hata.
- Biçim `dart format`. CI'da `dart format --set-exit-if-changed` ve `flutter analyze --fatal-infos`.
- Klasör yapısı özellik bazlı:
  ```
  lib/
    app/            # MaterialApp, router, tema bağlama, bağımlılıklar
    features/
      appointments/
        data/       # API çağrıları (api-client-dart üzerinden), repository
        domain/     # uygulama içi modeller (gerekirse)
        presentation/  # ekranlar, widget'lar, controller'lar (Riverpod)
    shared/
  ```
- Dosya adı `snake_case.dart`, sınıf `PascalCase`.

## Durum ve yönlendirme
- Riverpod (kod üretimli `@riverpod`), `AsyncValue` ile yükleme/hata/veri. `setState` yalnız saf görsel yerel durumda.
- go_router; derin bağlantılar: müşteri uygulamasında `omvila.com/r/{code}` ve `omvila.com/{brandSlug}` (App Links / Universal Links). İşletme uygulamasında bildirimden randevu detayı.
- Arka plana geçip dönünce kritik veriler yenilenir (Bugün, randevu detayı).

## API
- Yalnız `packages/api-client-dart` (sözleşmeden üretilmiş). Üretilen kod düzenlenmez.
- Jeton yenileme Dio interceptor'ında. Yenileme jetonu `flutter_secure_storage`'da.
- Hata eşleme `omvila_ui/lib/errors/tr.dart` (`code` → Türkçe mesaj). Bilinmeyen enum değerleri `unknownDefaultOpenApi` olarak gelir ve arayüz bunu genel durum olarak gösterir; çökmez.
- `Idempotency-Key` ve `If-Match` yardımcıları API katmanında.
- Çevrimdışı: v1'de yerel yazma kuyruğu yok. Bağlantı yoksa "Bağlantı yok" deseni gösterilir, işlem gönderilmez. Okumalar son başarılı yanıttan önbellekten gösterilebilir (salt okunur, "son güncelleme" etiketiyle).

## Tema ve bileşenler
- Tema `packages/omvila_ui/lib/theme/omvila_theme.dart` (tokenlardan üretilir, elle düzenlenmez). `MaterialApp(theme: OmvilaTema.acik(), darkTheme: OmvilaTema.koyu(), themeMode: ThemeMode.system, locale: Locale('tr','TR'))`.
- Renk yalnız `Theme.of(context).colorScheme` veya `context.omvilaRenkler` (ThemeExtension) ile. `Color(0xFF…)`, `Colors.red` yasak 🔒 (özel lint kuralı `custom_lint`).
- Ortak widget'lar `omvila_ui`'da: `OmvilaDurumRozeti`, `OmvilaBosDurum`, `RandevuSatiri`, `RandevuKarti`, `PaketKarti`, `GunSeridi`, `SaatIzgarasi`, `HizmetSecimi`, `PersonelSecici`, `RezervasyonAdimlari`, `SalonBasligi`, `PuanSecici`, `AltOzetCubugu`, `YuzenSekmeCubugu`. Uygulama içinde aynı işi gören ikinci widget yazılmaz.
- İkon `lucide_icons`; logo `flutter_svg` ile `packages/design-tokens/assets/` SVG'leri.
- Dokunma alanı en az 44×44 (`MaterialTapTargetSize.padded`); sistem yazı ölçeği 1.3× ile metin kesilmez, kırılır (golden testte doğrulanır).
- Platform uyumu: `Switch.adaptive`, `showAdaptiveDialog`; kök ekranlarda iOS büyük başlık.
- Hareket: `Duration(milliseconds: 80/150/240/360)`, eğriler `Cubic(.2,0,0,1)` vb. `omvila_ui/motion.dart`'tan; `MediaQuery.disableAnimations` açıkken yalnız opaklık.

## Metin, tarih, para
- Biçimlendirme yalnız `omvila_ui/format.dart`: `formatMoney`, `formatDate`, `formatTime`, `formatDuration`, `formatPhone`. `intl` doğrudan ekranda çağrılmaz.
- Zaman: API'den gelen `DateTime` UTC'dir; gösterim `omvila_ui/time.dart` ile şubenin `timeZone`'una çevrilir (`timezone` paketi). Cihaz saat dilimi kullanılmaz.
- SMS kodu otomatik doldurma: iOS `AutofillHints.oneTimeCode`, Android SMS Retriever (`sms_autofill`).

## Bildirimler
- İzin önce kendi ekranımızla istenir; sistem istemi yalnız "Bildirimlere izin ver"e basılınca açılır. "Şimdi değil" kapatılabilir şerit bırakır.
- FCM jetonu girişten sonra `POST /me/devices` veya `/manage/devices` ile kaydedilir, değişince güncellenir, çıkışta silinir.

## Yasaklar
- Uygulama içi ödeme, kart formu (ürün kuralı 1). Müşteri uygulamasında salon arama/keşif sekmesi (ürün kuralı 2).
- Sağlık bilgisi isteyen alan.
- Boş durumda sahte sayı veya sekme rozeti.
