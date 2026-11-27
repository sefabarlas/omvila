# Dikey dilimler

Her dilim, uçtan uca çalışan ve gösterilebilen bir parçadır: sözleşme + backend + en az bir istemci + testler. Dilimler sırayla tamamlanır. Bir dilim içinde işler paralel yürüyebilir (backend ve istemciler sözleşme ve mock sayesinde birbirini beklemez).

| # | Dilim | Ana yüzeyler | Durum |
|---|---|---|---|
| 1 | [Giriş + salon kurulumu](dilim-01-giris-ve-kurulum.md) | api, salon-panel | Uygulandı; ürün sahibi demosu ve açık soru onayları bekliyor |
| 2 | [Müsaitlik + rezervasyon sayfası + panel takvimi](dilim-02-musaitlik-rezervasyon-takvim.md) | api, rezervasyon, salon-panel | Uygulandı; ürün sahibi demosu ve açık soru onayları bekliyor |
| 3 | [Onay akışı + SMS](dilim-03-onay-ve-sms.md) | api, salon-panel, rezervasyon | Uygulandı (gerçek SMS sağlayıcısı canlıya çıkışta, T-03); ürün sahibi demosu ve açık soru onayları bekliyor |
| 4 | [Kapora havalesi](dilim-04-kapora-havalesi.md) | api, rezervasyon, salon-panel | Uygulandı (müşteri uygulaması ekranları ve kriter 9 Dilim 5'te, V8); ürün sahibi demosu ve açık soru onayları bekliyor |
| 5 | [Mobil uygulamalar + bekleme listesi + kapıda sıra + paket + kasa](dilim-05-mobil-bekleme-sira.md) | api, isletme, musteri, salon-panel, rezervasyon | Uygulandı (mağaza yayını K4'e bağlı; açık kalanlar [karşılaştırma kaydında](dilim-05-tasarim-karsilastirma.md#açık-kalanlar)); ürün sahibi demosu ve açık soru onayları (T-86…T-114) bekliyor |
| 6 | [Abonelik + platform paneli + raporlar](dilim-06-abonelik-platform-rapor.md) | api, platform-panel, salon-panel, tanitim | Uygulandı (kart akışı T-06, fiyat ve OMVILA hesabı S-14 kararını bekliyor; açık kalanlar [karşılaştırma kaydında](dilim-06-tasarim-karsilastirma.md#açık-kalanlar)); ürün sahibi demosu ve açık soru onayları (T-116…T-132) bekliyor |

## Bir dilime başlarken (ajan)
1. Dilim belgesini, ilgili ekranları (`design/ekran-envanteri.md`) ve açık soruları okuyun.
2. Sözleşmede eksik uç varsa `contracts/yol-haritasi.md`'den alıp ekleyin (ayrı PR: `contract(...)`).
3. İşi PR'lara bölün: backend (modül/özellik bazında) ve istemci (ekran bazında). Her PR kabul kriterlerinden birini veya birkaçını kapatır.
4. Dilim, kabul kriterlerinin tümü uçtan uca testle geçince ve ürün sahibi demoyu onaylayınca biter (`tanim-bitti.md § Dilim tamamlandı`).

Kabul kriterleri Given/When/Then biçimindedir ve doğrudan entegrasyon veya uçtan uca test adına dönüşür.
