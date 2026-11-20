# Ürün kuralları (değişmez)

Bu kurallar her yüzeyde, her katmanda geçerlidir. Bir kuralı değiştirmek ürün kararıdır. Kararı ürün sahibi verir ve önce `KARAR-GUNLUGU.md`'ye yazılır. Bir ajan bu kurallarla çelişen bir istek alırsa işi durdurur ve sorar.

## Söz verme sınırları

1. **OMVILA ödeme almaz.** Uygulamalarda ve rezervasyon sayfasında kartla ödeme yoktur. Kapora, müşterinin salonun IBAN'ına yaptığı havaledir; salon "Kapora geldi" ile işaretler. İadeyi salon yapar. Kodda ödeme sağlayıcısı (iyzico, PayTR, Stripe…) **müşteri ödemesi için** kullanılmaz. Tek istisna: salonun OMVILA'ya ödediği abonelik kartla çekilebilir.
2. **Salon keşfi yoktur.** Müşteri salonu bağlantı veya QR ile ekler. Arama, harita, "yakındaki salonlar" yazılmaz; metinde mevcut özellik gibi anılmaz.
3. **v1'de WhatsApp yoktur.** Kanallar SMS, push, e-posta. Metinlerde WhatsApp hatırlatması vaat edilmez. Rezervasyon bağlantısını müşterinin kendisinin WhatsApp'ta paylaşması bu kapsama girmez.
4. **Sağlık verisi tutulmaz.** Alerji, hassasiyet, rahatsızlık gibi alan, etiket veya örnek metin yoktur. Not alanlarının yanında uyarı yazar. Doğum tarihi yerine yalnız doğum **yılı** tutulur.
5. **Müşteri verisi işletmeye aittir.** Bir salon yalnız kendi müşterilerini görür. Başka markanın notu, etiketi veya geçmişi hiçbir uçtan dönmez.
6. **Üyeliksiz randevu.** Rezervasyon sayfası şifre, hesap veya uygulama indirme istemez. Telefon + SMS kodu yeterlidir.
7. **Randevu mesajları kapatılamaz.** Onay, hatırlatma, değişiklik ve iptal işlem mesajıdır. Müşteri yalnız salon bazında kampanya iznini açıp kapatır.
8. **"Reddedildi" denmez.** Onaylanmayan veya süresi dolan talep müşteriye "iptal" olarak gösterilir.

## Değişmez iş kuralları (özet; ayrıntı `docs/alan/`)
- Randevu durumları yedi tanedir: `Draft`, `PendingApproval`, `Confirmed`, `CheckedIn`, `Completed`, `Cancelled`, `NoShow`. "Kapora bekleniyor" türetilmiş gösterimdir.
- Onay süresi varsayılan 120 dk; dolunca otomatik iptal.
- Havale süresi varsayılan 2 saat (1 / 2 / 4); dolar ve salon işaretlemezse otomatik iptal. Açıklama kodu `OMV-xxxx`.
- IBAN tanımlı değilse hizmete kapora konamaz.
- İptal ve saat değişikliğinin ayrı son süreleri vardır; müşteri iptalde neden seçmek zorundadır.
- Bekleme listesinde açılan saat 15 dk tutulur.
- Prim randevu `Completed` olunca doğar; iptalde ters kayıt açılır.
- Telefon maskesi yalnız Personel rolünde uygulanır.
- Salon tarafında giriş yalnız telefon + SMS kodudur.
- Abonelik: kartla otomatik çekim veya havale/EFT (`ABN-…`); havale platform panelinde elle eşleştirilir.

## Ses ve ton (arayüz metni)
- Hitap **"siz"**. Sistem kendini "ben" diye anmaz.
- Önce sonuç, sonra ayrıntı: "Randevu iptal edildi. Müşteriye SMS gönderildi."
- Butonlar fiille başlar, 1–3 kelime, cümle düzeninde, noktasız: "Randevu al", "Kapora geldi".
- Ünlem, emoji, "lütfen", "başarıyla", "kolayca", "sadece" kullanılmaz.
- Hata mesajı ne olduğunu ve ne yapılacağını söyler: "Bu saat dolu. Başka bir saat seçin." Teknik hata metni (stack trace, `500`, İngilizce exception) kullanıcıya gösterilmez.
- Boş durum davettir: başlık alanı adlandırır, tek cümle ne zaman dolacağını söyler, tek eylem.
- Durum adları sabittir: Onay bekliyor, Kapora bekleniyor, Onaylandı, Geldi, Tamamlandı, Gelmedi, İptal.
- Marka adı metinde her zaman **OMVILA**. Uygulama adları "OMVILA İşletme" ve "OMVILA".
- Türkçe büyük harf dönüşümü `lang="tr"` / `Locale('tr')` ile yapılır ("i" → "İ"). Elle büyük harf yazılmaz.

## Biçimler
| Tür | Biçim | Örnek |
|---|---|---|
| Tarih | Gün Ay Haftagünü | 21 Kasım Cuma; panelde "Salı, 25 Kasım" |
| Dar tarih | | 21 Kas Cum |
| Sayısal tarih | gg.aa.yyyy | 21.11.2026 |
| Saat | 24 saat | 09:30, 14:45 |
| Süre | | 45 dk, 1 sa 30 dk |
| Tutar | binlik nokta, kuruş virgül, sembol sonda, araya boşluk | 1.250 ₺, 85,50 ₺ |
| Telefon | | 0532 123 45 67; maskeli 0532 *** ** 67 |

Biçimlendirme tek yerde yapılır: web'de `packages/ui-web/format`, Flutter'da `omvila_ui/format`. Bileşen içinde `toLocaleString` / `DateFormat` doğrudan çağrılmaz.

## Tasarım sistemi kuralları (özet)
- Renk yalnız semantik tokenla verilir (`zemin`, `metin`, `marka`, `vurgu`, `durum-*`). Ham palet (`mercan-800`) ve sabit hex yasaktır. `zemin-vurgu-yumusak` üzerindeki metin `metin-vurgu` kullanır.
- Her ekran açık ve koyu temada çalışır.
- Listesi olan her ekranın boş hâli vardır. Boş ekranda sahte sayı, sayaç veya rozet yoktur.
- Mobilde dokunma alanı en az 44×44. Panelde dokunmatik ekranda (`pointer: coarse`) aynı kural geçerlidir.
- Hareket tokenları (`--sure-*`, `--egri-*`); azaltılmış harekette yalnız 120 ms opaklık. Sayılar sayarak artmaz. 150 ms'den kısa yüklemede iskelet gösterilmez.
- Durum yalnız renkle anlatılmaz; rozet adı yazılır, ekran okuyucu durumu okur.
- İkonlar Lucide. Emoji ikon yerine kullanılmaz.

Kaynaklar: `design/tasarim-sistemleri/{marka,panel,mobil}/README.md`, `docs/urun/teslim-notlari.md`.
