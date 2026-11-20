# Ekran envanteri

Her tasarım ekranının dosyası, ne olduğu, hangi dilimde yapılacağı ve hangi API işlemlerini kullandığı. Ekran görüntüleri `design/ekranlar/<yüzey>/<Ekran>.png` (açık) ve `<Ekran>Koyu.png` (koyu). Sıra, tasarımdaki akış sırasıdır.

Bir ekranı yapan ajan: açık ve koyu görüntüyü açar, ilgili tasarım sistemi README'sini ve bileşen README'lerini okur, bittiğinde kendi ekran görüntüsünü yan yana koyar. İşlem adları `contracts/openapi.yaml` veya `contracts/yol-haritasi.md` içindedir.

Bu dosya `scripts/gen_inventory.py` ile üretildi; ekran eklenince betik güncellenir.

## Rezervasyon sayfası

Proje: `web/rezervasyon` · Klasör: `design/ekranlar/rezervasyon-sayfasi/`

| Ekran | Ne | Dilim | API işlemleri | Koyu |
|---|---|---|---|---|
| [Sube](ekranlar/rezervasyon-sayfasi/Sube.png) | Şube seçimi (çok şubeli salon) | 2 | publicGetBrand | ✓ |
| [Main](ekranlar/rezervasyon-sayfasi/Main.png) | Salon kartı + hizmet seçimi (adım 1) | 2 | publicGetBrand, publicGetBranchCatalog | ✓ |
| [Personel](ekranlar/rezervasyon-sayfasi/Personel.png) | Personel seçimi, "Fark etmez", "İki hizmet de aynı kişiden" | 2 | publicGetBranchCatalog | ✓ |
| [Saat](ekranlar/rezervasyon-sayfasi/Saat.png) | Gün şeridi + saat ızgarası | 2 | publicSearchDays, publicSearchSlots | ✓ |
| [DoluGun](ekranlar/rezervasyon-sayfasi/DoluGun.png) | Dolu gün + bekleme listesi formu | 2 (liste: 5) | publicSearchDays, publicJoinWaitlist | ✓ |
| [Telefon](ekranlar/rezervasyon-sayfasi/Telefon.png) | Ad, telefon, not, aydınlatma ve kampanya onayı | 2 | publicRequestOtp | ✓ |
| [Kod](ekranlar/rezervasyon-sayfasi/Kod.png) | SMS kodu | 2 | publicVerifyOtp | ✓ |
| [Onay](ekranlar/rezervasyon-sayfasi/Onay.png) | Son kontrol: özet, paketten düşme, kapora kutusu | 2 (paket 5, kapora 4) | publicListMyPackages, publicCreateAppointment | ✓ |
| [Talep](ekranlar/rezervasyon-sayfasi/Talep.png) | "Talebiniz alındı" (onaylı salon) | 3 | publicCreateAppointment | ✓ |
| [Havale](ekranlar/rezervasyon-sayfasi/Havale.png) | Kapora havale bilgileri, geri sayım, "Havaleyi yaptım" | 4 | publicReportDepositTransfer | ✓ |
| [Basari](ekranlar/rezervasyon-sayfasi/Basari.png) | "Randevunuz onaylandı" | 2 | — | ✓ |
| [Randevum](ekranlar/rezervasyon-sayfasi/Randevum.png) | SMS bağlantısından randevu | 2 | publicGetAppointmentByCode | ✓ |
| [Ertele](ekranlar/rezervasyon-sayfasi/Ertele.png) | Saat değiştir | 2 | publicSearchRescheduleSlots, publicRescheduleAppointment | ✓ |
| [Iptal](ekranlar/rezervasyon-sayfasi/Iptal.png) | İptal, neden zorunlu, kapora uyarısı | 2 | publicCancelAppointment | ✓ |
| [BeklemeTeklif](ekranlar/rezervasyon-sayfasi/BeklemeTeklif.png) | Bekleme teklifi, 15 dk | 5 | publicGetWaitlistOffer, publicAcceptWaitlistOffer | ✓ |
| [BeklemeDoldu](ekranlar/rezervasyon-sayfasi/BeklemeDoldu.png) | Teklif süresi doldu | 5 | publicDeclineWaitlistOffer, publicLeaveWaitlist | ✓ |
| [Masaustu](ekranlar/rezervasyon-sayfasi/Masaustu.png) | Masaüstü iki kolon düzeni | 2 | — | ✓ |
| [Durumlar](ekranlar/rezervasyon-sayfasi/Durumlar.png) | Durum desenleri | 2–4 | — | ✓ |

