# AppointmentTicket

Müşteri uygulamasında randevu kartı: salon, hizmet ve personel, tarih/saat/süre, tutar, eylemler.

- Gelecek randevuda "İptal et" ve "Takvime ekle"; geçmişte "Değerlendir". Onay bekleyen randevuda eylem yerine "Salon onaylayınca SMS ile bildireceğiz" yazar.
- İptal salonun iptal süresi geçtiyse devre dışı ve nedeni yazılır.

**Flutter:** `Card` + `Column`; kesik çizgi için `CustomPaint`; takvime ekleme `add_2_calendar`.
