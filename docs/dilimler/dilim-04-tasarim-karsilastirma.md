# Dilim 4: uçtan uca akış 4 ve tasarım karşılaştırması

Dilim 4 ekranlarının tasarımla karşılaştırma kaydı, kritik akış 4 ve 5'in (test-stratejisi.md) uçtan uca testleri ve kabul kriterlerinin test eşlemesi. Yöntem Dilim 2 ve 3'teki gibi ([dilim-03-tasarim-karsilastirma.md](dilim-03-tasarim-karsilastirma.md)): ekran görüntüleri Playwright ile alınır, `design/ekranlar/` altındaki görüntüyle yan yana gözle karşılaştırılır; otomatik piksel karşılaştırması yok.

- Salon paneli: `pnpm --filter @omvila/salon-panel e2e` → `web/salon-panel/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`, `e2e/onay.e2e.ts`). Masaüstü 1440×900. Saat sabit: Çarşamba 25 Kasım 2026 12:10 İstanbul.
- Rezervasyon sayfası: `pnpm --filter @omvila/rezervasyon e2e` → `web/rezervasyon/e2e-results/ekranlar/` (`e2e/ekranlar.e2e.ts`, `e2e/randevum.e2e.ts`). Telefon 390×844. Saat sabit: Salı 24 Kasım 2026 10:00.
- CI: `e2e` işi iki paketi de çalıştırır; görüntüler yapıt olarak saklanır.

## Kritik akış 4 ve 5

"Kapora: randevu → havale ekranı → 'Havaleyi yaptım' → panelde 'Kapora geldi' → onaylandı" ve "Kapora süresi dolması → otomatik iptal (saat ileri alınarak)". Uygulamalar ayrı Playwright paketlerinde ve Prism mock'a karşı çalıştığı için akış Dilim 3'teki gibi üç yerde sınanır (T-72):

| Parça | Test | Kapsadığı |
|---|---|---|
| Müşteri: kapora ve havale | `web/rezervasyon/e2e/kapora.e2e.ts` | Katalogda "Kapora 150 ₺" → Onay'da kapora kutusu, "Kapora (havale ile) −150 ₺", "Salonda ödenecek 600 ₺" → oluştur → Havale ekranı, geri sayım; alıcı, IBAN (boşluksuz), açıklama kodu ve tutar ("150") kopyalanır (KK3) → "Havaleyi yaptım, salona bildir" → "Salon onayı bekleniyor", "onaylandı" yazmaz (KK4, S-03); SMS bağlantısında salon kaporayı alınca "Onaylandı" ve "Kapora alındı (havale ile)" (KK5); süre sayfa açıkken dolunca yeniden okunur, "Kapora süresinde gelmedi" ve "İptal" (KK6, akış 5). |
| Salon: işaret | `web/salon-panel/e2e/onay.e2e.ts` (`Kapora: …`) | Takvimde "Kapora bekleniyor" bloğu → yan panel; düz "Onayla" yok; "Kapora geldi" (If-Match + Idempotency-Key) → "Onaylandı", "Kapora 150 ₺ · havale ile alındı" (KK5); "Kaporasız onayla" `waiveDeposit: true` (KK7); Genel bakış "Gizem Arslan kapora bekliyor · Havale bildirildi · kontrol edin" satırından "Kapora geldi" (KK4, KK5); "Hatırlat" bir kez, sonra "Hatırlatıldı · 12:10" ve pasif, durum değişmez (V6; bu PR'da eklendi); "Salon iade etti": alınanı aşan tutar istemcide reddedilir, kısmi 100 ₺ + not, If-Match ve Idempotency-Key, "Salon iade etti · 100 ₺" (V7; bu PR'da eklendi). |
| API ve SMS | `api/tests/Omvila.Api.IntegrationTests/` `BankAccountTests`, `DepositBookingTests`, `DepositReceivedTests`, `DepositFollowUpTests` | Gerçek backend ve sahte SMS sağlayıcısıyla bütün geçişler (A2 kaporasız, A3, A5 kapora gelmedi, A13/A14 saat değişikliği), `deposit.awaiting` / `received` / `not_received` SMS'leri, `staff.deposit_reported` push'u, `Payment` ve tahsis kaydı, 403/404/409/412. |

