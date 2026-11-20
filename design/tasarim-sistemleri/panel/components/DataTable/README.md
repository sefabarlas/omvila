# DataTable

Müşteri, hizmet, kasa hareketleri gibi listeler.

- Satır 48px; ilk kolon kişi/öğe (avatar + iki satır), sayılar ve tutarlar sağa hizalı ve `tabular-nums`.
- Satır eylemleri sondaki "…" menüsünde; satıra tıklamak detayı açar.
- Seçim varsa toplu eylem çubuğu tablonun üstünde belirir.
- Altbilgide toplam ve sayfalama: "1.284 müşteriden 1–4".
- Tüketici: `columns` (`key`, `label`, `align`, `numeric`, `render`), `rows`, `selectable`, `footer`.