## Salon paneli

Proje: `web/salon-panel` · Klasör: `design/ekranlar/salon-paneli/`

| Ekran | Ne | Dilim | API işlemleri | Koyu |
|---|---|---|---|---|
| [Giris](ekranlar/salon-paneli/Giris.png) | Panele giriş (telefon) | 1 | manageRequestOtp | ✓ |
| [GirisKod](ekranlar/salon-paneli/GirisKod.png) | Kod + 30 gün hatırla | 1 | manageVerifyOtp | ✓ |
| [Kayit](ekranlar/salon-paneli/Kayit.png) | İşletmenizi oluşturun | 1 | manageRequestOtp, manageSignup | ✓ |
| [KayitKurulum](ekranlar/salon-paneli/KayitKurulum.png) | İlk randevuya 4 adım | 1 | manageGetOnboarding | ✓ |
| [GenelBakis](ekranlar/salon-paneli/GenelBakis.png) | Genel bakış (bento, gün akışı, dikkat isteyenler, doluluk) | 3 / 6 | manageGetOverview, manageListPendingApprovals | ✓ |
| [GenelBakisBos](ekranlar/salon-paneli/GenelBakisBos.png) | İlk hafta: bağlantıyı paylaş | 1 | manageGetBookingPageSettings, manageRecordOnboardingEvent | ✓ |
| [GenelBakisGecikti](ekranlar/salon-paneli/GenelBakisGecikti.png) | Ödeme gecikti şeridi | 6 | manageGetSession | ✓ |
| [Main](ekranlar/salon-paneli/Main.png) | Takvim, gün görünümü | 2 | manageGetCalendar | ✓ |
| [HaftaTakvim](ekranlar/salon-paneli/HaftaTakvim.png) | Takvim, hafta görünümü | 2 | manageGetCalendar | ✓ |
| [TakvimBos](ekranlar/salon-paneli/TakvimBos.png) | Boş gün | 2 | manageGetCalendar | ✓ |
| [TakvimKapali](ekranlar/salon-paneli/TakvimKapali.png) | Kapalı gün + "Yalnızca bu gün için aç" | 2 | manageCreateBranchOverride | ✓ |
| [TakvimSaltOkunur](ekranlar/salon-paneli/TakvimSaltOkunur.png) | Salt okunur abonelik | 6 | manageGetSession | ✓ |
| [TakvimSurukle](ekranlar/salon-paneli/TakvimSurukle.png) | Sürükle-bırak | 2 | manageRescheduleAppointment | ✓ |
| [YeniRandevu](ekranlar/salon-paneli/YeniRandevu.png) | Yeni randevu modalı | 2 | manageSearchSlots, manageCreateAppointment, manageListCustomers | ✓ |
| [RandevuPanel](ekranlar/salon-paneli/RandevuPanel.png) | Onay bekleyen randevu yan paneli | 3 | manageGetAppointment, manageApproveAppointment, manageDeclineAppointment | ✓ |
| [RandevuOnayli](ekranlar/salon-paneli/RandevuOnayli.png) | Onaylı randevu yan paneli, Geldi | 2 | manageGetAppointment, manageCheckInAppointment, manageCancelAppointment | ✓ |
| [TekrarIptal](ekranlar/salon-paneli/TekrarIptal.png) | Tekrarlayan randevu iptali | 2 | manageCancelAppointment | ✓ |
| [OdemeAl](ekranlar/salon-paneli/OdemeAl.png) | Tamamla ve ödeme al | 5 | manageCompleteAppointment | ✓ |
| [KapidaSira](ekranlar/salon-paneli/KapidaSira.png) | Kapıda sıra | 5 | manageListWalkIns, manageStartWalkIn | ✓ |
| [KapidaSiraBos](ekranlar/salon-paneli/KapidaSiraBos.png) | Kapıda sıra boş | 5 | — | ✓ |
| [Musteriler](ekranlar/salon-paneli/Musteriler.png) | Müşteri listesi | 2 | manageListCustomers | ✓ |
| [MusterilerBos](ekranlar/salon-paneli/MusterilerBos.png) | Müşteri yok | 2 | — | ✓ |
| [MusterilerAramaYok](ekranlar/salon-paneli/MusterilerAramaYok.png) | Arama sonucu yok | 2 | manageListCustomers | ✓ |
| [MusteriEkle](ekranlar/salon-paneli/MusteriEkle.png) | Müşteri ekle, aynı numara uyarısı | 2 | manageCreateCustomer | ✓ |
| [MusteriKarti](ekranlar/salon-paneli/MusteriKarti.png) | Müşteri kartı | 2 (sekmeler 5) | manageGetCustomer | ✓ |
| [MusteriBirlestir](ekranlar/salon-paneli/MusteriBirlestir.png) | Kayıtları birleştir | 5 | manageMergeCustomers | ✓ |
| [MusteriIceAktar](ekranlar/salon-paneli/MusteriIceAktar.png) | CSV içe aktar | 5 | managePreviewImport, manageCommitImport | ✓ |
| [MusteriSil](ekranlar/salon-paneli/MusteriSil.png) | KVKK silme talebi | 5 | manageRequestCustomerDeletion | ✓ |
| [BeklemeListesi](ekranlar/salon-paneli/BeklemeListesi.png) | Bekleme listesi | 5 | manageListWaitlist, manageOfferSlot | ✓ |
| [BeklemeListesiBos](ekranlar/salon-paneli/BeklemeListesiBos.png) | Bekleme listesi boş | 5 | — | ✓ |
| [Kasa](ekranlar/salon-paneli/Kasa.png) | Kasa ve sayım | 5 | manageGetCashDay, manageCloseCashDay | ✓ |
| [KasaBos](ekranlar/salon-paneli/KasaBos.png) | Kasa boş | 5 | — | ✓ |
| [Degerlendirmeler](ekranlar/salon-paneli/Degerlendirmeler.png) | Değerlendirmeler, yanıt | 5 | manageListReviews, manageReplyReview | ✓ |
| [DegerlendirmelerBos](ekranlar/salon-paneli/DegerlendirmelerBos.png) | Değerlendirme yok | 5 | — | ✓ |
| [Hizmetler](ekranlar/salon-paneli/Hizmetler.png) | Hizmet listesi | 1 | manageListServices, manageListServiceCategories | ✓ |
| [HizmetlerBos](ekranlar/salon-paneli/HizmetlerBos.png) | Hizmet yok, hazır liste önerisi | 1 | manageListServiceTemplates | ✓ |
| [HizmetEkle](ekranlar/salon-paneli/HizmetEkle.png) | Hizmet ekle (aşama, kapora, gelişmiş) | 1 | manageCreateService, manageUpdateService | ✓ |
| [KategoriEkle](ekranlar/salon-paneli/KategoriEkle.png) | Kategori ekle | 1 | manageCreateServiceCategory | ✓ |
| [Personel](ekranlar/salon-paneli/Personel.png) | Personel listesi + saatler + hizmetler + prim | 1 | manageListStaff, manageGetStaff | ✓ |
| [PersonelBos](ekranlar/salon-paneli/PersonelBos.png) | Personel yok | 1 | — | ✓ |
| [PersonelEkle](ekranlar/salon-paneli/PersonelEkle.png) | Personel ekle (yan panel) | 1 | manageCreateStaff, manageSetStaffServices | ✓ |
| [PersonelRolu](ekranlar/salon-paneli/PersonelRolu.png) | Personel rolüyle panel görünümü | 1 / 2 | manageGetSession | ✓ |
| [IzinEkle](ekranlar/salon-paneli/IzinEkle.png) | İzin veya tatil ekle | 1 | manageCreateTimeOff | ✓ |
| [Paketler](ekranlar/salon-paneli/Paketler.png) | Paket tanımları, süresi yaklaşanlar | 5 | — | ✓ |
| [PaketlerBos](ekranlar/salon-paneli/PaketlerBos.png) | Paket yok | 5 | — | ✓ |
| [PaketOlustur](ekranlar/salon-paneli/PaketOlustur.png) | Paket oluştur | 5 | — | ✓ |
| [PaketSat](ekranlar/salon-paneli/PaketSat.png) | Müşteriye paket sat | 5 | manageSellPackage | ✓ |
| [Raporlar](ekranlar/salon-paneli/Raporlar.png) | Raporlar ve prim | 6 | manageGetReportOverview, manageGetCommissionReport | ✓ |
| [RaporlarBos](ekranlar/salon-paneli/RaporlarBos.png) | Rapor yok | 6 | — | ✓ |
| [BildirimPaneli](ekranlar/salon-paneli/BildirimPaneli.png) | Bildirim paneli | 3 | manageListInAppNotifications | ✓ |
| [Ayarlar](ekranlar/salon-paneli/Ayarlar.png) | Randevu kuralları, kapora, iptal ve gelmeme, gizlilik | 1 (kapora 4) | manageGetBrandSettings, manageUpdateBrandSettings | ✓ |
| [AyarlarIsletme](ekranlar/salon-paneli/AyarlarIsletme.png) | İşletme bilgileri | 1 | manageGetBrand, manageUpdateBrand | ✓ |
| [AyarlarSube](ekranlar/salon-paneli/AyarlarSube.png) | Şube saatleri, özel günler, takvim düzeni | 1 | manageSetBranchWorkingHours, manageCreateBranchOverride, manageUpdateBranch | ✓ |
| [SubeEkle](ekranlar/salon-paneli/SubeEkle.png) | Şube ekle | 1 | manageCreateBranch | ✓ |
| [AyarlarRezervasyon](ekranlar/salon-paneli/AyarlarRezervasyon.png) | Rezervasyon sayfası | 1 | manageGetBookingPageSettings, manageUpdateBookingPageSettings | ✓ |
| [AyarlarOdeme](ekranlar/salon-paneli/AyarlarOdeme.png) | Kapora ve IBAN | 4 | manageGetBankAccount, manageUpdateBankAccount | ✓ |
| [AyarlarBildirim](ekranlar/salon-paneli/AyarlarBildirim.png) | Bildirimler ve SMS | 3 | manageGetNotificationSettings, manageGetSmsOverview | ✓ |
| [AyarlarRoller](ekranlar/salon-paneli/AyarlarRoller.png) | Roller ve yetkiler | 6 | — | ✓ |
| [AyarlarKvkk](ekranlar/salon-paneli/AyarlarKvkk.png) | KVKK ve veri | 5 / 6 | — | ✓ |
| [AyarlarAbonelik](ekranlar/salon-paneli/AyarlarAbonelik.png) | Abonelik ve fatura | 6 | manageGetSubscription, manageNotifyTransfer | ✓ |
| [TabletGenel](ekranlar/salon-paneli/TabletGenel.png) | Tablet: genel bakış | 1 | — | ✓ |
| [TabletTakvim](ekranlar/salon-paneli/TabletTakvim.png) | Tablet: takvim | 2 | — | ✓ |
| [TabletDikeyTakvim](ekranlar/salon-paneli/TabletDikeyTakvim.png) | Tablet dikey: takvim | 2 | — | ✓ |
| [TabletDikeyMusteriler](ekranlar/salon-paneli/TabletDikeyMusteriler.png) | Tablet dikey: müşteriler | 2 | — | ✓ |
| [Durumlar](ekranlar/salon-paneli/Durumlar.png) | Durum desenleri | 1–6 | — | ✓ |

