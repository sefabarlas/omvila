// Hesap silme metinleri tek yerde: hukuk metni (docs/acik-sorular.md H-04)
// gelince yalnız bu dosya değişir. Davranış T-104 (7) ve W7: silme hemen
// uygulanır, yaklaşan randevular iptal edilir, salonlardaki kayıtlar
// anonimleşir.

/// HesapSil başlığı.
const hesapSilBaslik = 'Hesabınız ve verileriniz silinsin mi?';

/// "2 yaklaşan randevunuz iptal edilecek."
String iptalEdilecekBaslik(int count) =>
    '$count yaklaşan randevunuz iptal edilecek.';

/// Uyarı kutusunun son cümlesi.
const iptalEdilecekNot = 'Salonlara iptal bildirimi gider.';

/// "Silinenler" ve "Salonlarda kalanlar" maddeleri: (başlık, açıklama).
const silinenler = [
  ('Profiliniz', 'Ad, telefon, doğum yılı ve izinleriniz.'),
  ('Bildirim ve hatırlatmalar', 'Size SMS, push ya da e-posta gönderilmez.'),
];

/// Salonlarda adınız gizlenmiş olarak kalan kayıtlar.
const kalanKayitlar = (
  'Geçmiş randevu ve ödeme kayıtları',
  'Salonlar yasal yükümlülükleri için bu kayıtları adınız gizlenmiş olarak '
      'saklayabilir.',
);

/// Kullanılabilir paketi olana.
const kalanPaketler = (
  'Kalan paket seansları',
  'Salondaki paketiniz kullanılamaz hale gelir. Önce salonla görüşmenizi '
      'öneririz.',
);

/// HesapSil ana eylemi.
const hesapSilEylem = 'SMS koduyla onayla ve sil';

/// Kod ekranı açıklaması.
String hesapSilKodAciklama(String phoneMasked) =>
    '$phoneMasked numarasına gönderilen 6 haneli kodu girin. Kod doğrulanınca '
    'hesabınız hemen silinir ve bu işlem geri alınamaz.';

/// HesapSilindi.
const hesapSilindiBaslik = 'Hesabınız silindi';

/// HesapSilindi açıklaması.
const hesapSilindiAciklama =
    "Verileriniz OMVILA'dan silindi ve yaklaşan randevularınız iptal edildi. "
    'Aynı numarayla yeniden kayıt olursanız yeni ve boş bir hesap açılır.';
