# Dilim 3: uçtan uca akış 3 ve tasarım karşılaştırması

Dilim 3 ekranlarının tasarımla karşılaştırma kaydı ve kritik akış 3'ün (test-stratejisi.md) uçtan uca testleri. Yöntem Dilim 2'deki gibi ([dilim-02-tasarim-karsilastirma.md](dilim-02-tasarim-karsilastirma.md)): ekran görüntüleri Playwright ile alınır, `design/ekranlar/` altındaki görüntüyle yan yana gözle karşılaştırılır; otomatik piksel karşılaştırması yok.

- Salon paneli: `pnpm --filter @omvila/salon-panel e2e` → `web/salon-panel/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`, `e2e/onay.e2e.ts`, `e2e/bildirimler.e2e.ts`). Masaüstü 1440×900. Saat sabit: Çarşamba 25 Kasım 2026 12:10 İstanbul.
- Rezervasyon sayfası: `pnpm --filter @omvila/rezervasyon e2e` → `web/rezervasyon/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`, `e2e/randevum.e2e.ts`). Telefon 390×844. Saat sabit: Salı 24 Kasım 2026 10:00.
- CI: `e2e` işi iki paketi de çalıştırır; görüntüler yapıt olarak saklanır.

## Kritik akış 3

"Aynı akış onaylı salonda → Talebiniz alındı → panelde onay → müşteri SMS'i (sahte sağlayıcı günlüğü)". Uygulamalar ayrı Playwright paketlerinde ve Prism mock'a karşı çalıştığı için akış üç yerde sınanır:

| Parça | Test | Kapsadığı |
|---|---|---|
| Müşteri: talep ve durum | `web/rezervasyon/e2e/onay.e2e.ts` | Hizmet → personel → saat → telefon → kod → onay → "Talebiniz alındı" (kriter 1); SMS bağlantısında "Onay bekliyor" ve kalan süre; salon onaylayınca "Onaylandı" (kriter 2); reddedince "İptal", "Talebiniz iptal edildi", "onaylayamadı", sayfada "reddedil…" geçmez (kriter 3); saat 121 dk ileri alınınca "zamanında yanıtlayamadı" ile "İptal" (kriter 4). |
| Salon: karar | `web/salon-panel/e2e/onay.e2e.ts` | Genel bakış "Dikkat isteyenler" ve bento ("ilki 1 sa 35 dk içinde iptal olur") → "İncele" → yan panel ("1 sa 35 dk içinde yanıtlayın.", müşteri notu); Onayla (If-Match ile, kuyruk boşalır; kriter 2); Reddet (talep takvimden düşer; kriter 3); süre dolumu: saat tutmanın ötesine alınınca ve sunucu 409 `hold_expired` dönünce "Yanıt süresi doldu", karar butonları yok (kriter 4); personel rolünde onay butonları ve kuyruk isteği yok (kriter 5). |
| Müşteri SMS'i | `api/tests/Omvila.Api.IntegrationTests/ApprovalTests.cs` | Sahte SMS sağlayıcısıyla `Requested`, `Approved`, `Declined` ("iptal", "reddedildi" yok), `ApprovalExpired` mesajları; A1, A2, A4, A5 geçişleri ve 403 (kriter 1–6). |

Salonun kararının müşteri sayfasına yansıması rezervasyon testinde sunucu yanıtı değiştirilerek verilir; iki uygulamayı ve gerçek backend'i aynı testte birleştiren gece koşusu henüz yok (acik-sorular.md T-72).

## Salon paneli

