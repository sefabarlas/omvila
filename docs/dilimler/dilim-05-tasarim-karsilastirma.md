# Dilim 5: uçtan uca akışlar 8–10 ve tasarım karşılaştırması

Dilim 5 ekranlarının tasarımla karşılaştırma kaydı, kritik akış 8, 9 ve 10'un (test-stratejisi.md) uçtan uca testleri, kabul kriterlerinin test eşlemesi ve açık kalanlar (T-114). Yöntem Dilim 4'teki gibi ([dilim-04-tasarim-karsilastirma.md](dilim-04-tasarim-karsilastirma.md)): web görüntüleri Playwright ile, mobil görüntüler golden testlerle alınır ve `design/ekranlar/` altındaki görüntüyle yan yana gözle karşılaştırılır; otomatik piksel karşılaştırması yok.

- Salon paneli: `pnpm --filter @omvila/salon-panel e2e` → `web/salon-panel/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`). Masaüstü 1440×900; Kasa ve Paketler 1199/899, Değerlendirmeler 899 kırılımları da alınır. Saat sabit: Çarşamba 25 Kasım 2026 12:10 İstanbul. 148 test.
- Rezervasyon sayfası: `pnpm --filter @omvila/rezervasyon e2e` → `web/rezervasyon/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`). Telefon 390×844. Saat sabit: Salı 24 Kasım 2026 10:00. 88 test.
- Mobil: CI goldenleri (`goldens/ci/`) metni blok olarak çizer; karşılaştırma gerçek yazı tipli platform goldenleriyle yapıldı (T-87: depoya girmez). Komut: `OMVILA_PLATFORM_GOLDENS=1 fvm flutter test --update-goldens test/goldens` (`mobile/musteri`) ve `… test/goldens_test.dart test/goldens_screens_test.dart` (`mobile/isletme`); çıktı `goldens/macos/` (gitignore). Her golden 1170×874: açık, koyu ve 1.3× yazı yan yana. Sabit saat Çarşamba 25 Kasım 2026.
- CI: `e2e` işi iki web paketini çalıştırır (görüntüler yapıtta); `flutter` işi goldenleri ve Prism'e karşı `integration_test`'leri çalıştırır.

## Kritik akış 8, 9 ve 10

Uygulamalar ayrı paketlerde ve Prism mock'a karşı çalıştığı için akışlar Dilim 3–4'teki gibi parça parça sınanır; iki uygulamayı ve gerçek backend'i aynı testte birleştiren gece koşusu henüz yok (T-72).

