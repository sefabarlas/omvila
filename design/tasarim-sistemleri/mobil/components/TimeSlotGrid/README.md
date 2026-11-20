# TimeSlotGrid

Uygun saatler 4 kolon; dolu saatler üstü çizili ve devre dışı.

- Saatler hizmetin toplam süresine göre üretilir.
- Hiç saat yoksa: "Bu gün dolu. En yakın boş gün: Perşembe".

**Flutter:** `GridView.count(crossAxisCount: 4, childAspectRatio: …)`; yükseklik `OmvilaBoyut.saatDilimi`.