Durum: ✓ tasarımla uyumlu, ≈ bilinçli/kapsam dışı farkla uyumlu, ✗ düzeltilmesi gereken fark.

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| GenelBakis | GenelBakis | GenelBakisKoyu | ≈ | Bento üç blok: "Bugünkü ciro" yerine "Bugün · randevu" (ciro D5), "Kapıda sırada" yok (D5), doluluk "—" (D6). "Dikkat isteyenler"de yalnız onay satırı; kapora (D4) ve gelmedi satırları yok. "Bu hafta doluluk" kartı yok (D6). "Gün akışı" günün bütün randevularını gösterir (tasarımda şimdiden sonrası). Üst barda genel arama ve "Randevu ekle" yok (T-57 (2)). Tasarımda tarih "Salı, 25 Kasım"; 25 Kasım 2026 Çarşamba, uygulama doğru. Bu PR'da görüntü düzeltildi: önceki hâli Prism oturumundaki `Onboarding` yüzünden kurulum adımlarını (KayitKurulum) çekiyordu. |
| RandevuPanel (onay bekleyen yan panel) | RandevuPanel | RandevuPanelKoyu | ≈ | Düzen, kalan süre şeridi, ücret satırları, not, geçmiş, Reddet / Onayla (1 : 2) tasarımdaki gibi. Kapora satırı ("Gerekmiyor") D4 (T-67 (1)). Süre "1 sa" (tasarımda "60 dk"; uygulamanın ortak süre biçimi). Başlıkta kanal da yazar ("· Online"). Yan panel tasarımdakinden geniş (Dilim 2 RandevuOnayli ile aynı bileşen). Arkadaki takvim bentosunda "Onay bekliyor 3": takvim özeti kapora bekleyen randevuyu da (durum PendingApproval) sayıyor, tasarımda 2 (aşağıda "Bulunan farklar"). Pencere görüntüsü (tam sayfada sabit yan panel kayıyor). |
| BildirimPaneli | BildirimPaneli | BildirimPaneliKoyu | ≈ | Prism örneğiyle üç bildirim (tasarımda dört; "Yeni değerlendirme" D5). Takvim sayfasında açılır; tasarımda genel bakış. Ayrıntı #74. |
| Durumlar: Kontör az | DurumKontorAz | DurumKontorAzKoyu | ✓ | Şerit ve "Kontör al" bağlantısı (Ayarlar → Bildirimler ve SMS). |
| AyarlarBildirim | AyarlarBildirim, AyarlarBildirim899 | …Koyu | ≈ | Üst barda "Kontör al" yok (T-70 (1)); "Kampanya" şablonu yok (T-70 (2)); gönderen adı kartında "Adı değiştir" bağlantısı var; ayarlar menüsünde D4+ öğeleri (Kapora, Roller, Abonelik, KVKK) yok. |
| GenelBakisBos, GenelBakisGecikti | — | — | — | Görüntü yok: boş hâl kritik akış 1 testinde (kayit.e2e.ts) ve birim testlerinde, ödeme gecikti şeridi D6. |

## Rezervasyon sayfası

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| Talep | Talep | TalepKoyu | ✓ | Tasarımda tarih "25 Kas Sal" (test verisinde 24 Kasım Salı). |
| Randevum: onay bekliyor | RandevumOnayBekliyor | RandevumOnayBekliyorKoyu | ≈ | Tasarımda ayrı görüntü yok; Randevum.png düzeni + "Salon onayı bekleniyor" kartı (T-68). |
| Randevum: talep iptal | RandevumTalepIptal | RandevumTalepIptalKoyu | ≈ | Tasarımda ayrı görüntü yok; Randevum.png'deki iptal düzeni. "Randevunuz · İptal", "Talebiniz iptal edildi", "onaylayamadı"; "reddedildi" geçmez. Bu PR'da eklendi. |

`bildirim-dagitim/` (SmsTelefon, PushIsletme, PushKatalog) uygulama ekranı değil, metin kaynağıdır; şablonlar SmsTemplatesTests (155 birim) ile denenir.

## Bulunan farklar

- **Takvim bentosu kapora bekleyeni de "Onay bekliyor" sayar:** `summarize` (calendar-view.ts) `status === 'PendingApproval'` ile sayar; kapora bekleyen randevu da bu durumdadır (`displayStatus = DepositPending`). Tasarımda (Main, RandevuPanel) kapora bekleyen varken sayı 2. Genel bakış sayıyı onay kuyruğundan aldığı için etkilenmez. Kapora Dilim 4'te geldiği için bu PR'da değiştirilmedi; D4'te `displayStatus` ile sayılmalı. **Düzeltildi (Dilim 4, T-84 (3)):** takvim bentosu `displayStatus` ile, Genel bakış kuyruktaki kapora bekleyenleri ayırarak sayar.
- **GenelBakis görüntüsü yanlış ekranı çekiyordu** (düzeltildi; test verisi, ürün kodu değil).
