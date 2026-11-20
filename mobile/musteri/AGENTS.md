# mobile/musteri/: OMVILA (müşteri uygulaması) ajan kuralları

İşletmelerin müşterilerinin uygulaması: "Randevularım bir arada." Ayda bir iki kez açılır; sakin, ferah, güven veren. Önce kök [AGENTS.md](../../AGENTS.md)'yi okuyun.

Zorunlu okuma: [kodlama-flutter.md](../../docs/standartlar/kodlama-flutter.md), [design/tasarim-sistemleri/mobil/README.md](../../design/tasarim-sistemleri/mobil/README.md) (iki ton, rezervasyon akışı, boş durumlar), [urun-kurallari.md](../../docs/urun/urun-kurallari.md).
Ekranlar: `design/ekranlar/musteri-uygulamasi/` (Acilis, Giris, GirisKod, BildirimIzni → Main, MainBos (ilk giriş), MainYaklasanYok, MainBildirimKapali, Paketler(+Bos), Profil → YRSalon(+Bos), YRHizmet, YRPersonel, YRSaat, YROnay, YRHavale, YRBasari → BeklemeKatil, BeklemeTeklif, BeklemeDoldu → RandevuDetay, RandevuErtele, ErteleKapali, Iptal → Gecmis(+Bos), Degerlendir → Odemelerim(+Bos), Izinler, HesapSil, HesapSilindi, ProfilDuzenle, Bildirimler(+Bos), Durumlar).

## Yapı ve yığın
`mobile/isletme` ile aynı yığın. Uygulama kimliği `com.omvila.app`; ad "OMVILA"; ikon açık kum zemin. Derin bağlantılar: `omvila.com/{slug}` (salonu ekle / randevu al), `omvila.com/r/{code}`, `/b/{code}`, `/d/{code}`.

## Özel kurallar
- API: `/api/v1/me` (müşteri jetonu) + randevu almak için `/api/v1/public` uçları müşteri jetonuyla (sözleşmede `meBearer` güvenliği). Uçlar Dilim 5'te sözleşmeye girer.
- Sekmeler: Randevularım, Paketlerim, Profil. **Keşif/arama sekmesi yok.**
- **İlk giriş (boş):** salon keşfi olmadığı için "QR kodu okut" ve "Bağlantıyı yapıştır" (`POST /me/brands`). "Nasıl çalışır" anlatımı.
- **Yaklaşan randevu yoksa** boş ekran değil, son randevudan "Aynısını tekrar al".
- **Randevu alma** rezervasyon sayfasıyla aynı akış ve kurallar ([web/rezervasyon/AGENTS.md](../../web/rezervasyon/AGENTS.md) § Akış kuralları). Telefon zaten doğrulandığı için kod adımı yoktur.
- **Ödemelerim salt okunur.** Kapora durumları: "Salon onayı bekleniyor", "Alındı", "Salon iade etti". "Havaleyi yaptım" yalnız salona bildirimdir.
- **Bildirim izni** kendi ekranımızla; "Şimdi değil" → "Hatırlatmalar SMS ile gelir" şeridi.
- **İzinlerim:** randevu mesajları kapatılamaz (açıklama metni). Salon bazında yalnız kampanya izni anahtarı.
- **Hesap silme:** yaklaşan randevular iptal edilir uyarısı, silinen/korunan veri listesi, SMS koduyla onay.
- **Değerlendirme:** yalnız tamamlanan randevu, bir kez; puan, isteğe bağlı yorum, personel. Etiket yok.
- **İptal:** neden zorunlu. Son süre geçmişse buton yerine "Salonu ara". Saat değişikliği ayrı son süre.

## Komutlar
```bash
cd mobile/musteri
fvm flutter run --dart-define=API_URL=http://10.0.2.2:4010
fvm flutter test
fvm flutter analyze --fatal-infos
```
