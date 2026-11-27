# Dilim 6: uçtan uca akışlar ve tasarım karşılaştırması

Dilim 6 ekranlarının tasarımla karşılaştırma kaydı, abonelik ve havale eşleştirme akışlarının uçtan uca testleri, kabul kriterlerinin test eşlemesi ve açık kalanlar (T-131). Yöntem Dilim 5'teki gibi ([dilim-05-tasarim-karsilastirma.md](dilim-05-tasarim-karsilastirma.md)): web görüntüleri Playwright ile alınır ve `design/ekranlar/` altındaki görüntüyle yan yana gözle karşılaştırılır (açık + koyu); otomatik piksel karşılaştırması yok.

- Salon paneli: `pnpm --filter @omvila/salon-panel e2e` → `web/salon-panel/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`, `e2e/abonelik.e2e.ts`). Masaüstü 1440×900; Raporlar 1199/899, AyarlarAbonelik, AyarlarRoller ve AyarlarKvkk 899 kırılımları da alınır. Saat sabit: Çarşamba 25 Kasım 2026 12:10 İstanbul (Raporlar: Pazar 29 Kasım 2026 20:00). 196 test.
- Platform paneli: `pnpm --filter @omvila/platform-panel e2e` → `web/platform-panel/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`). Masaüstü 1440×900; Genel bakış 1199/899. Onay, abonelik, denetim ve plan ekranlarında saat sabit: Cuma 20 Kasım 2026 10:00 İstanbul; Genel bakış ve Markalar tarayıcının saatiyle çekilir (başlıktaki "Bugün · …" koşu gününü yazar). Oturum Prism örneğindeki Destek rolüdür; Yönetici ayrıca çekilir. 42 test.
- Tanıtım sitesi: `pnpm --filter @omvila/tanitim e2e` → `web/tanitim/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`). Masaüstü 1440 px, mobil 390 px. 19 test.
- Rezervasyon sayfası: `pnpm --filter @omvila/rezervasyon e2e` (`e2e/kapali.e2e.ts`; görüntü `DurumOnlineKapali`). 90 test.
- Mobil ve e-posta: bu dilimin tek mobil ekranı ve üç e-postası için mevcut golden ve testlere atıf yapılır (aşağıda); yeniden üretilmedi.
- CI: `e2e` işi dört web paketini çalıştırır (görüntüler yapıtta).

## Kritik akışlar

Uygulamalar ayrı paketlerde ve Prism mock'a karşı çalıştığı için akışlar Dilim 3–5'teki gibi parça parça sınanır; salon paneli, platform paneli ve gerçek backend'i aynı testte birleştiren gece koşusu henüz yok (T-72).

