# Dilim 4: Kapora havalesi

## Amaç
Salon IBAN'ını tanımlar ve kapora politikasını seçer. Kapora gereken online randevuda müşteri havale bilgilerini (alıcı, IBAN, tutar, açıklama kodu) kopyalar, havaleyi yapıp salona bildirir. Salon "Kapora geldi" ile işaretler, süre dolarsa randevu otomatik iptal olur. **OMVILA hiçbir aşamada para almaz.**

## Kapsam
- Ayarlar → Kapora ve IBAN: alıcı adı, banka, IBAN (TR biçim + mod-97 doğrulaması), açıklama kodu öneki, havale süresi (1/2/4 sa), "Müşteri ne görür?" önizlemesi; politika (`None`/`RiskyCustomersOnly`/`Always`) ve riskli müşteri eşiği.
- Hizmet kaporası (D1'de alanlar vardı; IBAN tanımlanınca etkin).
- Kapora çözümü (`sureler-ve-ayarlar.md § Kapora`), yüzde → liraya yuvarlama, açıklama kodu üretimi (`OMV-7K2X`, karışmayan harfler, marka içinde benzersiz).
- Geçiş A3 ve A5 (`DepositNotReceived`), `deposit.*` SMS'leri, `staff.deposit_reported` push'u.
- Rezervasyon ve müşteri uygulaması Havale ekranı: kopyalanabilir alanlar, geri sayım, "Havaleyi yaptım, salona bildir", sonrasında "Salon onayı bekleniyor" (S-03).
- Panel: "Kapora bekleniyor" durum rozeti, "Havale bildirildi · kontrol edin", Kapora geldi / Hatırlat / Kaporasız onayla, iade işareti ("Salon iade etti").
- `Payment(Method=Transfer, IsDeposit)` ve randevu tahsis kaydı; "Salonda ödenecek" = toplam − kapora.
- Saat değişikliğinde kapora yeni saate aktarılır; salon iptalinde "Kapora iadesi için salon sizinle iletişime geçecek" metni.

## Ekranlar
`salon-paneli/`: AyarlarOdeme, Ayarlar (kapora bölümü), GenelBakis (kapora bekliyor satırı), Durumlar (kapora alınamaz). `rezervasyon-sayfasi/`: Onay (kapora kutusu), Havale, Basari, Iptal (kapora uyarısı), Durumlar (kapora gelmedi). `isletme-uygulamasi/`: KaporaOnay. `musteri-uygulamasi/`: YROnay, YRHavale, Odemelerim.

## Kabul kriterleri
1. IBAN yokken kapora ayarları pasif ve "Kapora alınamaz · IBAN ekle" deseni görünür. Geçersiz IBAN `validation_failed`.
2. Politika `RiskyCustomersOnly`, eşik 2: 2 kez gelmemiş müşteri Balyaj (%30, 2.800 ₺'den) alır → kapora 840 ₺, randevu `PendingApproval` (`DepositPending`), `holdExpiresUtc = +2 sa`; diğer müşteriye kapora istenmez.
3. Havale ekranında dört alan kopyalanır; SMS'te IBAN, alıcı, tutar, süre ve kod bulunur (≤ 155 karakter).
4. "Havaleyi yaptım" → `depositCustomerReported`, salon push'u, panelde "Havale bildirildi"; durum değişmez.
5. Salon "Kapora geldi" → `Confirmed`, `Payment` kaydı, müşteriye "kaporanız alındı" SMS'i; "Salonda ödenecek" düşer.
6. 2 saat geçer, işaretlenmez → `Cancelled`/`DepositNotReceived`, "Havale yaptıysanız salonu arayın" SMS'i, slot boşalır.
7. "Kaporasız onayla" (`waiveDeposit=true`) → `Confirmed`, kapora istenmiyor olarak kaydedilir.
8. Paketten düşülen kalemde kapora istenmez.
9. Müşteri uygulamasında Ödemelerim: "Salon onayı bekleniyor" → "Alındı" → (iade sonrası) "Salon iade etti".
10. Hiçbir ekranda ve bağımlılıkta kart formu veya ödeme SDK'sı yok (bağımlılık taraması + kod araması CI kontrolü).

## Uçtan uca ve tasarım
- Durum: uygulandı (#87–#95 ve kapanış); ürün sahibi demosu ve varsayımların (`acik-sorular.md` T-78…T-85) onayı bekleniyor.
- Kritik akış 4 ve 5 (`test-stratejisi.md`): `web/rezervasyon/e2e/kapora.e2e.ts`, `web/salon-panel/e2e/onay.e2e.ts` (Kapora); API ve SMS `DepositBookingTests`, `DepositReceivedTests`, `DepositFollowUpTests`, `BankAccountTests`.
- Kabul kriterlerinin test eşlemesi ve tasarım karşılaştırması: [dilim-04-tasarim-karsilastirma.md](dilim-04-tasarim-karsilastirma.md). Kriter 9 (Ödemelerim) Dilim 5'te (V8); kriter 8 alan testinde, paket kullanımı Dilim 5'te.
- Demo verisi kapora hesabı, politikası ve dört kapora durumunu içerir (T-85).
