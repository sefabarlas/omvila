# packages/omvila_ui/: Flutter ortak paketi

İki mobil uygulamanın ortak teması, widget'ları, biçimlendirme ve API katmanı.

## İçerik
| Yol | Ne |
|---|---|
| `lib/theme/omvila_theme.dart` | `packages/design-tokens`'dan **üretilir**, elle düzenlenmez |
| `lib/widgets/` | `OmvilaDurumRozeti`, `OmvilaRozet`, `OmvilaBosDurum`, `RandevuSatiri`, `RandevuKarti`, `PaketKarti`, `GunSeridi`, `SaatIzgarasi`, `HizmetSecimi`, `PersonelSecici`, `RezervasyonAdimlari`, `SalonBasligi`, `PuanSecici`, `AltOzetCubugu`, `YuzenSekmeCubugu`, `BentoBlok`, `OtpGirisi`, `KopyalanabilirAlan`, `BilgiSeridi` |
| `lib/format.dart` | Para, tarih, saat, süre, telefon biçimleri |
| `lib/time.dart` | Şube saat dilimiyle dönüşüm (`timezone` paketi) |
| `lib/motion.dart` | Süre ve eğri sabitleri; azaltılmış hareket |
| `lib/errors/tr.dart` | `ErrorCode` → Türkçe mesaj |
| `lib/api/` | Dio kurulumu, jeton saklama ve yenileme interceptor'ı, `Idempotency-Key`, `If-Match`, `ApiProblem` eşleme |

## Kurallar
- Widget ölçüleri ve davranışı: `design/tasarim-sistemleri/mobil/components/<Bileşen>/README.md` (her README'de **Flutter:** satırı karşılığı söyler) ve `design/tasarim-sistemleri/mobil/README.md`.
- Renkler yalnız `Theme.of(context)` / `OmvilaRenkler` ile; `Color(0x…)` yasak.
- Her widget'ın widget testi ve golden testi (açık, koyu, 1.3× yazı) vardır.
- `format.dart` testleri ürün kurallarındaki biçim örneklerini birebir doğrular.
- `errors/tr.dart` sözleşmedeki her `ErrorCode` için girdi içerir; test eksik kodu yakalar.

## Komutlar
```bash
cd packages/omvila_ui
fvm flutter test
fvm flutter test --update-goldens   # yalnız bilinçli görsel değişiklikte
```
