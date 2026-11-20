# Button

Mobil buton; ekranın ana eylemi tam genişlik ve altta.

| Varyant | Ne zaman |
|---|---|
| `primary` | Ekranın asıl işi: "Randevu al", "Devam et" |
| `secondary` | Alternatif: "Başka saat seç" |
| `ghost` | Düşük öncelik: "Daha sonra" |
| `danger` | Geri alınamaz işlem, onay diyaloğunda |

- Boyutlar: `lg` 52px (alt sabit ana eylem), `md` 48px, `sm` 36px (liste içi).
- Metin fiille başlar, 1–3 kelime. Uzun işlemde `loading` ve "Onaylanıyor…".
- Bir ekranda tek `primary`.

**Flutter:** `FilledButton` (primary), `OutlinedButton` (secondary), `TextButton` (ghost), `FilledButton` + `backgroundColor: r.hata` (danger). Ölçüler `OmvilaTema`'da hazır.
