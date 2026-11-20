# OMVILA Tasarım Teslim Notları

20 Kasım 2026 · Sefa Barlas

## Kaynaklar

Tasarımın tamamı iki tasarım sistemi ve sekiz tuvalde. Her tuval akış sırasına göre satırlara ayrılır; koyu tema kopyaları satırın sağında durur. Boş ve özel durumlar, ait oldukları dolu ekranın hemen sağındadır. (Bu pakette: `01-tasarim-sistemleri`, `02-ekran-goruntuleri`, `03-tasarim-kaynaklari`.)

| Yüzey | Klasör | İçerik |
| --- | --- | --- |
| Mobil tasarım sistemi | mobil | İşletme, müşteri, rezervasyon; Flutter teması |
| Web panel tasarım sistemi | panel | Salon ve platform paneli |
| Salon personeli uygulaması | isletme-uygulamasi | 6 akış satırı |
| Müşteri uygulaması | musteri-uygulamasi | 8 akış satırı |
| Herkese açık rezervasyon | rezervasyon-sayfasi | Seçim, doğrulama, SMS bağlantısından açılanlar |
| Salon paneli (web) | salon-paneli | 15 akış satırı, tablet görünümü dahil |
| İç ekip paneli | platform-paneli | Markalar, onaylar, abonelik, denetim, planlar |
| Bildirimler ve dağıtım | bildirim-dagitim | SMS, push, e-posta, ikon, mağaza, QR, hareket kuralları |
| Tanıtım sitesi | tanitim-sitesi | Ana sayfa ve fiyatlar, masaüstü ve mobil |

## Ortak kurallar

Renkler yalnızca semantik tokenlarla verilir; koyu tema `data-theme="dark"` ile aynı tokenların koyu değerlerini kullanır. Ham palet (`mercan-800` gibi) doğrudan kullanılmaz.