## Platform paneli

Proje: `web/platform-panel` · Klasör: `design/ekranlar/platform-paneli/`

| Ekran | Ne | Dilim | API işlemleri | Koyu |
|---|---|---|---|---|
| [Main](ekranlar/platform-paneli/Main.png) | Genel bakış, aktivasyon hunisi | 6 | adminGetOverview | ✓ |
| [PMarkalar](ekranlar/platform-paneli/PMarkalar.png) | Markalar | 6 | adminListBrands | ✓ |
| [PMarkaDetay](ekranlar/platform-paneli/PMarkaDetay.png) | Marka detayı, kontör hareketleri | 6 | adminGetBrand, adminGrantCredits | ✓ |
| [POnay](ekranlar/platform-paneli/POnay.png) | Onay kuyrukları | 6 | adminListApprovalQueue | ✓ |
| [POnayBos](ekranlar/platform-paneli/POnayBos.png) | Bekleyen onay yok | 6 | — | ✓ |
| [POnayDetay](ekranlar/platform-paneli/POnayDetay.png) | Havale bildirimi eşleştirme | 6 | adminGetPaymentNotice, adminMatchPaymentNotice | ✓ |
| [PAbonelikler](ekranlar/platform-paneli/PAbonelikler.png) | Abonelikler | 6 | adminListSubscriptions | ✓ |
| [PDenetim](ekranlar/platform-paneli/PDenetim.png) | Denetim kaydı | 6 | adminListAuditLog | ✓ |
| [PPlanlar](ekranlar/platform-paneli/PPlanlar.png) | Planlar ve fiyat | 6 | — | ✓ |

