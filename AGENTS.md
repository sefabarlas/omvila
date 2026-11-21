# OMVILA: ajan rehberi (kök)

OMVILA, randevuyla çalışan işletmeler (kuaför, berber, güzellik salonu, spa, diyetisyen) için bir randevu ve salon yönetimi platformudur. Ürün Türkiye pazarı içindir. Bu depodaki kodun tamamı yapay zekâ ajanlarıyla yazılır. Bu dosya her oturumun başlangıç noktasıdır. Çalıştığınız klasördeki `AGENTS.md` de ayrıca okunur ve bu dosyanın üstüne kural ekler.

## 1. Önce bunları bilin (değişmez ürün kuralları)
1. **OMVILA ödeme almaz.** Kartla ödeme, ödeme SDK'sı, kart formu yok. Kapora = müşterinin salonun IBAN'ına havalesi; salon "Kapora geldi" ile işaretler. (Tek istisna: salonun OMVILA aboneliği kartla çekilebilir.)
2. **Salon keşfi yok.** Müşteri salonu bağlantı/QR ile ekler.
3. **v1'de WhatsApp yok.** Kanallar SMS, push, e-posta.
4. **Sağlık verisi yok;** doğum tarihi yerine yalnız doğum yılı.
5. **Müşteri verisi işletmeye aittir;** bir marka başka markanın verisini asla görmez.
6. **Randevu durumları yedi tanedir:** `Draft, PendingApproval, Confirmed, CheckedIn, Completed, Cancelled, NoShow`. "Kapora bekleniyor" türetilmiş gösterimdir. Müşteriye "reddedildi" denmez, "iptal" denir.
7. **Salon girişi yalnız telefon + SMS kodu.** Rezervasyon sayfası üyeliksizdir.

Tam liste ve ses-tonu: [`docs/urun/urun-kurallari.md`](docs/urun/urun-kurallari.md). Bu kurallarla çelişen bir istek gelirse **durun ve sorun**.

## 2. Depo haritası
| Klasör | Ne | Kendi kuralları |
|---|---|---|
| `contracts/` | **Tek doğru kaynak** `openapi.yaml` | [contracts/AGENTS.md](contracts/AGENTS.md) |
| `api/` | .NET 10 modüler monolit + PostgreSQL | [api/AGENTS.md](api/AGENTS.md) |
| `web/salon-panel/` | Salon paneli, React + Vite SPA | [web/salon-panel/AGENTS.md](web/salon-panel/AGENTS.md) |
| `web/platform-panel/` | İç ekip paneli, React + Vite SPA | [web/platform-panel/AGENTS.md](web/platform-panel/AGENTS.md) |
| `web/rezervasyon/` | Herkese açık rezervasyon sayfası, Next.js | [web/rezervasyon/AGENTS.md](web/rezervasyon/AGENTS.md) |
| `web/tanitim/` | Tanıtım sitesi, Next.js statik | [web/tanitim/AGENTS.md](web/tanitim/AGENTS.md) |
| `mobile/isletme/` | OMVILA İşletme (Flutter) | [mobile/isletme/AGENTS.md](mobile/isletme/AGENTS.md) |
| `mobile/musteri/` | OMVILA müşteri uygulaması (Flutter) | [mobile/musteri/AGENTS.md](mobile/musteri/AGENTS.md) |
| `packages/api-client-ts/` | Sözleşmeden üretilen TS istemcisi | [packages/api-client-ts/AGENTS.md](packages/api-client-ts/AGENTS.md) |
| `packages/api-client-dart/` | Sözleşmeden üretilen Dart istemcisi | [packages/api-client-dart/AGENTS.md](packages/api-client-dart/AGENTS.md) |
| `packages/design-tokens/` | Renk, yazı, boşluk, hareket tokenları (CSS + Dart üretimi), logolar | [packages/design-tokens/AGENTS.md](packages/design-tokens/AGENTS.md) |
| `packages/ui-web/` | Web bileşenleri, biçimlendirme, zaman, hata mesajları | [packages/ui-web/AGENTS.md](packages/ui-web/AGENTS.md) |
| `packages/omvila_ui/` | Flutter ortak widget'lar ve tema | [packages/omvila_ui/AGENTS.md](packages/omvila_ui/AGENTS.md) |
| `design/` | Tasarım ekran görüntüleri ve tasarım sistemleri (salt okunur referans) | [design/README.md](design/README.md) |
| `docs/` | Ürün, alan modeli, mimari, standartlar, dilimler | aşağıda |

