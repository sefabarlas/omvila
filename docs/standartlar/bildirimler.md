# Bildirimler: olay kataloğu ve kurallar

Kaynak: `design/ekranlar/bildirim-dagitim/` (SMS şablonları `Main.png`, push `PushKatalog.png`, e-posta `Eposta*.png`). Metinler oradaki gibidir. Değiştirmek ürün kararıdır.

## Genel kurallar
- **Outbox:** her bildirim iş transaction'ında `notification_outbox` satırı olarak yazılır (ADR-0005). Kodda doğrudan SMS veya push gönderilmez.
- **Kanal planı:** müşteri için uygulaması (aktif `Device`) varsa `[Push, Sms]`, yoksa `[Sms]`. Push başarısız olursa veya 2 dk içinde teslim raporu gelmezse SMS'e düşer. **Doğrulama kodu ve kapora mesajları her zaman SMS'tir** (push denenmez).
- **İşlem / kampanya:** randevu mesajları `Transactional`'dır; rıza istemez, kapatılamaz. `Marketing` göndermeden önce rıza + İYS kontrolü zorunlu (v1'de kampanya gönderimi yok; altyapı hazır).
- **Sessiz saatler (22:00–08:00, şube yerel):** işletme push'ları ertelenir. Müşteri hatırlatması sessiz saate denk gelirse 08:00'e kaydırılır. Kod, kapora ve bekleme teklifi kaydırılmaz.
- **SMS:** salon adıyla ve iki noktayla başlar. Tek bağlantı. Türkçe karakter setiyle (GSM 03.38 Türkçe tekli kaydırma tablosu) **≤ 155 karakter = 1 kontör**. Tutar "150 TL" yazılır (₺ işareti SMS'te değil). Tarih "Sal 25 Kas 14:30". Her şablonun en uzun değişken değerleriyle 155'i geçmediği testle doğrulanır.
- **Push:** başlık ≤ 40, metin ≤ 90 karakter. Kilit ekranında müşteri adı "Ad S." biçiminde (soyadın baş harfi). Sağlık veya not içeriği yazılmaz.
- **Gönderen:** SMS'te salonun onaylı başlığı (`BrandSmsSender`), yoksa `OMVILA`. Giriş kodu her zaman `OMVILA`. E-postada müşteriye "Kuaför Elif <bildirim@omvila.com>" (ekranda "OMVILA, Kuaför Elif adına"), işletmeye "OMVILA".
- **Kontör:** SMS gönderimi öncesi bakiye düşülür (`CreditTransaction.Consume`). Yetersizse SMS `Skipped`, push denenir, salona "Kontör az" uyarısı. Doğrulama kodları (OTP) salon kontöründen değil, OMVILA'dan karşılanır.
- **Bağlantılar:** kişiye özel ve sürelidir, giriş istemez. `omvila.com/r/{code}` randevu, `omvila.com/b/{code}` bekleme teklifi, `omvila.com/d/{code}` değerlendirme, `omvila.com/{slug}` rezervasyon sayfası. Kodlar `AppointmentAccessCode` tablosundan gelir.
- **İptal:** randevu iptal olunca o randevuya ait bekleyen hatırlatma ve değerlendirme satırları `Cancelled` yapılır. Saat değişince hatırlatmalar yeniden planlanır.
- **Müşteriye bildir anahtarı:** panelde "Müşteriye SMS gönder / Müşteriye bildir" kapalıysa ilgili müşteri bildirimi hiç yazılmaz.

## Müşteri olayları

| EventCode | Tetikleyici | Kanal | SMS metni (şablon) | Push başlık / metin |
|---|---|---|---|---|
| `otp.code` | OTP isteği | SMS (OMVILA) | `OMVILA giriş kodunuz: {kod}. Bu kodu kimseyle paylaşmayın.` | — |
| `appointment.confirmed` | Online oluşturma (onaysız) veya panel oluşturma (bildir açık) | Push → SMS | `{salon}: Randevunuz onaylandı. {tarih} {saat}, {hizmet} ({personel}). Değişiklik ve iptal: {bağlantı}` | Randevunuz onaylandı / `{salon} · {tarih} {saat}. Görüşmek üzere.` |
| `appointment.requested` | Online oluşturma, onay gerekli | Push → SMS | `{salon}: Talebiniz alındı. {tarih} {saat} için salon onayladığında size haber vereceğiz. {bağlantı}` | Talebiniz alındı / `Salon onaylayınca haber vereceğiz.` |
| `appointment.approved` | A2 | Push → SMS | `{salon} talebinizi onayladı: {tarih} {saat}, {hizmet}. Görüşmek üzere! {bağlantı}` ² | Randevunuz onaylandı / … |
| `appointment.approval_expired` | A5 (kapora yok) | Push → SMS | `{salon}, {tarih} {saat} talebinizi zamanında yanıtlayamadı, talep iptal edildi. Başka bir saat seçin: {rezervasyon bağlantısı}` | Talebiniz iptal edildi / … |
| `appointment.declined` | A4 | Push → SMS | `appointment.approval_expired` ile aynı dil ("iptal edildi"); "reddedildi" yazılmaz: `{salon}, {tarih} {saat} talebinizi onaylayamadı, talep iptal edildi. Başka bir saat seçin: {rezervasyon bağlantısı}` | |
| `deposit.awaiting` | Kaporalı randevu oluşunca | **SMS** | `{salon}: {tarih} {saat} için {tutar} TL kaporayı {süre} içinde gönderin. IBAN {iban}, {alıcı}, açıklama {kod}` (133 kr) | — |
| `deposit.received` | A3 | **SMS** | `{salon}: {tutar} TL kaporanız alındı, {tarih} {saat} randevunuz kesinleşti. {bağlantı}` | Kaporanız alındı / … |
| `deposit.not_received` | A5 (kapora vardı) | **SMS** | `{salon}: {tutar} TL kapora {süre} içinde gelmediği için {tarih} {saat} randevunuz iptal edildi. Havale yaptıysanız salonu arayın.` | — |
| `appointment.reminder` | `ReminderOffsetsMinutes` (1440) | Push → SMS | `Hatırlatma: Yarın {saat}'da {salon}'te randevunuz var. Değişiklik için: {bağlantı}` | Hatırlatma / `Yarın {saat} {salon}.` |
| `appointment.reminder` | (120) | Push → SMS | `Bugün {saat}'da {salon}'te görüşmek üzere. Yol tarifi: {bağlantı}` | Bugün {saat} {salon} / `Görüşmek üzere. Yol tarifi için dokunun.` |
| `appointment.rescheduled` | A14 (salon veya müşteri) | Push → SMS | `{salon}: Randevunuz {yeni tarih} {saat} olarak güncellendi. Detay: {bağlantı}` | |
| `appointment.cancelled_by_salon` | A7 | Push → SMS | `{salon}, {tarih} {saat} randevunuzu iptal etti.` + kapora alınmışsa ` Kapora iadesi için salon sizinle iletişime geçecek.` + ` {rezervasyon bağlantısı}` | |
| `waitlist.offer` | Teklif oluştu | Push → SMS | `{salon}: Beklediğiniz {tarih} {saat} boşaldı. 15 dk içinde onaylarsanız sizin: {b bağlantısı}` | Beklediğiniz saat boşaldı / `{salon} · {tarih} {saat} · 15 dk.` |
| `walkin.almost_turn` | Önünde 1 kişi kalınca (telefon verildiyse) | SMS | `{salon}: Sıranız yaklaştı, önünüzde 1 kişi var. Salona yakın olun.` ¹ | — |
| `review.invite` | Completed + 2 saat | Push → SMS | `{salon}'teki randevunuz nasıldı? 10 saniyenizi alır: {d bağlantısı}` | Nasıldı? / `{salon}'teki randevunuzu değerlendirin.` |
| `appointment.confirmed.email` | Onay (e-posta adresi varsa) | E-posta | Bkz. `EpostaOnay.png` | |

¹ Tasarımdaki metinde "Lütfen salona yakın olun." yazıyor. Ses-tonu kuralı "lütfen" kullanılmasını yasakladığı için "Salona yakın olun." kullanılır. Tasarım düzeltilmeli (`docs/acik-sorular.md`).

² `appointment.approved` kodda ünlemsiz yazılır ("Görüşmek üzere."). `appointment.declined` ve `appointment.approval_expired` 155 birimi aşınca kısa metin gider: `{salon}, {tarih} {saat} talebiniz iptal edildi. Başka bir saat seçin: {rezervasyon bağlantısı}` (`docs/acik-sorular.md` T-65).

Türkçe ek uyumu (`'da/'de`, `'te/'ta`) şablon motorunda `{salon|bulunma}` gibi yardımcılarla yapılır. Salon adının son hecesine göre ek seçilir; sabit "'te" yazılmaz.

## İşletme olayları (işletme uygulaması push + panel bildirim paneli)

| EventCode | Alıcı | Push başlık | Push metin | Açılan ekran |
|---|---|---|---|---|
| `staff.approval_needed` | Owner, BranchManager | `Onay bekliyor · {kalan süre}` | `{Müşteri} · {tarih saat} {hizmet}. Yanıtlanmazsa otomatik iptal olur.` | Randevu detayı |
| `staff.appointment_created` | İlgili personel (+ yöneticiler, tercihe göre) | `Yeni randevu` | `{Müşteri} · {saat} {hizmet} · sizinle.` | Randevu detayı |
| `staff.appointment_changed` | İlgili personel | `İptal` / `Saat değişti` | `{Müşteri} · {saat} iptal etti. Neden: {neden}.` / `{eski} → {yeni}` | Takvim |
| `staff.customer_checked_in` | İlgili personel | `Müşteri geldi` | `{Müşteri} check-in yaptı.` | Bugün |
| `staff.deposit_reported` | Owner, BranchManager | `Havale bildirildi` | `{Müşteri} · {tutar} kaporayı gönderdiğini söyledi. Kontrol edin.` | Kapora geldi mi? |
| `staff.review_received` | Owner, BranchManager (+ personel kendi) | `Yeni değerlendirme` | `{puan} yıldız · {Müşteri}` | Değerlendirmeler |
| `staff.walkin_queue` | Tercihe göre | `Sırada {n} kişi` | `Kapıda sıra · tahmini bekleme {dk} dk.` | Sıra |
| `owner.credits_low` | Owner | `SMS kontörü azaldı` | `{kalan} SMS kaldı, tahmini {gün} gün yeter.` | Panel · kontör |
| `owner.subscription_payment_failed` | Owner | `Abonelik ödemesi alınamadı` | `{gün} gün içinde kartı güncelleyin ya da havale ile ödeyin; {tarih} salt okunur.` | Panel · abonelik |
| `owner.payment_notice_matched` | Owner | taslak (T-116) | Havale eşleşti: abonelik dönemi ödendi veya kontör eklendi | Panel · abonelik |
| `owner.payment_notice_not_found` | Owner | taslak (T-116) | Havale OMVILA hesabında bulunamadı; platformun notu eklenir, salon yeniden bildirebilir | Panel · abonelik |
| `owner.payment_notice_rejected` | Owner | taslak (T-116) | Havale bildirimi reddedildi (yinelenen veya geçersiz); platformun notu eklenir | Panel · abonelik |

### İşletme SMS'leri

| EventCode | Tetikleyici | Kanal | SMS metni (şablon) |
|---|---|---|---|
| `staff.invite` | Personel telefonla eklendi, "Davet SMS'i gönder" açık | SMS | `{salon}: Ekibimize eklendiniz. Bu numarayla OMVILA'ya giriş yapın: {bağlantı}` — **taslak**, tasarımda yok (acik-sorular.md T-32) |
| `staff.daily_summary` | Tercihe göre (kapalı varsayılan) | `Gün sonu özeti` | `{n} randevu tamamlandı, {tutar} tahsilat.` | Bugün |

## E-posta (işletmeye, gönderen OMVILA)
| EventCode | Ne zaman |
|---|---|
| `owner.trial_ending` | Deneme bitmeden 3 gün önce |
| `owner.credits_low` | Kontör eşiğin altına inince (push ile birlikte) |
| `owner.subscription_payment_failed` | Çekim başarısız / havale gelmedi |
| `owner.payment_notice_matched` | Havale bildirimi eşleşti (push ile birlikte; metin taslak, T-116) |
| `owner.data_export_ready` | "Tüm verileri dışa aktar" arşivi hazır: indirme bağlantısı (`DataExportLinkValidityDays` geçerli; alıcı isteği yapanın verdiği adres; metin taslak, T-121) |
| `owner.payment_notice_not_found` / `owner.payment_notice_rejected` | Havale bulunamadı / bildirim reddedildi (push ile birlikte; metin taslak, T-116) |

Platform ekibine ayrı bildirim olayı yoktur: yeni havale bildirimi ve SMS adı başvurusu platform panelindeki "Onay kuyrukları" rozetinde (`AdminSession.pendingApprovalCount`) görünür (T-116).

## Şablon testi (zorunlu)
`Notifications.Tests/TemplateLengthTests`: her SMS şablonu, en uzun gerçekçi değerlerle (salon adı 30 karakter, en uzun Türkçe ay ve gün kısaltması, 4 haneli tutar, 26 karakter IBAN, 10 karakter kod) render edilir. GSM-7 Türkçe ile 155 karakteri geçmemeli. Geçerse test başarısız olur; şablon kısaltılır veya değişken kırpılır (salon adı kısaltması).