Salonun işaretinin müşteri sayfasına yansıması rezervasyon testinde sunucu yanıtı değiştirilerek verilir; iki uygulamayı ve gerçek backend'i aynı testte birleştiren gece koşusu henüz yok (T-72). Demo verisinde kapora bekleyen, havale bildirilen, kaporası alınan ve kaporasız onaylanan birer randevu vardır; akış elle de denenebilir (test-ortami.md § Demo verisi, T-85).

## Kabul kriterleri

| # | Kriter | Kapsayan testler |
|---|---|---|
| 1 | IBAN yokken kapora ayarları pasif, "Kapora alınamaz · IBAN ekle"; geçersiz IBAN `validation_failed` | `BankAccountTests` (`Put_InvalidOrMissingIban_Is400`, `DepositAvailable_FollowsAccount_AndAllowsDepositPolicy`, `ServiceDepositAlways_WithoutAccount409_WithAccountCreated`); panel `ekranlar.e2e.ts` AyarlarKaporaYok görüntüsü; `bank-account.test.tsx` (istemci mod-97) |
| 2 | RiskyCustomersOnly, eşik 2: Balyaj %30 / 2.800 ₺ → 840 ₺, `DepositPending`, +2 sa; diğer müşteriye kapora yok | `DepositBookingTests.Criterion2And3_…`, `Criterion2_CustomerWithoutNoShows_NoDeposit_Confirmed`, `Criterion2_PanelViews_CarryDepositStatus_WithoutInstructions`; `DepositResolverTests`; demo verisi (`DemoSeedTests.Seed_AddsDepositAccountAndDepositAppointments`) |
| 3 | Havale ekranında dört alan kopyalanır; SMS'te IBAN, alıcı, tutar, süre, kod (≤ 155) | `kapora.e2e.ts` (dört kopyalama; bu PR'da alıcı ve tutar eklendi); `DepositBookingTests.Criterion2And3_…`; `SmsTemplatesTests` (en uzun değerler, V2) |
| 4 | "Havaleyi yaptım" → `depositCustomerReported`, salon push'u, panelde "Havale bildirildi"; durum değişmez | `DepositFollowUpTests.Criterion4_…`, `Report_Twice_SameResult_NotifiesOnce`; `kapora.e2e.ts`; panel `onay.e2e.ts` Genel bakış satırı ve RandevuPanelHavale |
| 5 | "Kapora geldi" → `Confirmed`, `Payment`, "kaporanız alındı" SMS'i; "Salonda ödenecek" düşer | `DepositReceivedTests.Criterion5_…` (iki test), `DepositReceiptTransactionTests`; panel `onay.e2e.ts` "Kapora geldi"; `kapora.e2e.ts` SMS bağlantısı |
| 6 | 2 saat geçer → `Cancelled`/`DepositNotReceived`, "Havale yaptıysanız salonu arayın" SMS'i, slot boşalır | `DepositBookingTests.Criterion6_…`, `DepositFollowUpTests.Criterion6_…`, `DepositHoldTests`; `kapora.e2e.ts` (süre dolunca) |
| 7 | "Kaporasız onayla" → `Confirmed`, kapora istenmiyor olarak kaydedilir | `DepositReceivedTests.Criterion7_…`, `Approve_DepositAwaited_WithoutWaive_IsDepositRequired`; panel `onay.e2e.ts` "Kaporasız onayla" |
| 8 | Paketten düşülen kalemde kapora istenmez | `DepositResolverTests.PackageRedemption_NoDeposit` (alan kuralı). Paket kullanımı Dilim 5'te; uçtan uca test orada |
| 9 | Müşteri uygulamasında Ödemelerim | Dilim 5'e taşındı (V8). API tarafı hazır: `DepositView` durumları ve iade alanları (`DepositFollowUpTests.Refund_…`) |
| 10 | Kart formu veya ödeme SDK'sı yok | CI `scripts/check_no_payments.py` (bağımlılık + kaynak taraması, #87) |

