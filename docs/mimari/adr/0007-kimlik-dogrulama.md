# ADR-0007: Kimlik doğrulama ve oturum

- Durum: Kabul edildi
- Tarih: 2026-11-20

## Bağlam
Salon tarafında giriş yalnız telefon + SMS kodudur. Müşteri uygulaması da telefonla girer. Rezervasyon sayfası üyeliksizdir ama telefon doğrulaması ister. Üç ayrı kullanıcı türü (salon, platform, müşteri) birbirinin uç noktasına erişmemelidir.

## Karar
- **OTP:** 6 hane, 180 sn geçerli, 60 sn yeniden gönderme beklemesi, 5 deneme, sonra 5 dk kilit. Kod HMAC-SHA256 ile hash'lenip saklanır. Amaç (`Purpose`) koda bağlıdır; giriş kodu rezervasyonda geçmez. Telefon başına saatte 5, IP başına ayrı sınır.
- **Erişim jetonu:** JWT, ES256 imzalı, 15 dk. Anahtarlar döndürülebilir (JWKS uç noktası, `kid`). `aud`: `manage`, `admin`, `me`, `public-booking`.
  - `manage` iddiaları: `sub` (UserId), `brand_id`, `membership_id`, `role`, `branch_ids` (Owner için `*`), `staff_id?`. İzinler jetona **yazılmaz**; sunucu rolden çözer (rol değişince jeton beklemeden etkili olur, önbellek 60 sn).
  - `admin`: `sub`, `platform_role`.
  - `me`: `sub` (CustomerId).
  - `public-booking`: `phone` (E.164), `otp_id`; 30 dk; yalnız rezervasyon oluşturma ve bekleme listesine katılma uçlarında geçerli.
- **Yenileme jetonu:** opak, 256 bit rastgele, yalnız hash saklanır, her kullanımda döndürülür. Eski jeton tekrar kullanılırsa aile (`FamilyId`) tümden iptal edilir.
- **Web paneller:** erişim jetonu bellekte. Yenileme jetonu `HttpOnly; Secure; SameSite=Strict; Path=/api/v1/manage/auth` çerezinde ("Bu bilgisayarda 30 gün hatırla" işaretliyse 30 gün, değilse oturum çerezi). Yenileme ucu ayrıca `X-Requested-With` başlığı ister (CSRF). Panel ve API aynı kayıtlı alan altında (`panel.omvila.com`, `api.omvila.com`).
- **Mobil:** yenileme jetonu `flutter_secure_storage`'da (Keychain / Keystore).
- **Rezervasyon sayfası:** OTP sonrası `public-booking` jetonu (bellekte). Ayrıca `TrustedDevice` çerezi (30 gün): aynı tarayıcı aynı telefonla tekrar geldiğinde kod istenmez. Bunun için OTP isteği sunucuda atlanır ve doğrudan jeton verilir.
- **SMS bağlantısı:** `omvila.com/r/{code}` erişim kodu jeton değildir. Uç nokta kodu doğrular ve yalnız o randevu üzerinde işlem yaptırır.
- **Kayıt (signup):** `otp/verify` kullanıcı yoksa kısa ömürlü `signupToken` döner. `POST /manage/auth/signup` marka, ilk şube ve Owner üyeliğini tek transaction'da oluşturur.
- **Personel daveti:** Owner personeli telefonla ekler. "Davet SMS'i gönder" açıksa SMS gider, `UserMembership.Status = Invited`. Personel aynı numarayla ilk girişte `Active` olur.

## Sonuçlar
- Şifre, e-posta doğrulama, sosyal giriş yok.
- Hesap silme ve kritik işlemler yeni OTP ister (`Purpose = AccountDeletion`).
- Jeton çalınmasına karşı kısa ömür + rotation; şüpheli tekrar kullanımda tüm oturum düşer.

## Değerlendirilen alternatifler
- ASP.NET Identity: şifre merkezli, gereksiz tablolar.
- Harici kimlik sağlayıcı (Auth0, Keycloak): SMS OTP maliyeti ve Türkiye SMS sağlayıcı entegrasyonu zor; ek bağımlılık.
- Cookie oturumu (her yerde): mobil ve çok alanlı yapı için JWT daha uygun.
