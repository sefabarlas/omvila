# packages/design-tokens/: tasarım tokenları ve marka varlıkları

Renk, yazı, boşluk, köşe, gölge, boyut ve hareket tokenlarının **tek kaynağı**. Web için CSS değişkenleri, Flutter için Dart teması buradan üretilir.

## İçerik
```
src/tokens.marka.json   # marka sistemi (primitif paletler + semantik adlar)
src/tokens.panel.json   # panel ölçeği (salon ve platform paneli)
src/tokens.mobil.json   # mobil ölçeği (uygulamalar ve rezervasyon sayfası)
reference/omvila_theme.dart.txt   # tasarım ekibinin elle hazırladığı ilk Flutter teması (API referansı)
assets/logos/*.svg      # logolar (omvila-logo-yatay.svg, -ters, amblem, amblem-kucuk, mono…)
assets/app/*.svg        # uygulama ikonları ve favicon
fonts/                  # Plus Jakarta Sans (Latin + Latin Ext; ikisi birlikte yüklenir)
```
Üretilenler (`dist/`, commit edilir):
- `dist/panel.css`, `dist/mobil.css`, `dist/marka.css`: ölçek başına `:root` (açık tema ve temasız tokenlar) + `[data-theme='dark']` (koyu temada değişen renk ve gölgeler). Yazı stilleri `--yazi-<stil>-boyut|satir|agirlik|harf`, aile `--yazi-aile-sans`.
- `dist/fonts.css`: `@font-face` tanımları (yollar `../fonts/`).
- `dist/tokens.ts`: ölçek başına tip güvenli token adları (`panelTokens.renk`… ve `PanelColorToken` tipi).
- `../omvila_ui/lib/theme/omvila_theme.dart` (`scripts/generate-dart.ts`, kaynak `tokens.mobil.json`): `OmvilaRenkler` (yalnız semantik renkler, açık/koyu `ThemeExtension`), `OmvilaGolgeler`, `OmvilaBosluk`, `OmvilaKose`, `OmvilaBoyut`, `OmvilaHareket` (süre ve eğri), `OmvilaYazi`, `OmvilaTema` (ThemeData) ve `context.omvilaRenkler`. Sınıf ve alan adları referans dosyayla aynı; referanstaki `OmvilaRampa` (ham palet) üretilmez, snackbar ve switch semantik tokenlarla boyanır (T-87). Hex yalnız bu dosyada geçer.
- Hareket tokenları (`sure-*`, `egri-*`) yalnız mobil ölçekte; `mobil.css`'e değişken olarak da yazılır. `prefers-reduced-motion` kuralı üretilmez (T-34).

## Kurallar
- Token adı ve değeri yalnız `src/*.json` içinde değişir; üretilen dosyalar elle düzenlenmez.
- Tasarım ekibi yeni token eklerse önce burada, sonra kullanım. Semantik token eklemek serbesttir. Primitif palete (`petrol-*`, `kum-*`, `mercan-*`) dokunmak marka kararıdır.
- Bileşenler yalnız **semantik** tokenları kullanır. `zemin-vurgu-yumusak` üzerindeki metin `metin-vurgu`dur.
- Logo kullanım kuralları: [design/tasarim-sistemleri/marka/README.md § Logo](../../design/tasarim-sistemleri/marka/README.md). 48 px altında `omvila-amblem-kucuk.svg`.
- Üretim aracı `scripts/build.ts` (bağımlılıksız Node; kaynak biçimi Style Dictionary/DTCG değil, dönüştürmek yerine doğrudan okunur). Testler `dist/`'in ve Dart temasının kaynakla güncel olduğunu denetler (CI `web` işinde "Token çıktıları güncel" adımı): JSON değişince `build` çalıştırılıp `dist/` ve `omvila_theme.dart` commit edilir. Dart çıktısı referans dosyayla aynı sınıf ve alan adlarını korur; `dart format` dışıdır (`// dart format off`) ve `flutter analyze --fatal-infos` temiz üretilir.

## Komutlar
```bash
pnpm --filter @omvila/design-tokens build   # dist/ ve omvila_ui/lib/theme/omvila_theme.dart üretir
pnpm --filter @omvila/design-tokens test    # kontrast (her metin/zemin çifti iki temada ≥ 4.5:1), başvurular, dist/ güncelliği
```