| Akış | Parça | Test | Kapsadığı |
|---|---|---|---|
| 8 Bekleme listesi | Motor (API) | `api/tests/Omvila.Api.IntegrationTests/WaitlistTests.cs` (`Criterion3_CancelledSlot_OfferedToMerve_HeldFor15Minutes_AcceptCreatesAppointment`, `Criterion3_OfferExpires_NextInLineGetsOffer_MerveStaysOnList`, `HeldSlot_IsClosedToOthers_OpenToOfferee`, `Accept_InApprovalSalon_CreatesPendingApproval_RequestedEvent`, `CustomerCancelFromLink_TriggersOffer`) | Boşalan saat → uygun bekleyene teklif (push → SMS), 15 dk tutma, kabulde randevu, süre dolunca sıradakine geçiş, Merve listede kalır; onaylı salonda PendingApproval (W4). |
| 8 | Müşteri (web) | `web/rezervasyon/e2e/bekleme.e2e.ts` | Dolu günde listeye katıl → kod → "listeye katıldınız"; teklifte geri sayım → "Randevuyu al" (If-Match) → Başarı; kapora isteyen salonda Havale (W4); "Bu saat olmaz" → listede kalınır, "Listeden çık"; sayfa açıkken süre dolar → BeklemeDoldu. |
| 8 | Salon (web) | `web/salon-panel/e2e/bekleme-sira.e2e.ts` (`bekleme listesi: teklifi geri al ve saat öner`) | Bekleyen teklif şeridi, "Teklifi geri al" (If-Match), "Saat öner" → `manageOfferWaitlistSlot`. |
| 8 | Müşteri (mobil) | `mobile/musteri/test/features/bildirim_ve_bekleme_test.dart` | BeklemeKatil, BeklemeTeklif (kabul → YRBasari / YRHavale), BeklemeDoldu (widget, sahte API). |
| 9 Kapıda sıra | Ekle ve başlat | `bekleme-sira.e2e.ts` (`kapıda sıra: telefonsuz sıraya ekle ve ilk kişiyi randevuya çevir`); `WalkInTests.cs` (`Criterion4_ThreeInQueue_NumbersFromOne_StartCreatesCheckedInWalkInAppointment_NextDayRestarts`); `mobile/isletme/test/features/walk_in_test.dart` | Üç kişilik sıra, telefonsuz "Sıraya ekle", "Randevuya çevir" → "… takvime “Kapıdan” kanalıyla eklendi." (bu PR'da eklendi); sunucuda numara şube/gün bazında 1'den, CheckedIn + WalkIn. |
| 9 | Tamamla ve ödeme al | `web/salon-panel/e2e/odeme.e2e.ts` (`tamamla ve ödeme al: kapora düşülür, Kart 1.240 ₺ kaydedilir (KK5)`); `CompletionTests.cs` (`Criterion5_DepositThenCard_Completed_PaymentsCommissionCountersEvent`); `mobile/isletme/test/features/appointment_detail_test.dart` (OdemeAl) | 1.250 + 190 − 200 = 1.240 ₺ Kart → Completed, %24 prim Accrued, sayaçlar, değerlendirme daveti olayı. |
| 10 Müşteri uygulaması | Giriş → randevu al → detay (Prism) | `mobile/musteri/integration_test/app_test.dart` | Telefon + SMS kodu (`meRequestOtp`/`meVerifyOtp`) → Randevularım → Kuaför Elif → Saç kesimi → Elif Kaya → 14:30 → Son kontrol → `publicCreateAppointment` (me jetonu) → "Randevunuz onaylandı" → randevu ayrıntısı (`meGetAppointment`). Bu PR'da eklendi. |
| 10 | İptal ve değerlendirme | `mobile/musteri/test/features/randevu_yonetimi_test.dart`, `randevularim_test.dart`; `MeAppointmentTests.cs` (`Cancel_ReasonRequired_CancelsWithEvent_AndReturnsThePackageSession`, `Criterion2And8_PastRow_CanRebook_NotReviewed_ReviewOnceOnlyWhenCompleted`) | İptal nedeni zorunlu ("Rahatsızlandım"), iptal sınırı, değerlendirme bir kez. |
| 10 | İşletme: giriş → Bugün → randevu ayrıntısı (Prism) | `mobile/isletme/integration_test/app_test.dart` | Yönetim OTP'si → Bugün (`manageGetToday`) → randevuya dokun → ayrıntı (`manageGetAppointment`). Bu PR'da eklendi. |

Flutter `integration_test`'leri emülatörsüz, `flutter_tester` aygıtında Prism'e karşı koşar: `pnpm --filter @omvila/contracts mock` ve `melos run integration` (içinde `flutter --show-test-device test integration_test -d flutter-tester --dart-define=API_URL=http://127.0.0.1:4010`). `-d flutter-tester` verilmezse araç cihaz arar; Prism yalnız IPv4 dinlediği için adres `127.0.0.1`. Sözleşmede `publicGetBranchCatalog`, `publicSearchDays`, `publicSearchSlots` ve `manageGetAppointment` için yanıt örneği olmadığından müşteri testi ilk üçünü test verisiyle yanıtlar (Dio interceptor), işletme ayrıntısında yalnız yapı denetlenir (T-114 (3)).

## Kabul kriterleri

| # | Kriter | Kapsayan testler |
|---|---|---|
| 1 | İlk girişte salon yoksa "QR kodu okut / Bağlantıyı yapıştır"; salon eklenince `BrandCustomer` oluşmaz | `MeAppTests.Criterion1_SaveBrand_DoesNotCreateBrandCustomer_AndSalonDoesNotSeeTheCustomer`; `mobile/musteri/test/features/randevularim_test.dart` (MainBos); golden `main_bos` |
| 2 | Geçmişi olan, yaklaşanı olmayan kullanıcı "Aynısını tekrar al" görür | `MeAppointmentTests.Criterion2And8_PastRow_CanRebook_…` (`canRebook`); `randevularim_test.dart` (MainYaklasanYok); golden `main_yaklasan_yok` |
| 3 | İptal edilen saat Merve'ye teklif (push → SMS), 15 dk; kabul → randevu; süre dolunca sıradakine, Merve listede | `WaitlistTests.Criterion3_…` (iki test); `web/rezervasyon/e2e/bekleme.e2e.ts`; panel `bekleme-sira.e2e.ts`; `mobile/musteri/test/features/bildirim_ve_bekleme_test.dart` |
| 4 | Kapıda sırada 3 kişi, numara 1'den; "Başlat" → CheckedIn, takvimde "Kapıdan" | `WalkInTests.Criterion4_…`; panel `bekleme-sira.e2e.ts` ("Kapıdan" denetimi bu PR'da eklendi); `mobile/isletme/test/features/walk_in_test.dart` |
| 5 | 1.250 + 190 − 200 = 1.240 ₺ Kart → Completed, Elif'e %24 prim, 2 saat sonra davet | `CompletionTests.Criterion5_…`; `ReviewTests.Invite_AfterCompletion_PlansPushThenSms_TwoHoursLater_Once`; panel `odeme.e2e.ts` (KK5); demo verisi (`DemoSeedTests.Seed_AddsDilim5Data`: geçmiş randevular tahsilat ve %24 primle tamamlanmış) |
| 6 | 10 seanslık Fön paketi %10 satış primi; online "Fön paketinizden düşülsün"; iptal → seans geri, tarihli; gelmedi → dönmez | `PackageTests.Criterion6_SellTenSessionFon_PaymentAndTenPercentSaleCommission`, `Criterion6_OnlineReserve_CancelReturnsDated_NoShowBurns`; panel `paketler.e2e.ts`; rezervasyon `bekleme.e2e.ts` (paket kutusu); demo verisi (Fön paketi %10, `DemoSeedTests.Seed_AddsDilim5Data`) |
| 7 | Beklenen 3.450, sayılan 3.400 → fark −50 ve açıklama | `CashDayTests.Criterion7_Expected3450Counted3400_DifferenceMinus50_NoteRequired`; panel `odeme.e2e.ts` (kasa sayımı); demo verisi (dünün kapatılmış günü, fark −50) |
| 8 | Değerlendirme yalnız tamamlananda ve bir kez; ikincisi `already_reviewed` | `ReviewTests.Criterion8_Link_ShowsForm_Submit_ThenAlreadyReviewed`, `Submit_NotCompleted_IsInvalidStateTransition_…`; `MeAppointmentTests.Criterion2And8_…`; rezervasyon `bekleme.e2e.ts` (değerlendirme) |
| 9 | Hesap silme SMS koduyla; yaklaşan randevular iptal; tüm markalarda anonim; kayıtlar anonim kalır | `MeAppTests.Criterion9_AccountDeletion_AnonymizesEveryBrand_KeepsRecords_CancelsUpcoming_AndSignInFindsANewAccount`; `mobile/musteri/test/features/hesap_test.dart`; salona iptal bildirimi `MeAppointmentTests.CustomerCancel_NotifiesStaffAndManagersOnce_…` (#120) |
| 10 | Personel yalnız kendi randevularını ve primini görür, telefon maskeli, arama çalışır | `BusinessAppTests.Today_Staff_SeesOnlyOwnAppointments_…`, `CommissionSummary_IsOnlyTheUsersOwn`, `RevealPhone_IsAudited_NotCached_StaffOnlyForOwnCustomers`; `mobile/isletme/test/features/today_screen_test.dart`, `appointment_detail_test.dart`, `customers_test.dart` |
| 11 | Golden testler: ana ekranlar açık/koyu ve 1.3× | `mobile/musteri/test/goldens/`, `mobile/isletme/test/goldens_test.dart`, `goldens_screens_test.dart` (CI goldenleri); eksik: işletme MainSaltOkunur, müşteri Durumlar'ın üç deseni (aşağıda) |

**Dilim 4 KK9 (müşteri uygulamasında Ödemelerim) de kapandı:** `MeAppointmentTests.Deposit_ReportedFromApp_ShowsAwaitingConfirmation_ThenRecordedAfterSalonMarksIt`, `Packages_ListAllBrands_…_AndPackagePurchaseInPayments`; `mobile/musteri` Ödemelerim ekranı (T-112 (4)), golden `odemelerim`, `odemelerim_bos`.

## Salon paneli

Durum: ✓ tasarımla uyumlu, ≈ bilinçli/kapsam dışı farkla uyumlu, ✗ düzeltilmesi gereken fark. Bütün panel görüntülerinde Prism oturum örneğinden gelen "Deneme süreniz bugün bitiyor" ve "SMS kontörünüz 180'e düştü" şeritleri, üst barda arama kutusunun olmaması (T-57), tek şubede şube seçicinin olmaması, "Raporlar ve prim" öğesinin olmaması (Dilim 6, K2) ve kenar menüde sayı rozetlerinin olmaması (T-98 (2), T-99 (1)) ortaktır; aşağıda yinelenmez.

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| BeklemeListesi | BeklemeListesi | BeklemeListesiKoyu | ≈ | Üç bento, "Perşembe 11:00 boşaldı" şeridi, tablo ve alt not tasarımdaki gibi. Tercih kolonu sözleşmedeki alanlardan ("Per–Cum · sabah"; tasarımdaki "15:00 sonrası" sözleşmede yok, T-98 (5)); eylem kolonu başlığı "Eylem" (T-98 (6)). |
| BeklemeListesiBos | BeklemeListesiBos | BeklemeListesiBosKoyu | ✓ | Boş durum metni ve "Listeye ekle"; "15 dakika" `offerHoldMinutes`'tan (T-98 (3)). |
| KapidaSira | KapidaSira | KapidaSiraKoyu | ≈ | Sırada 3 · en eskisi 12 dk, ~35 dk, Bugün sıradan 7; ilk satırda "Randevuya çevir", diğerlerinde "Sıradan çıkar". Menüde Takvim seçili (Takvim altında alt rota; T-98 (1)). |
| KapidaSiraBos | KapidaSiraBos | KapidaSiraBosKoyu | ✓ | "Sırada kimse yok" ve metin tasarımdaki gibi. |
| OdemeAl | OdemeAl | OdemeAlKoyu | ≈ | Kalemler, "Kapora (havale, 11 Kas) −200 ₺", "Alınacak 1.240 ₺", Kart, "İkinci ödeme ekle", prim/davet metni tasarımdaki gibi. "Paketten" yalnız uygun paket kaleminde görünür (T-97 (3)); pencere 480 px (T-97 (7)). |
| Kasa (+1199, 899) | Kasa, Kasa1199, Kasa899 | …Koyu | ✓ | Dört bento, tahsilat tablosu (Kart/Nakit/Paketten/Havale), "Kasa sayımı" kartı (sayılan 3.400, beklenen 3.450, fark −50 ₺); 899'da bentolar 2×2. Başlıkta "Çarşamba, 25 Kasım" (tasarımdaki "Salı" tarihle uyuşmuyor). |
| KasaBos | KasaBos | KasaBosKoyu | ✓ | Sıfır bentolar, boş durum, "Bugünkü randevular". |
| Paketler (+1199, 899) | Paketler, Paketler1199, Paketler899 | …Koyu | ≈ | Bentolar, sekmeler, tanım tablosu, satır menüsü, "Süresi yaklaşanlar" tasarımdaki gibi; kartlar son güne göre sıralı; "Hepsine SMS" yok (T-102 (2)). |
| PaketlerBos | PaketlerBos | PaketlerBosKoyu | ✓ | Boş durum ve "Paket oluştur"; satılacak paket yokken satış butonu gizli (T-102 (1)). |
| PaketOlustur | PaketOlustur | PaketOlusturKoyu | ≈ | Alanlar, içerik satırları, geçerlilik, satış primi, şube anahtarı tasarımdaki gibi; seans sayısı satırda düzenlenir, satış primi alt metni T-102 (5). |
| PaketSat | PaketSat | PaketSatKoyu | ≈ | Müşteri, paket, satan personel, geçerlilik, tutar, yöntem, "1.700 ₺ al ve paketi tanımla" tasarımdaki gibi. Müşteri seçici yeni randevu deseninde (T-102 (6)); "son Bugün" büyük harf hatası bu PR'da düzeltildi. |
| Degerlendirmeler (+Tümü, 899) | Degerlendirmeler, DegerlendirmelerTumu, Degerlendirmeler899 | …Koyu | ≈ | Bentolar, filtre çipleri, yanıt alanı, "Yanıtınız · Elif Kaya" bloğu tasarımdaki gibi; yıldızlar amber (bu PR'da düzeltildi). Varsayılan "Yanıt bekleyen" filtresinde yanıtlanmış kart görünmez (T-99 (1)). |
| DegerlendirmelerBos | DegerlendirmelerBos | DegerlendirmelerBosKoyu | ✓ | Boş durum metni ("2 saat", "En az 5"; T-99 (1)). |
| MusteriKarti: Paketler | MusteriKartiPaketler | MusteriKartiPaketlerKoyu | ≈ | Paket bentosu, sekmeler, paket satırı. Üst barda "Paket sat" + "Randevu ver" (T-102 (7)); sekme sayı rozetleri yok (T-99 (2)); sol kartta "İzinler (KVKK)" yok (manage ucu yok; T-57, T-111). |
| MusteriKarti: Ödemeler | MusteriKartiOdemeler | MusteriKartiOdemelerKoyu | ≈ | Tasarımda görüntü yok; tarih, kalem, yöntem, durum rozeti, tutar (T-99 (2)). |
| MusteriKarti: Değerlendirmeler | MusteriKartiDegerlendirmeler | MusteriKartiDegerlendirmelerKoyu | ≈ | Tasarımda görüntü yok; Degerlendirmeler kart deseni. |
| MusteriBirlestir | MusteriBirlestir | MusteriBirlestirKoyu | ≈ | Kalacak / Birleşecek kartları, "Notları birleştir" tasarımdaki gibi; birleşecek kayıt pencerede aranır (T-99 (3)). |
| MusteriIceAktar | MusteriIceAktar | MusteriIceAktarKoyu | ≈ | Dosya kartı, üç bento, "Hatalı satırları indir", "1.242 kaydı içe aktar"; SMS onay kutusu varsayılan kapalı (T-99 (5)). |
| MusteriSil | MusteriSil | MusteriSilKoyu | ✓ | Geri alınamaz uyarısı, Silinecek / Korunacak / Aktif paket, ad yazma alanı (T-99 (4)). |
| AyarlarKvkk | AyarlarKvkk | AyarlarKvkkKoyu | ≈ | "Silme talepleri" tablosu tasarımdaki gibi; aydınlatma metni, veri saklama ve "Tüm verileri dışa aktar" Dilim 6 (T-99 (6)). |

## Rezervasyon sayfası

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| DoluGun: bekleme listesi formu | DoluGun | DoluGunKoyu | ≈ | "Perşembe dolu" kartı, saat dilimi çipleri, "Başka personel de olur", "Bekleme listesine katıl" tasarımdaki gibi. "Bağlantıdaki süre içinde" (tutma süresi public sözleşmede yok; T-100 (3)); çipler başlangıçta seçili değil (T-100 (1)). |
| BeklemeTeklif | BeklemeTeklif | BeklemeTeklifKoyu | ≈ | Tutma kartı, geri sayım, özet, "Randevuyu al" / "Bu saat olmaz" tasarımdaki gibi; ilerleme çubuğu açılışta dolu (toplam tutma süresi bilinmiyor; T-100 (4)). |
| BeklemeDoldu | BeklemeDoldu | BeklemeDolduKoyu | ≈ | Kum saati, "Bu saatin süresi doldu", "Başka saat seç", "Listeden çık"; "15 dakikalık" yerine "ayrılan süre" (T-100 (3)). |
| Onay: paket kutusu | OnayPaket | OnayPaketKoyu | ≈ | "Fön paketinizden düşülsün" kutusu, özet "Fön · Paketten", "Salonda ödenecek 350 ₺"; kapora satırı ve metni Dilim 4 kararlarıyla (T-80 (2), T-100 (6)). |
| Değerlendir (/d/) | Degerlendir, DegerlendirZaten | …Koyu | ≈ | Web tasarımı yok; `musteri-uygulamasi/Degerlendir` ile karşılaştırıldı (T-100 (5)). DegerlendirZaten: "Bu randevuyu zaten değerlendirdiniz". |

## Müşteri uygulaması

Bütün goldenlerde ortak olan ve aşağıda yinelenmeyen farklar (≈): üstteki şerit alchemist'in sahte durum çubuğudur ve golden 874 px'tir (alt güvenli alan yok, uzun ekranlar kesilir); gövde metni tasarım sistemi gereği 16 px olduğundan daha erken kırılır; yüzen sekme çubuğunda seçili hap ve koyu temada kapalı switch başparmağı T-87'deki gibi; kök gezginsiz pump edilen ekranlarda geri oku çizilmez; sabit saat ve sayılar test verisidir.

| Ekran | Golden | Durum | Not |
|---|---|---|---|
| Acilis | acilis | ✓ | |
| Giris | giris | ≈ | Golden boş telefon; "aydınlatma metnini" bağlantısız (`PRIVACY_NOTICE_URL` tanımsız; T-106 (5)). |
| GirisKod | giris_kod | ≈ | Golden boş kod. |
| BildirimIzni | bildirim_izni | ≈ | Örnek bildirim metni dilbilgisi düzeltmesiyle. |
| Main | main | ≈ | Zil noktası test verisinde yok; açık temada "Yol tarifi" zemini kum. |
| MainBos | main_bos | ≈ | Koyu temada "Bağlantıyı yapıştır" dolu zeminli (küçük). |
| MainYaklasanYok | main_yaklasan_yok | ≈ | "Geçmiş randevular" bağlantısı eklendi (T-108 (5)). |
| MainBildirimKapali | main_bildirim_kapali | ✓ | |
| Paketler | paketler | ✗ | "Az kaldı" rozeti uyarı tonu yerine beyaz (`paketler_ekrani.dart`). |
| PaketlerBos | paketler_bos | ✓ | |
| Profil | profil | ✗ | Kullanıcı avatarı ve salon kapak rengi eşlemesi ters (petrol → kum, kum → nane; `profil_ekrani.dart`, `salon_secimi_ekrani.dart`). "Profil" başlığı bilinçli (T-112 (2)). |
| ProfilDuzenle | profil_duzenle | ✗ | 1.3×'te telefon ikiye bölünüyor; yardım metni girintili. Dil seçimi yok (T-101 (2), ≈). |
| Odemelerim | odemelerim | ✓ | |
| OdemelerimBos | odemelerim_bos | ✓ | |
| Izinler | izinler | ✓ | |
| HesapSil | hesap_sil | ✓ | "Salonlara iptal bildirimi gider" bu PR'da eklendi (bildirim #120'de bağlandı; T-112 (7) güncellendi). |
| HesapSilindi | hesap_silindi | ✗ | "Kapat" ikincil buton çerçeveli (ortak fark 1). |
| Bildirimler | bildirimler | ≈ | "15 dakika içinde" yerine "kısa süre" (T-112 (8)). |
| BildirimlerBos | bildirimler_bos | ✓ | |
| BeklemeKatil | bekleme_katil | ≈ | Tutma süresi sayısız (T-112 (8)). |
| BeklemeTeklif | bekleme_teklif | ≈ | İlerleme çubuğu açılışta dolu (rezervasyon sayfasıyla aynı; T-100 (4)). |
| BeklemeDoldu | bekleme_doldu | ✗ | "Başka saat seç" birincil (tasarımda ikincil). "Bir sonraki boşalmada haber ver" yok (T-112 (9), ≈). |
| Durumlar | durumlar | ≈ | Golden iki deseni çizer; "Saat az önce doldu", "Kapora gelmedi", "Bağlantı yok" goldeni yok. |
| YRSalon | salon_secimi | ≈ | Avatar renkleri kapak eşlemesine bağlı (Profil ✗). |
| YRSalonBos | yr_salon_bos | ✓ | |
| YRHizmet | yr_hizmet | ≈ | Kategori test verisi. |
| YRPersonel | yr_personel | ✗ | Personel avatarları hep nane (ortak fark 3). |
| YRSaat | yr_saat | ≈ | 17:30 "Akşam" grubunda (`SaatIzgarasi` dönem eşiği). |
| YROnay | yr_onay | ≈ | Kapora metni süresiz (T-108 (1)). |
| YRHavale | yr_havale | ✗ | Başlık `buyukBaslik` ile iki satır ve açıklamaya yapışık (`havale_ekrani.dart`; tasarımda `baslik-1`). |
| YRBasari | yr_basari | ✗ | "Yol tarifi" / "Paylaş" çerçeveli (ortak fark 1). |
| RandevuDetay | randevu_detay | ✗ | Eylem butonları çerçeveli (ortak fark 1); salon avatarı nane. |
| RandevuErtele | randevu_ertele | ✗ | "Şu anki randevunuz" tarihi 1×'te kırılıyor. |
| ErteleKapali | ertele_kapali | ✓ | |
| Iptal | iptal | ≈ | "Hastayım" yerine "Rahatsızlandım" (T-108 (4)). |
| Gecmis | gecmis | ✗ | Segment kontrolü yerine çip; "600 / ₺" satır sonunda bölünüyor (ortak fark 2). |
| GecmisBos | gecmis_bos | ✗ | Boş hâlde süzgeç çipleri görünüyor (küçük). |
| Degerlendir | degerlendir | ≈ | Kapat düğmesi zeminsiz; salon avatarı nane. |

## İşletme uygulaması

| Ekran | Golden | Durum | Not |
|---|---|---|---|
| Acilis | acilis | ✗ | Amblem tasarımdan çok küçük; koyu temada arkasında koyu kare. |
| Giris | giris | ✗ | Telefon alanında etiket ve "+90" öneki yok (müşteri uygulamasında var); amblem küçük. |
| GirisKod | giris_kod | ≈ | Golden boş kod. |
| BildirimIzni | bildirim_izni | ≈ | "2 saat içinde" yazılmadı (T-107 (7)). |
| Main (Bugün) | bugun | ✗ | "+ Randevu" siyah hap yerine marka yeşili; "50 dk sonra" çipi beyaz ve yukarıda; sekme çubuğunda okunmamış rozeti verilmiyor (`app_shell.dart`); 1.3×'te gün akışı adı sıkışıyor. |
| MainBos | bugun_bos | ≈ | "ÇALIŞMA 10–20" bloğu yok (T-107 (6)). |
| MainBildirimKapali | bugun_bildirim_kapali | ✗ | Main satırındaki farklar. |
| MainSaltOkunur | — | ✗ | Golden yok; Bugün'de salt okunur şerit ve pasif "+ Randevu" uygulanmamış (takvim ve yeni randevuda var). |
| Takvim | takvim | ≈ | "Bekleme · müsait" kutusu düz çerçeveli (küçük). |
| TakvimBos | takvim_bos | ✗ | "Randevu ekle" ikincil (tasarımda birincil). |
| TakvimKapali | takvim_kapali | ≈ | Seçili kapalı gün marka zemininde (küçük). |
| RandevuDetay | randevu_detay | ≈ | Personelde ara/SMS gizleme notu yok (T-109 (4)). |
| RandevuIslem | randevu_islem | ≈ | "Hizmet ekle" ve "SMS gönder" yok (T-109 (5)). |
| OdemeAl | odeme_al | ≈ | "Kapora (havale)" (ürün kuralı 1); varsayılan Nakit (T-109 (7)). |
| KaporaOnay | kapora_onay | ✗ | "Henüz gelmedi, hatırlat" çerçeveli (ortak fark 1). |
| YeniRandevu | yeni_randevu | ≈ | Tek hizmet (T-109 (8)). |
| Sira | sira | ✗ | Koyu temada ilk satırın numarası ve "Başlat" okunmuyor (`walk_in_screen.dart`). |
| SiraBos | sira_bos | ✗ | "Sıraya ekle" ikincil (tasarımda birincil). |
| SiraEkle | sira_ekle | ✗ | Ad/Telefon etiketleri ve "+90" öneki yok; "No 11" rozeti yok (T-111 (1), ≈). |
| Musteriler | musteriler | ≈ | Süzgeçler Düzenli/Yeni/Paketli (T-111 (2)); avatarlar hep nane. |
| MusterilerAramaYok | musteriler_arama_yok | ✗ | "Filtreyi temizle" çerçeveli (ortak fark 1). |
| MusterilerBos | musteriler_bos | ✗ | "Müşteri ekle" ikincil (tasarımda birincil). |
| MusteriKarti | musteri_karti | ✗ | Ad altında tam telefon yazıyor (tasarımda yok; T-111 (3) ile karşılaştırılmalı). Not düzenleme ve İzinler yok (T-111 (3), ≈). |
| MusteriKartiYeni | musteri_karti_yeni | ≈ | "Not ekle" yok (T-111 (3)). |
| Bildirimler | bildirimler | ✗ | Sekme rozeti yok; 1.3×'te segment etiketleri çakışıyor; "Reddet" çerçeveli. |
| BildirimlerBos | bildirimler_bos | ✗ | Boş hâlde segment kontrolü görünüyor; 1.3× çakışması. |
| Profil | profil | ✗ | "Bu hafta" grafiği yalnız verideki günleri çiziyor (sabit haftalık sütun olmalı); avatar nane. |
| BildirimAyar | bildirim_ayar | ≈ | Sessiz saat salt okunur (T-111 (6)). |

## Bulunan farklar

**Bu PR'da düzeltilenler (web):**
- Panelde değerlendirme yıldızları siyahtı: `--yildiz` yalnız mobil ölçekte tanımlıydı; panel ölçeğine aynı değerde semantik `yildiz` tokenı eklendi.
- Müşteri seçicide "son Bugün" cümle ortasında büyük harfle, müşteri kartında "21 ekim" küçük harfle yazılıyordu; `inlineDayLabel` (birim testli).
- Panel e2e demolarında sahibin izin listesi eksikti (menü ve butonlar ekrandan ekrana değişiyordu): ortak `ownerPermissions`.
- KK4 "takvimde Kapıdan kanalıyla" adımı panel testine eklendi.
- Müşteri uygulaması HesapSil uyarısına "Salonlara iptal bildirimi gider." eklendi (bildirim #120'de bağlandı).

**Mobilde açık kalan ✗ farklar (ayrı düzeltme PR'ı; T-114 (6)):**
1. Ortak: `OmvilaButon(tur: ikincil)` çerçeveli (`packages/omvila_ui/lib/widgets/buton.dart`); tasarımda dolu nötr (`zemin-ikincil`).
2. Ortak: `formatMoney` tutar ile "₺" arasında normal boşluk (`packages/omvila_ui/lib/format.dart`); bölünmez boşluk olmalı.
3. Ortak: avatar tonları hep nane; tasarımda mercan/kum/nane dönüşümlü.
4. Müşteri: kapak rengi eşlemesi ters; "Az kaldı" rozeti beyaz; YRHavale başlığı; RandevuErtele tarih kırılması; Geçmiş segment kontrolü ve boş hâlde süzgeç; BeklemeDoldu birincil buton; ProfilDuzenle 1.3×.
5. İşletme: Bugün'de salt okunur hâl yok; sekme çubuğunda okunmamış rozeti verilmiyor; kapıda sıra koyu temada ilk satır okunmuyor; telefon alanları etiketsiz ve öneksiz; boş durumların ana eylemi ikincil; Bugün "+ Randevu" ve "50 dk sonra" çipi; amblem küçük; Bildirimler 1.3× segment çakışması; Profil haftalık grafik; müşteri kartında tam telefon.
6. Golden kapsamı: işletme MainSaltOkunur; müşteri Durumlar'ın "Saat az önce doldu", "Kapora gelmedi", "Bağlantı yok" desenleri.

Web'de başka ✗ fark yok; listelenen bütün web ekranlarının görüntüsü e2e'de vardır.

## Açık kalanlar

Dilim 5 kapsamında bilinçli olarak yapılmayan veya dışa bağlı işler (T-114):

- İşletme uygulamasında müşteri notu düzenleme: uç yok (sözleşme).
- Müşteri kartında KVKK izinleri (panel ve işletme uygulaması): manage ucu yok.
- Kişisel bildirim tercihlerinde sessiz saat aralığı salt okunur (sözleşme).
- `staff.daily_summary` (günlük özet push'u) işi yazılmadı.
- Salona "Saat değişti" push'u bağlı değil (müşteri erteleme).
- Paketler "Hepsine SMS": bildirim kataloğunda olay yok.
- Menü sayaç rozetleri (panel kenar menüsü, müşteri kartı sekmeleri).
- Mağaza hesapları, imzalama, Firebase/APNs (K4): sahte push ve emülatör; mağaza yayını ayrı adım.
- Uygulama ikonu ve açılış görseli (mağaza varlıkları).
- i18n: yalnız Türkçe.
- H-04 hukuk metinleri (hesap silme, kapora/iade, SSS).
- S-10 paket iade hesabı 🔴 (şimdilik W9: tutarı salon elle girer).
- Mobil tasarım farkları (yukarıdaki ✗ listesi).
- Uçtan uca gece koşusu (iki uygulama + gerçek backend aynı testte) yok (T-72).
