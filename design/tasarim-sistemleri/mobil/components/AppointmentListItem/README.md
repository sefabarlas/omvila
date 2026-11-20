# AppointmentListItem

Salon uygulamasında günün randevu listesi satırı.

- Solda saat, durum noktası, müşteri ve hizmet. "Onaylandı" durumunda sağda "Geldi", "Onay bekliyor" durumunda "Onayla" eylemi; diğer durumlar rozetle.
- Sola kaydırma: Geldi; sağa kaydırma: Ara. Salon `MaskCustomerPhoneFromStaff` açtıysa personel için "Ara" gösterilmez. Kaydırma eylemleri butonla da yapılabilir.
- İptal edilen satır üstü çizili kalır.

**Flutter:** `Dismissible` yerine `flutter_slidable`; satır `ListTile` benzeri özel widget.
