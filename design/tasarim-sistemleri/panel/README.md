Bu sistem OMVILA'nın web panelleri içindir: işletmenin ana çalışma alanı olan **Salon paneli** ve OMVILA ekibinin iç kullanımındaki **Platform paneli**. Marka kararları (logo, renk paleti, yazı ailesi, ses ve ton, söz verme sınırları) **OMVILA Marka Sistemi**'nden gelir. Buradaki renk primitifleri ve semantik token adları markayla birebir aynıdır; panel yalnızca yoğunluk, ölçek ve bileşen ekler.

## Panel nasıl hissettirmeli

Panel, gün boyu açık kalan bir iş aracıdır. Resepsiyonist telefonla konuşurken randevu ekler, salon sahibi akşam kasaya bakar. Bu yüzden:

- **Bir bakışta okunur.** Takvim, durum renkleri ve sayılar ilk bakışta anlaşılır; süsleme yoktur.
- **Yoğun ama sıkışık değildir.** 14px gövde, 36px kontroller, 48px tablo satırı.
- **Marka kenarda durur.** Petrol renkli kenar menüsü markayı taşır; çalışma alanı sakin ve açık renklidir. Mercan yalnızca dikkat gerektiren yerlerde kullanılır: bildirim sayısı, şu anki saat, yeni müşteri.

## Yerleşim

- Kenar menüsü `sidebar-genislik` (248px), 1280px altında 64px'e daralır. Üst bar `ustbar-yukseklik` (56px).
- Sayfa zemini `zemin`; kartlar, tablolar ve takvim `zemin-yukseltilmis` üzerinde, `cizgi` kenarlı ve `golge-sm` gölgeli.
- İçerik kenar boşluğu `bosluk-6` (24px), kartlar arası `bosluk-6`, bölümler arası `bosluk-8`. İçerik en fazla `icerik-genislik` (1440px).
- Her sayfa: üst barda sayfa adı ve tek `primary` eylem → isteğe bağlı filtre çubuğu → içerik.

## Renk

