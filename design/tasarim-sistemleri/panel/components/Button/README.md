# Button

Eylem butonu; bir ekranda en fazla bir `primary`.

| Varyant | Ne zaman |
|---|---|
| `primary` | Sayfanın asıl işi: "Randevu ekle", "Kaydet" |
| `secondary` (varsayılan) | Diğer eylemler: "Vazgeç", "Düzenle" |
| `ghost` | Araç çubuğu, tablo içi, düşük öncelikli |
| `danger` | Geri alınamaz işlem: "Randevuyu iptal et" (onay modalı ile) |

- Boyutlar: `sm` 28px (tablo, kart içi), `md` 36px, `lg` 40px (modal ve form sayfası ana eylemi).
- Metin fiille başlar, 1–3 kelime, cümle düzeninde: "Randevu ekle", "Geldi olarak işaretle".
- Uzun süren işlemde `loading`; metin "Kaydediliyor…" olur, buton genişliği değişmez.
- Devre dışı buton yerine mümkünse etkin bırakıp nedenini mesajla söyleyin.
