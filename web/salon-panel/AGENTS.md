# web/salon-panel/: Salon paneli ajan kuralları

İşletmenin ana çalışma alanı: takvim, randevular, müşteriler, kasa, hizmetler, personel, paketler, raporlar, ayarlar. Önce kök [AGENTS.md](../../AGENTS.md)'yi okuyun.

Zorunlu okuma: [kodlama-typescript.md](../../docs/standartlar/kodlama-typescript.md), [design/tasarim-sistemleri/panel/README.md](../../design/tasarim-sistemleri/panel/README.md), [roller-yetkiler.md](../../docs/alan/roller-yetkiler.md).
Ekranlar: `design/ekranlar/salon-paneli/` (açık ve `…Koyu.png`). Ekran → dilim → uç eşlemesi: [ekran-envanteri.md](../../design/ekran-envanteri.md).

## Yığın
React 19, TypeScript strict, Vite, TanStack Router (dosya tabanlı), TanStack Query, React Hook Form + Zod, CSS Modules + `@omvila/design-tokens`, `@omvila/ui-web`, `@omvila/api-client-ts`, `@dnd-kit` (takvim sürükle-bırak), `lucide-react`. Test: Vitest + Testing Library, Playwright.

## Yapı
```
src/
  app/            # router, query client, auth sağlayıcı, tema, düzen (kenar menü, üst bar)
  routes/         # TanStack Router dosya tabanlı rotalar: /giris, /kayit, /, /takvim, /musteriler/$id, /ayarlar/...
  features/<alan>/  # calendar, appointments, customers, services, staff, settings, onboarding…
    api.ts          # sorgu ve mutasyon kancaları (yalnız api-client-ts)
    components/     # özelliğe özel bileşenler
  shared/
```
Rota adları Türkçe URL'dir (`/takvim`, `/musteriler`, `/ayarlar/kapora`), bileşen ve kod adları İngilizce.

## Panel özel kuralları
- **Düzen:** kenar menü 248 px (1280 px altında 64 px; tasarımın tablet kırılımları 1199 → 72 px ikon şeridi, 899 → tek sütun bölümler). Üst bar 56 px. Her sayfada üst barda tek `primary` eylem.
- **Menü ve butonlar izinle:** `useSession().can('appointment.approve')`. İzin listesi `GET /manage/session`'dan. Personel rolünde menü: Takvimim, Müşteriler, Kapıda sıra, Primim.
- **Takvim:** kendi bileşenimiz (`ui-web/calendar`): 1 saat = 64 px, 15 dk = 16 px; blok rengi durumu anlatır (`durum-*` tokenları), personel rengi yalnız kolon başlığı avatar halkasında. Bekleme dilimi blok içinde taralı "Bekleme · müsait" şeridi. Şu anki saat mercan çizgi. Boş alana tıklama → o saatte yeni randevu. Bloğa tıklama → sağ yan panel.
- **Saat dilimi:** takvim şubenin `timeZone`'u ile çizilir; tarayıcı saat dilimi kullanılmaz.
- **Şerit desenleri:** deneme süresi (bilgi), ödeme gecikti (turuncu, "Kartı güncelle / Havale ile öde"), salt okunur (kırmızı; yeni randevu butonu pasif ve nedeni yazar). Kaynak: `session.subscription`.
- **Eşzamanlı düzenleme:** `412` → "Kayıt çakışması" şeridi + "Yenile". Sürükle-bırakta `409 slot_taken` → blok eski yerine döner + uyarı.
- **Çalışma saati dışı:** `409 outside_working_hours` → onay iletişim kutusu → `allowOutsideWorkingHours: true` ile tekrar.
- **Bento blokları:** her renkten en fazla bir blok; boşta `0` veya `—`.
- **Kayıt formu (T-130):** alan sırası Adınız soyadınız (`autocomplete="name"`, zorunlu), İşletme adı, Ne tür bir işletme?, Cep telefonunuz, onay. Ad kuralları `@omvila/ui-web/format` `personNameError` / `normalizePersonName` (sunucudaki `PersonName` ile aynı); ad kod doğrulanana kadar formda bekler, `signup` isteğinde `ownerFullName` olarak gider. Kenar menü adı `session.user.fullName` (boşsa marka adı).
- **Durum desenleri:** `design/ekranlar/salon-paneli/Durumlar.png` (kayıt çakışması, saat doldu, SMS kod sınırı, kontör az, 404, 403, ödeme gecikti, salt okunur, Google bağlantısı koptu, kapora alınamaz). Hepsi `Banner` veya `EmptyState` ile.

## Komutlar
```bash
pnpm --filter @omvila/salon-panel dev          # http://localhost:5173 (API: VITE_API_URL, varsayılan Prism :4010)
pnpm --filter @omvila/salon-panel test
pnpm --filter @omvila/salon-panel lint
pnpm --filter @omvila/salon-panel e2e          # Playwright, Prism mock'a karşı
```

## Ekran tamamlandı sayılmaz, eğer
Açık ve koyu temada tasarım görüntüsüyle yan yana karşılaştırılmadıysa; boş, yükleniyor ve hata hâlleri yoksa; 1199/899 px kırılımları denenmediyse; rolü olmayan kullanıcıda gizlenmiyorsa.