| Akış | Parça | Test | Kapsadığı |
|---|---|---|---|
| Abonelik durumları (KK1) | Motor (API) | `api/tests/Omvila.Api.IntegrationTests/SubscriptionTests.cs` (`Criterion1_TrialEnds_PastDue_ThenReadOnly_BookingClosed_NewAppointmentForbidden_ExistingReminderSent`, `ReadOnly_CancelledAfterConfiguredDays_ThenBrandArchived`); `api/tests/Omvila.Modules.Platform.Tests/SubscriptionTests.cs` (`MarkPastDue_…`, `MarkReadOnly_…`) | Deneme biter → `PastDue`; hatırlatma süresi sonunda `ReadOnly`; rezervasyon kapanır, yeni randevu 403, var olan randevunun hatırlatması gider. |
| KK1 | Salon (web) | `web/salon-panel/e2e/salt-okunur.e2e.ts` (iki test; bu çalışmada eklendi); `e2e/abonelik.e2e.ts` (`abonelik şeritleri`) | Ödeme geciktiğinde uyarı (turuncu) şeridi "3. gün", "5 gün içinde havale ile ödemezseniz…", yeni randevu açık; salt okunurda hata (kırmızı) şeridi, "Randevu ekle" pasif ve nedeni yazar, takvimde boş dilim buton değil, boş alana tıklama pencere açmaz, var olan randevu açılır. "Havale ile öde" Abonelik ve fatura sayfasını açar. |
| KK1 | Müşteri (web) | `web/rezervasyon/e2e/kapali.e2e.ts` (iki test; bu çalışmada eklendi) | `bookingEnabled: false` salonda akış açılmaz: "Kuaför Elif şu an online randevu almıyor.", "Salonu ara" (`tel:`); adım adresi elle yazılsa da açılmaz. Akış ortasında oluşturma 403 `booking_disabled` dönerse aynı desen, "Randevunuz onaylandı" yazmaz. |
| Havale ile abonelik (KK2) | Salon (web) | `web/salon-panel/e2e/abonelik.e2e.ts` (`abonelik havalesini bildir: gönderen adı ve dekont multipart gider`, `kontör al: paket → KNT- havale bilgisi → bildir (purpose Credits)`) | "Havaleyi bildir" (purpose, gönderen adı, dekont); kontörde KNT- kodu ve `creditPackageId`. |
| KK2 | Platform (web) | `web/platform-panel/e2e/eslestirme.e2e.ts` (`havale bildirimi "Kod tutuyor" hareketle eşleştirilir`) | Destek: Onay kuyrukları → İncele → "Kod tutuyor" hareketi seçili → "Seçili hareketle eşleştir" (If-Match) → "… markasına bildirim gitti." → kuyruk. |
| KK2 | API | `AdminApprovalTests.Criterion2_SalonNotifies_SupportMatchesCodeMatchingTransaction_InvoicePaid_SubscriptionActive_BrandNotified_Audited`; `Omvila.Modules.Platform.IntegrationTests/BillingIntegrationTests.Match_SubscriptionPastDue_IssuesAndPaysInvoice_Activates_WritesAuditAndEvent` | Bildirim → eşleştirme → fatura `Paid`, abonelik `Active`, markaya bildirim, denetim kaydı. |
| Uyuşmayan bildirim (KK3) | Platform (web) | `web/platform-panel/e2e/karar-ve-yetki.e2e.ts` (`uyuşmayan harekette "Kontrol edin" nedeni; "Bulunamadı, markaya yaz" → NotFound`; bu çalışmada eklendi); `eslestirme.e2e.ts` ("Kontrol edin:" görünür ve kaybolur) | Kodu olmayan ve tutarı farklı adaylarda uyarı nedeni; uyarıya rağmen eşleştirme açık; "Bulunamadı, markaya yaz" notu ve If-Match ile gider. |
| KK3 | API | `AdminApprovalTests.Criterion3_MismatchShowsCheckRequired_NotFoundAndReject_NotifyBrandWithMessage` | `CheckRequired`, `NotFound` / `Rejected`, markaya notlu bildirim. |
| Rol sınırı (KK4) | Platform (web) | `karar-ve-yetki.e2e.ts` (`Destek: "Kontör ekle" ve "Planı değiştir" kilitli, nedeni ipucunda`, `Yönetici kontör ekler: adet ve neden gider`; bu çalışmada eklendi) | Destek'te iki buton pasif, ipucu "Yönetici yetkisi ister", yazma isteği gitmez; Yönetici `{ credits, reason }` gönderir (gövdeyi Prism sözleşmeye göre doğrular). |
| KK4 | API | `AdminPlatformTests.Criterion4_SupportAndFinanceGet403_AdminGrantsCredits_AndEachWriteIsAudited`, `Criterion4_AdminChangesStatusAndPlan_WithIfMatch_LimitsAndAudit`; `AdminApprovalTests.Criterion4_SupportCannotSavePlansPackagesOrImport_FinanceSavesPlans_AndIsAudited` | 403, kontör ve plan değişikliği, her yazmanın denetim kaydı. |
| Raporlar (KK6) | Salon (web) | `web/salon-panel/e2e/raporlar.e2e.ts` (iki test; bu çalışmada eklendi) | Hak edilen gelir 61.200 ₺ ve tahsilat 67.950 ₺ ayrı kartlarda ve ayrı alt metinle; günlük tahsilat tablosu; prim toplamı; dönem adreste ve sorguda (hafta Pazartesi – bugün, ay 1'i – bugün); boş hâlde tutar yok. |
| KK6 | API | `ReportTests.Criterion6_PrepaidPackage_FullInCollections_EarnedAsSessionsAreUsed`, `EarnedRevenueChange_AgainstPreviousEqualRange` | Peşin paket tahsilatta tam, hak edilen gelirde seans kullanıldıkça. |
| Tanıtım sitesi (KK7) | Web | `web/tanitim/e2e/icerik.e2e.ts` (altı test; bu çalışmada eklendi); `e2e/gezinme.e2e.ts` | İki sayfada köşeli parantezli yer tutucu, kanıtsız sayı, yorum ve yabancı görsel yok; plan fiyatı, sınırlar ve kontör paketleri `content/pricing.json`'dan (değer yoksa "Fiyat yakında açıklanacak"); sayfadaki bütün "ücretsiz dene" bağlantıları panel `/kayit` adresine gider. |

## Kabul kriterleri

| # | Kriter | Kapsayan testler |
|---|---|---|
| 1 | Deneme bitti, ödeme yok → `PastDue`, turuncu şerit; 7 gün sonra `ReadOnly`: rezervasyon "Online randevu kapalı", panelde yeni randevu pasif, var olan randevular ve hatırlatmalar sürer | `SubscriptionTests.Criterion1_TrialEnds_PastDue_ThenReadOnly_…`; `PublicBookingTests.Criterion17_ReadOnlySubscription_BookingDisabled`; `AppointmentTests.Criterion17_ReadOnlySubscription_BlocksNewAppointment`; panel `salt-okunur.e2e.ts`, `abonelik.e2e.ts` (şeritler), `subscription.test.tsx` (gün sayıları, rol); rezervasyon `kapali.e2e.ts`; ui-web `calendar.test.tsx` (`tıklama işleyicisi yokken boş dilimler buton değil, düz hücredir`); mobil `today_screen_test.dart` (salt okunur şerit, pasif "+ Randevu") |
| 2 | "Havaleyi bildir" → platform onay kuyruğu; Destek "Kod tutuyor" hareketle eşleştirir → fatura `Paid`, abonelik `Active`, bildirim, denetim | `AdminApprovalTests.Criterion2_…`; `BillingIntegrationTests.Match_SubscriptionPastDue_…`; panel `abonelik.e2e.ts`; platform `eslestirme.e2e.ts`, `approvals.test.tsx` (`"Kod tutuyor" adayı listeden eşleştirilir (If-Match)`) |
| 3 | Tutar veya kod uyuşmazsa "Kontrol edin"; "Bulunamadı, markaya yaz" → `NotFound` | `AdminApprovalTests.Criterion3_…`; platform `karar-ve-yetki.e2e.ts` (KK3), `eslestirme.e2e.ts`, `approvals.test.tsx` (`"Bulunamadı, markaya yaz" isteğe bağlı notla NotFound yapar`, `ret notu zorunlu`) |
| 4 | Destek kontör ekleyemez, plan değiştiremez (butonlar pasif, API 403); Yönetici ekler, denetime yazılır | `AdminPlatformTests.Criterion4_…` (iki test); `AdminApprovalTests.Criterion4_…`; platform `karar-ve-yetki.e2e.ts` (KK4, iki test), `ekranlar.e2e.ts` (PMarkaDetay, PMarkaDetayYonetici), `brands.test.tsx` |
| 5 | Marka detayını açmak `AuditLog(View)` üretir; telefonlar maskeli | `AdminPlatformTests.Criterion5_EveryBrandOpeningAndTabIsAudited_PhonesMasked`; `AdminApprovalTests.AuditLog_FiltersSearchMasksAndExport_OnlyAdminExports`; platform `giris.e2e.ts` ("Bu görüntüleme denetim kaydına yazıldı.", "0532 *** ** 05") |
| 6 | Peşin satılan paket tahsilatta; hak edilen gelirde seans kullanıldıkça | `ReportTests.Criterion6_PrepaidPackage_FullInCollections_EarnedAsSessionsAreUsed`; panel `raporlar.e2e.ts`, `reports.test.tsx` |
| 7 | Tanıtım sitesinde uydurma sayı, yorum, logo yok; fiyatlar yapılandırmadan; "Ücretsiz dene" panel kaydına | tanıtım `icerik.e2e.ts`, `gezinme.e2e.ts`, `pages.test.tsx` (`sektöre daraltan, yer tutucu veya ödeme alan ifade yok`, `üç plan, önerilen rozeti ve değer yoksa "Fiyat yakında açıklanacak"`) |

## Salon paneli

Durum: ✓ tasarımla uyumlu, ≈ bilinçli/kapsam dışı farkla uyumlu, ✗ düzeltilmesi gereken fark. Bütün panel görüntülerinde ortak olan ve aşağıda yinelenmeyen farklar: Prism oturum örneğinden gelen "Deneme süreniz bugün bitiyor" ve "SMS kontörünüz 180'e düştü" şeritleri; üst barda arama kutusunun olmaması (T-57); tek şubede şube seçicinin olmaması; kenar menüde sayı rozetlerinin olmaması (T-98 (2), T-99 (1)); kullanıcı satırındaki profil menüsü oku (T-129; tasarım görüntüleri menüden önce çizilmiş); avatar zemininin ada göre renk alması.

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| AyarlarAbonelik (+899) | AyarlarAbonelik, AyarlarAbonelik899 | …Koyu | ≈ | Deneme şeridi, PLAN / ŞUBE / PERSONEL kartları, Faturalar tablosu (Ödendi, PDF), "Havale / EFT ile öde" kutusu tasarımdaki gibi. "Kartla öde" kartı ve üst bardaki "Ödeme yöntemini kaydet" yok (T-06; T-125 (1)), havale kutusu tam genişlik; satırlar kopyalanabilir (T-125 (3)); yer tutucular yerine demo değerleri; PERSONEL kartında sınır tanımsızken alt satır yok (T-125 (2)); metinler T-125 (4). |
| AyarlarAbonelik: tanımsız | AyarlarAbonelikTanimsiz | …Koyu | ≈ | Tasarımda görüntü yok. OMVILA hesabı ve fiyat tanımsızken (S-14) "OMVILA hesap bilgisi henüz tanımlı değil…" notu, "Havaleyi bildir" yok. |
| HavaleBildir, KontorAl, KontorAlHavale | aynı adlar | …Koyu | ≈ | Tasarımda görüntü yok; panel pencere deseni (T-125 (5), (6)). |
| GenelBakisGecikti | GenelBakisGecikti | …Koyu | ≈ | Turuncu şerit "Abonelik ödemesi alınamadı · 3. gün" ve bento tasarımdaki gibi. Buton "Kartı güncelle" yerine "Havale ile öde", metin "Kartınızdan … çekilemedi" yerine "5 gün içinde havale ile ödemezseniz…" (T-06; T-125 (7)); üst barda "Randevu ekle" yok (T-125 (9)); "hedefin %72'si" yok (T-116); Gün akışı günün bütün randevularını listeler; selamlama saate göre. |
| TakvimSaltOkunur | TakvimSaltOkunur | …Koyu | ≈ | Kırmızı şerit "Panel salt okunur", "Ödemeyi tamamla", pasif "Randevu ekle", bento ve ızgara tasarımdaki gibi. Pasif butonun yanında nedeni yazar ("Abonelik salt okunur; yeni randevu eklenemez."; panel kuralı), şerit metnine "Var olan randevuların hatırlatmaları gider." eklendi (T-125 (7)); "Kapıda sıra" butonu (Dilim 5). Boş dilimler artık buton değil (bu çalışmada düzeltildi). |
| DurumOdemeGecikti, DurumSaltOkunur | aynı adlar | …Koyu | ≈ | `Durumlar.png` abonelik kartları; salt okunur metni TakvimSaltOkunur'dan (T-125 (7)). |
| Raporlar (+1199, 899) | Raporlar, Raporlar1199, Raporlar899 | …Koyu | ≈ | Dört bento (Hak edilen gelir koyu, Tahsilat beyaz, Gelmedi mercan, Online pay kum), "Günlük tahsilat" grafiği, "Personel ve prim" tablosu ve toplam tasarımdaki gibi. "Dışa aktar" yok, personel avatarında takvim rengi halkası yok (T-122 (4)); hafta Pazartesi–Pazar (23–29 Kasım), iki kapalı gün olduğundan "Pazartesi ve Pazar kapalı" (T-122 (1), (3)). |
| RaporlarBos | RaporlarBos | …Koyu | ≈ | Boş durum ve "Prim kurallarını ayarla" tasarımdaki gibi. Kartlar dolu hâlin başlıklarıyla (Gelmedi, Online pay; tasarımda Doluluk, Prim) ve metin "gelmeyenler" der (T-122 (2)); boş durum başlığı ortak `EmptyState` ölçüsünde (tasarım daha iri; bütün boş ekranlarda aynı). |
| Primim | Primim | …Koyu | ≈ | Panel tasarımı yok; işletme uygulaması Profil içeriğinin uyarlaması (T-122 (6)). |
| AyarlarRoller (+899) | AyarlarRoller, AyarlarRoller899 | …Koyu | ≈ | Kullanıcılar tablosu, durum rozetleri, rol matrisi ("Kendi", "—", onay işareti) ve dipnot tasarımdaki gibi. Üst barda "Değişiklikleri kaydet" yok (kayıt satır penceresinden; T-127 (1)); oturum sahibinde "(siz)"; eylem kolonu başlığı "Eylem". "Kullanıcı ekle" marka renginde (bu çalışmada düzeltildi). |
| AyarlarKvkk (+899) | AyarlarKvkk, AyarlarKvkk899 | …Koyu | ✓ | Aydınlatma metni kartı (Önizle, Kendi metnimi kullan), Veri saklama (3 yıl), "Dışa aktarma iste", Silme talepleri tablosu ve üst barda "Değişiklikleri kaydet" tasarımdaki gibi. Dışa aktarma isteği e-posta sorar (T-127 (2)). |
| PersonelGoogle | PersonelGoogle | …Koyu | ≈ | Ayrı Google Takvim ekranı tasarlanmadı; Personel ekranındaki kart (T-127 (3)). |
| ProfilMenu | ProfilMenu | ProfilMenuKoyu | ≈ | Tasarım görüntüsü depoda yok (ürün sahibinin 23 Kasım 2026 tasarımı; T-129). Kimlik, Tema (Açık · Koyu · Sistem), "Sistem, cihazınızın ayarını izler." ve "Oturumu kapat" T-129'daki tarifle karşılaştırıldı; görüntü bu çalışmada `ekranlar` testine eklendi. |

## Platform paneli

Ortak farklar (aşağıda yinelenmez): üst bar araması "Marka, telefon veya bağlantı ara" (T-123 (4)); "Dışa aktar" yalnız Denetim kaydında (T-123 (4), T-126 (6)); tablolarda son kolon başlığı "Aç"; avatar zemini ada göre renk alır; kullanıcı satırında profil menüsü oku (T-129).

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| Main (+1199, 899) | Main, Main1199, Main899 | …Koyu | ✓ | Dört bento, "Aktivasyon hunisi · son 30 gün", "Dikkat isteyen markalar" tasarımdaki gibi. Başlıktaki tarih koşu günü; sektör etiketi veriden ("Tırnak"). |
| PMarkalar | PMarkalar | PMarkalarKoyu | ≈ | Süzgeç çipleri, tablo, durum rozetleri tasarımdaki gibi. Dipnot "Rolünüz markaları görüntüler, değiştiremez…" (rol adından bağımsız); marka adı bağlantı altı çizgisiz. |
| PMarkaDetay | PMarkaDetay | PMarkaDetayKoyu | ≈ | Kilitli "Kontör ekle" / "Planı değiştir", sekmeler, dört bento, Marka bilgileri, Kontör hareketleri ve denetim notu tasarımdaki gibi. "Online oranı %61", "SMS gönderimi (günlük)" (T-123 (7)); SMS gönderen adı satırı sözleşmedeki alanlardan. |
| PMarkaDetay: Yönetici, Kontör ekle, Kontör sekmesi | PMarkaDetayYonetici, PMarkaDetayKontorEkle, PMarkaDetayKontor | …Koyu | ≈ | Tasarımda görüntü yok; butonlar açık, pencere panel deseninde, sekme Kontör hareketleri tablo deseninde (T-123 (6)). |
| POnay | POnay | POnayKoyu | ≈ | Sekmeler, satır kartları, "Bankada eşleşen hareket var" rozeti, "Kontrol edin:" şeridi ve dipnot tasarımdaki gibi. Havale sekmesinde SMS satırı yok (sekme istemcide süzer); "Eşleştir" yalnız "Kod tutuyor" adayı olan satırda, diğerinde "Banka hareketlerinde ara" + "İncele" (T-126 (1), (2)); satırda "N gündür bekliyor". |
| POnaySms | POnaySms | POnaySmsKoyu | ≈ | Tasarımda SMS satırı Havale sekmesinde; ayrı sekmede aynı satır deseni (T-126 (1)). |
| POnayBos | POnayBos | POnayBosKoyu | ≈ | "Bekleyen onay yok" ve "Denetim kaydı" tasarımdaki gibi. Görüntüde menü ve zil rozeti 7 gösterir: sayaç Prism oturum örneğinden gelir, kuyruk test verisiyle boştur (yalnız görüntü tutarsızlığı). Başlık ortak `EmptyState` ölçüsünde. |
| POnayDetay | POnayDetay | POnayDetayKoyu | ≈ | Markanın bildirimi, Dekont, aday hareketler ("Kod tutuyor" seçili), Karar kartı ve açıklama tasarımdaki gibi. İkincil "Reddet" eklendi (T-126 (2)); uyuşmayan adaylarda "Kontrol edin" rozeti; yer tutucular yerine demo tutarları. |
| POnayDetay: kontrol | POnayDetayKontrol | …Koyu | ≈ | Tasarımda görüntü yok; uyuşmayan aday seçiliyken Karar kartında "Kontrol edin:" şeridi (KK3). |
| PBankaHareketleri | PBankaHareketleri | …Koyu | ≈ | Tasarımda ekran yok; PMarkalar tablo deseni (T-126 (4)). |
| PAbonelikler | PAbonelikler | PAboneliklerKoyu | ≈ | Dört bento, çipler, tablo ve dipnot tasarımdaki gibi. "7 günlük hatırlatma süresinde" → "hatırlatma süresinde", dipnotta "(7 gün)" yok (süre ayardan; T-126 (6)); salt okunur satırında "Kart eklenince açılır" → "Ödeme gelince açılır" (T-06). |
| PDenetim | PDenetim | PDenetimKoyu | ≈ | Çipler, arama, tablo (Zaman, Kim, İşlem, Marka, Ayrıntı) tasarımdaki gibi. "Son 24 saat" çip yerine ayrı seçim, ilk çip "Tümü" (T-126 (5)); Destek'te "Dışa aktar" kilitli; dipnotta yıl yok ("saklama süresi boyunca"). |
| PPlanlar | PPlanlar | PPlanlarKoyu | ≈ | Üç plan kartı (en çok markalı plan koyu), Ortak kurallar, Kontör paketleri tasarımdaki gibi. Yer tutucular yerine "Fiyat belirlenmedi" / "Belirlenmedi" (fiyat tanımlıyken metrik ölçüsünde yazar), sınır yokken "Sınırsız"; "Paketleri düzenle" ayrı buton (T-126 (7)). |
| ProfilMenu | ProfilMenu | ProfilMenuKoyu | ≈ | Salon paneliyle aynı bileşen (T-129); tasarım görüntüsü depoda yok. |

## Tanıtım sitesi

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| Main | Main | MainKoyu | ≈ | Hero, altı özellik kartı, "Uygulama zorunlu değil", üç adım, deneme şeridi ve alt bilgi tasarımdaki gibi. Üst başlık ve alt bilgi metni sektöre daraltılmadı; "omvila.com/salonunuz" yerine "Size özel bağlantıdan" (T-117 (1)); tasarımdaki kırık görsel yer tutucuları yerine ürünün gerçek ekran görüntüleri (T-117 (4)); üst barda tema düğmesi, alt bilgide tema seçici (T-129); görüntü 1440 px (tasarım daha dar çizilmiş). |
| MainMobil | MainMobil | MainMobilKoyu | ≈ | Tek sütun ve menü butonu tasarımdaki gibi; hero'da panel görüntüsü var, telefon görüntüsü gizli (T-117 (4)). |
| Fiyatlar | Fiyatlar | FiyatlarKoyu | ≈ | Üç plan ("Önerilen" rozeti, koyu kart), SMS kontörü kartı, Sık sorulanlar tasarımdaki gibi. Başlık "İşletmenize göre plan"; yer tutucular yerine "Fiyat yakında açıklanacak" ve "… yakında açıklanacak" (S-14; T-117 (2)); SSS'de yalnız cevabı olan soru, cevapta "kartla ya da" yok (T-117 (3)). |
| FiyatlarMobil | FiyatlarMobil | FiyatlarMobilKoyu | ≈ | Aynı farklar; kontör satırlarında uzun metin iki satıra kırılır. |
| TemaMenu | TemaMenu | TemaMenuKoyu | ≈ | Tasarım görüntüsü depoda yok (T-129). Açık · Koyu · Sistem, seçili satırda onay işareti ve "Sistem, cihazınızın ayarını izler." T-129'daki tarifle karşılaştırıldı; görüntü bu çalışmada `ekranlar` testine eklendi. |

## Rezervasyon sayfası

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| Durumlar: online randevu kapalı | DurumOnlineKapali | DurumOnlineKapaliKoyu | ✓ | Dilim 2'de karşılaştırıldı ([dilim-02-tasarim-karsilastirma.md](dilim-02-tasarim-karsilastirma.md)); bu dilimde davranışı `kapali.e2e.ts` sınar (KK1). |

## İşletme uygulaması ve e-postalar

| Ekran | Kaynak | Durum | Not |
|---|---|---|---|
| isletme-uygulamasi/MainSaltOkunur | golden `mobile/isletme/test/goldens/ci/bugun_salt_okunur.png` (`goldens_test.dart` "Bugün salt okunur"); `test/features/today_screen_test.dart` (`salt okunur abonelik: şerit ve pasif + Randevu`) | ≈ | Golden ve widget testi var; Dilim 5 belgesinde eksik sayılan golden sonradan eklenmiş. Bu çalışmada yeniden üretilmedi ve platform goldeniyle yan yana bakılmadı. |
| bildirim-dagitim/EpostaDeneme, EpostaOdemeHata | `api/src/Modules/Notifications/…/Templates/EmailTemplates.cs`, `IntegrationEventHandlers/EmailOwnerAboutSubscription.cs`; `SubscriptionTests.Criterion1_…` (gönderim) | ≈ | Metin ve gönderim API testlerinde; HTML görünümü tasarımla yan yana karşılaştırılmadı (aşağıda). |
| bildirim-dagitim/EpostaKontor | `IntegrationEventHandlers/EmailCreditsLow.cs`; `CreditsLowEmail_SentOnThresholdCrossing_NotWithoutBrandEmail`, `CreditsLow_MatchesCatalogText` | ≈ | Aynı. |

## Bu çalışmada düzeltilenler

- **ui-web takvim ızgarası (T-125 (8)):** boş dilime tıklama işleyicisi (`onSlotClick`) verilmediğinde boş dilimler artık "Randevu ekle: …" etiketli işlevsiz buton değil; rolü, etiketi ve sekme durağı olmayan düz hücredir (ızgara ölçüsü aynı). İşleyici varken davranış değişmedi. Salt okunur takvimde erişilebilirlik ağacında bu butonlar yok (`calendar.test.tsx`, `salt-okunur.e2e.ts`, `ekranlar` TakvimSaltOkunur).
- **AyarlarRoller:** kart başlığındaki "Kullanıcı ekle" ikincil metin rengindeydi; tasarımdaki gibi marka renginde (`metin-marka`).
- **Görüntüler:** ProfilMenu (salon ve platform paneli) ve TemaMenu (tanıtım sitesi) `ekranlar` testlerine eklendi.

## Açık kalanlar

- **Kart akışı (T-06):** "Kartla öde", "Kart ekle", "Kartı güncelle", "Ödeme yöntemini kaydet" ve tanıtım sitesindeki "kartla" ifadesi sağlayıcı seçilene kadar yok; AyarlarAbonelik ve GenelBakisGecikti bu yüzden ≈.
- **Fiyat ve OMVILA hesabı (S-14):** değerler tanımsız; ekranlar "tanımlı değil / belirlenmedi / yakında açıklanacak" yazar. "Dolu" görüntülerdeki alıcı, IBAN ve tutarlar kurgusaldır.
- **Sözleşmede olmayanlar:** Raporlar, Markalar ve Abonelikler'de "Dışa aktar"; Genel bakış "hedefin %72'si"; prim tablosunda personel takvim rengi; başka personele "Bağlantı linki gönder" (T-122 (4), T-123 (4), T-126 (6), T-127 (3)). KVKK dışa aktarma isteğinin durumunu okuyan uç ve SMS gönderen adı kararının panel bildirimi ayrı sözleşme PR'ında eklendi (T-132).
- **Tasarımı olmayan ekranlar:** Banka hareketleri, marka detayının Abonelik ve fatura / Kontör / Ödemeler / Denetim kaydı sekmeleri, POnaySms, Primim (web), HavaleBildir ve KontorAl pencereleri, Google Takvim bağlantısı, yasal sayfalar. Mevcut desenlerle kuruldu; tasarım gelince karşılaştırılmalı.
- **ProfilMenu ve TemaMenu tasarım görüntüleri** `design/ekranlar/` altında yok; karşılaştırma T-129'daki tarifle yapıldı. Görüntüler depoya eklenince yan yana bakılmalı.
- **Genel bakışta "Randevu ekle"** (GenelBakisGecikti tasarımında var) hâlâ yok (T-125 (9)).
- **Boş durum başlığı:** RaporlarBos ve POnayBos tasarımında başlık ortak `EmptyState` bileşenindekinden iri; bileşen tasarım sistemindeki ölçüde olduğundan değiştirilmedi (önceki dilimlerin boş ekranlarıyla aynı).
- **POnayBos görüntüsünde rozet:** oturum örneğindeki `pendingApprovalCount` (7) boş kuyrukla çelişir; yalnız e2e verisi.
- **E-postalar ve MainSaltOkunur:** tasarımla yan yana görüntü karşılaştırması bu çalışmada yapılmadı; kapsam testlerdedir.
- **Uçtan uca birleşik koşu:** salon bildirimi → platform eşleştirmesi → salon şeridinin kalkması tek testte ve gerçek backend'le koşmuyor (T-72); parçalar ayrı ayrı sınanır.
- **Demo verisinde salt okunur marka yok:** Gecikti markası seed'den 4 gün sonra kendiliğinden salt okunur olur (T-131 (2)).
- **Ürün sahibi demosu** ve T-116…T-132 varsayımlarının onayı.
