# Sözlük: ekran adı ↔ kod adı

Kod tanımlayıcıları (sınıf, tablo, alan, API) **İngilizce**dir. Arayüz metni **Türkçe**dir. Bu tablo ikisini eşler. Yeni bir kavram çıkarsa önce buraya satır eklenir. Aynı kavram için ikinci bir kod adı uydurulmaz.

## Temel kavramlar

| Ekranda (TR) | Kodda (EN) | Not |
|---|---|---|
| İşletme / Marka | `Brand` | Salon tarafında "İşletme", platform panelinde "Marka" |
| Şube | `Branch` | |
| Personel | `Staff` | Uygulamaya giren kullanıcı `User`, ayrı kavram |
| Kullanıcı | `User` | |
| İşletme sahibi / Şube yöneticisi / Personel (rol) | `Owner` / `BranchManager` / `Staff` | |
| Müşteri (salonun) | `BrandCustomer` | Panel ve işletme uygulamasında |
| Müşteri (kişi, uygulama kullanıcısı) | `Customer` | Global |
| Hizmet | `Service` | |
| Kategori | `ServiceCategory` | |
| Aşama / Bekleme dilimi | `ServiceStage` (`StaffBusy = false`) | "Bekleme · müsait" şeridi |
| Tampon süre | `BufferBefore/AfterMinutes` | |
| Oda / cihaz | `Resource`, `ResourceType` | |
| Paket (tanım) | `Package` | |
| Müşteri paketi | `CustomerPackage` | "Fön paketi · 2/5 seans kaldı" |
| Seans | `PackageSessionUsage` | |
| Randevu | `Appointment` | |
| Randevu kalemi (hizmet satırı) | `AppointmentItem` | |
| Talep (onay bekleyen online randevu) | `Appointment` + `Status = PendingApproval` | "Talebiniz alındı" |
| Tekrarlayan randevu | `AppointmentSeries` | |
| Bekleme listesi | `WaitlistEntry` | |
| Bekleme teklifi / "Beklediğiniz saat boşaldı" | `WaitlistOffer` | |
| Kapıda sıra | `WalkInTicket` | |
| Kapora | `Deposit` (randevu alanları) | |
| Havale / EFT | `Transfer` (`PaymentMethod.Transfer`) | |
| Açıklama kodu (OMV-…) | `DepositTransferCode` | |
| Kapora hesabı (alıcı, banka, IBAN) | `BrandBankAccount` (API: `BankAccount`) | Ayarlar → Kapora ve IBAN |
| Kapora durumu | `DepositStatus`: `Awaiting`, `CustomerReported`, `Received`, `Refunded`, `Waived` | Kapora bekleniyor, Havale bildirildi, Alındı, Salon iade etti, Kaporasız onaylandı |
| Abonelik kodu (ABN-…) | `Subscription.TransferCode` | |
| Havale bildirimi | `PaymentNotice` | Platform onay kuyruğu |
| Tahsilat / Ödeme (salonun kaydı) | `Payment` | |
| Kasa | `Payment` listesi + `CashDayClose` | |
| Günü kapat | `CashDayClose` | |
| Prim | `CommissionEntry` | |
| Değerlendirme | `Review` | |
| Kontör | `CreditTransaction` (bakiye) | 1 kontör = 1 SMS (≤155 karakter) |
| Gönderen adı (SMS başlığı) | `BrandSmsSender` | |
| Aydınlatma metni okundu | `ConsentType.PrivacyNoticeAcknowledged` | |
| Kampanya izni | `ConsentType.Marketing*` | |
| KVKK silme talebi | `DataDeletionRequest` | |
| Rezervasyon sayfası | `BookingPage` (web/rezervasyon) | `omvila.com/{brandSlug}` |
| SMS bağlantısı (omvila.com/r/…) | `AppointmentAccessCode` | |
| Plan | `Plan` | Tekil = `solo`, Salon = `salon`, Salon Plus = `salon_plus` |
| Deneme | `SubscriptionStatus.Trialing` | |
| Ödeme gecikti | `SubscriptionStatus.PastDue` | |
| Salt okunur | `SubscriptionStatus.ReadOnly` | |
| Denetim kaydı | `AuditLog` | |
| Onay kuyrukları | `PaymentNotice` + `BrandSmsSender` (Pending) | |
| Özel gün / Kapalı gün | `WorkingHoursOverride` | |
| İzin / tatil (personel) | `TimeOff` | |

## Randevu durumları

| Ekranda | `Status` / `displayStatus` | Token öneki |
|---|---|---|
| Onay bekliyor | `PendingApproval` | `durum-onay` |
| Kapora bekleniyor | `DepositPending` (yalnız displayStatus) | `durum-kapora` |
| Onaylandı | `Confirmed` | `durum-onaylandi` |
| Geldi | `CheckedIn` | `durum-geldi` |
| Tamamlandı | `Completed` | `durum-tamamlandi` |
| Gelmedi | `NoShow` | `durum-gelmedi` |
| İptal | `Cancelled` | `durum-iptal` |

