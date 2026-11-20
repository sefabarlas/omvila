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
- Henüz yok (T-34): `../omvila_ui/lib/theme/omvila_theme.dart` (Flutter işleri başlarken eklenir; referans dosyadaki API ile aynı) ve `prefers-reduced-motion` kuralları (kaynakta hareket tokenı yok).

## Kurallar
- Token adı ve değeri yalnız `src/*.json` içinde değişir; üretilen dosyalar elle düzenlenmez.
- Tasarım ekibi yeni token eklerse önce burada, sonra kullanım. Semantik token eklemek serbesttir. Primitif palete (`petrol-*`, `kum-*`, `mercan-*`) dokunmak marka kararıdır.
- Bileşenler yalnız **semantik** tokenları kullanır. `zemin-vurgu-yumusak` üzerindeki metin `metin-vurgu`dur.
- Logo kullanım kuralları: [design/tasarim-sistemleri/marka/README.md § Logo](../../design/tasarim-sistemleri/marka/README.md). 48 px altında `omvila-amblem-kucuk.svg`.
- Üretim aracı `scripts/build.ts` (bağımlılıksız Node; kaynak biçimi Style Dictionary/DTCG değil, dönüştürmek yerine doğrudan okunur). Testler `dist/`'in kaynakla güncel olduğunu denetler: JSON değişince `build` çalıştırılıp `dist/` commit edilir. Dart çıktısı eklendiğinde referans dosyayla aynı sınıf ve alan adlarını korur.

## Komutlar
```bash
pnpm --filter @omvila/design-tokens build   # dist/ üretir
pnpm --filter @omvila/design-tokens test    # kontrast (her metin/zemin çifti iki temada ≥ 4.5:1), başvurular, dist/ güncelliği
```
