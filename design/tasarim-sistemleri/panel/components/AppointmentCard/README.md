# AppointmentCard

Takvimde bloğa tıklanınca açılan randevu detayı.

- Duruma göre ana eylem: Onay bekliyor → "Onayla" / "Reddet", Kapora bekleniyor → "Kapora linki gönder", Onaylandı → "Geldi olarak işaretle".
- `channel` randevunun nereden geldiğini gösterir: Online, Panel, Telefon, Kapıdan (`Appointment.Channel`).
- İptal ve diğer işlemler "…" menüsünde.
- Tüketici: `appointment`, `staff`, `phone`, `price`, `channel`, `onCheckIn`. `MaskCustomerPhoneFromStaff` açıksa ve kullanıcı personelse `phone` verilmez.
