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
- `dist/tokens.css`: `:root` (açık) + `[data-theme="dark"]` (koyu) + `prefers-reduced-motion` kuralları; panel ve mobil için ayrı dosyalar: `dist/panel.css`, `dist/mobil.css`
- `dist/tokens.ts`: tip güvenli token adları (CSS değişkeni adlarını string literal olarak)
- `../omvila_ui/lib/theme/omvila_theme.dart`: `OmvilaRenkler`, `OmvilaYazi`, `OmvilaBosluk`, `OmvilaKose`, `OmvilaBoyut`, `OmvilaTema` (referans dosyadaki API ile aynı)

## Kurallar
- Token adı ve değeri yalnız `src/*.json` içinde değişir; üretilen dosyalar elle düzenlenmez.
- Tasarım ekibi yeni token eklerse önce burada, sonra kullanım. Semantik token eklemek serbesttir. Primitif palete (`petrol-*`, `kum-*`, `mercan-*`) dokunmak marka kararıdır.
- Bileşenler yalnız **semantik** tokenları kullanır. `zemin-vurgu-yumusak` üzerindeki metin `metin-vurgu`dur.
- Logo kullanım kuralları: [design/tasarim-sistemleri/marka/README.md § Logo](../../design/tasarim-sistemleri/marka/README.md). 48 px altında `omvila-amblem-kucuk.svg`.
- Üretim aracı Style Dictionary (`build.mjs`). Dart çıktısı referans dosyayla aynı sınıf ve alan adlarını korur; ilk üretimde fark raporu çıkarılır ve referansla hizalanır.

## Komutlar
```bash
pnpm --filter @omvila/design-tokens build   # dist/ ve omvila_theme.dart üretir
pnpm --filter @omvila/design-tokens test    # kontrast testleri: her metin/zemin çifti iki temada ≥ 4.5:1
```
