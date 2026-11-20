# SalonHeader

Rezervasyon sayfası ve salon detayının başlığı: kapak, salon monogramı veya logosu, ad, kategori, puan, adres.

- Salon fotoğrafı varsa kapak fotoğraf olur; yoksa marka bloğu.
- Puan her zaman değerlendirme sayısıyla birlikte.

**Flutter:** `Stack` (kapak + taşan logo) + `Column`; puan `Icon(LucideIcons.star, color: r.yildiz)`.