- Semantik tokenlar Marka Sistemi ile aynıdır (`zemin`, `metin`, `marka`, `vurgu`, `cizgi`, `odak`…). Panele özgü olanlar: `zemin-hover`, `zemin-secili`, `zemin-devre-disi`, `sidebar-*`, bildirim zeminleri (`basari-zemin`, `uyari-zemin`, `hata-zemin`, `bilgi-zemin`) ve randevu durumları, özet blok renkleri (`blok-marka`, `blok-vurgu`, `blok-kum`, `blok-kum-metin`).
- Sayfa zemini `zemin` panelde nötr sıcak gridir (#F4F4F1); kartlar beyaz (`zemin-yukseltilmis`) ve çerçevesizdir. Kum marka rengi olarak özet bloklarda yaşar.
- **Özet bloklar** (Bento) sayfanın üstündeki 3–4 sayılık şerittir: `blok-marka` en önemli tek bilgi (sıradaki randevu, günün cirosu), `blok-vurgu` dikkat isteyen sayı (onay bekleyen), `blok-kum` sakin bilgi (sıra, bekleme listesi), kalanlar beyaz. Bir sayfada her renkten en fazla bir blok olur.
- **Randevu durumları** her yerde aynı üç tokenla gösterilir: `durum-<durum>-zemin`, `-metin`, `-nokta`. Durumlar: `onay` (Onay bekliyor), `kapora` (Kapora bekleniyor), `onaylandi`, `geldi`, `tamamlandi`, `gelmedi`, `iptal`. Veritabanındaki `Appointment.Status` ile eşlemesi `StatusBadge` kartındadır; `Draft` hiçbir ekranda görünmez. Renk her zaman durum adı veya erişilebilir etiketle birlikte kullanılır.
- `marka` dolgusu ana eylem, seçili saat ve etkin sekme içindir. Seçili satır ve menü öğesi `zemin-secili`.
- `vurgu` (mercan) panelde yalnızca: bildirim sayısı, kenar menüsünde etkin ikon, takvimde şu anki saat ve "Yeni müşteri" rozeti.
- Her metin rengi, kullanım notunda adı geçen zeminlerde açık ve koyu temada en az 4.5:1 sağlar; durum noktaları, `odak` ve `cizgi-guclu` en az 3:1.

## Tipografi

Plus Jakarta Sans, panel ölçeğinde:

- Sayfa başlığı `sayfa-basligi` (28px, 800, sıkı harf aralığı); bölüm ve modal `bolum-basligi`; kart ve tablo `kart-basligi`.
- Varsayılan metin `govde` (14px); satırdaki birincil değer `govde-guclu`; ikincil satır `kucuk` (13px).
- Buton, sekme, etiket `etiket` (13px); butonlar 700 ağırlıkla yazılır.
- Saat ve tutarlar `rakam`, istatistik `metrik`; ikisi de `tabular-nums`. Tablolarda sayılar sağa hizalanır.

## Boşluk, köşe, gölge

- 4px tabanlı ölçek; panelde 2px ve 6px ara adımlar vardır (`bosluk-0-5`, `bosluk-1-5`).
- Köşeler mobilden bir adım sıkıdır: buton ve giriş alanı `kose-md` (10px), kart ve tablo `kose-lg` (16px), özet blok, takvim kabı ve modal `kose-xl` (20px), takvim bloğu `kose-sm` (8px).
- Kartlar çerçevesizdir, zeminden `golge-sm` ile ayrılır. `cizgi` yalnızca kart içindeki satır ayraçlarında, tablo satırlarında ve takvim ızgarasında kalır. `golge-md` açılır menü ve toast, `golge-lg` modal ve yan panel.
- Panel mobilden yoğundur: tablolar ve takvim sıkı kalır, ferahlık yalnızca sayfa başı ve özet bloklardadır.
- Klavye odağı her etkileşimli öğede `odak-halkasi`; kenar menüsünde `odak-sidebar`.

## Kontroller

- Yükseklikler `kontrol-sm` (28px: tablo ve kart içi), `kontrol-md` (36px: varsayılan), `kontrol-lg` (40px: modal ve form sayfası ana eylemi).
- Bir görünümde tek `primary` buton. Silme ve iptal `danger` ile, onay modalı arkasında.
- Form etiketleri her zaman görünür; hata mesajı alanın altında, ne yapılacağını söyler.

## Takvim

Panelin kalbi takvimdir.

- Günlük görünümde her personel bir kolon, haftalık görünümde her gün bir kolon.
- Bir saat `takvim-saat` (64px), 15 dakika 16px. Yarım saat çizgisi siliktir.
- Randevu bloğu durum zemininde, üstünde 3px durum şeridi; müşteri adı, başlangıç saati, hizmet. Kısa randevular tek satır. Bekleyen randevular kesik çizgili kenarlıdır.
- Personelin takvim rengi (`Staff.CalendarColor`) yalnızca kolon başlığındaki avatar halkasında kullanılır; blok rengi her zaman durumu anlatır.
- Şu anki saat mercan çizgi ve saat etiketiyle bütün kolonları keser.
- Bloğa tıklamak randevu detay kartını açar; boş alana tıklamak o saatte yeni randevu formunu açar.

## Yazım (panel)

Marka Sistemi'ndeki ses ve ton geçerlidir. Panelde ayrıca:

- Hitap "siz"; sistem kendini "ben" diye anmaz.
- Butonlar fiille başlar: "Randevu ekle", "Geldi olarak işaretle", "Kapora geldi".
- Onay mesajları kısa ve geçmiş zamanda: "Randevu oluşturuldu", "Müşteri kaydedildi".
- Durum adları sabittir ve her yerde aynı yazılır: Onay bekliyor, Kapora bekleniyor, Onaylandı, Geldi, Tamamlandı, Gelmedi, İptal.
- Tarih "Salı, 25 Kasım"; saat `14:30`; tutar `1.250 ₺`; telefon `0532 123 45 67`.

## İkonografi

Lucide çizgi ikonları, 1.75 çizgi kalınlığı. Menüde 18px, butonda 16–18px, boş durumda 24px. Pakette kullanılan ikonlar `Icon` bileşeniyle gelir.

## Bileşenleri kullanmak

Bileşenler `components/bundle.js` içinde `window.OmvilaPanel` olarak durur ve React 18 bekler. Stiller `components/bundle.css` içindedir ve yalnızca tokenları okur. Ürün kodu henüz bir framework'e bağlı değildir; bileşenlerin davranışı, ölçüleri ve durumları buradaki önizlemelerde ve README'lerde tanımlıdır. Hangi stack seçilirse bileşenler bu tanıma göre yeniden yazılır, tokenlar aynen taşınır.


## Kurallardan gelen desenler

Bu bölüm `omvila-teknik-kurallar-ve-kisitlar.md` ile hizalanmıştır.

- **Durumlar:** `Appointment.Status` yedi değerdir; "Kapora bekleniyor" ayrı bir durum değil, `PendingApproval` + istenen kapora + henüz işaretlenmemiş havaleden türetilen görünümdür (`durum-kapora-*`). Onay bekleyen randevuda kalan süre `HoldExpiresUtc`'den gösterilir; süre dolunca randevu otomatik **iptal** olur ("reddedildi" denmez).
- **Gelmedi otomatiktir:** `NoShowAfterMinutes` sonrası randevu kendiliğinden "Gelmedi" olur. Arayüz elle "gelmedi işaretle" istemez, "Geldiyse geri al" sunar.
- **Bekleme dilimi:** Aşamalı hizmetlerde (`ServiceStage`) personelin müsait olduğu dilim takvim bloğunun içinde taralı şeritle gösterilir (`appointment.gap = {start, end}`).
- **Tekrarlayan randevu:** blokta `repeat` ikonu; iptal ve ertelemede "Yalnızca bu" / "Bu ve sonrakiler" seçimi zorunludur, geçmiş randevular etkilenmez.
- **Para:** Tahsilat ile hak edilen gelir ayrı gösterilir. Online ödeme yoktur: kapora müşterinin salonun IBAN'ına yaptığı havaledir. Salon havaleyi görünce randevuda "Kapora geldi" der; süre (varsayılan 2 saat) dolarsa randevu otomatik iptal olur. IBAN tanımlı değilse kapora ayarları kapalıdır. Abonelik ve kontör kartla ya da havale/EFT ile ödenir; havaleleri platform ekibi açıklama koduyla eşleştirir.
- **Prim:** Randevu **tamamlanınca** doğar; iptal ve gelmemede ters kayıt açılır.
- **Telefon:** `Staff` rolü maskeli, yönetici ve sahip tam numara görür.
- **Kişisel veri:** Sağlık verisi (alerji, hassasiyet, rahatsızlık) için alan, etiket veya örnek metin kullanılmaz. Doğum tarihi yerine doğum yılı.
- **Sistem durumları** (Salon Paneli tuvalinde "Durum desenleri"): kayıt çakışması (RowVersion), saat az önce doldu, SMS kodu deneme sınırı, kontör az, bulunamadı (404), yetkiniz yok (403), abonelik ödeme gecikti / salt okunur, Google Takvim bağlantısı koptu, kapora alınamaz. Hepsi `Banner` veya `EmptyState` ile kurulur.

## Tablet ve dokunmatik (son kontrol)
- **1199px ve altı:** kenar menü 72px ikon şeridine iner (etiketler gizlenir, sayı rozeti ikonun köşesine geçer), arama kutusu 220px olur.
- **899px ve altı (tablet dikey):** bento 2 sütun, arama kutusu gizlenir, iki sütunlu bölümler (`ov-split`) tek sütuna iner, tablolar kutusu içinde yatay kaydırılır (en az 720px).
- **Dokunmatik (`pointer: coarse`):** `ov-btn-sm`, `ov-chip`, `ov-tab` 44px dokunma alanı alır.
- **`metin-vurgu`** yeni token: `zemin-vurgu-yumusak` üzerindeki metin ve ikon (açık `mercan-800`, koyu `mercan-300`).
- İki sütunlu sayfa bölümlerine `ov-split` sınıfını verin; tek sütuna inme kuralı buna bağlı.

## Boş durumlar
- Her liste sayfasının boş hali tuvalde dolu sayfanın hemen sağında durur.
- Yapı: kart içinde ortalanmış ikon, "Henüz … yok" başlığı, verinin nereden geleceğini anlatan bir cümle, en fazla iki eylem, gerekirse altında iki üç ipucu kutusu.
- Bento sayıları boşta `0` ya da `—` gösterir; menü sayaçları ve bildirim rozeti gizlenir.
- **Genel bakış** ilk haftada rezervasyon bağlantısını paylaşmayı öne çıkarır.
- **Hizmetler** sektör hazır listesiyle başlamayı önerir.
- **Raporlar** veri oluşana kadar boş kalır; prim yalnızca tamamlanan randevudan doğar.
- Yeni ikonlar: `share-2`, `list-checks`, `eye-off`, `scissors`, `qr-code`.

## Hareket ve geçiş
- Süreler: `--sure-anlik` 80ms (basma), `--sure-hizli` 150ms (hover, çip, anahtar, sekme), `--sure-orta` 240ms (alt sayfa, çekmece, sayfa geçişi, menü daralması), `--sure-yavas` 360ms (tam ekran modal).
- Eğriler: `--egri-standart` cubic-bezier(.2,0,0,1) yerinde değişim; `--egri-giris` cubic-bezier(0,0,0,1) ekrana giren; `--egri-cikis` cubic-bezier(.3,0,1,1) ekrandan çıkan. Çıkış girişten kısa: 180ms.
- Alt sayfa aşağıdan, çekmece sağdan kayar; modal %96'dan büyür. Sekmeler arası geçişte hareket yoktur.
- İskelet: 150ms'den kısa yüklemede gösterilmez, gösterilirse en az 300ms kalır.
- Sayılar sayarak artmaz.
- `prefers-reduced-motion` açıkken kayma ve büyüme kapanır, yalnızca 120ms opaklık kalır (tokens.css bunu kendisi yapar).
- Flutter: `Duration(milliseconds: 80/150/240/360)`, eğriler `Cubic(.2,0,0,1)`, `Cubic(0,0,0,1)`, `Cubic(.3,0,1,1)`.
