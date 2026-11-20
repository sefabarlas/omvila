Bu sistem OMVILA'nın mobil yüzeyleri içindir: çalışanların kullandığı **OMVILA İşletme** (salon uygulaması), işletmelerin müşterilerinin kullandığı **OMVILA** (müşteri uygulaması) ve müşterilerin çoğunlukla telefondan açtığı **rezervasyon sayfası** (omvila.com/salon-adi). Uygulamalar Flutter ile yazılır. Rezervasyon sayfası web'dir ama aynı bileşenleri ve ölçüleri kullanır. Marka kararları **OMVILA Marka Sistemi**'nden, renk primitifleri, semantik adlar ve randevu durumları **Ürün Panel Sistemi** ile ortaktır.

## İki uygulama, iki ton

Bileşenler ortaktır ama iki uygulama farklı hissettirir:

| | OMVILA İşletme | OMVILA (müşteri) ve rezervasyon sayfası |
|---|---|---|
| Kullanıcı | Salon çalışanı, gün içinde ayakta | İşletmenin müşterisi, ayda bir iki kez |
| His | "İşim cebimde": hızlı, yoğun, durum odaklı | "Randevularım bir arada": sakin, ferah, güven veren |
| Ana ekran | Bugünün randevu listesi | Yaklaşan randevu kartı |
| Yoğunluk | Liste satırları, kaydırma eylemleri, rozetler | Büyük kartlar, bol boşluk, tek ana eylem |
| Sekmeler | Bugün, Takvim, Müşteriler, Bildirimler, Profil | Randevularım, Paketlerim, Profil |
| Uygulama ikonu | Koyu petrol zemin | Açık kum zemin |

## Yerleşim

- Tasarım referansı `ekran-genislik` (390pt). Ekran kenar boşluğu `bosluk-4` (16), bölümler arası `bosluk-6` (24).
- Güvenli alanlar her zaman korunur. Alt sekme çubuğu `tabbar` (64) yüksekliğinde, kenarlardan 16, alttan 24 içeride **yüzen koyu bir hap**tır. Seçili sekme beyaz hap içinde ikon ve adıyla, diğerleri yalnızca ikonla görünür. Kaydırılan içerik çubuğun altından geçer, son öğenin altına 104 boşluk bırakılır.
- Ekranın ana eylemi altta, tam genişlikte, `kontrol-lg` (52) yüksekliğindedir. Kaydırılan içerik bu butonun arkasında kalmaz.
- Rezervasyon sayfası masaüstünde iki kolona geçer: solda salon kartı ve canlı güncellenen randevu özeti (en fazla 380px), sağda adımlar (en fazla 680px). Dar pencerede kolonlar alt alta iner ve mobil düzene döner.

## Dokunma ve erişilebilirlik

- Her dokunma alanı en az `dokunma-min` (44×44). Görseli küçük olan öğenin de dokunma alanı bu kadardır.
- Mobilde okunan metin en az 16px (`govde`). İstisnalar: `aciklama` (12) ve `sekme` (11), yalnızca kısa etiketlerde.
- Sistem yazı boyutu büyütmesi desteklenir. 1.3× ölçekte satırlar kırılır, kesilmez.
- Kaydırma eylemlerinin (Geldi, Ara) her zaman bir buton karşılığı vardır.
- Durum yalnızca renkle anlatılmaz. Rozet adı yazar, ekran okuyucu durumu okur.
- Hareket azaltma açıkken iskelet nabzı ve geçiş animasyonları kapanır.

## Renk

