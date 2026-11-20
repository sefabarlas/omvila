# Güvenlik ve KVKK

## Kişisel veri envanteri

| Veri | Nerede | Sınıf | Koruma |
|---|---|---|---|
| Telefon (müşteri, kullanıcı) | `customers.customers`, `identity.users` | Kişisel | Maskeli gösterim (Personel), günlükte yok |
| Ad soyad | aynı | Kişisel | Günlükte yok |
| E-posta | aynı | Kişisel | Günlükte yok |
| Doğum yılı, cinsiyet | `customers.customers` | Kişisel | İsteğe bağlı |
| Salon notları | `brand_customers.notes` | Kişisel (serbest metin) | Sağlık verisi yazılmaz uyarısı; yalnız marka görür; günlükte yok |
| IBAN (salon) | `tenancy.brand_bank_accounts` | Finansal | Uygulama düzeyinde şifreli, listelerde son 4 hane |
| OAuth jetonları (Google) | `integrations.calendar_links` | Gizli | Şifreli |
| OTP kodu | `identity.otp_challenges` | Gizli | HMAC hash, düz metin yok |
| Yenileme jetonu | `identity.refresh_tokens` | Gizli | SHA-256 hash |
| IP adresi | `consent_records`, `audit_log` | Kişisel | Saklama süresi |
| Dekont dosyası | dosya deposu | Finansal | Özel bucket, imzalı kısa ömürlü URL |

**Tutulmayan veri:** sağlık verisi (alerji, hassasiyet, rahatsızlık), tam doğum tarihi, kart bilgisi, kimlik numarası, müşteri fotoğrafı.

## Roller (KVKK)
- Salon (Brand): müşteri verisinin **veri sorumlusu**. OMVILA: **veri işleyen** (varsayım H-02, hukuk onayı bekliyor).
- Aydınlatma metni: varsayılan OMVILA standart metni salon adına gösterilir, salon kendi metnini yükleyebilir. Okundu bilgisi `ConsentRecord(PrivacyNoticeAcknowledged, DocumentVersion)` olarak tutulur.

## Haklar ve işleyiş
| Hak | Nasıl |
|---|---|
| Bilgi alma / erişim | Müşteri uygulaması "İzinlerim"; salon panelinden CSV dışa aktarma |
| Düzeltme | Müşteri profilini düzenler; salon kendi `DisplayName`'ini düzenler |
| Silme (müşteri) | Uygulamada "Hesabımı ve verilerimi sil" (SMS koduyla). Tüm markalarda anonimleştirme, yaklaşan randevular iptal |
| Silme (salona talep) | Panelde "KVKK silme talebi" (yalnız o marka). Ad yazılarak onaylanır; geri alınamaz |
| İtiraz (kampanya) | Salon bazında kampanya izni kapatılır; İYS'ye iletilir |

**Anonimleştirme:** `FullName` → "Silinmiş müşteri", telefon → `+000…{id hash}` (benzersizlik korunur), e-posta ve notlar `null`, `AnonymizedUtc` dolar. Randevu, ödeme ve prim satırları kalır (işlem kaydı yasal yükümlülüğü), müşteri bağı anonim kayıttır. İşlem `AuditLog(Anonymize)` olarak yazılır.

**Saklama:** `docs/alan/sureler-ve-ayarlar.md` § KVKK. Süresi dolan veriyi `ApplyRetention` günlük işi anonimleştirir veya siler.

## Uygulama güvenliği
- **Kimlik:** ADR-0007. OTP hız sınırları, deneme kilidi, jeton döndürme.
- **Yetki:** her uç nokta izin işaretli (mimari test); kiracı izolasyonu çift katman (ADR-0006); kapsam dışı → 404.
- **Girdi:** tüm girdiler sözleşme şemasıyla ve FluentValidation ile doğrulanır. Serbest metinler uzunluk sınırlı. HTML kabul edilmez, çıktıda kaçırılır.
- **SQL:** yalnız EF veya parametreli SQL.
- **Dosya yükleme:** logo ve dekont. MIME ve boyut kontrolü (logo ≤ 2 MB, dekont ≤ 5 MB, PDF/JPG/PNG), yeniden kodlama (resim), ayrı alan adından sunum.
- **CORS:** yüzey bazında izinli kökenler (`panel.`, `admin.`, rezervasyon alanı). Joker yok.
- **Başlıklar:** HSTS, `Content-Security-Policy` (web uygulamaları), `X-Content-Type-Options`, `Referrer-Policy: strict-origin-when-cross-origin`.
- **Erişim kodları:** 8–10 karakter kriptografik rastgele base62. Deneme hız sınırı IP başına; geçersiz kodlar 404.
- **Webhook'lar:** SMS teslim raporları imza veya IP listesiyle doğrulanır.
- **Gizli değerler:** depoda yok (gitleaks CI). Yapılandırma ortam değişkeni / kasa. Geliştirme için `.env.example`.
- **Bağımlılıklar:** Dependabot/Renovate; yüksek ve kritik açık birleştirmeyi durdurur.
- **Platform erişimi:** her görüntüleme denetime yazılır; platform kullanıcıları marka verisini değiştiremez; telefon maskeli.

## İYS (İleti Yönetim Sistemi)
- Kampanya (ticari elektronik ileti) göndermeden önce alıcının İYS'de onaylı olması gerekir. v1'de kampanya gönderimi yok. Rıza toplanır ve `ConsentRecord`'a yazılır; İYS'ye iletim SMS sağlayıcısı seçilince eklenir (T-09).
- Randevu bildirimleri işlem mesajı varsayımıyla İYS dışındadır (H-01).

## Barındırma
- Veri yerleşimi kararı açık (T-07). Yurt dışı barındırma seçilirse KVKK yurt dışı aktarım şartları hukukla değerlendirilir.
- Yedekleme: günlük tam + PITR (en az 7 gün); geri yükleme ayda bir denenir.
