# OMVILA karar günlüğü

Eski hesaptaki tasarım sohbetinde alınan kararlar, gerekçeleri ve çalışma tercihleri. Teslim notları "ne"yi anlatır; bu dosya "neden"i ve "nasıl çalışıyoruz"u anlatır. Yeni oturum bu dosyayı teslim notlarıyla birlikte okumalı.

## Ürün kararları

| Karar | Gerekçe / not |
|---|---|
| **Online ödeme yok.** Kapora müşteriden salonun IBAN'ına havale ile alınır; uygulama kart bilgisi istemez, ödeme almaz. | Sektörde online ödeme az kullanılıyor; müşteriler IBAN'a havale ya da nakit ödüyor (Kasım 2026 kararı). Daha önce tasarımda iyzico alt üye işyeri / kartla kapora vardı; tamamen çıkarıldı. |
| Kapora akışı: randevu oluşur → müşteriye IBAN, alıcı, tutar, açıklama kodu (OMV-xxxx) gösterilir → müşteri "Havaleyi yaptım" der → salon "Kapora geldi" ile işaretler. Süre dolarsa otomatik iptal. | Banka entegrasyonu yok; eşleştirmeyi salon elle yapar. Açıklama kodu eşleştirmeyi kolaylaştırır. |
| Havale süresi varsayılan **2 saat**, salon 1 / 2 / 4 saat seçebilir. | Tasarımda varsayım olarak konuldu; kullanıcı onayı bekliyor. |
| **Abonelik** (salon → OMVILA): kartla otomatik çekim **ve** havale/EFT seçeneği (açıklama ABN-…). Havale platform panelinde banka hareketiyle elle eşleştirilir. | Kullanıcı "Havale/EFT seçeneği de ekle" dedi; kart kaldırılmadı, yanına eklendi. |
| "Kartla, salonda" ödemesi kalabilir. | Salonun kendi POS'u; OMVILA üzerinden ödeme değil. |
| Randevu durumları: Draft, PendingApproval, Confirmed, CheckedIn, Completed, Cancelled, NoShow. "Kapora bekleniyor" ayrı durum değil, türetilmiş gösterim. | Backend durum makinesiyle uyum. |
| Onay süresi 120 dk; dolarsa "iptal" (asla "reddedildi" denmez). | Müşteriye suçlayıcı olmayan dil. |
| Müşteri iptalde neden seçmek zorunda; iptal ve saat değişikliğinin ayrı son süreleri var. | |
| Bekleme listesinde açılan saat 15 dk tutulur. | |
| Salon keşfi yok; müşteri salonu bağlantı veya QR ile ekler. | Ürün pazar yeri değil, salonun kendi aracı. |
| v1'de WhatsApp yok. Kanallar SMS, push, e-posta. | |
| Sağlık verisi tutulmaz; doğum tarihi yerine yalnız doğum yılı. Telefon maskesi yalnız Personel rolünde. | KVKK riski. |
| Prim, randevu Completed olunca doğar. | |

## Tasarım kararları

- Her ekranın açık ve koyu ("Koyu") hali var; koyu kopyalar tuvalde sağda durur.
- Tuvaller **akışa göre satırlara** dizilir (salon panelinde başlayan düzen tüm tuvallere uygulandı). Her satırın üstünde başlık notu var.
- Listesi olan her ekranın **boş hali** var ve dolu ekranın hemen sağında durur (ör. müşteri ana ekranı ilk giriş, paketlerim boş, kapalı gün). Boş ekranda sahte sayı, sayaç veya rozet yok.
- Renkler yalnız semantik tokenlarla; ham palet kullanılmaz. `zemin-vurgu-yumusak` üzerindeki metin `metin-vurgu` tokenını kullanır (koyu temadaki kontrast hatası bu yüzden eklendi).
- Hareket tokenları (`--sure-*`, `--egri-*`) ve azaltılmış hareket kuralı tasarım sistemlerinde.
- Mobilde küçük buton ve çiplerin dokunma alanı 44 px; panelde dokunmatik ekranda aynı kural. Panelin tablet kırılımları 1199 ve 899 px.
- Tanıtım sitesinde **uydurma iddia yok**: sahte sayı, müşteri yorumu, marka logosu kullanılmaz. "En çok seçilen" gibi kanıtsız ifadeler yerine "Önerilen" kullanıldı. KDV gibi bilinmeyen bilgiler köşeli parantezli yer tutucu.
- Sitenin menüsündeki Özellikler / Nasıl çalışır aynı sayfada bölüme kaydırır.
- Bilinmeyen değerler her yerde aynı yer tutucuyla yazılır: [AYLIK FİYAT], [FİYAT], [PLAN SINIRI], [DENEME KONTÖRÜ], [KDV NOTU], [OMVILA ŞİRKET UNVANI], [OMVILA IBAN]. Değer gelince hepsi birlikte değiştirilir.

## Yazılım kararları (Kasım 2026)

- Eski backend bırakıldı; **.NET ile sıfırdan** yazılacak.
- Mobil uygulamalar Flutter. Web panellerin çatısı henüz seçilmedi.
- Kodun tamamı yapay zekâ ajanlarıyla yazılacak; bu yüzden önce kurallar, CLAUDE.md / AGENTS.md dosyaları, alan modeli ve OpenAPI sözleşmesi hazırlanacak.
- Önerilen yaklaşım: sözleşme önce (contracts/openapi.yaml tek doğru kaynak), modüler monolit + PostgreSQL + zamanlanmış iş kuyruğu, monorepo, dikey dilimlerle ilerleme (bkz. YAZILIM-OTURUMU-ILK-MESAJ.md).
- Tasarım ve yazılım ayrı oturumlarda yürür.

## Çalışma tercihleri

- İletişim Türkçe.
- Kullanıcı gece boyu bırakılan işlerde Claude'un kendi kararıyla ilerlemesini, varsayımları sonunda bildirmesini istiyor; geri dönüşü zor kararlarda sorulur.
- İş bitince son kontrol yapılır (ekranlar render edilip gözle bakılır, taşma ve kontrast kontrol edilir).
- Yayınlarken yalnız değişen dosyalar gönderilir; önce sürüm kontrol edilir ki kullanıcının tuvalde yaptığı düzenlemeler ezilmesin.
- Bir karar değişince (ör. online ödemenin kaldırılması) tüm yüzeyler birlikte güncellenir: uygulamalar, paneller, bildirim şablonları, site, tasarım sistemi README'leri ve teslim notları.

## Bekleyen işler

- Yer tutucuların gerçek değerleri.
- Tanıtım sitesinin yasal sayfaları (KVKK, kullanım koşulları, iletişim).
- SSS, hesap silme ve kapora/iade metinleri için hukuk kontrolü.
- 3–4 salon sahibi ve birkaç müşteriyle prototip testi.
- Backend açık sorular notu (yazılım oturumunda alan modeliyle birlikte ele alınacak).
