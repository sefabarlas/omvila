# Tanım-bitti (Definition of Done)

Bir iş ancak aşağıdaki maddelerin **hepsi** sağlandığında bitmiştir. Ajan, işi teslim ederken bu listeyi PR açıklamasında işaretler. İşaretleyemediği madde varsa nedenini yazar. "Sonra yapılır" maddesi bırakılmaz; ayrı iş olarak kaydedilir.

## Her iş için
- [ ] İstenen davranış, ilgili belgedeki kurala birebir uyuyor (`docs/alan/`, `docs/urun/urun-kurallari.md`). Belgeyle çelişen kod yok.
- [ ] Belgede olmayan bir karar verildiyse belge güncellendi veya `docs/acik-sorular.md`'ye varsayım olarak işlendi.
- [ ] Testler yazıldı ve geçiyor (`docs/standartlar/test-stratejisi.md`). Hiçbir test atlanmadı veya zayıflatılmadı.
- [ ] Lint, biçim ve analizör uyarısız.
- [ ] Gizli değer, gerçek kişi verisi, sabit fiyat veya yer tutucu değeri kodda yok.
- [ ] Commit ve PR biçimi `git-akisi.md`'ye uygun.

## Backend özelliği
- [ ] Önce sözleşme: uç `contracts/openapi.yaml`'da, lint + breaking geçiyor, istemciler yeniden üretildi.
- [ ] `WithName` = `operationId`; yanıtlar sözleşme testinden geçiyor.
- [ ] Yetki: `RequirePermission` doğru izinle; 403 ve 404 (başka marka/şube) testleri var.
- [ ] Kiracı: yeni tablo `IBrandOwned`, RLS politikası migration'da, izolasyon testi geçiyor.
- [ ] Durum değişikliği aggregate metoduyla; her geçiş için test.
- [ ] Zaman `IClock` ile; para `Money` ile; telefon `PhoneNumber` ile.
- [ ] Yan etkiler (bildirim, sayaç, prim) outbox/olay ile ve aynı transaction'da.
- [ ] Yeni hata kodu katalogda, `ErrorCode` enum'unda ve iki istemci mesaj dosyasında.
- [ ] Migration ileri çalışıyor; boş veritabanında ve dolu (seed) veritabanında denendi.
- [ ] Günlükte PII yok.
- [ ] Yeni ayar veya süre `sureler-ve-ayarlar.md`'de.

## Web/mobil ekranı
- [ ] Tasarım ekranıyla yan yana karşılaştırıldı (`design/ekranlar/<yüzey>/<Ekran>.png` ve `…Koyu.png`). Ekran görüntüleri PR'da.
- [ ] Açık ve koyu tema.
- [ ] Durumlar: yükleniyor (150 ms kuralı), boş (tasarımdaki boş hâl), hata (ilgili durum deseni), dolu. Boş durumda sahte sayı yok.
- [ ] Yalnız semantik tokenlar; sabit renk ve ham palet yok.
- [ ] Erişilebilirlik: klavye, odak halkası, `aria-label`, durum metni; mobilde 44 px dokunma, 1.3× yazı ölçeği.
- [ ] Metinler ses-tonuna uygun ("siz", ünlemsiz, "lütfen/başarıyla" yok, fiille başlayan buton).
- [ ] Tarih, saat, para, telefon biçimlendirme yardımcısından; saat dilimi şubeninki.
- [ ] Hata `code`'una göre doğru desen ("Saat az önce doldu", "Kayıt çakışması"…).
- [ ] Rol: izni olmayan kullanıcıda menü ve buton gizli, doğrudan URL'de "yetkiniz yok".
- [ ] Panel: 1199 ve 899 px kırılımları, `pointer: coarse`.
- [ ] Rezervasyon: performans bütçesi (Lighthouse CI).

## Dilim tamamlandı
- [ ] `docs/dilimler/dilim-0X.md` kabul kriterlerinin hepsi geçiyor (uçtan uca testlerle).
- [ ] İlgili kritik akışın Playwright/integration testi var.
- [ ] Ürün sahibine demo (staging veya ekran kaydı) yapıldı ve onay alındı.
- [ ] Açık sorulardan bu dilimi etkileyenler kapandı veya varsayım onaylandı.
