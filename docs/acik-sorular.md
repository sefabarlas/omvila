# Açık sorular ve alınan varsayımlar

Her satır ya **karar bekliyor** ya da **varsayımla ilerlendi** durumundadır. Varsayımla ilerlenen konu kodda ilgili ayar veya tek bir sınıf arkasında durur; değişirse tek yer değişir. Bir ajan bu listeye dokunan bir işe başlarsa varsayımı uygular. Varsayımı değiştirmez; gerekirse kullanıcıya sorar.

Durum: 🟡 varsayımla ilerlendi · 🔴 karar bekliyor (iş başlamadan) · ✅ kapandı

## Ürün

| # | Soru | Varsayım / durum | Etkilediği dilim |
|---|---|---|---|
| S-01 | Kapora ve onay birlikte gerekiyorsa tek süre mi? | 🟡 Tek tutma süresi `TransferWindowMinutes`; "Kapora geldi" onay sayılır | 4 |
| S-02 | Kapora istenmiş talebi salon kapora gelmeden onaylayabilir mi? | 🟡 Evet, açık `waiveDeposit = true` ile ("Kaporasız onayla") | 4 |
| S-03 | Havale ekranından sonra müşteriye ne gösterilir? Tasarımda `YRBasari` / `Basari` "Randevunuz onaylandı" diyor, `Havale` ise "Salon havaleyi görünce onaylar" diyor | 🟡 "Havaleyi yaptım"dan sonra "Salon onayı bekleniyor" (Talep ekranı deseni). "Randevunuz onaylandı" yalnız gerçekten `Confirmed` olunca. Tasarımda düzeltilmeli | 4 |
| S-04 | Salon müşteri adına iptal veya erteleme yaparken son süreye tabi mi? | 🟡 Hayır, salon sınırsız | 2 |
| S-05 | Salt okunur abonelikte salon elle randevu ekleyebilir mi? Eski kural evet, tasarım hayır | 🟡 Tasarım: hayır | 6 |
| S-06 | Online akışta çok hizmetli randevuda hizmet başına farklı personel | ✅ Evet ("İki hizmet de aynı kişiden" anahtarı kapalıyken) | 2 |
| S-07 | Personel kendi randevusunu iptal edebilir mi, kapora işaretleyebilir mi? | 🟡 v1'de hayır | 3 |
| S-08 | Panelden eklenen randevuya kapora istenebilir mi? | 🟡 v1'de hayır | 4 |
| S-09 | "Gelmedi" olan randevunun paket seansı geri döner mi? | 🟡 Hayır (seans yanar); salon elle geri verebilir (v2) | 5 |
| S-10 | Kısmi kullanılmış paketin iade hesabı | 🔴 Karar yok. v1'de iade tutarını salon elle girer | 5 |
| S-11 | Müşteri saat değiştirince salon onaylı çalışıyorsa yeniden onaya düşer mi? | ✅ Evet (RandevuErtele ekranı) | 3 |
| S-12 | Tekrarlayan randevuda çakışan tekrar | 🟡 Atlanır, salona liste gösterilir | 2 |
| S-13 | Hatırlatma, sessiz saatler, değerlendirme zamanı, havale süresi, iptal/erteleme süreleri | 🟡 Teslim notlarındaki örnek değerler ("Onay" bekleyenler) | 3 |
| S-14 | Yer tutucular: fiyat, plan sınırı, deneme kontörü, KDV, OMVILA banka, alan adı | 🔴 Değer yok; veriden/yapılandırmadan okunur, kodda yazılmaz. Dilim 1 değer beklemeden ilerler: geliştirmede sınırsız plan seed'i ve yapılandırmadan deneme kontörü (kullanıcı onayı 2026-11-20) | 1, 6 |
| S-15 | Değerlendirmeler otomatik mi yayınlanır? | 🟡 Evet; salon gizleyemez, yalnız yanıtlar (v1) | 5 |
| S-16 | Kullanıcı birden çok markaya üye olabilir mi? | 🟡 v1'de hayır | 1 |
| S-17 | "Kapıda sıra" SMS'inde "Lütfen" geçiyor; ses-tonu kuralı yasaklıyor | 🟡 "Salona yakın olun." kullanılır; tasarım düzeltilmeli | 5 |
| S-18 | Panelde onay bekleyen talepte "Reddet" butonu var; müşteri tarafında hiçbir zaman "reddedildi" denmez | ✅ `Cancelled` + `DeclinedBySalon`; müşteriye "iptal" metni | 3 |
| S-19 | SMS bağlantı kodu tasarımda 4 karakter (`7Kx2`) | 🟡 Güvenlik için 8 karakter; SMS uzunluk testleri buna göre | 3 |
| S-20 | Panel menüsünde henüz yapılmamış bölümler (Takvim, Müşteriler, Bekleme listesi, Kasa, Değerlendirmeler, Paketler, Raporlar) gösterilsin mi? Tasarımda yeni hesapta da rozet var (4, 2, zil 3) | 🟡 Bölüm, ait olduğu dilimde açılana kadar menüde gizlenir. Rozet yalnız gerçek sayıyla gösterilir; tasarımdaki yeni hesap rozetleri düzeltilmeli | 1 |
| S-21 | Kayıt ekranında ad soyad alanı yok; KayitKurulum "Hoş geldiniz, Elif" diyor | 🟡 Kayıtta ad sorulmaz (tasarıma uyulur); `ownerFullName` boş kalır, selamlama "Hoş geldiniz" olur. Ad, personel kaydından düzenlenir | 1 |
| S-22 | Randevu kuralları ekranındaki kapora bölümü Dilim 1'de nasıl görünür? | 🟡 "Kapora istenemiyor · IBAN tanımlı değil" deseniyle pasif gösterilir; IBAN ayarı Dilim 4 | 1 |
| S-23 | Personel kartlarındaki doluluk, "bu hafta" ve Google Takvim kartı | 🟡 Doluluk ve "bu hafta" Dilim 2'ye kadar "—"; Google Takvim kartı Dilim 6'ya kadar gizli | 1 |
| S-24 | Hazır listeden eklenen hizmetin fiyatı (şablonda fiyat yok, hizmette zorunlu) | 🟡 Hazır liste seçiminde fiyat zorunlu girilir; sıfır fiyatla hizmet oluşturulmaz | 1 |
| S-25 | "Resmi tatiller otomatik eklenir" (AyarlarSube) | 🟡 Tatiller `PublicHoliday` seed'inden öneri olarak gelir; şube kabul edince `WorkingHoursOverride` oluşur, tatil kendiliğinden kapatmaz | 1 |
| S-26 | Şube ekle bilgi kutusu ("Aboneliğinize 1 ek şube eklenir") plan verisi ister | 🟡 Dilim 1'de fiyat ve sayı içermeyen sabit bilgi metni; plan verisi Dilim 6'da | 1 |
| S-27 | Personel takvim rengi: sözleşme hex saklıyor, arayüzde sabit hex yasak | 🟡 Seçenekler token paletinden üretilir; hex yalnız veri olarak saklanır, bileşen kodunda hex geçmez | 1 |

