# DayCalendar

Günlük takvim; her personel bir kolon, 15 dakikalık ızgara.

- Bir saat `takvim-saat` (64px); yarım saat çizgisi silik.
- Şu anki saat mercan çizgi ve saat etiketiyle tüm kolonları keser.
- Kolon başlığında personelin adı, günlük randevu sayısı (iptaller hariç) ve bekleyen randevu varsa uyarı. Avatarın çevresindeki halka personelin takvim rengidir (`Staff.CalendarColor`); bloklar ise her zaman duruma göre renklenir.
- Çok şubeli personel yalnızca seçili şubedeki randevularıyla görünür.
