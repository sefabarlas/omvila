# web/rezervasyon/: Rezervasyon sayfası ajan kuralları

İşletmenin müşterisinin üyeliksiz randevu aldığı sayfa (`omvila.com/{salon}`) ve SMS bağlantılarıyla açılan sayfalar (`/r/{kod}` randevu yönetimi, `/b/{kod}` bekleme teklifi, `/d/{kod}` değerlendirme). Çoğunlukla telefonda, yavaş bağlantıda açılır. Önce kök [AGENTS.md](../../AGENTS.md)'yi okuyun.

Zorunlu okuma: [kodlama-typescript.md](../../docs/standartlar/kodlama-typescript.md) (Next.js bölümü dahil), [design/tasarim-sistemleri/mobil/README.md](../../design/tasarim-sistemleri/mobil/README.md) (rezervasyon akışı ve mobil ölçüler), [musaitlik.md](../../docs/alan/musaitlik.md) (ne gösterildiğini anlamak için).
Ekranlar: `design/ekranlar/rezervasyon-sayfasi/` (akış sırası: Sube → Main(hizmet) → Personel → Saat / DoluGun → Telefon → Kod → Onay → Talep | Havale → Basari; SMS bağlantısı: Randevum, Ertele, Iptal, BeklemeTeklif, BeklemeDoldu; Masaustu; Durumlar).

## Yığın
Next.js (App Router), React 19, TypeScript strict, CSS Modules + `@omvila/design-tokens`, `@omvila/ui-web/booking` (mobil tasarım sisteminin web bileşenleri), `@omvila/api-client-ts`. Test: Vitest, Playwright, Lighthouse CI.

## Rotalar
| Rota | Render | Not |
|---|---|---|
| `/[brandSlug]` | Sunucu (`generateMetadata` ile Open Graph: salon adı, logo) + istemci adımları | Şube tekse şube adımı atlanır |
| `/r/[code]` | İstemci | Randevum; saat değiştir `?islem=saat`, iptal `?islem=iptal` (T-59) |
| `/b/[code]` | İstemci | Bekleme teklifi (15 dk geri sayım) |
| `/d/[code]` | İstemci | Değerlendirme (Dilim 5) |

## Akış kuralları
- Dört adım: **Hizmet → Personel → Saat → Onay.** Hiçbir adımda şifre veya üyelik istenmez.
- Adım durumu URL'de (`?branch=…&services=a,b&staff=…&date=…&time=…`); yenileme ve geri tuşu akışı korur.
- Alt çubukta toplam süre ve tutar canlı güncellenir ("2 hizmet · 1 sa 15 dk").
- Personel adımında ilk seçenek "Fark etmez" (marka ayarı kapalıysa gösterilmez; personel seçimi kapalıysa adım atlanır).
- Gün şeridi `POST …/availability/days`, saat ızgarası `POST …/availability/slots`. Dolu gün: "Perşembe dolu · en yakın uygun saat …" + "Bekleme listesine katılın".
- Telefon → `public/otp/request` → kod → `verify` → `bookingToken` (bellekte). `AlreadyVerified` gelirse kod adımı atlanır. "Tekrar hoş geldiniz, Ayşe" `knownCustomerFirstName`'den.
- Bilgiler ekranı (Telefon.png): ad, telefon, not, aydınlatma metni onayı (zorunlu), kampanya izni (isteğe bağlı, varsayılan kapalı).
- Onay ekranı: özet, paketten düşme anahtarı (`my-packages`), kapora kutusu (hizmette kapora varsa bilgi). Alt not: "Ödeme sayfadan alınmaz; havaleyi kendi bankanızdan yaparsınız."
- `Randevuyu oluştur` → `Idempotency-Key` ile. Sonuç:
  - `Confirmed` → Başarı ("Randevunuz onaylandı").
  - `PendingApproval` + kapora yok → Talep ("Talebiniz alındı", "en geç 2 saat içinde").
  - `DepositPending` → Havale ekranı: alıcı, IBAN, açıklama kodu, tutar **kopyalanabilir**; kalan süre geri sayımı `holdExpiresUtc`'den. "Havaleyi yaptım, salona bildir" (Dilim 4). Bildirimden sonra "Salon onayı bekleniyor" gösterilir; "onaylandı" yalnız gerçekten onaylanınca (S-03).
- `409 slot_taken` → "Saat az önce doldu" + alternatif saat çipleri. `403 booking_disabled` → "Online randevu kapalı · Salonu ara". `429` → kod deneme sınırı deseni. `404 access_code_invalid` → "Bu randevu bulunamadı".
- Masaüstünde iki kolon: solda salon kartı + canlı özet (≤ 380 px), sağda adımlar (≤ 680 px).
- Saatler şubenin `timeZone`'unda gösterilir.

## Performans ve erişilebilirlik
- Salon sayfası JS < 120 KB gzip, LCP < 2,5 sn (yavaş 4G), CLS < 0,1. Lighthouse CI eşikleri PR'da kontrol edilir.
- Mobil metin en az 16 px; dokunma alanı 44 px; ekran okuyucu durumları okur.

## Yasaklar
Kart formu, ödeme SDK'sı, salon arama/keşif, giriş/kayıt zorunluluğu, üçüncü taraf izleme betiği (KVKK; analitik kararı ayrıca alınır).

## Komutlar
```bash
pnpm --filter @omvila/rezervasyon dev     # http://localhost:3000/kuafor-elif
pnpm --filter @omvila/rezervasyon test
pnpm --filter @omvila/rezervasyon e2e
pnpm --filter @omvila/rezervasyon lighthouse
```