## Teknik

| # | Soru | Varsayım / durum |
|---|---|---|
| T-01 | Veritabanı | ✅ PostgreSQL 17 (kullanıcının teknik kararı; ADR-0001) |
| T-02 | Web çatısı | ✅ Paneller React + Vite SPA; rezervasyon ve tanıtım Next.js (ADR-0008) |
| T-03 | SMS sağlayıcısı | 🔴 Seçilmedi. Kriterler: başlık (originator) onay süreci, İYS entegrasyonu, Türkçe karakter ve 155 sınırı, teslim raporu webhook'u, fiyat. Adaylar: Netgsm, İleti Merkezi, Mutlucell, Verimor. `ISmsSender` arkasında; geliştirmede sahte sağlayıcı |
| T-04 | Push | 🟡 Firebase Cloud Messaging (iOS için APNs FCM üzerinden) |
| T-05 | E-posta | 🟡 Amazon SES veya Postmark; `IEmailSender` arkasında |
| T-06 | Abonelik kart çekimi sağlayıcısı | 🔴 Seçilmedi (adaylar: iyzico abonelik API'si, Param). Dilim 6'ya kadar gerekmiyor |
| T-07 | Barındırma | 🔴 Seçilmedi. KVKK açısından yurt içi veri merkezi değerlendirilmeli (açık rıza olmadan yurt dışı aktarım riski) |
| T-08 | Dosya depolama (logo, dekont) | 🟡 S3 uyumlu; `IFileStorage` arkasında |
| T-09 | İYS entegrasyonu | 🔴 SMS sağlayıcısıyla birlikte; kampanya gönderimi dilim 6 sonrası |
| T-10 | Hata izleme / gözlemlenebilirlik | 🟡 OpenTelemetry + Serilog; hedef (Sentry, Grafana) barındırmayla birlikte |
| T-11 | Bağlantı adı (slug) çakışır veya ayrılmış ad ise hata kodu (D1) | ✅ Yeni `slug_unavailable` kodu, 409 (eklemedir, kırıcı değil). Başka işletmenin yönlendirmesi süren eski bağlantısı da kullanılamaz; işletme kendi eski adına dönebilir. Kullanıcı onayı 2026-11-20 |
| T-12 | Logo yükleme ucu sözleşmede yok (AyarlarIsletme "Logo yükle") (D1) | ✅ Dilim 1'de buton gizli; uç sonraki bir dilimde önce `yol-haritasi.md`'ye eklenir. Kullanıcı onayı 2026-11-20 |
| T-13 | Şubeler arası geçiş süresi tasarımda şubeye özel, sözleşmede yalnız marka ayarı (D1) | ✅ Şubeye alan eklenmez: alan modelinde süre şube çifti için (`BranchTransferTime(From, To)`), şubeye tek değer belirsiz kalır. AyarlarSube'deki seçim `BrandSettings.defaultBranchTransferMinutes`'ı okur ve yazar. Çift bazında ayar gerekirse ayrı uç olarak eklenir. Kullanıcı kararı 2026-11-20 (ilk onaylanan şube alanının yerine) |
| T-14 | "Oda veya cihaz" (ResourceType) uçları sözleşmede yok (HizmetEkle) (D1) | ✅ Dilim 1'de alan gizli; kaynak yönetimi ayrı bir dilimde. Kullanıcı onayı 2026-11-20 |
| T-15 | `publicGetBrand` Dilim 2 ucu, ama Dilim 1 kabul kriteri 16 eski bağlantının 301 dönmesini istiyor | 🟡 Dilim 1'de uç yalnız slug çözümlemesini yapar (301 / 404); 200 gövdesi Dilim 2'de dolar |
| T-16 | Personelin iki şubedeki saatleri çakışırsa hata kodu | 🟡 Yeni kod yok: `validation_failed` + alan hatası |
| T-17 | Yabancı telefon numaraları (turist, yurt dışı müşteri) | 🟡 v1'de yalnız +90 numaraları kabul edilir (`PhoneNumber`); SMS maliyeti ve doğrulama Türkiye için. Gerekirse genel E.164 desteği eklenir |
| T-18 | Sözleşmede genel sunucu hatası kodu yok; 500'de hangi `code` döner? | 🟡 Çerçevenin ürettiği 5xx yanıtları `try_again` kodunu taşır (istemci bir kez tekrar dener). Ayrı bir `internal_error` kodu gerekirse sözleşmeye eklenir |
| T-19 | ADR-0007: rol jetona yazılmaz gibi sunucudan çözülür, değişiklik 60 sn önbellekle etkili olur. Üyelik tablosu henüz yok | 🟡 PR-6a'da rol jetondaki `role` iddiasından çözülür; değişiklik jeton yenilenince (en geç 15 dk) etkili olur. Üyelik tablosu (PR-12) gelince rol ve üyelik durumu veritabanından 60 sn önbellekle okunur |
| T-20 | Panelde iki sekme aynı yenileme jetonuyla aynı anda yenilerse ADR-0007'deki katı kural (tekrar kullanım → aile iptali) kullanıcıyı çıkarır | 🟡 ADR olduğu gibi uygulandı (test: ikinci yenileme zinciri düşürür). Panel, sekmeler arası tek yenileme yapacak (BroadcastChannel / Web Locks). Sorun görülürse kısa bir tolerans penceresi ayrı kararla eklenir |

## Hukuk (kod beklemez, metin bekler)
- H-01 Randevu hatırlatması işlem mesajı mı (İYS dışı)? Varsayım: evet.
- H-02 KVKK veri sorumlusu / veri işleyen ayrımı (salon sorumlu, OMVILA işleyen varsayımı).
- H-03 Saklama süreleri (işlem kayıtları 3 yıl, denetim 2 yıl).
- H-04 SSS, hesap silme ve kapora/iade metinleri.
- H-05 Tanıtım sitesi yasal sayfaları.
