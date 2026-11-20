---
description: Tasarım ekranını web veya Flutter'da uygula ve tasarımla karşılaştır
argument-hint: <yüzey/Ekran, ör. salon-paneli/YeniRandevu>
---
`$ARGUMENTS` ekranını uygula.

1. `design/ekran-envanteri.md`'de satırını bul: proje, dilim, API işlemleri.
2. `design/ekranlar/$ARGUMENTS.png` ve `…Koyu.png` görüntülerini aç. Aynı akıştaki komşu ekranlara ve varsa `…Bos` boş hâline de bak.
3. İlgili tasarım sistemi README'sini (panel veya mobil) ve kullanılan bileşenlerin `components/<Bileşen>/README.md` dosyalarını oku.
4. Projenin `AGENTS.md`'sini oku. Var olan ortak bileşenleri kullan (`@omvila/ui-web` / `omvila_ui`). Eksik ortak bileşen varsa önce oraya ekle.
5. Veri için yalnız üretilmiş API istemcisini kullan. Backend hazır değilse Prism mock'a karşı çalış.
6. Durumlar: yükleniyor, boş, hata (`code` → desen), dolu. Rol ve izin. Açık/koyu tema. Kırılımlar (panel 1199/899 px; mobil 1.3× yazı).
7. Testler: bileşen veya widget testi, golden (Flutter), gerekiyorsa Playwright akışı.
8. Kendi ekran görüntüsünü al (açık + koyu) ve tasarım görüntüsüyle yan yana karşılaştır. Farkları listele ve düzelt. Kalan bilinçli farkları gerekçesiyle yaz.
9. `docs/standartlar/tanim-bitti.md` § Web/mobil ekranı maddelerini işaretle.
