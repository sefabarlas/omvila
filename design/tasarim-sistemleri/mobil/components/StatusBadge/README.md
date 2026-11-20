# StatusBadge

Randevu durumu; panel ile aynı yedi durum ve aynı renkler.

| `status` | Etiket |
|---|---|
| `onay` | Onay bekliyor |
| `kapora` | Kapora bekleniyor |
| `onaylandi` | Onaylandı |
| `geldi` | Geldi |
| `tamamlandi` | Tamamlandı |
| `gelmedi` | Gelmedi |
| `iptal` | İptal |

- Veritabanı eşlemesi Panel Sistemi'ndeki StatusBadge kartındadır; `Draft` gösterilmez.
- Müşteri uygulamasında "Gelmedi" yerine "Kaçırıldı" yazılmaz; durum adları her yerde aynıdır.

**Flutter:** `OmvilaDurumRozeti(durum: Durum.onayBekliyor)`; API'nin `displayStatus` alanından eşlenir; renkler `r.durumKaporaZemin / Metin / Nokta`.
