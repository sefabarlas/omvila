# ADR-0004: Çakışan randevu engeli

- Durum: Kabul edildi
- Tarih: 2026-11-20

## Bağlam
Aynı personele aynı anda iki randevu yazılmamalı. Bu kural, aynı slota paralel gelen online isteklerde, iki şubede çalışan personelde, bekleme dilimli hizmetlerde (boya beklerken personel boşta) ve oda/cihaz kaynaklarında da geçerli olmalı.

## Karar
Üç katman (ayrıntı `docs/alan/musaitlik.md`):
1. **Exclusion constraint** son garantidir. Kısıt randevu tablosuna değil, `appointment_item_segments` üzerindeki `(staff_id, during)` çiftine konur. Koşulu `staff_busy AND is_active` olduğu için bekleme dilimi personeli kilitlemez, iptal edilen randevu saati boşaltır. Tek kapasiteli kaynaklar için `appointment_item_resources` üzerinde aynı yapı kullanılır.
2. **Advisory lock** (`pg_advisory_xact_lock`, personel + yerel gün, sonra kaynak + gün; sıralı) kısıtın ifade edemediklerini korur: şubeler arası geçiş süresi, çok kapasiteli kaynak, çalışma saati. Kilit altında müsaitlik yeniden hesaplanır.
3. **Idempotency-Key** aynı isteğin tekrarını engeller.

`23P01` hatası `409 slot_taken`'e çevrilir.

## Sonuçlar
- Paralel istek testi (C1–C3) CI'da zorunlu.
- Segment satırı her durum değişikliğinde `is_active` güncellemesi ister. Bunu yalnız aggregate metodu yapar.
- Müsaitlik sorgusu okuma tarafında kilitsizdir. Kullanıcı dolmuş bir saati görebilir; "Saat az önce doldu" deseni bunu karşılar.

## Değerlendirilen alternatifler
- Yalnız uygulama kilidi (eski sistem): tek garanti kod; bir uç kilidi unutursa çakışma yazılır.
- Serializable izolasyon: yüksek yeniden deneme oranı, karmaşık hata yönetimi.
