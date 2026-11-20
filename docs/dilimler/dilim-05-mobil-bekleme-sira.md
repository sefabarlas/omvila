# Dilim 5: Mobil uygulamalar + bekleme listesi + kapıda sıra + paket + kasa

## Amaç
İki Flutter uygulaması yayına hazır hâle gelir. Bekleme listesi ve kapıda sıra çalışır. Randevu "Tamamla ve ödeme al" ile kapanır; paket satışı ve seans düşümü, kasa ve gün kapatma, değerlendirme ve prim doğumu devreye girer.

Büyük bir dilimdir. Alt dilimlere bölünerek yürütülür: **5a** müşteri uygulaması (`/me`), **5b** işletme uygulaması, **5c** bekleme listesi, **5d** kapıda sıra, **5e** tamamla + kasa + paket + prim + değerlendirme.

## Kapsam
- `/me` yüzeyi (yol haritası): giriş, profil, randevular (yaklaşan/geçmiş), iptal/erteleme, kayıtlı salonlar (QR/bağlantı), paketler, ödemeler, değerlendirme, rızalar (kampanya), hesap silme, cihaz kaydı, bildirimler.
- Müşteri uygulaması tüm ekranları (`mobile/musteri/AGENTS.md`), derin bağlantılar.
- İşletme uygulaması tüm ekranları (`mobile/isletme/AGENTS.md`), push bildirimleri ve tercihleri.
- Bekleme listesi: katılma (public ve panel), teklif motoru (boşalan saat → sıradaki uygun kişi → 15 dk tutma), `/b/` teklif sayfası, kabul/ret/süre dolması, panel listesi ve "Saat öner".
- Kapıda sıra: bilet, günlük numara, tahmini bekleme, "Sırası yaklaştı" SMS'i, "Başlat" → randevu (`WalkIn`, `CheckedIn`).
- Tamamla ve ödeme al (A11): yöntemler Nakit/Kart/Havale/Paketten, ikinci ödeme, tahsis; sayaçlar; `CommissionEntry(Service)`; değerlendirme daveti.
- Paket tanımları, müşteriye paket satma (`CommissionEntry(Sale)`), seans ayırma/geri dönme (randevu oluşurken/iptalde), `my-packages`, süresi geçen paketler işi.
- Kasa: günün tahsilatları (yöntem bazında), gün kapatma ve kasa sayımı.
- Değerlendirmeler: müşteri gönderir, salon yanıtlar, rezervasyon sayfasında puan eşiği.
- Müşteri kartı sekmeleri (paketler, ödemeler, değerlendirmeler), birleştirme, CSV içe/dışa aktarma, KVKK silme talebi.

## Ekranlar
`musteri-uygulamasi/*`, `isletme-uygulamasi/*`, `rezervasyon-sayfasi/` (BeklemeTeklif, BeklemeDoldu, DoluGun "listeye katıl"), `salon-paneli/` (BeklemeListesi(+Bos), KapidaSira(+Bos), OdemeAl, Kasa(+Bos), Paketler(+Bos), PaketOlustur, PaketSat, Degerlendirmeler(+Bos), MusteriKarti, MusteriBirlestir, MusteriIceAktar, MusteriSil, AyarlarKvkk), `bildirim-dagitim/` (PushMusteri, MagazaIsletme, MagazaMusteri, QrKart, BaglantiKiti, Ikonlar).

## Kabul kriterleri (seçme)
1. Müşteri uygulamasında ilk girişte salon yoksa "QR kodu okut / Bağlantıyı yapıştır"; salon eklenince `BrandCustomer` **oluşmaz**, salon panelinde görünmez.
2. Geçmişi olan ama yaklaşan randevusu olmayan kullanıcı boş ekran yerine "Aynısını tekrar al" görür.
3. İptal edilen 16:00 saati Waitlist'teki Merve'ye teklif edilir (push → SMS), 15 dk tutulur; Merve kabul ederse randevu oluşur; süre dolarsa sıradakine geçer ve Merve listede kalır.
4. Kapıda sırada 3 kişi; numaralar şube ve gün bazında 1'den; "Başlat" ile `CheckedIn` randevu oluşur ve takvimde "Kapıdan" kanalıyla görünür.
5. "Tamamla ve ödeme al": Boya 1.250 + Fön 190 − kapora 200 = 1.240 ₺ Kart → randevu `Completed`, Elif'e %24 prim `Accrued`, 2 saat sonra değerlendirme daveti.
6. 10 seanslık Fön paketi satılır (satış primi %10); online randevuda "Fön paketinizden düşülsün" → seans ayrılır; randevu iptal → seans geri döner ve tarihli görünür; gelmedi → seans geri dönmez.
7. Gün kapatma: beklenen nakit 3.450 ₺, sayılan 3.400 ₺ → fark −50 ₺ ve açıklama kaydı.
8. Değerlendirme yalnız tamamlanan randevuda ve bir kez; ikinci deneme `already_reviewed`.
9. Hesap silme SMS koduyla onaylanır; yaklaşan randevular iptal; tüm markalarda anonimleşir; randevu ve ödeme kayıtları anonim kalır.
10. İşletme uygulamasında personel rolü yalnız kendi randevularını ve primini görür, telefon maskeli, arama çalışır.
11. Golden testler: tüm ana ekranlar açık/koyu ve 1.3× yazı ölçeğinde.
