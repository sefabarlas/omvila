# Dilim 3: Onay akışı + SMS

## Amaç
Onaylı çalışan salonda online randevu talep olarak düşer, salon 120 dk içinde onaylar veya reddeder; yanıtlanmazsa otomatik iptal olur. Gerçek SMS sağlayıcısıyla bütün işlem mesajları gider. Push altyapısı (işletme uygulaması için) hazırlanır.

## Kapsam
- Durum geçişleri A1 (PendingApproval), A2, A4, A5, A13; `ExpireAppointmentHolds` işi.
- Panel: "Dikkat isteyenler" kartı, onay bekleyen bento (`blok-vurgu`), randevu yan paneli Onayla / Reddet, kalan süre ("1 sa 35 dk içinde yanıtlayın"), bildirim paneli.
- Rezervasyon: Talep ekranı ("Talebiniz alındı · en geç 2 saat içinde"), Randevum'da "Onay bekliyor".
- Bildirim gönderimi: `DispatchNotifications` worker, SMS sağlayıcı adaptörü (T-03 kararıyla), teslim raporu webhook'u, kontör düşümü, yetersiz kontörde atlama + uyarı, sessiz saat kaydırması, push → SMS düşüşü (push adaptörü FCM; cihaz kaydı uçları).
- SMS şablonlarının tamamı (`bildirimler.md`) ve 155 karakter testleri. Türkçe ek uyumu yardımcısı.
- Ayarlar → Bildirimler ve SMS: kontör özeti, hatırlatma zamanları, değerlendirme daveti anahtarı, şablon ezmeleri, gönderen adı başvurusu.
- İşletme uygulaması için `staff.*` push olayları (uygulama D5'te; olaylar ve outbox burada).

## Ekranlar
`salon-paneli/`: GenelBakis, RandevuPanel (onay bekleyen), BildirimPaneli, AyarlarBildirim, Durumlar (kontör az). `rezervasyon-sayfasi/`: Talep, Randevum (onay bekliyor). `bildirim-dagitim/`: SmsTelefon, PushIsletme, PushKatalog.

## Kabul kriterleri
1. `RequiresApproval=true` salonda online randevu → `PendingApproval`, `holdExpiresUtc = +120 dk`; müşteriye "Talebiniz alındı" SMS'i; işletme kullanıcılarına `staff.approval_needed`; slot başkasına kapalı.
2. Salon "Onayla" → `Confirmed`, müşteriye "talebinizi onayladı" SMS'i, hatırlatmalar planlanır.
3. Salon "Reddet" → `Cancelled`, neden `DeclinedBySalon`; müşteri SMS'inde ve Randevum sayfasında "iptal" yazar, "reddedildi" geçmez.
4. 120 dk yanıtsız (saat ileri) → `Cancelled`/`ApprovalExpired`, "zamanında yanıtlayamadı" SMS'i, slot boşalır.
5. Personel rolü onay butonlarını görmez; `approve` çağrısı 403.
6. Müşteri saat değiştirince (onaylı salon) randevu yeniden `PendingApproval`.
7. Kontör 0 iken SMS `Skipped`; panelde "Kontör az" şeridi; push'u olan müşteriye push gider.
8. 23:30'a düşen müşteri hatırlatması 08:00'e kaydırılır; işletme push'u sessiz saatte gönderilmez.
9. Her SMS şablonu en uzun değerlerle ≤ 155 GSM-7 Türkçe karakter (test).
10. Teslim raporu webhook'u imzasız istekleri reddeder; teslim edilen mesaj `Sent`, başarısızlar üstel geri çekilmeyle tekrar denenir, 5. denemede `Failed`.
11. Gönderen adı başvurusu `Pending`; onaylanana kadar mesajlar `OMVILA` adıyla gider.

## Uçtan uca ve tasarım
- Kritik akış 3 (`test-stratejisi.md`): `web/rezervasyon/e2e/onay.e2e.ts`, `web/salon-panel/e2e/onay.e2e.ts`; müşteri SMS'leri `ApprovalTests`.
- Tasarım karşılaştırması: [dilim-03-tasarim-karsilastirma.md](dilim-03-tasarim-karsilastirma.md).
