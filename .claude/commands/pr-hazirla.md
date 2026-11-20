---
description: Mevcut dal için tanım-bitti kontrolü yapıp PR açıklaması hazırla
---
1. `git log origin/main..HEAD` ve `git diff --stat origin/main...HEAD` ile değişiklikleri özetle.
2. İlgili projelerde lint, test ve (backend için) sözleşme kontrollerini çalıştır. Başarısız olan varsa PR hazırlama; önce düzelt.
3. `docs/standartlar/tanim-bitti.md` listesinden bu değişikliğe uyan maddeleri tek tek doğrula.
4. `.github/pull_request_template.md` biçiminde PR açıklaması yaz: Ne, Neden/Bağlam (dilim, belgeler, açık sorular), Nasıl doğrulandı, Tanım-bitti işaretleri, Varsayımlar.
5. PR başlığını commit biçiminde öner (`feat(salon-panel): …`).
6. Verilen varsayımları `docs/acik-sorular.md`'ye işle (henüz işlenmediyse).