## Müşteri uygulaması

Proje: `mobile/musteri` · Klasör: `design/ekranlar/musteri-uygulamasi/`

| Ekran | Ne | Dilim | API işlemleri | Koyu |
|---|---|---|---|---|
| [Acilis](ekranlar/musteri-uygulamasi/Acilis.png) | Açılış | 5 | — | ✓ |
| [Giris](ekranlar/musteri-uygulamasi/Giris.png) | Giriş (telefon) | 5 | meRequestOtp | ✓ |
| [GirisKod](ekranlar/musteri-uygulamasi/GirisKod.png) | SMS kodu | 5 | meVerifyOtp | ✓ |
| [BildirimIzni](ekranlar/musteri-uygulamasi/BildirimIzni.png) | Bildirim izni (önce kendi ekranımız) | 5 | meRegisterDevice | ✓ |
| [Main](ekranlar/musteri-uygulamasi/Main.png) | Randevularım (yaklaşan kart) | 5 | meListAppointments | ✓ |
| [MainBos](ekranlar/musteri-uygulamasi/MainBos.png) | İlk giriş: QR / bağlantı | 5 | meSaveBrand | ✓ |
| [MainYaklasanYok](ekranlar/musteri-uygulamasi/MainYaklasanYok.png) | "Aynısını tekrar al" | 5 | meListAppointments | ✓ |
| [MainBildirimKapali](ekranlar/musteri-uygulamasi/MainBildirimKapali.png) | Bildirim kapalı şeridi | 5 | — | ✓ |
| [Paketler](ekranlar/musteri-uygulamasi/Paketler.png) | Paketlerim | 5 | meListPackages | ✓ |
| [PaketlerBos](ekranlar/musteri-uygulamasi/PaketlerBos.png) | Paket yok | 5 | — | ✓ |
| [Profil](ekranlar/musteri-uygulamasi/Profil.png) | Profil | 5 | meGetProfile | ✓ |
| [ProfilDuzenle](ekranlar/musteri-uygulamasi/ProfilDuzenle.png) | Profil düzenle | 5 | meUpdateProfile | ✓ |
| [YRSalon](ekranlar/musteri-uygulamasi/YRSalon.png) | Randevu al: salon seç | 5 | meListBrands | ✓ |
| [YRSalonBos](ekranlar/musteri-uygulamasi/YRSalonBos.png) | Salon yok | 5 | — | ✓ |
| [YRHizmet](ekranlar/musteri-uygulamasi/YRHizmet.png) | Hizmet | 5 | publicGetBranchCatalog | ✓ |
| [YRPersonel](ekranlar/musteri-uygulamasi/YRPersonel.png) | Personel | 5 | publicGetBranchCatalog | ✓ |
| [YRSaat](ekranlar/musteri-uygulamasi/YRSaat.png) | Saat | 5 | publicSearchDays, publicSearchSlots | ✓ |
| [YROnay](ekranlar/musteri-uygulamasi/YROnay.png) | Son kontrol | 5 | publicCreateAppointment | ✓ |
| [YRHavale](ekranlar/musteri-uygulamasi/YRHavale.png) | Kapora havale | 5 (4) | meReportDepositTransfer | ✓ |
| [YRBasari](ekranlar/musteri-uygulamasi/YRBasari.png) | Başarı | 5 | — | ✓ |
| [BeklemeKatil](ekranlar/musteri-uygulamasi/BeklemeKatil.png) | Bekleme listesine katıl | 5 | publicJoinWaitlist | ✓ |
| [BeklemeTeklif](ekranlar/musteri-uygulamasi/BeklemeTeklif.png) | Teklif | 5 | publicAcceptWaitlistOffer | ✓ |
| [BeklemeDoldu](ekranlar/musteri-uygulamasi/BeklemeDoldu.png) | Teklif süresi doldu | 5 | — | ✓ |
| [RandevuDetay](ekranlar/musteri-uygulamasi/RandevuDetay.png) | Randevu detayı | 5 | meGetAppointment | ✓ |
| [RandevuErtele](ekranlar/musteri-uygulamasi/RandevuErtele.png) | Saat değiştir | 5 | meRescheduleAppointment | ✓ |
| [ErteleKapali](ekranlar/musteri-uygulamasi/ErteleKapali.png) | Saat değişikliği için geç | 5 | — | ✓ |
| [Iptal](ekranlar/musteri-uygulamasi/Iptal.png) | İptal (neden zorunlu) | 5 | meCancelAppointment | ✓ |
| [Gecmis](ekranlar/musteri-uygulamasi/Gecmis.png) | Geçmiş | 5 | meListAppointments | ✓ |
| [GecmisBos](ekranlar/musteri-uygulamasi/GecmisBos.png) | Geçmiş yok | 5 | — | ✓ |
| [Degerlendir](ekranlar/musteri-uygulamasi/Degerlendir.png) | Değerlendirme | 5 | meCreateReview | ✓ |
| [Odemelerim](ekranlar/musteri-uygulamasi/Odemelerim.png) | Ödemelerim (salt okunur) | 5 | meListPayments | ✓ |
| [OdemelerimBos](ekranlar/musteri-uygulamasi/OdemelerimBos.png) | Ödeme yok | 5 | — | ✓ |
| [Izinler](ekranlar/musteri-uygulamasi/Izinler.png) | İzinlerim (kampanya izni) | 5 | meListConsents, meUpdateMarketingConsent | ✓ |
| [HesapSil](ekranlar/musteri-uygulamasi/HesapSil.png) | Hesabı sil | 5 | meRequestAccountDeletion | ✓ |
| [HesapSilindi](ekranlar/musteri-uygulamasi/HesapSilindi.png) | Hesap silindi | 5 | — | ✓ |
| [Bildirimler](ekranlar/musteri-uygulamasi/Bildirimler.png) | Bildirimler | 5 | meListNotifications | ✓ |
| [BildirimlerBos](ekranlar/musteri-uygulamasi/BildirimlerBos.png) | Bildirim yok | 5 | — | ✓ |
| [Durumlar](ekranlar/musteri-uygulamasi/Durumlar.png) | Durum desenleri | 5 | — | ✓ |

