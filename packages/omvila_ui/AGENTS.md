# packages/omvila_ui/: Flutter ortak paketi

İki mobil uygulamanın ortak teması, widget'ları, biçimlendirme ve API katmanı.

## İçerik
| Yol | Ne |
|---|---|
| `lib/theme/omvila_theme.dart` | `packages/design-tokens`'dan **üretilir** (`pnpm --filter @omvila/design-tokens build`), elle düzenlenmez: `OmvilaRenkler`, `OmvilaGolgeler`, `OmvilaBosluk`, `OmvilaKose`, `OmvilaBoyut`, `OmvilaHareket`, `OmvilaYazi`, `OmvilaTema.acik()/koyu()`, `context.omvilaRenkler` |
| `fonts/` | Plus Jakarta Sans değişken TTF (Latin + Latin Ext; `design-tokens/fonts` woff2 dosyalarından dönüştürüldü, OFL). Tema aileyi `package: 'omvila_ui'` ile ister |
| `lib/widgets/` | Var: `OmvilaDurumRozeti` (+ `RandevuDurumu.fromDisplayStatus`), `OmvilaButon`, `OmvilaBosDurum`, `BentoBlok`, `RandevuKarti`, `OtpGirisi`, `YuzenSekmeCubugu`. Uygulama PR'larında eklenecek: `OmvilaRozet`, `RandevuSatiri`, `PaketKarti`, `GunSeridi`, `SaatIzgarasi`, `HizmetSecimi`, `PersonelSecici`, `RezervasyonAdimlari`, `SalonBasligi`, `PuanSecici`, `AltOzetCubugu`, `KopyalanabilirAlan`, `BilgiSeridi` |
| `lib/format.dart` | Para, tarih, süre, telefon, IBAN, sayı biçimleri; `packages/ui-web/src/format` ile aynı kurallar ve test örnekleri |
| `lib/time.dart` | `Clock` / `SystemClock` / `FixedClock` ("şimdi" yalnız buradan) ve şube saat dilimiyle dönüşüm (`timezone` paketi); `packages/ui-web/src/time` ile aynı işlevler |
| `lib/motion.dart` | Azaltılmış hareket (`reduceMotion`, `motionDuration`); süre ve eğri sabitleri temadaki `OmvilaHareket` |
| `lib/errors/tr.dart` | `ErrorCode` → Türkçe mesaj; metinler `packages/ui-web/src/errors/tr.ts` ile birebir (test denetler) |
| `lib/api/` | `createOmvilaDio` + `createOmvilaApi` (üretilmiş `omvila_api`), `AuthSession` (jeton deposu ve yenileme uygulamanın), `OmvilaInterceptor` (Authorization, POST'ta eksikse `Idempotency-Key`, `401 token_expired` sonrası tek yenileme, `503 try_again` sonrası bir tekrar), `newIdempotencyKey`, `ifMatch`, `ApiProblem`; `packages/api-client-ts/src/client.ts` ile aynı davranış |

## Kurallar
- Widget ölçüleri ve davranışı: `design/tasarim-sistemleri/mobil/components/<Bileşen>/README.md` (her README'de **Flutter:** satırı karşılığı söyler) ve `design/tasarim-sistemleri/mobil/README.md`.
- Renkler yalnız `Theme.of(context)` / `OmvilaRenkler` ile; `Color(0x…)` yasak.
- Her widget'ın widget testi ve golden testi (açık, koyu, 1.3× yazı; `test/helpers/golden.dart` → `omvilaGoldenTest`) vardır. Depoya yalnız alchemist CI goldenleri girer (`test/**/goldens/ci/`: metin blok çizilir, yazı tipi ve işletim sisteminden bağımsız). Gerçek yazı tipiyle bakmak için platform goldenleri yerelde üretilir (git dışı).
- `test/source_rules_test.dart`: `lib/` içinde sabit renk (`Color(0x…)`, `Colors.*`; `Colors.transparent` hariç) ve `DateTime.now()` (time.dart dışında) yakalanır.
- `format.dart` testleri ürün kurallarındaki biçim örneklerini birebir doğrular.
- `errors/tr.dart` sözleşmedeki her `ErrorCode` için girdi içerir; test eksik kodu yakalar.

## Komutlar
```bash
cd packages/omvila_ui
fvm flutter test
fvm flutter test --update-goldens   # yalnız bilinçli görsel değişiklikte
OMVILA_PLATFORM_GOLDENS=1 fvm flutter test --update-goldens   # gerçek yazı tipiyle bakmak için (goldens/macos/, git dışı)
```
