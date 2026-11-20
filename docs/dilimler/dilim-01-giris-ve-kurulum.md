# Dilim 1: Giriş + salon kurulumu

## Amaç
Bir salon sahibi telefonuyla kaydolur, işletmesini oluşturur, şube saatlerini, hizmetlerini ve personelini girer ve rezervasyon bağlantısını alır. Personel davet SMS'iyle girer. Bu dilimin sonunda Dilim 2'nin ihtiyaç duyduğu bütün katalog ve takvim verisi panelden girilebilir.

## Kapsam
**İçinde**
- Teknik temel (aşağıda): monorepo iskeleti, CI, backend çekirdeği, tokenlar, ui-web temel bileşenleri.
- Salon kaydı ve girişi (telefon + SMS kodu), "Bu bilgisayarda 30 gün hatırla", çıkış.
- Oturum (`/manage/session`): rol, izinler, şubeler, abonelik özeti (deneme).
- Kurulum ekranı: "İlk randevunuza 4 adım" ve "Neden önce saatler?" kutusu.
- Ayarlar: İşletme bilgileri, Şubeler ve saatler (özel günler dahil), Şube ekle, Randevu kuralları (yalnız ayar kaydı; davranışı Dilim 2–3'te), Rezervasyon sayfası (bağlantı adı, görünüm).
- Hizmetler: kategoriler, hizmet ekle/düzenle (aşamalar, tampon, kapora alanları görünür ama IBAN yoksa pasif), sektör hazır listesi, şube fiyatı.
- Personel: ekle (davet SMS'i), düzenle, verdiği hizmetler (özel süre, ek ücret), şube saatleri, izin ekle, rol (BranchManager, Staff).
- Sahte SMS sağlayıcı (geliştirme ve test). Gerçek sağlayıcı Dilim 3'te.
- Kiracı izolasyonu ve izin altyapısı (tüm sonraki dilimlerin temeli).

**Dışında:** takvim ve randevu (D2), kapora/IBAN ayarı (D4), bildirim ayarları (D3), roller sayfası (D6), abonelik sayfası (D6), Google Takvim (D6).

## Ekranlar (`design/ekranlar/salon-paneli/`)
Giris, GirisKod, Kayit, KayitKurulum, GenelBakisBos (yalnız ilk hafta bağlantı paylaşma kutusu; bento sayıları 0/—), Ayarlar, AyarlarIsletme, AyarlarSube, SubeEkle, AyarlarRezervasyon, Hizmetler, HizmetlerBos, HizmetEkle, KategoriEkle, Personel, PersonelBos, PersonelEkle, IzinEkle, PersonelRolu (personel girişi sonrası görünüm: menü kısıtlı). Tablet: TabletGenel. Durumlar: SMS kodu deneme sınırı, 403, 404.

## Uçlar (sözleşmede hazır)
`manage-auth/*`, `manageGetSession`, `manageGetOnboarding`, `manage-brand/*`, `manage-branches/*`, `manage-catalog/*`, `manage-staff/*`.

## Alan kuralları
- Kayıt tek transaction: `Brand(Onboarding)` + `BrandSettings` (varsayılanlar `sureler-ve-ayarlar.md`) + `BookingPageSettings` + ilk `Branch` (ad "Merkez", saat dilimi `Europe/Istanbul`, haftalık saat boş) + `User` + `UserMembership(Owner, Active)` + `Staff` (sahibin kendisi, `IsBookableOnline=true`) + `Subscription(Trialing, TrialEndsOn = bugün + 14)` + kontör hediyesi (`CreditTransaction(Grant, [DENEME KONTÖRÜ])`, değer yapılandırmadan) + `TermsAcceptance` (kullanım koşulları ve aydınlatma metni, sürüm + zaman).
- Slug: işletme adından üretilir (Türkçe karakter dönüşümü: ç→c, ğ→g, ı→i, İ→i, ö→o, ş→s, ü→u), çakışırsa `-2`, `-3`. Ayrılmış adlar (`r`, `b`, `d`, `api`, `giris`, `kayit`, `admin`, `panel`, `www`…) kullanılamaz.
- Kurulum adımı tamamlanma kuralları: WorkingHours = şubede en az bir gün aralığı var; Services = en az bir aktif hizmet; Staff = sahibin kendisi dışında en az bir personel **veya** sahip "tek başıma çalışıyorum" dedi (ayar: `onboarding.soloConfirmed`); ShareBookingLink = bağlantı kopyalandı veya QR indirildi (istemci `manageRecordOnboardingEvent` ile bildirir). Hepsi tamamlanınca `Brand.Status = Active`.
- Personel saatleri: "İşletme saatleriyle aynı" seçilirse şube saatleri kopyalanır (bağ tutulmaz). Bir personelin iki şubedeki saatleri çakışamaz.
- Hizmette aşama varsa `durationMinutes` = aşama toplamı (doğrulama). Kapora `mode = Always` iken IBAN yoksa `409 deposit_unavailable_no_iban`.
- Plan sınırları (`Plan.StaffLimit`, `IncludedBranchCount`) personel ve şube eklerken kontrol edilir. Değerler veriden okunur (geliştirmede sınırsız seed).

## Kabul kriterleri
1. **Kayıt:** Kayıtlı olmayan `0532 418 77 05` ile "İşletmenizi oluşturun" → kod → işletme adı "Kuaför Elif", tür "Kuaför" → panel açılır; `GET /manage/session` rol `Owner`, abonelik `Trialing`, deneme bitişi 14 gün sonra; kurulum ekranında 1/5 tamamlandı.
2. **Giriş:** Kayıtlı numarayla giriş → kod → panel. Yanlış kod → `otp_invalid` + kalan deneme; 5. yanlışta `otp_attempts_exceeded` ve 5 dk bekleme; 60 sn dolmadan tekrar kod → `otp_resend_too_soon`.
3. **Numara sızdırmaz:** Kayıtlı olmayan numarayla "Panele giriş" de 202 döner; doğrulamada `SignupRequired` gelir ve kayıt ekranına yönlendirilir.
4. **Hatırla:** "Bu bilgisayarda 30 gün hatırla" işaretliyken tarayıcı kapanıp açılınca oturum sürer; işaretsizken düşer.
5. **Şube saatleri:** Salı 10:00–13:00 ve 13:45–19:00 kaydedilir, okunur; örtüşen aralık `validation_failed`; gece yarısını aşan aralık `validation_failed`.
6. **Özel gün:** 1 Ocak kapalı ve Ramazan Bayramı (3 gün) eklenir; listede "Tek gün" ve "3 gün" görünür.
7. **Hizmet:** "Saç kesimi · 45 dk · 450 ₺ · Elif, Mert, Can" eklenir. "Boya" 45 uygulama + 40 bekleme (personel boşta) + 35 fön aşamalarıyla eklenir; süre 120 dk olarak hesaplanır. IBAN yokken kapora "Her zaman iste" → `deposit_unavailable_no_iban`.
8. **Hazır liste:** Kuaför sektörü şablonlarından 6 hizmet seçilir → kategoriler ve hizmetler oluşur.
9. **Şube fiyatı:** Nişantaşı için "Saç kesimi 550 ₺" → `GET /manage/services?branchId=nisantasi` 550 ₺ ve `isBranchPrice: true` döner; Kadıköy 450 ₺.
10. **Personel:** "Deniz Kara · Personel · Kadıköy" telefonla eklenir; davet SMS'i sahte sağlayıcı günlüğüne düşer; listede "Davet gönderildi". Deniz aynı numarayla giriş yapınca üyelik `Active` olur ve menüde yalnız Takvimim, Müşteriler, Kapıda sıra, Primim görünür.
11. **Personel hizmetleri:** Elif için "Keratin 75 dk (özel)" ve "Balyaj +300 ₺" kaydedilir; okunur.
12. **İzin:** Elif'e 2–4 Aralık yıllık izin eklenir; yanıt `affectedAppointments` boş liste (henüz randevu yok).
13. **Yetki:** Staff rolüyle `PUT /manage/brand/settings` → 403; BranchManager (Kadıköy) ile Nişantaşı şubesini `GET` → 404.
14. **Kiracı izolasyonu:** İkinci marka "Berber Kaan" oluşturulur; Kuaför Elif jetonuyla Berber Kaan'ın şube, hizmet ve personel kimlikleri her uçta 404 döner (otomatik izolasyon testi).
15. **Eşzamanlı düzenleme:** İki oturum aynı hizmeti açar; ilki kaydeder, ikincisi eski `If-Match` ile kaydedince 412 `concurrency_conflict` alır ve panel "Kayıt çakışması · Yenile" şeridini gösterir.
16. **Rezervasyon bağlantısı:** Bağlantı adı `kuafor-elif` → `elif-kuafor` yapılır; eski adres `GET /public/brands/kuafor-elif` 301 ile yeniye yönlenir (Dilim 2 public ucu; bu dilimde yönlendirme tablosu yazılır).
17. **Tasarım:** Listelenen ekranlar açık ve koyu temada tasarımla yan yana karşılaştırılır; boş hâller (HizmetlerBos, PersonelBos, GenelBakisBos) tasarımdaki gibidir.

## Teknik temel (bu dilimde kurulur)
| İş | Çıktı |
|---|---|
| Monorepo | `pnpm-workspace.yaml`, `turbo.json`, `tsconfig.base.json`, ESLint/Prettier/stylelint, kök `pubspec.yaml` (workspace) + `melos.yaml`, `.fvmrc`, `docker-compose.yml` (postgres, prism, mailpit) |
| CI | `.github/workflows/ci.yml`: yol filtreli işler; contracts (lint, breaking, generate diff), api (build, test, arch, contract), web (lint, test, build), flutter (analyze, test), commitlint, gitleaks |
| Backend çekirdeği | Çözüm, `SharedKernel`, `Infrastructure` (bağlantı + transaction + `set_config` RLS + outbox + idempotency + iş zamanlayıcı iskeleti), `Omvila.Host` (sağlık, OpenAPI dışa aktarma, problem+json, hız sınırı), mimari testler, `TestKit`, izolasyon test altyapısı |
| Sözleşme araçları | `contracts/` betiklerinin çalışır hâli; Prism mock docker'da |
| Tokenlar | `packages/design-tokens` build (CSS + Dart), kontrast testleri |
| ui-web temel | Button, Input, Select, Switch, Tabs, Chip, StatusBadge, Banner, EmptyState, Modal, Drawer, Table, Bento, Sidebar, Topbar, Toast, Skeleton; `format/`, `time/`, `errors/tr.ts` |
| salon-panel iskeleti | Router, auth akışı (bellekte jeton + yenileme çerezi), oturum ve izin bağlamı, düzen (kenar menü, üst bar, tablet kırılımları), tema anahtarı |

## Testler
- Alan: slug üretimi, telefon normalleştirme, haftalık saat doğrulama, hizmet süre/aşama doğrulaması, kapora kuralı, plan sınırı.
- Entegrasyon: kabul kriterleri 1–16 (API düzeyinde).
- Uçtan uca (Playwright, Prism mock): kayıt → kurulum adımları; personel daveti → personel girişi menüsü.