## İşletme uygulaması

Proje: `mobile/isletme` · Klasör: `design/ekranlar/isletme-uygulamasi/`

| Ekran | Ne | Dilim | API işlemleri | Koyu |
|---|---|---|---|---|
| [Acilis](ekranlar/isletme-uygulamasi/Acilis.png) | Açılış | 5 | — | ✓ |
| [Giris](ekranlar/isletme-uygulamasi/Giris.png) | Giriş | 5 | manageRequestOtp | ✓ |
| [GirisKod](ekranlar/isletme-uygulamasi/GirisKod.png) | SMS kodu | 5 | manageVerifyOtp | ✓ |
| [BildirimIzni](ekranlar/isletme-uygulamasi/BildirimIzni.png) | Bildirim izni | 5 | manageRegisterDevice | ✓ |
| [Main](ekranlar/isletme-uygulamasi/Main.png) | Bugün | 5 | manageGetToday | ✓ |
| [MainBos](ekranlar/isletme-uygulamasi/MainBos.png) | Bugün randevu yok | 5 | — | ✓ |
| [MainBildirimKapali](ekranlar/isletme-uygulamasi/MainBildirimKapali.png) | Bildirim kapalı şeridi | 5 | — | ✓ |
| [MainSaltOkunur](ekranlar/isletme-uygulamasi/MainSaltOkunur.png) | Salt okunur | 6 | manageGetSession | ✓ |
| [Takvim](ekranlar/isletme-uygulamasi/Takvim.png) | Takvim | 5 | manageGetCalendar | ✓ |
| [TakvimBos](ekranlar/isletme-uygulamasi/TakvimBos.png) | Boş gün | 5 | — | ✓ |
| [TakvimKapali](ekranlar/isletme-uygulamasi/TakvimKapali.png) | Kapalı gün | 5 | — | ✓ |
| [RandevuDetay](ekranlar/isletme-uygulamasi/RandevuDetay.png) | Randevu detayı | 5 | manageGetAppointment | ✓ |
| [RandevuIslem](ekranlar/isletme-uygulamasi/RandevuIslem.png) | Randevu işlemleri alt sayfası | 5 | manageRescheduleAppointment, manageCancelAppointment | ✓ |
| [KaporaOnay](ekranlar/isletme-uygulamasi/KaporaOnay.png) | Kapora geldi mi? | 5 (4) | manageMarkDepositReceived, manageRemindDeposit | ✓ |
| [OdemeAl](ekranlar/isletme-uygulamasi/OdemeAl.png) | Tamamla ve ödeme al | 5 | manageCompleteAppointment | ✓ |
| [YeniRandevu](ekranlar/isletme-uygulamasi/YeniRandevu.png) | Yeni randevu | 5 | manageSearchSlots, manageCreateAppointment | ✓ |
| [Sira](ekranlar/isletme-uygulamasi/Sira.png) | Kapıda sıra | 5 | manageListWalkIns | ✓ |
| [SiraEkle](ekranlar/isletme-uygulamasi/SiraEkle.png) | Sıraya ekle | 5 | manageCreateWalkIn | ✓ |
| [SiraBos](ekranlar/isletme-uygulamasi/SiraBos.png) | Sıra boş | 5 | — | ✓ |
| [Musteriler](ekranlar/isletme-uygulamasi/Musteriler.png) | Müşteriler | 5 | manageListCustomers | ✓ |
| [MusterilerAramaYok](ekranlar/isletme-uygulamasi/MusterilerAramaYok.png) | Arama sonucu yok | 5 | — | ✓ |
| [MusterilerBos](ekranlar/isletme-uygulamasi/MusterilerBos.png) | Müşteri yok | 5 | — | ✓ |
| [MusteriKarti](ekranlar/isletme-uygulamasi/MusteriKarti.png) | Müşteri kartı | 5 | manageGetCustomer | ✓ |
| [MusteriKartiYeni](ekranlar/isletme-uygulamasi/MusteriKartiYeni.png) | Yeni müşteri kartı | 5 | manageGetCustomer | ✓ |
| [Bildirimler](ekranlar/isletme-uygulamasi/Bildirimler.png) | Bildirimler | 5 | manageListInAppNotifications | ✓ |
| [BildirimlerBos](ekranlar/isletme-uygulamasi/BildirimlerBos.png) | Bildirim yok | 5 | — | ✓ |
| [Profil](ekranlar/isletme-uygulamasi/Profil.png) | Profil + primim | 5 | manageGetSession, manageGetCommissionReport | ✓ |
| [BildirimAyar](ekranlar/isletme-uygulamasi/BildirimAyar.png) | Bildirim ayarları | 5 | manageGetMyNotificationPreferences | ✓ |