## 3. Hangi belgeyi ne zaman okursunuz
| Durum | Okuyun |
|---|---|
| Her iş | Bu dosya, çalıştığınız klasörün `AGENTS.md`'si, [docs/standartlar/tanim-bitti.md](docs/standartlar/tanim-bitti.md) |
| Bir dilimde iş | [docs/dilimler/](docs/dilimler/) içindeki dilim belgesi (kapsam, ekranlar, kabul kriterleri) |
| Varlık, alan, ilişki | [docs/alan/alan-modeli.md](docs/alan/alan-modeli.md), [docs/alan/sozluk.md](docs/alan/sozluk.md) |
| Durum değişikliği | [docs/alan/durum-makinesi.md](docs/alan/durum-makinesi.md) |
| Süre, eşik, varsayılan | [docs/alan/sureler-ve-ayarlar.md](docs/alan/sureler-ve-ayarlar.md) |
| Yetki, rol, menü | [docs/alan/roller-yetkiler.md](docs/alan/roller-yetkiler.md) |
| Müsaitlik, takvim, çakışma | [docs/alan/musaitlik.md](docs/alan/musaitlik.md) |
| API tasarımı | [docs/sozlesme/api-kurallari.md](docs/sozlesme/api-kurallari.md), [contracts/yol-haritasi.md](contracts/yol-haritasi.md) |
| Mimari karar | [docs/mimari/mimari.md](docs/mimari/mimari.md) ve [ADR'ler](docs/mimari/adr/) |
| Test ortamı, dağıtım, demo verisi | [docs/mimari/test-ortami.md](docs/mimari/test-ortami.md) |
| Bildirim metni | [docs/standartlar/bildirimler.md](docs/standartlar/bildirimler.md) |
| Kişisel veri, güvenlik | [docs/standartlar/guvenlik-kvkk.md](docs/standartlar/guvenlik-kvkk.md) |
| Ekran yaparken | [design/ekran-envanteri.md](design/ekran-envanteri.md) ve ilgili `design/ekranlar/<yüzey>/*.png` |
| Belirsizlik | [docs/acik-sorular.md](docs/acik-sorular.md) |

Belgeler arasında çelişki görürseniz öncelik şöyledir: `docs/urun/KARAR-GUNLUGU.md` > `docs/urun/teslim-notlari.md` > tasarım ekranları > `docs/alan/*` > `docs/referans/*` (eski backend; yalnız fikir). Çelişkiyi düzeltmeden koda geçmeyin; belgeyi düzeltin veya sorun.

## 4. Çalışma akışı
1. **Kapsamı netleştirin:** dilim belgesindeki kabul kriterlerini ve ilgili ekranları okuyun. Belirsizse `docs/acik-sorular.md`'ye bakın. Orada da yoksa, geri dönüşü zor bir kararsa sorun. Değilse makul varsayımla ilerleyin ve varsayımı PR'da ve `acik-sorular.md`'de yazın.
2. **Önce sözleşme:** API değişiyorsa `contracts/openapi.yaml` → `pnpm --filter @omvila/contracts lint` → `generate`. Backend ve istemciler sözleşmeye uyar, tersi olmaz.
3. **Önce test:** kural için testi yazın (alan kuralı → alan testi; uç nokta → entegrasyon testi), sonra kodu.
4. **Küçük adım:** bir PR = bir özellik dilimi parçası (< 400 satır üretim kodu).
5. **Doğrulayın:** derleme, lint, testler, ekranlar için tasarımla yan yana ekran görüntüsü (açık + koyu).
6. **Teslim:** [tanim-bitti.md](docs/standartlar/tanim-bitti.md) listesini PR açıklamasında işaretleyin; varsayımları yazın.

Dal, commit ve PR biçimi: [docs/standartlar/git-akisi.md](docs/standartlar/git-akisi.md). Örnek commit: `feat(api/scheduling): onay süresi dolan talepleri otomatik iptal et`.

## 5. Evrensel kurallar (her projede)
- **Kod İngilizce, arayüz Türkçe.** Tanımlayıcılar `docs/alan/sozluk.md`'deki İngilizce karşılıklardır; eş anlamlı uydurulmaz.
- **Zaman:** sakla UTC, göster şubenin saat diliminde (`Europe/Istanbul` varsayılan). Cihazın veya sunucunun saat dilimi kullanılmaz. "Şimdi" yalnız enjekte edilen saatten (`IClock`).
- **Para:** kuruş cinsinden tamsayı (`amountMinor`) + para birimi. Ondalıklı sayı ile para hesabı yok.
- **Telefon:** E.164 saklanır, `0532 123 45 67` gösterilir; Personel rolünde maskeli.
- **Renk:** yalnız semantik tasarım tokenı. Sabit hex ve ham palet (`mercan-800`) yasak.
- **Metin:** "siz" hitabı; ünlem, emoji, "lütfen", "başarıyla", "kolayca", "sadece" yok; butonlar fiille başlar. Durum adları sabit: Onay bekliyor, Kapora bekleniyor, Onaylandı, Geldi, Tamamlandı, Gelmedi, İptal.
- **Boş durum:** her listenin boş hâli vardır; sahte sayı, sayaç veya rozet gösterilmez.
- **Sabit değer yok:** süre, eşik, fiyat kodda yazılmaz. `sureler-ve-ayarlar.md`'deki ayardan veya veriden okunur. Köşeli parantezli yer tutucular (`[AYLIK FİYAT]`…) kodda geçmez.
- **Gizlilik:** telefon, ad, IBAN, not içeriği günlüğe yazılmaz. Test verisinde gerçek kişi yok.

## 6. Yasaklar
- Sözleşmeyi atlayıp elle `fetch`/`http` ile API çağırmak; üretilmiş istemci kodunu elle düzenlemek.
- Testi silmek, atlamak (`skip`) veya beklentisini gevşeterek geçirmek.
- `contracts/`'ta kırıcı değişiklik, `docs/urun/` kurallarını değiştirmek: yalnız kullanıcı onayıyla.
- Modül sınırını delmek (başka modülün tablosuna veya iç sınıfına erişmek), `IgnoreQueryFilters()`, kiracı kimliğini istekten almak.
- `DateTime.Now`, `new Date()` ile yerel saat hesabı, `Guid.NewGuid()` (backend'de UUID v7 üretici var).
- Yeni bağımlılığı gerekçesiz eklemek (PR'da neden yazılır).
- Gizli değeri (anahtar, parola, jeton) depoya koymak.
- Tasarımda olmayan ekran veya özellik uydurmak. Gerekirse sorun.

## 7. Durup sorun
- Ürün kuralı veya karar günlüğüyle çelişen istek.
- Kırıcı API değişikliği, veri silen migration, kiracı izolasyonuna dokunan değişiklik.
- `docs/acik-sorular.md`'de 🔴 olan bir konuya bağımlı iş.
- Tasarım ekranı ile belge arasında çözemediğiniz çelişki.

## 8. Komutlar (kök)
```bash
docker compose up -d                       # PostgreSQL 17, Prism mock, Mailpit
pnpm install                               # JS/TS bağımlılıkları (pnpm workspace)
pnpm --filter @omvila/contracts lint       # sözleşme denetimi
pnpm --filter @omvila/contracts generate   # TS + Dart istemcileri
pnpm turbo build test lint                 # tüm web paketleri
(cd api && dotnet test --solution Omvila.slnx)   # backend testleri
melos bootstrap && melos run test          # Flutter paketleri
python3 scripts/check_no_payments.py       # ödeme SDK'sı / kart formu taraması (kural 1; CI `odeme-taramasi`)
```
Projeye özel komutlar ilgili `AGENTS.md`'dedir.

## 9. Durum
[Dilim 1](docs/dilimler/dilim-01-giris-ve-kurulum.md), [Dilim 2](docs/dilimler/dilim-02-musaitlik-rezervasyon-takvim.md) ve [Dilim 3](docs/dilimler/dilim-03-onay-ve-sms.md) uygulandı: kimlik, kiracı, katalog, personel, müşteriler, randevu (müsaitlik motoru, çakışma engeli, bildirim outbox'ı, sistem işleri); onay akışı (onay/ret uçları, süre dolumu işi), SMS gönderimi (kontör, geri çekilmeli tekrar, sessiz saat, teslim raporu, sahte sağlayıcı), push altyapısı (cihaz kaydı, FCM adaptörü, SMS'e geri düşme), uygulama içi bildirimler ve bildirim ayarları; salon panelinde kurulum, ayarlar (Bildirimler ve SMS dahil), hizmetler, personel, takvim, müşteriler, yeni randevu, onay yan paneli, Genel bakış, bildirim zili; rezervasyon sayfası (online randevu akışı, talep ekranı, SMS bağlantısı); Playwright uçtan uca testleri ve tasarım karşılaştırmaları ([Dilim 2](docs/dilimler/dilim-02-tasarim-karsilastirma.md), [Dilim 3](docs/dilimler/dilim-03-tasarim-karsilastirma.md)). Gerçek SMS sağlayıcısı canlıya çıkışta seçilecek (T-03); o zamana kadar sahte sağlayıcı. Dilimlerin kapanması için ürün sahibi demosu ve `docs/acik-sorular.md`'deki varsayımların (Dilim 1: T-37…T-45, Dilim 2: T-46…T-61, Dilim 3: T-62…T-72) onayı bekleniyor. Sıradaki iş [Dilim 4](docs/dilimler/dilim-04-kapora-havalesi.md)'tür.
