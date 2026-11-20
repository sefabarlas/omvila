OMVILA; berber, kuaför, güzellik salonu, spa ve diyetisyen gibi randevuyla çalışan işletmeler için bir randevu ve salon yönetim platformudur. İşletmeler takvimini, ekibini, hizmetlerini, müşterilerini ve tahsilatını tek yerden yönetir. Müşteriler üye olmak zorunda kalmadan birkaç adımda randevu alır. Tek şubeli bir berber de, çok şubeli bir zincir de aynı sistemi kullanır.

**Slogan:** Her randevu tek yerde.

Bu sistem markanın temelidir: logo, renk, yazı, boşluk, ses ve ton. Platforma özel kurallar (bileşenler, yoğunluk, ekran kalıpları) bu sistemi temel alan **Ürün Panel Sistemi** ve **Ürün Mobil Sistemi**'nde tanımlanır. Bu iki sistem buradaki primitif renkleri ve semantik token adlarını aynen kullanır, yalnızca ölçek ve bileşen ekler.

## Kimin için, nerede

OMVILA'nın iki kullanıcı grubu vardır. Ürünün müşterisi işletmedir; işletmenin müşterisi ürünü ücretsiz kullanır.

| Yüzey | Kimin için | Ton | Bağlı sistem |
|---|---|---|---|
| Salon paneli (web) | Salon sahibi, şube müdürü, resepsiyon | Net, yoğun, iş odaklı | Ürün Panel Sistemi |
| OMVILA Platform paneli | OMVILA ekibi (iç kullanım) | Net, yoğun | Ürün Panel Sistemi |
| Rezervasyon sayfası (omvila.com/salon-adi) | İşletmenin müşterisi | Sıcak, ferah, yönlendirici | Ürün Mobil Sistemi |
| Salon mobil uygulaması: "İşim cebimde." | İşletme çalışanları | Hızlı, durum odaklı | Ürün Mobil Sistemi |
| Müşteri mobil uygulaması: "Randevularım bir arada." | İşletmenin müşterisi | Sıcak, sakin | Ürün Mobil Sistemi |

## Ses ve ton

OMVILA sakin, güvenilir ve işini bilen bir yardımcı gibi konuşur. Kısa cümleler kurar, ne olduğunu söyler, süslemez.

- **Hitap "siz"dir**, tüm yüzeylerde. Samimi ama saygılı: "Randevunuz onaylandı.", "Ekibinizi ekleyin."
- **Önce sonuç, sonra ayrıntı.** "Randevu iptal edildi. Müşteriye SMS gönderildi."
- **Fiille başlayan eylemler.** Butonlar 1–3 kelime, cümle düzeninde, noktasız: "Randevu al", "Saat seç", "Müşteri ekle".
- **Ünlem kullanılmaz** sistem metinlerinde. "Kaydedildi", "Kaydedildi!" değil.
- **"Lütfen", "başarıyla", "kolayca", "sadece" kullanılmaz.** "Dosya yüklendi" yeterlidir.
- **Hata mesajı ne olduğunu ve ne yapılacağını söyler.** "Bu saat dolu. Başka bir saat seçin." Teknik hata metni gösterilmez.
- **Boş durumlar davettir, özür değil.** Başlık alanı adlandırır, tek satır açıklar, buton fiildir: "İlk hizmetinizi ekleyin" / "Hizmet ekle".
- **Emoji kullanılmaz** ürün arayüzünde ve resmi iletişimde.

| Yapın | Yapmayın |
|---|---|
| Randevunuz 21 Kasım Cuma 14:30 için onaylandı. | Tebrikler! Randevunuz başarıyla oluşturuldu 🎉 |
| Telefonunuza gelen 6 haneli kodu girin. | Lütfen doğrulama kodunu giriniz. |
| Kapora doğrudan salonun hesabına geçer. | Ödemeniz OMVILA tarafından tahsil edilir. |
| Randevu almak için üye olmanız gerekmez. | Hemen ücretsiz üye olun! |

## Söz verme sınırları

Markayı yazan herkes (pazarlama, ürün metni, destek) bu sınırlara uyar:

- OMVILA'yı "güzellik sektörü yazılımı" diye daraltmayın. "Randevuyla çalışan her işletme için" denir; örnekler berber, kuaför, spa, diyetisyen.
- Müşteri uygulamasını anlatırken bugünün ağırlığı randevu takibi ve paket görüntülemedir. Salon keşfi (pazaryeri) yol haritasındadır; mevcut özellik gibi anlatılmaz.
- Kapora için "OMVILA ödemeyi tahsil eder" denmez. Doğrusu: ödeme doğrudan işletmenin hesabına gider; OMVILA işletmenin parasını tutmaz.
- WhatsApp hatırlatma ilk sürümde yoktur. Hatırlatmalar SMS, bildirim ve e-posta ile gider.
- Müşteri verisi işletmeye aittir. Bir salon yalnızca kendi müşterilerini görür; bu söz her yerde aynı biçimde verilir.