## Bildirim ve dağıtım

Proje: `api (Notifications) + mağaza` · Klasör: `design/ekranlar/bildirim-dagitim/`

| Ekran | Ne | Dilim | API işlemleri | Koyu |
|---|---|---|---|---|
| [Main](ekranlar/bildirim-dagitim/Main.png) | SMS şablonları (15 olay) | 3 | — | — |
| [SmsTelefon](ekranlar/bildirim-dagitim/SmsTelefon.png) | SMS telefonda görünüm | 3 | — | — |
| [PushMusteri](ekranlar/bildirim-dagitim/PushMusteri.png) | Müşteri push | 3 / 5 | — | — |
| [PushIsletme](ekranlar/bildirim-dagitim/PushIsletme.png) | İşletme push | 3 / 5 | — | — |
| [PushKatalog](ekranlar/bildirim-dagitim/PushKatalog.png) | Push şablonları | 3 | — | — |
| [EpostaOnay](ekranlar/bildirim-dagitim/EpostaOnay.png) | Randevu onayı e-postası | 3 | — | — |
| [EpostaDeneme](ekranlar/bildirim-dagitim/EpostaDeneme.png) | Deneme bitiyor | 6 | — | — |
| [EpostaKontor](ekranlar/bildirim-dagitim/EpostaKontor.png) | Kontör azaldı | 6 | — | — |
| [EpostaOdemeHata](ekranlar/bildirim-dagitim/EpostaOdemeHata.png) | Ödeme alınamadı | 6 | — | — |
| [Ikonlar](ekranlar/bildirim-dagitim/Ikonlar.png) | Uygulama ikonları | 5 | — | — |
| [MagazaIsletme](ekranlar/bildirim-dagitim/MagazaIsletme.png) | Mağaza: İşletme | 5 | — | — |
| [MagazaMusteri](ekranlar/bildirim-dagitim/MagazaMusteri.png) | Mağaza: müşteri | 5 | — | — |
| [QrKart](ekranlar/bildirim-dagitim/QrKart.png) | QR masa kartı | 1 | — | — |
| [BaglantiKiti](ekranlar/bildirim-dagitim/BaglantiKiti.png) | Bağlantı paylaşım kiti | 1 | — | — |
| [Hikaye](ekranlar/bildirim-dagitim/Hikaye.png) | Hikâye görseli | 1 | — | — |
| [PaylasimOnizleme](ekranlar/bildirim-dagitim/PaylasimOnizleme.png) | Bağlantı önizlemesi (Open Graph) | 2 | publicGetBrand | — |
| [Hareket](ekranlar/bildirim-dagitim/Hareket.png) | Hareket kuralları | 1 | — | — |

## Tanıtım sitesi

Proje: `web/tanitim` · Klasör: `design/ekranlar/tanitim-sitesi/`

| Ekran | Ne | Dilim | API işlemleri | Koyu |
|---|---|---|---|---|
| [Main](ekranlar/tanitim-sitesi/Main.png) | Ana sayfa (masaüstü) | 6 | — | ✓ |
| [MainMobil](ekranlar/tanitim-sitesi/MainMobil.png) | Ana sayfa (mobil) | 6 | — | ✓ |
| [Fiyatlar](ekranlar/tanitim-sitesi/Fiyatlar.png) | Fiyatlar (masaüstü) | 6 | — | ✓ |
| [FiyatlarMobil](ekranlar/tanitim-sitesi/FiyatlarMobil.png) | Fiyatlar (mobil) | 6 | — | ✓ |
