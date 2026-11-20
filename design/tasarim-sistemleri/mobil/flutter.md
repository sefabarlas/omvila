# Flutter

Bu sistemin tokenları `flutter/omvila_theme.dart.txt` dosyasında hazırdır (sayfa `.dart` uzantısı sunamadığı için `.txt` ile saklanır; projeye `omvila_theme.dart` adıyla koyun). Dosya `tokens.json`'dan üretilir, elle düzenlenmez. Bir token değiştiğinde dosya yeniden üretilir. Sayfadaki **Files** bölümünden indirilebilir.

## Kurulum

1. `flutter/omvila_theme.dart.txt` dosyasını `lib/theme/omvila_theme.dart` olarak kaydedin.
2. Plus Jakarta Sans'ın değişken TTF dosyasını (Google Fonts, OFL lisanslı, Türkçe karakterler dahil) `assets/fonts/` altına koyup `pubspec.yaml`'a ekleyin:

```yaml
flutter:
  fonts:
    - family: PlusJakartaSans
      fonts:
        - asset: assets/fonts/PlusJakartaSans[wght].ttf
```

3. İkonlar için `lucide_icons`, logolar için `flutter_svg` paketlerini ekleyin. Logo SVG'leri Marka Sistemi'nin Logolar grubundadır.
4. Temayı bağlayın:

```dart
MaterialApp(
  theme: OmvilaTema.acik(),
  darkTheme: OmvilaTema.koyu(),
  themeMode: ThemeMode.system,
  locale: const Locale('tr', 'TR'),
);
```

Dosya Flutter 3.27 veya üstü gerektirir (`CardThemeData`, `DialogThemeData`, `WidgetStateProperty`).

## Dosyada ne var

| Sınıf | İçerik | Kullanım |
|---|---|---|
| `OmvilaRenkler` | Bütün semantik renkler, açık ve koyu (`ThemeExtension`) | `final r = Theme.of(context).extension<OmvilaRenkler>()!; r.durumKaporaZemin` |
| `OmvilaRampa` | Renk rampaları (temadan bağımsız) | Yalnızca yeni bir semantik renk tanımlarken |
| `OmvilaYazi` | 14 yazı stili | `Text('Bugün', style: OmvilaYazi.buyukBaslik)` |
| `OmvilaBosluk` | 4 tabanlı boşluk ölçeği | `EdgeInsets.all(OmvilaBosluk.bosluk4)` |
| `OmvilaKose` | Köşe yarıçapları | `BorderRadius.circular(OmvilaKose.koseLg)` |
| `OmvilaBoyut` | Dokunma alanı, kontrol, üst bar, sekme çubuğu, saat dilimi | `SizedBox(height: OmvilaBoyut.kontrolLg)` |
| `OmvilaTema` | `ThemeData`: ColorScheme, buton, giriş alanı, kart, NavigationBar, alt sayfa, diyalog, çip, snackbar, switch | `MaterialApp(theme: OmvilaTema.acik())` |

## ColorScheme eşlemesi

| Material | OMVILA tokenı |
|---|---|
| `primary` / `onPrimary` | `marka` / `marka-ustu` |
| `primaryContainer` / `onPrimaryContainer` | `zemin-secili` / `metin-marka` |
| `secondary` / `onSecondary` | `vurgu` / `vurgu-ustu` |
| `error` | `hata` |
| `surface` / `onSurface` | `zemin-yukseltilmis` / `metin` |
| `onSurfaceVariant` | `metin-ikincil` |
| `surfaceContainerHighest` | `zemin-ikincil` |
| `outline` / `outlineVariant` | `cizgi-guclu` / `cizgi` |
| `scaffoldBackgroundColor` | `zemin` |

Randevu durumları, sekme çubuğu, iskelet ve yıldız renkleri Material şemasında karşılığı olmadığı için yalnızca `OmvilaRenkler` üzerinden okunur.

## Yazılması önerilen ortak widget'lar

Material'da doğrudan karşılığı olmayan bileşenler için ortak bir `omvila_ui` paketi açın ve aşağıdaki widget'ları orada toplayın. Her birinin ölçüsü ve davranışı bu sistemdeki önizlemede ve README'sinde tanımlıdır:

`OmvilaDurumRozeti`, `OmvilaRozet`, `OmvilaBosDurum`, `RandevuSatiri` (AppointmentListItem), `RandevuKarti` (AppointmentTicket), `PaketKarti`, `GunSeridi` (DayStrip), `SaatIzgarasi` (TimeSlotGrid), `HizmetSecimi` (ServiceItem), `PersonelSecici` (StaffPicker), `RezervasyonAdimlari` (BookingSteps), `SalonBasligi` (SalonHeader), `PuanSecici` (RatingInput), `AltOzetCubugu` (StickyFooter).

## Platform farkları

- `Switch.adaptive`, `AlertDialog.adaptive` ve `showAdaptiveDialog` kullanın. iOS'ta Cupertino görünümü otomatik gelir.
- Kök ekranlarda iOS'ta `CupertinoSliverNavigationBar` (büyük başlık), Android'de `SliverAppBar.large` kullanın.
- Geri hareketi iOS'ta kenardan kaydırmadır. Android'de sistem geri tuşu alt sayfayı ve diyaloğu önce kapatır.
- SMS kodunu otomatik doldurmak için iOS'ta `AutofillHints.oneTimeCode`, Android'de SMS Retriever API kullanın.
