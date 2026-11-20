---
description: Bir dikey dilime başlamadan önce kapsamı çıkar ve iş planını yap
argument-hint: <dilim numarası, ör. 2>
---
Dilim $ARGUMENTS için hazırlık yap. Kod yazma, yalnız plan çıkar.

1. `AGENTS.md`, `docs/dilimler/README.md` ve `docs/dilimler/dilim-0$ARGUMENTS-*.md` dosyalarını oku.
2. Dilimin ekranlarını `design/ekran-envanteri.md`'den bul. Her ekranın açık görüntüsünü aç ve kısa not al.
3. `docs/acik-sorular.md`'de bu dilimi etkileyen maddeleri listele. 🔴 olan varsa en başta belirt ve dur.
4. Sözleşmede eksik uçları `contracts/yol-haritasi.md` ile karşılaştır.
5. İşi PR'lara böl. Her PR için şunları yaz: başlık (commit biçiminde), kapsadığı kabul kriterleri (numaralarla), dokunacağı projeler, bağımlılığı (hangi PR'dan sonra). Sıra: sözleşme → backend alan + testler → backend uçlar → istemciler.
6. Riskleri ve varsayımları yaz.

Çıktı: yukarıdaki başlıklarla Türkçe bir plan. Plan onaylanmadan koda geçme.