## Yazım kuralları

- **Marka adı** metin içinde her zaman büyük harfle **OMVILA** yazılır. Logodaki geniş harf aralığı metinde taklit edilmez.
- **Uygulama adları:** "OMVILA İşletme" (salon uygulaması) ve "OMVILA" (müşteri uygulaması). Mağaza ikonlarında aynı isimler kullanılır.
- **Büyük/küçük harf:** Başlık, buton, sekme ve etiketler cümle düzenindedir: "Bugünün randevuları", "Bugünün Randevuları" değil.
- **Türkçe büyük harf:** `ust-etiket` gibi büyük harfli stiller `lang="tr"` altında `text-transform: uppercase` ile üretilir; böylece "i" → "İ", "ı" → "I" doğru dönüşür. Elle büyük harf yazılmaz.
- **Saat:** 24 saat, iki nokta ile: `09:30`, `14:45`. Süre: `45 dk`, `1 sa 30 dk`.
- **Tarih:** "21 Kasım Cuma"; dar alanda "21 Kas Cum". Sayısal biçim gerekiyorsa `21.11.2026`.
- **Tutar:** binlik ayırıcı nokta, kuruş virgül, sembol sonda: `1.250 ₺`, `85,50 ₺`.
- **Telefon:** `0532 123 45 67`.
- Saat ve tutarlar `rakam` stiliyle (`tabular-nums`) yazılır; listelerde rakamlar alt alta hizalanır.

## Logo

Logo iki parçadan oluşur: **takvim kadranı** amblemi ve geniş aralıklı **OMVILA** kelime markası.

Amblem dört katmanlı bir hikâye anlatır. Mercan başlık ve iki halka takvimi, içindeki kadran zamanı anlatır. Kadranın açık renkli sağ alt çeyreği dolu bir saat dilimini gösterir. İbreler birlikte bir onay işareti çizer: doğru zamanda, onaylanmış randevu. Kelime markası Plus Jakarta Sans SemiBold ile, %12 harf aralığıyla çizilmiştir ve dış hatlara çevrilmiştir.

- **Varsayılan:** `omvila-logo-yatay.svg`, açık zeminlerde (`zemin`, `zemin-yukseltilmis`, `zemin-ikincil`).
- **Koyu zemin:** `omvila-logo-yatay-ters.svg`, `zemin-marka` ve koyu tema için. Ters sürümde takvim gövdesi kum-50'ye, kadran petrol-900'e döner.
- **Sloganlı:** `omvila-logo-slogan.svg` / `-ters.svg`; açılış sayfası, sunum kapağı, basılı materyal. Ürün arayüzünde slogansız sürüm kullanılır.
- **Tek renk:** `omvila-logo-mono-petrol.svg` / `omvila-logo-mono-beyaz.svg`; tek renk baskı, kabartma, fotoğraf üstü, `marka` dolgulu alanlar.
- **Amblem tek başına:** `omvila-amblem.svg` / `omvila-amblem-ters.svg`; avatar, sosyal medya profili, kare alanlar.
- **Küçük boy:** 48px'in altında `omvila-amblem-kucuk.svg` kullanılır. Kadran ve dolu çeyrek çıkarılmıştır; mercan başlık ve onay işareti kalır. Favicon bu sürümdür.
- **Koruma alanı:** logonun her yanında en az takvim halkası deliğinin çapının üç katı boşluk bırakın.
- **En küçük boy:** yatay logo 96px (dijital) / 25mm (baskı) genişliğin altına inmez; daha dar alanda amblem kullanılır.
- **Yapmayın:** amblem ile kelime markası arasındaki oranı ve boşluğu değiştirmek, kelime markasını başka bir fontla yeniden yazmak, gerdirmek, döndürmek, gölge veya degrade eklemek, mercan başlığı başka bir renge boyamak (tek renk sürümler hariç), logoyu `vurgu` zemine koymak.

## Renk

Palet üç aileye dayanır: **petrol** (güven ve marka), **kum** (sıcak zemin) ve **mercan** (vurgu). Arayüzde her zaman semantik tokenlar kullanılır; ramp adımları (`petrol-600` gibi) yalnızca yeni bir semantik token tanımlarken kullanılır.