- Semantik tokenlar Marka ve Panel sistemleriyle aynıdır (`zemin`, `metin`, `marka`, `vurgu`, `durum-*`…). Mobile özgü olanlar: `tabbar-*`, `blok-marka`, `blok-vurgu`, `blok-kum`, `blok-kum-metin`, `iskelet`, `tutamak`, `yildiz`.
- Ekran zemini `zemin`: mobilde kum değil, nötr sıcak beyazdır (#FAFAF8). Kum marka rengi olarak bloklarda yaşar.
- Kartlar `zemin-yukseltilmis` üzerinde, çizgisiz ve `golge-sm` ile ayrılır. Liste satırları arasında ayraç çizgisi yerine 8 boşluk bırakılır.
- **Bento bloklar** ana ekranların özetidir: `blok-marka` (petrol) en önemli tek bilgi için (sıradaki randevu), `blok-vurgu` (mercan) dikkat isteyen sayı için (onay bekleyen), `blok-kum` sakin bilgi için (sıra, paket). Bir ekranda her renkten en fazla bir blok olur.
- Seçili gün, saat ve çip `blok-marka` ile dolar. `marka` dolgusu ana eylem butonudur.
- `vurgu` (mercan) bildirim rozeti, "Yeni" etiketi ve onay bloğu için kullanılır.
- Koyu tema iki uygulamada da desteklenir ve sistem ayarını izler.

## Tipografi

Plus Jakarta Sans, mobil ölçekte:

- Sekme kök ekranlarında `buyuk-baslik` (34, 800, sıkı harf aralığı). Kaydırınca `ust-bar-basligi`na (17) küçülür.
- Detay başlığı `baslik-1` (24), bölüm başlığı `baslik-2` (20), kart başlığı `baslik-3` (17).
- Gövde `govde` (16), ikinci satır `ikincil` (14), buton `etiket` (16, 600).
- Saat ve tutarlar `rakam`, bloktaki büyük sayılar `metrik` (36, 800). İkisi de tabular rakamlarla yazılır.
- Bölüm başlıkları küçük gri etiket değil, 18px ExtraBold koyu başlıktır (`om-bolum`).

## Köşe ve gölge

- Mobil köşeler belirgin yuvarlaktır: çip ve saat `kose-sm` (12), buton ve giriş alanı `kose-md` (14), liste kartı `kose-lg` (20), bento blok ve büyük kart `kose-xl` (24), alt sayfa 28.
- Gölge yumuşak ve geniştir (`golge-sm`: 1px + 20px yayılım, çok düşük opaklık). Kenarlık yalnızca giriş alanlarında ve seçilmemiş çip/saat/günde kalır.

## Rezervasyon akışı

Üyeliksiz randevu, markanın en önemli sözlerinden biridir. Akış dört adımdır ve hiçbir adımda şifre ya da üyelik istenmez:

1. **Hizmet:** bir veya birden fazla hizmet seçilir. Toplam süre ve tutar alt çubukta görünür.
2. **Personel:** ilk seçenek "Fark etmez" (en erken uygun saat).
3. **Saat:** gün şeridi ve saat ızgarası.
4. **Onay:** özet, telefon numarası, SMS doğrulama kodu ve gerekiyorsa kapora. Kapora istenirse randevu oluşunca havale ekranı açılır: alıcı, IBAN, tutar ve açıklama kodu kopyalanabilir; kalan süre gösterilir. Uygulamada ödeme alınmaz.

## Yazım (mobil)

Marka Sistemi'ndeki ses ve ton geçerlidir. Mobilde ayrıca:

- Hitap "siz". Bildirimler kısa ve eyleme dönüktür: "Yarın 14:30'da Kuaför Elif'te randevunuz var."
- Hatırlatmalar SMS, bildirim ve e-posta ile gider. WhatsApp ilk sürümde yoktur ve metinlerde anılmaz.
- Müşteri uygulamasında salon keşfi (pazaryeri) yol haritasındadır. Sekme, boş durum ya da tanıtım metninde mevcut bir özellik gibi anlatılmaz.
- Durum adları panelle aynıdır: Onay bekliyor, Kapora bekleniyor, Onaylandı, Geldi, Tamamlandı, Gelmedi, İptal.

## İkonografi

Lucide çizgi ikonları (Flutter'da `lucide_icons`). Sekme çubuğunda 24, listede ve butonda 20. Seçili sekme ikonu biraz daha kalın çizilir.

## Bileşenleri kullanmak

Önizlemeler web'de çalışır (`components/bundle.js` içinde `window.OmvilaMobil`, React 18). Flutter karşılıklarının hepsi her bileşenin README'sinde **Flutter:** satırında yazılıdır. Tokenlar `flutter/omvila_theme.dart.txt` dosyasında hazırdır. Kurulum için **Flutter** bölümüne bakın.


## Kurallardan gelen desenler

Bu bölüm `omvila-teknik-kurallar-ve-kisitlar.md` ile hizalanmıştır; panel sistemindeki aynı başlıklı bölümle birlikte okunur.

- **Randevu mesajları kapatılamaz.** Onay, hatırlatma ve değişiklik işlem mesajıdır; müşteriye kanal anahtarı gösterilmez. Uygulama bildirimi açıksa önce bildirim gider, ulaşmazsa SMS. Müşteri yalnızca salon bazında **kampanya** iznini açıp kapatır.
- **Müşteri iptali sebep ister** (zorunlu alan). İptal ve erteleme süreleri ayrı ayarlardır; sınırın altında buton yerine "salonu arayın" gösterilir.
- **Değerlendirme:** yalnızca tamamlanan randevu, bir kez; puan, isteğe bağlı yorum ve personel. Ek etiket (ör. "Neler iyiydi?") veri modelinde yok, kullanılmaz.
- **Bekleme listesi teklifi** 15 dakika tutulur.
- **Ödemeler** müşteride salt okunurdur. Kapora yalnızca havale ile gönderilir; durumları "Salon onayı bekleniyor", "Alındı", "Salon iade etti". "Havaleyi yaptım" yalnızca salona bildirimdir, onay salonun işaretidir.
- **Kişisel veri:** sağlık verisi için alan veya örnek metin kullanılmaz.
- **Sistem durumları** (Müşteri uygulaması ve rezervasyon tuvallerinde "Durum desenleri"): saat az önce doldu, SMS kodu deneme sınırı, online randevu kapalı (abonelik salt okunur), bağlantı geçersiz / randevu bulunamadı, bağlantı yok.
- **İşletme uygulaması:** bekleme dilimli hizmetlerde takvim bloğunun içinde taralı "Bekleme · müsait" şeridi; randevu "Tamamla ve ödeme al" alt sayfasıyla kapanır; yeni randevuda tekrar seçeneği.

## Erişilebilirlik ve yeni desenler (son kontrol)
- **`metin-vurgu`** yeni token: `zemin-vurgu-yumusak` üzerindeki metin ve ikon için. Açık temada `mercan-800`, koyu temada `mercan-300` (iki temada da 4.5:1 üstü). `mercan-800`'ü doğrudan kullanmayın; koyu temada okunmuyordu.
- **Dokunma alanı:** `om-btn-sm` ve `om-chip` 36px görünür ama 44px dokunma alanı alır (dikeyde 4px görünmez genişleme). Flutter'da `MaterialTapTargetSize.padded` ile aynı sonuç.
- **Yeni ikonlar:** `bell-off`, `bell-ring`, `qr-code`, `code`, `globe`, `send`, `sparkles`.
- **Bildirim izni önce kendi ekranımızla istenir:** sistem istemi yalnızca "Bildirimlere izin ver"e basınca açılır. "Şimdi değil" ana ekranda kapatılabilir bir bilgi şeridi bırakır (müşteride: hatırlatmalar SMS ile gelir; işletmede: onay bekleyen talepler kaçabilir).
- **Bekleme listesi teklifi:** büyük geri sayım (`blok-vurgu`), "Randevuyu al" birincil, "Bu saat olmaz" ikincil. Süre dolunca ayrı ekran; kişi listede kalır.
- **Saat değiştirme** iptalden ayrı bir sınırla çalışır (örnekte 12 saat). Sınır geçtiyse alt sayfa "Salonu ara" önerir; kapora yeni saate aktarılır.
- Devre dışı metin (`metin-devre-disi`) kontrast şartından muaftır; devre dışı kontrolün yanında nedenini yazın.

## Boş durumlar
- Listesi olan her ekranın bir boş hali vardır ve tuvalde dolu ekranın hemen sağında durur.
- Yapı: 80px ikon kutusu (`zemin-secili` üzerinde `metin-marka`), tek satır başlık ("Henüz … yok"), ne zaman dolacağını anlatan bir cümle, varsa tek bir eylem.
- **Müşteri ilk girişi:** salon keşfi olmadığı için boş ekran ilk salonun nasıl ekleneceğini anlatır: "QR kodu okut" ve "Bağlantıyı yapıştır".
- **Geçmişi olan ama yaklaşan randevusu olmayan kullanıcı** boş ekran görmez: son randevudan "Aynısını tekrar al" önerilir.
- Boş ekranlarda sahte sayı veya rozet gösterilmez (sekme rozeti dahil).
- Yeni ikonlar: `scan-line`, `clipboard-paste`, `camera`.

## Hareket ve geçiş
- Süreler: `--sure-anlik` 80ms (basma), `--sure-hizli` 150ms (hover, çip, anahtar, sekme), `--sure-orta` 240ms (alt sayfa, çekmece, sayfa geçişi, menü daralması), `--sure-yavas` 360ms (tam ekran modal).
- Eğriler: `--egri-standart` cubic-bezier(.2,0,0,1) yerinde değişim; `--egri-giris` cubic-bezier(0,0,0,1) ekrana giren; `--egri-cikis` cubic-bezier(.3,0,1,1) ekrandan çıkan. Çıkış girişten kısa: 180ms.
- Alt sayfa aşağıdan, çekmece sağdan kayar; modal %96'dan büyür. Sekmeler arası geçişte hareket yoktur.
- İskelet: 150ms'den kısa yüklemede gösterilmez, gösterilirse en az 300ms kalır.
- Sayılar sayarak artmaz.
- `prefers-reduced-motion` açıkken kayma ve büyüme kapanır, yalnızca 120ms opaklık kalır (tokens.css bunu kendisi yapar).
- Flutter: `Duration(milliseconds: 80/150/240/360)`, eğriler `Cubic(.2,0,0,1)`, `Cubic(0,0,0,1)`, `Cubic(.3,0,1,1)`.