## Salon paneli

Durum: ✓ tasarımla uyumlu, ≈ bilinçli/kapsam dışı farkla uyumlu, ✗ düzeltilmesi gereken fark. Panel görüntülerinin hepsinde Prism oturum örneğinden gelen "Deneme süreniz bugün bitiyor" ve "SMS kontörünüz 180'e düştü" şeritleri ile D5+ menü öğelerinin yokluğu ortaktır; aşağıda yinelenmez.

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| AyarlarOdeme (Kapora ve IBAN) | AyarlarOdeme, AyarlarOdeme1199, AyarlarOdeme899 | …Koyu | ≈ | Bilgi şeridi, iki sütun (hesap formu + "Müşteri ne görür?"), alanlar ve alt metinler tasarımdaki gibi. IBAN alanı kayıtlı hesapta maskeli (`TR29 •••• 6789 01`) açılır ve alt metin "Kayıtlı IBAN maskeli görünür…" (tam IBAN sunucudan dönmez; T-79 (1)). Üst barda arama yok (T-57). |
| Ayarlar (Randevu kuralları, kapora bölümü) | Ayarlar | AyarlarKoyu | ≈ | "Kapora IBAN'ı tanımlı · Elif Kaya · TR29 •••• 6789 01" şeridi, politika, riskli müşteri tanımı tasarımdaki gibi. Havale süresi seçici değil "2 saat · Değiştir" (yalnız Kapora ve IBAN'da düzenlenir; V1). Seçici değerleri Prism örneğinden ("Hiçbir zaman", "1 kez gelmeyen"). |
| Durumlar: kapora istenemiyor | AyarlarKaporaYok | AyarlarKaporaYokKoyu | ✓ | Durumlar.png "Kapora istenemiyor" deseni (bilgi şeridi, "IBAN ekle") Randevu kuralları kapora bölümünde; politika ve eşik pasif, havale süresi gizli. |
| GenelBakis: kapora satırı | GenelBakis | GenelBakisKoyu | ≈ | "Gizem Arslan kapora bekliyor · 14:30 · havale işaretlenmedi · 1 sa 20 dk kaldı", "Hatırlat" + "Kapora geldi" tasarımdaki gibi; bento "Onay bekliyor 2" kapora bekleyeni saymaz (T-84 (3)). Diğer farklar Dilim 3 kaydında (ciro, kapıda sıra, doluluk, gelmedi satırı). |
| RandevuPanel: kapora bekleniyor | RandevuPanelKapora | RandevuPanelKaporaKoyu | ≈ | Tasarımda panel görüntüsü yok; KaporaOnay içeriği (tutar, açıklama kodu, müşteri bildirimi, kalan süre, "Banka uygulamanızda…") RandevuPanel düzeninde, alt butonlar Reddet / "Kapora geldi" (1 : 2), "Hatırlat" ve "Kaporasız onayla" bölümde (T-84 (1)). Kapora alınana kadar "Salonda ödenecek" toplamdır (sunucunun `amountDue`'su; düşüş KK5'te). |
| RandevuPanel: havale bildirildi | RandevuPanelHavale | RandevuPanelHavaleKoyu | ≈ | Üstte "Havale bildirildi · kontrol edin" uyarı şeridi, müşteri bildirimi saatiyle; takvim bloğunda "Havale bildirildi" rozeti. |
| RandevuPanel: kapora alındı | RandevuPanelKaporaAlindi | RandevuPanelKaporaAlindiKoyu | ≈ | RandevuOnayli düzeni + "Kapora 150 ₺ · havale ile alındı" eylem satırı ("Salon iade etti olarak işaretleyin"), "Salonda ödenecek 300 ₺". |
| RandevuPanel: iade | RandevuPanelKaporaIade | RandevuPanelKaporaIadeKoyu | ≈ | Tasarımda görüntü yok; "Salon iade etti · 100 ₺" satırı (V7). Bu PR'da eklendi. |
| Takvim: kapora rozeti | Main | MainKoyu | ✓ | Gizem Arslan 14:30 bloğu "Kapora bekleniyor" renginde, kesik çerçeve. Bento "Onay bekliyor 2 · ilki 1 sa 35 dk içinde iptal olur" (bu PR'da test verisi düzeltildi, aşağıda). |

## Rezervasyon sayfası

| Ekran | Açık | Koyu | Durum | Not |
|---|---|---|---|---|
| Onay: kapora kutusu | OnayKapora | OnayKaporaKoyu | ≈ | Kapora kutusu, özet satırları ("Toplam", "Kapora (havale ile) −150 ₺", "Salonda ödenecek 600 ₺") ve alt not tasarımdaki gibi. Kutuda "Kuaför Elif'in" yerine "salonun", "2 saat" yerine "belirtilen süre"; iptal sınırı satırı yok (süre ve sınır herkese açık uçlarda yok; T-80 (2)). Paket anahtarı D5; "Tekrar hoş geldiniz" yeni müşteride çıkmaz. |
| Havale | Havale | HavaleKoyu | ≈ | Kırmızı kapora kartı, geri sayım, dört kopyalanabilir alan, bilgi kutusu ve alt buton tasarımdaki gibi. Bilgi kutusunda "2 saat içinde" yerine son zaman ("Kapora 11:58'e kadar gelmezse"; T-80 (2)). Tam sayfa görüntüde sabit alt çubuk bilgi kutusunun üstüne düşer; kaydırınca görünür (görüntü kipi, ürün farkı değil). |
| Havale bildirildi | HavaleBildirildi | HavaleBildirildiKoyu | ≈ | Tasarımda ayrı görüntü yok; aynı ekran "Salon onayı bekleniyor" kutusuyla, buton yok (S-03, T-80 (4)). |
| Randevum: kapora bekleniyor | RandevumHavale | RandevumHavaleKoyu | ≈ | SMS bağlantısında Havale ekranı + "Plan değişti mi?" kartı (T-80 (5)). |
| Randevum: kapora alındı | RandevumKaporaAlindi | RandevumKaporaAlindiKoyu | ≈ | Randevum.png düzeni; satır "Kapora alındı (havale ile) −150 ₺" (tasarımda "150 ₺ alındı · havale"), "Salonda ödenecek 600 ₺". "Takvime ekle" ve uygulama kartı yok (Dilim 2 kaydı). |
| Randevum: iade | — | — | ≈ | Görüntü yok; "Kapora · Salon iade etti" satırı birim testinde (`AppointmentManager.test.tsx`). |
| Iptal: kapora uyarısı | IptalKapora | IptalKaporaKoyu | ✓ | "150 ₺ kaporanın iadesi salonun iade koşullarına bağlıdır…" uyarısı tasarımdaki gibi; yalnız kapora alınmışsa (T-80 (5)). |
| Durumlar: kapora gelmedi | DurumKaporaGelmedi | DurumKaporaGelmediKoyu | ≈ | Desen Randevum iptal kartında: "Kapora süresinde gelmedi, randevu iptal edildi. Havaleyi yaptıysanız dekontla salonu arayın." + "Salonu ara". Tasarımdaki "2 saat içinde" ve "salon randevuyu yeniden açabilir" yazılmaz (V9, T-80 (2)). |
| Basari (kapora sonrası) | — | — | — | Kapora alınınca müşteri SMS ve Randevum'da "Onaylandı" görür; ayrı başarı ekranı yok (Dilim 2 Basari görüntüsü geçerli). |

`isletme-uygulamasi/KaporaOnay` ve `musteri-uygulamasi/` (YROnay, YRHavale, Odemelerim) Dilim 5'tedir (V8).

## Bulunan farklar

- **Takvim bentosu "ilki 10 sa 50 dk içinde iptal olur" gösteriyordu (Main, RandevuPanel görüntüleri):** panel e2e takvim demosu onay bekleyen taleplerin tutmasını 23:00'e yazıyordu; Genel bakış kuyruğu (Prism) ve tasarım 13:45 ("1 sa 35 dk"). Ürün kodu doğru; test verisi düzeltildi (`e2e/calendar-demo.ts`).
- Başka ✗ fark bulunmadı.
