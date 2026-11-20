---
description: Bir PR'ı veya çalışma dalını OMVILA kurallarına göre bağımsız olarak incele
argument-hint: <dal adı veya PR numarası>
---
`$ARGUMENTS` değişikliklerini incele. Değişikliği sen yazmadıysan en iyi sonucu verir; yazdıysan tarafsız davran.

`git diff origin/main...$ARGUMENTS` (veya `gh pr diff $ARGUMENTS`) ile değişiklikleri al ve şunları kontrol et:

1. **Ürün kuralları** (`docs/urun/urun-kurallari.md`): ödeme/kart, keşif, WhatsApp, sağlık verisi, "reddedildi" metni, ses-tonu.
2. **Alan kuralları:** durum geçişleri `durum-makinesi.md` ile birebir mi? Süreler ayardan mı? Para `Money`, zaman `IClock`, telefon `PhoneNumber` mı?
3. **Sözleşme:** API değişikliği önce `contracts/`'ta mı? `operationId` = `WithName`? Yeni hata kodu dört yere de eklendi mi?
4. **Güvenlik ve kiracı:** izin işareti, 404 kapsam dışı, RLS politikası, istekten marka kimliği okunmuyor, günlükte PII yok.
5. **Testler:** kural başına test var mı? Atlanan veya gevşetilen test var mı? Paralellik testleri (çakışma) duruyor mu?
6. **Arayüz:** semantik token, boş/yükleniyor/hata hâlleri, açık/koyu, erişilebilirlik, biçimlendirme yardımcıları, şube saat dilimi.
7. **Git:** commit/PR biçimi, PR boyutu, varsayımlar yazılmış mı?

Çıktı: her bulgu için dosya:satır, kural kaynağı (belge bağlantısı), önem (engelleyici / önemli / öneri) ve önerilen düzeltme. Sonda "birleştirilebilir mi" kararı ve gerekçesi.