- **Randevu durumları:** Draft, PendingApproval, Confirmed, CheckedIn, Completed, Cancelled, NoShow. "Kapora bekleniyor" ayrı bir durum değil, türetilmiş gösterimdir.
- **Onay süresi:** varsayılan 120 dakika; dolarsa talep otomatik iptal olur. Arayüzde "reddedildi" değil "iptal" yazar.
- **İptal ve saat değişikliği:** ayrı son süreleri vardır (örnekte 24 ve 12 saat). Müşteri iptalde neden seçmek zorundadır.
- **Bekleme listesi:** açılan saat 15 dakika tutulur, sonra sıradakine geçer.
- **Kapora (havale):** müşteriye IBAN, alıcı adı, tutar ve açıklama kodu (OMV-…) gösterilir, alanlar kopyalanabilir. Havale süresi varsayılan 2 saat (salon 1, 2 veya 4 saat seçer); süre dolar ve salon işaretlemezse randevu otomatik iptal olur. Salon IBAN tanımlamadıysa hizmetlere kapora konamaz.
- **Abonelik (salonun OMVILA'ya ödemesi):** kartla otomatik çekim ya da havale/EFT. Havale açıklamasında abonelik kodu (ABN-…) yazılır; platform panelinde banka hareketiyle eşleştirilir.
- **Prim:** randevu Completed olduğunda doğar.
- **Gizlilik:** telefon maskesi yalnızca Personel rolünde; doğum tarihi yerine yalnızca doğum yılı; not alanlarında sağlık bilgisi yazılmaz.
- **Giriş:** salon tarafında yalnızca telefon + SMS kodu.

**Söz verme sınırları**

- OMVILA ödeme almaz ve uygulamada kartla ödeme yoktur. Kaporayı müşteri salonun IBAN'ına havale eder, salon "Kapora geldi" ile işaretler; iadeyi salon yapar.
- Uygulamada salon keşfi yoktur; müşteri salonu bağlantı veya QR ile ekler.
- v1'de WhatsApp yoktur; kanallar SMS, push ve e-posta.
- Sağlık verisi tutulmaz.

## Yüzey bazında akışlar

| Yüzey | Akış satırları |
| --- | --- |
| Müşteri uygulaması | Açılış ve giriş (açılış, giriş, SMS kodu, bildirim izni) · Ana sekmeler · Randevu alma (kapora havale ile) · Bekleme listesi · Randevu yönetimi (saat değiştir, iptal) · Geçmiş ve değerlendirme · Profil, hesap ve hesap silme · Durum desenleri |
| İşletme uygulaması | Açılış ve giriş · Ana sekmeler (Bugün, Takvim, Müşteriler, Bildirimler, Profil) · Randevu (detay, işlem, kapora geldi mi, ödeme, yeni randevu) · Kapıda sıra · Müşteri kartı · Ayarlar |
| Rezervasyon sayfası | Seçimler (şube, hizmet, personel, saat, dolu gün) · Doğrulama ve sonuç (havale bilgisi dahil) · SMS bağlantısından randevu yönetimi · SMS bağlantısından bekleme teklifi · Masaüstü · Durum desenleri |
| Salon paneli | Giriş ve kayıt · Genel bakış · Takvim ve randevu · Kapıda sıra · Müşteriler · Bekleme listesi · Kasa · Değerlendirmeler · Hizmetler · Personel · Paketler · Raporlar ve prim · Ayarlar (10 alt sayfa, kapora ve IBAN dahil) · Tablet görünümü · Durum desenleri |
| Platform paneli | Genel bakış · Markalar ve marka detayı · Onay kuyrukları ve havale eşleştirme · Abonelikler · Denetim kaydı ve planlar |

**Rol farkı:** Personel rolüyle panel yalnızca Takvimim, Müşteriler, Kapıda sıra ve Primim menüsünü görür; kasa, rapor, hizmet ve ayarlar kapalıdır; onay bekleyen talepleri yalnızca yönetici ve işletme sahibi onaylar.

**Platform paneli:** Destek rolü görüntüler; kontör, plan ve durum değişikliği yönetici yetkisi ister. Her görüntüleme denetim kaydına yazılır.

## Boş, özel ve hata durumları

Listesi olan her ekranın boş hali vardır ve dolu ekranın hemen sağında durur. Boş ekranda sahte sayı, menü sayacı veya bildirim rozeti gösterilmez.

| Durum | Nerede | Kural |
| --- | --- | --- |
| İlk giriş (boş) | Müşteri Randevularım, Randevu al | İlk salon QR veya bağlantı ile eklenir; "Nasıl çalışır" gösterilir |
| Yaklaşan randevu yok | Müşteri Randevularım | Boş ekran yerine "Aynısını tekrar al" |
| Liste boş | Paketler, Bildirimler, Geçmiş, Ödemeler, Müşteriler, Sıra, Bekleme, Kasa, Değerlendirmeler, Hizmetler, Personel, Raporlar | Ne zaman dolacağı + tek eylem |
| İlk hafta | Salon paneli Genel bakış | Rezervasyon bağlantısını paylaşma öne çıkar |
| Arama sonucu yok | Panel ve işletme Müşteriler | Açık filtre söylenir; "Filtreyi temizle" ve "Yeni müşteri olarak ekle" |
| Boş gün / kapalı gün | Panel ve işletme Takvim | Kapalı günde randevu ekleme pasif; "Yalnızca bu gün için aç" panelden |
| Bildirimler kapalı | Müşteri ve işletme ana ekran | Kapatılabilir şerit; müşteride hatırlatmalar SMS ile gelir |
| Ödeme gecikti | Salon paneli Genel bakış | Turuncu şerit, "Kartı güncelle" veya "Havale ile öde" |
| Salt okunur | Panel Takvim, işletme Bugün | Kırmızı şerit; yeni randevu, online rezervasyon ve SMS kapalı, veriler durur |
| Kapora gelmedi | Müşteri ve rezervasyon durum desenleri | Havale süresi dolunca randevu iptal; havale yapıldıysa salonu arama önerisi |
| Yeni müşteri | İşletme Müşteri kartı | Geçmiş yok, ilk ziyaret öne çıkar |
| Hesap silme | Müşteri İzinlerim | Yaklaşan randevular iptal edilir; SMS koduyla onay |

Genel hata desenleri (çevrimdışı, 404/403, aynı anda düzenleme çakışması, saat dolu, kod deneme sınırı, abonelik durumları) her tuvalin "Durum desenleri" panosundadır.

## Bildirim içerikleri

Kanal sırası: uygulaması olana önce push, ulaşmazsa SMS. Doğrulama kodu ve kapora mesajları her zaman SMS ile gider.

| Kanal | Kural | Örnekler |
| --- | --- | --- |
| SMS | Salon adıyla başlar, tek bağlantı; Türkçe karakterle en fazla 155 karakter = 1 kontör | 15 olay: kod, onay, talep, kapora bekleniyor / alındı / gelmedi, hatırlatma, saat değişti, iptal, bekleme teklifi, sıra, değerlendirme |
| Push | Başlık ≤ 40, metin ≤ 90 karakter; kilit ekranında ad + soyadın baş harfi; sessiz saatler 22:00–08:00 | Müşteri 6, işletme 6 (havale bildirildi dahil), salon sahibi 2 (kontör azaldı, ödeme alınamadı) |
| E-posta | Müşteriye gönderen salon adı ("OMVILA, X adına"); işletmeye gönderen OMVILA | Randevu onayı, deneme bitiyor, kontör azaldı, abonelik ödemesi alınamadı |

Kapora geçen her metinde havalenin salonun IBAN'ına yapılacağı, tutar, süre ve açıklama kodu yazılır; "Kapora bekleniyor" SMS'i 1 kontörü aşmayacak şekilde kısa tutulur (133 karakter). Şablonların tamamı bildirim-dagitim klasöründedir.

## Tablet, erişilebilirlik ve hareket

**Tablet (salon paneli):** 1199 px ve altında kenar menü 72 px ikon şeridine iner. 899 px ve altında bento 2 sütuna, `ov-split` bölümleri tek sütuna iner; tablolar kutusunda yatay kaydırılır (en az 720 px).

**Erişilebilirlik:**

- Tüm metin/zemin çiftleri iki temada da 4.5:1 üstünde. İstisna devre dışı metin; WCAG muaf tutar.
- `zemin-vurgu-yumusak` üzerindeki metin için `metin-vurgu` kullanılır (açık `mercan-800`, koyu `mercan-300`).
- Mobilde küçük buton ve çipler 36 px görünür, 44 px dokunma alanı alır. Panelde aynı kural dokunmatik ekranda (`pointer: coarse`) geçerli.

**Hareket:**

| Token | Değer | Nerede |
| --- | --- | --- |
| `--sure-anlik` | 80 ms | Basma geri bildirimi |
| `--sure-hizli` | 150 ms | Hover, çip, anahtar, sekme, bildirim şeridi |
| `--sure-orta` | 240 ms | Alt sayfa, çekmece, mobil sayfa geçişi, menü daralması |
| `--sure-yavas` | 360 ms | Tam ekran modal, başarı ekranı |

Ekrana giren `--egri-giris`, çıkan `--egri-cikis` (180 ms), yerinde değişen `--egri-standart` kullanır. 150 ms'den kısa yüklemede iskelet gösterilmez. Sayılar sayarak artmaz. Azaltılmış hareket açıkken yalnızca 120 ms opaklık kalır.

## Tanıtım sitesi

Ana sayfa ve Fiyatlar; masaüstü, mobil ve ikisinin koyu teması. Sayfalar akışkan: 900 px altında tek sütuna iner, üst menü yerine menü butonu çıkar.

- Görseller gerçek ürün ekranlarının görüntüleridir; koyu temada koyu ekranlar kullanılır.
- Uydurma sayı, müşteri yorumu veya marka logosu yoktur.
- Menüde Özellikler ve Nasıl çalışır aynı sayfada bölüme kaydırır.
- "Giriş yap" ve "Ücretsiz dene" salon panelinin giriş ve kayıt ekranlarına gidecek; tasarımda bağlı değil.
- Alt bilgideki KVKK, kullanım koşulları ve iletişim sayfaları henüz tasarlanmadı.

## Yer tutucular ve açık kalanlar

Tasarımda köşeli parantezle duran değerler ürün kararı bekliyor; karar gelince tüm ekranlarda birlikte değiştirilecek.

| Konu | Şu an | Gereken |
| --- | --- | --- |
| Aylık plan fiyatları | [AYLIK FİYAT] | Fiyat kararı |
| SMS kontör paketleri | [FİYAT] | Fiyat kararı |
| Plan sınırları | [PLAN SINIRI] | Personel ve şube sayıları |
| Deneme kontörü | [DENEME KONTÖRÜ] | Kaç SMS verileceği |
| KDV notu | [KDV NOTU] | Dahil mi, hariç mi |
| OMVILA banka bilgisi (abonelik havalesi) | [OMVILA ŞİRKET UNVANI], [OMVILA IBAN] | Gerçek hesap bilgisi |
| Kapora havale süresi | Varsayılan 2 saat (1 / 2 / 4 saat seçilebilir) | Onay |
| Hatırlatma zamanları | 24 saat ve 2 saat önce | Onay |
| Sessiz saatler | 22:00–08:00 | Onay |
| Değerlendirme mesajı | Randevudan 2 saat sonra | Onay |
| Sitede puan eşiği | En az 5 değerlendirme | Onay |
| Alan adı | omvila.com (örnek) | Onay |

- [ ] Sitedeki SSS, hesap silme metni ve kapora/iade cümleleri için hukuk kontrolü
- [ ] Tanıtım sitesinin yasal sayfaları (KVKK, kullanım koşulları, iletişim)
- [ ] 3–4 salon sahibi ve birkaç müşteriyle prototip testi
- [ ] Backend açık sorular notu