- **Zeminler:** sayfa `zemin`, kart `zemin-yukseltilmis`, gruplanmış alan `zemin-ikincil`. Koyu marka bloğu `zemin-marka`, üstündeki metin `metin-ters`.
- **Metin:** gövde `metin`, yardımcı `metin-ikincil`, bağlantı `metin-marka`.
- **Eylem:** ana eylem `marka` dolgusu ve üzerinde `marka-ustu`. Bir ekranda tek ana eylem olur; diğerleri ikincil butondur.
- **Vurgu:** `vurgu` (mercan) küçük dozlarda kullanılır: amblemdeki takvim başlığı, bildirim noktası, "yeni" rozeti. Büyük alanları mercanla boyamayın. Mercan dolgu üstünde metin `vurgu-ustu`'dur, beyaz değil.
- **Durumlar:** `basari`, `uyari`, `hata`, `bilgi`. Durum hiçbir zaman yalnızca renkle anlatılmaz; ikon veya kelime eşlik eder ("Onaylandı", "Gelmedi"). `hata` mercandan koyu ve daha kırmızıdır; mercan asla hata veya silme anlamında kullanılmaz.
- **Oran:** bir ekranın yaklaşık %70'i zemin ve nötr, %20'si petrol, en fazla %10'u mercan ve durum renkleri.

Her metin rengi, kullanım notunda adı geçen zeminlerde açık ve koyu temada en az 4.5:1 kontrast sağlar; `odak` ve `cizgi-guclu` en az 3:1.

## Tipografi

Tek aile kullanılır: **Plus Jakarta Sans** (değişken, 200–800). Açık lisanslıdır (SIL OFL), dosyaları `fonts/` altındadır. Türkçe karakterler (ğ, ş, ı, İ) ve ₺ işareti ikinci dosyadadır (`Plus Jakarta Sans TR`); `sans` yığını iki dosyayı birlikte çağırır, ikisini her zaman birlikte yükleyin.

- Başlıklar `display`, `baslik-1` … `baslik-4`; 600–700 ağırlık ve hafif sıkı harf aralığı.
- Gövde `govde` (16px); mobilde okunacak hiçbir metin 16px'in altına inmez. Panelde tablo hücreleri `govde-kucuk` (14px) olabilir.
- Buton, sekme ve form etiketi `etiket`.
- Meta bilgi `aciklama`; uzun metin için kullanılmaz.
- Saat ve tutarlar `rakam`.
- Satır uzunluğu en fazla 72 karakter.

## Boşluk, köşe, gölge

- 4px tabanlı ölçek: `bosluk-1` (4) … `bosluk-20` (80). Değerler ölçek dışına çıkmaz.
- Köşeler yumuşaktır ama yuvarlak değildir: saat dilimi ve giriş alanı `kose-sm`, buton ve panel kartı `kose-md`, mobil kart ve modal `kose-lg`. Hap ve avatar `kose-tam`.
- Gölgeler petrol tonludur ve az kullanılır. Kartlar önce `cizgi` ile ayrılır; gölge yalnızca yüzen öğelerde (menü, modal, sürüklenen kart) kullanılır.

## İkonografi

- **Lucide** ikon seti kullanılır: çizgi ikon, 24px ızgara, 1.75 çizgi kalınlığı, yuvarlak uçlar. Plus Jakarta Sans'ın yuvarlak formlarıyla uyumludur.
- İkonlar metin rengini alır (`currentColor`); dolu ikon yalnızca seçili sekmede kullanılır.
- Arayüzde emoji ikon yerine kullanılmaz.
- Sektöre özel ikonlar (makas, jilet, havlu) yalnızca işletmenin kendi hizmet listesinde, işletmenin seçimiyle görünür; marka iletişiminde tek bir sektörü öne çıkaran ikon kullanılmaz.

## Görsel dil

- **Fotoğraf:** gerçek işletmeler, gerçek ekipler, doğal ışık. Berber, kuaför, spa, diyetisyen dengeli temsil edilir. Abartılı makyaj, stok "mutlu kadın" pozları ve aşırı rötuş kullanılmaz.
- **Biçim:** düz renk blokları ve `kose-xl` köşeli kırpılmış fotoğraflar. Degrade, cam efekti ve neon kullanılmaz.
- **Motif:** takvimdeki saat dilimlerinden gelen yuvarlatılmış kısa bloklar ve amblemdeki dolu çeyrek. Kapaklarda ve kampanya görsellerinde dekoratif öğe olarak kullanılabilir.

## Erişilebilirlik

- Klavye odağı: 2px `zemin` boşluk + 2px düz `odak` halkası. Odak hiçbir zaman gizlenmez.
- Dokunma hedefleri mobilde en az 44×44px.
- Renk tek başına bilgi taşımaz; durumlarda ikon ve metin birlikte kullanılır.
- Açık ve koyu tema eşit desteklenir; koyu temada `marka` ve `vurgu` açıklaşır, üstlerindeki metin koyulaşır (`marka-ustu`, `vurgu-ustu`).