## Eylemler (buton ↔ uç nokta)

| Buton | API |
|---|---|
| Randevu ekle / Randevu oluştur | `POST /manage/appointments` |
| Randevuyu oluştur (müşteri) | `POST /public/branches/{branchId}/appointments` |
| Onayla | `POST /manage/appointments/{id}/approve` |
| Reddet | `POST /manage/appointments/{id}/decline` |
| Kapora geldi | `POST /manage/appointments/{id}/deposit/mark-received` |
| Havaleyi yaptım, salona bildir | `POST /public/appointments/{code}/deposit/report` |
| Kaporasız onayla | `POST /manage/appointments/{id}/approve` (`waiveDeposit = true`) |
| Henüz gelmedi, hatırlat | `POST /manage/appointments/{id}/deposit/remind` |
| Salon iade etti | `POST /manage/appointments/{id}/deposit/refund` |
| Geldi | `POST /manage/appointments/{id}/check-in` |
| Geldiyse geri al | `POST /manage/appointments/{id}/undo-no-show` |
| Tamamla ve ödeme al | `POST /manage/appointments/{id}/complete` |
| Saati değiştir | `POST …/appointments/{id}/reschedule` |
| Randevuyu iptal et | `POST …/appointments/{id}/cancel` |
| Sıraya ekle / Randevuya çevir / Sıradan çıkar | `POST /manage/walk-in-tickets`, `POST …/{id}/start`, `POST …/{id}/leave` |
| Saat öner / Teklifi geri al (bekleme listesi) | `POST /manage/waitlist/{id}/offer` / `POST …/{id}/offer/withdraw` |
| Listeye ekle (panel) | `POST /manage/waitlist` |
| Listeden çık | `POST /public/waitlist/{code}/leave` |
| Günü kapat | `POST /manage/cash/close` |
| Paket oluştur / Müşteriye paket sat | `POST /manage/packages` / `POST /manage/customers/{id}/packages` |
| Yanıtla (değerlendirme) | `POST /manage/reviews/{id}/reply` |
| Kayıtları birleştir | `POST /manage/customers/{id}/merge` |
| İçe aktar / Dışa aktar | `POST /manage/customers/imports` + `…/imports/{id}/commit` / `GET /manage/customers/export` |
| Kalıcı olarak anonimleştir (KVKK silme talebi) | `POST /manage/customers/{id}/deletion` |
| Listeye katıl | `POST /public/branches/{branchId}/waitlist` |
| Randevuyu al / Bu saat olmaz (teklif) | `POST /public/waitlist-offers/{code}/accept` / `decline` |
| Kod gönder / Giriş (müşteri uygulaması) | `POST /me/auth/otp/request` / `…/otp/verify` |
| Çıkış yap (müşteri uygulaması) | `POST /me/auth/logout` |
| Kaydet (Profil bilgilerim) | `PATCH /me/profile` |
| QR kodu okut / Bağlantıyı yapıştır | `POST /me/brands` |
| Randevuyu iptal et / Saati değiştir (müşteri uygulaması) | `POST /me/appointments/{id}/cancel` / `…/{id}/reschedule` |
| Havaleyi yaptım, salona bildir (müşteri uygulaması) | `POST /me/appointments/{id}/deposit/report` |
| Aynısını tekrar al | `GET /me/appointments?scope=Past&limit=1` → rezervasyon akışı (`publicSearchDays`, `publicCreateAppointment`, `meBearer`) |
| Değerlendir / Gönder (müşteri uygulaması) | `POST /me/reviews` |
| Kampanya izni anahtarı (İzinlerim) | `PUT /me/consents` |
| SMS koduyla onayla ve sil | `POST /me/account/deletion/otp` → `POST /me/account/deletion` |
| Ara / SMS (maskeli numara, işletme uygulaması) | `POST /manage/customers/{id}/reveal-phone` |

## Müşteri listesi filtreleri (tanımlar)

| Filtre | Kural |
|---|---|
| Düzenli | Son 180 günde ≥ 3 tamamlanan randevu |
| Yeni | İlk ziyaret son 30 günde **veya** hiç tamamlanan randevusu yok |
| Paketi olan | En az bir `Active` paket |
| Gelmeme riski | `NoShowCount >= RiskyCustomerNoShowThreshold` |
| 60+ gündür gelmeyen | `LastVisitOn < bugün − 60` ve yaklaşan randevu yok |
| "Ortalama aralık 34 gün" | Son 10 tamamlanan ziyaret arasındaki gün farklarının ortalaması |

## Biçimler (arayüz)
Tarih "Salı, 25 Kasım"; dar alanda "25 Kas Sal"; sayısal `25.11.2026`. Saat `14:30` (24 saat). Süre `45 dk`, `1 sa 30 dk`. Tutar `1.250 ₺`, `85,50 ₺`. Telefon `0532 123 45 67`. Ayrıntı: `docs/urun/urun-kurallari.md`.
