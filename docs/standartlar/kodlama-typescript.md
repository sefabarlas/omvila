# Kodlama standartları: TypeScript (web/, packages/ui-web, packages/api-client-ts)

## Genel
- TypeScript `strict: true`, `noUncheckedIndexedAccess: true`, `exactOptionalPropertyTypes: true`. `any` yasak (ESLint `no-explicit-any: error`); gerçekten bilinmiyorsa `unknown` + daraltma.
- ESLint (flat config, `typescript-eslint` strict-type-checked) + Prettier. CI'da uyarı da hata sayılır.
- Modül çözümü ESM; içe aktarmalarda proje içi takma ad `@/`.
- Dosya adı: bileşen `PascalCase.tsx`, diğerleri `kebab-case.ts`. Bir dosyada bir dışa aktarılan bileşen.

## API erişimi
- **Yalnız** `@omvila/api-client-ts` kullanılır (sözleşmeden üretilmiş `openapi-fetch` istemcisi). Elle `fetch('/api/...')` yazılmaz.
- Sunucu durumu TanStack Query'de; sorgu anahtarları `queryKeys.ts` fabrikasından (`queryKeys.calendar(branchId, from, to)`).
- Mutasyonlarda `Idempotency-Key` istemci yardımcısı tarafından otomatik üretilir. `If-Match` sorgu verisindeki `version`'dan gelir.
- `412 concurrency_conflict`: kullanıcıya "Kayıt çakışması" şeridi, "Yenile" ile sorgu geçersizlenir. Sessizce üzerine yazılmaz.
- `409 slot_taken`: "Saat az önce doldu" + `meta.alternatives` çipleri.
- Hata mesajı `code` → `packages/ui-web/src/errors/tr.ts`. Bilinmeyen kod genel mesaj alır. `title`/`detail` gösterilmez.
- Erişim jetonu bellekte tutulur; `localStorage`'a jeton yazılmaz.

## Bileşenler ve stil
- UI bileşenleri `packages/ui-web`'den gelir (panel tasarım sistemi bileşenlerinin TypeScript karşılıkları: Button, Input, Select, Tabs, Badge/StatusBadge, Banner, EmptyState, Modal, Drawer, Table, Calendar, Bento…). Uygulama içinde aynı görevi gören ikinci bir bileşen yazılmaz; eksikse `ui-web`'e eklenir.
- Stil: CSS Modules + tasarım tokenları (`var(--zemin)`, `var(--metin-ikincil)`). Sabit renk (`#fff`, `rgb()`), ham palet değişkeni (`--mercan-800`) ve sabit piksel aralık yasak; aralık `var(--bosluk-4)` 🔒 (stylelint kuralı).
- Koyu tema `data-theme="dark"` ile; bileşen kendi koyu rengini tanımlamaz, token zaten değişir.
- Erişilebilirlik: her etkileşimli öğe klavyeyle erişilir, odak halkası görünür (`--odak-halkasi`); ikon butonun `aria-label`'ı vardır; durum rozeti metin taşır. `eslint-plugin-jsx-a11y` hata seviyesinde.
- Dokunmatik: `@media (pointer: coarse)` altında küçük butonlar 44 px dokunma alanı alır (ui-web'de hazır).
- Hareket: `--sure-*`, `--egri-*` tokenları; `prefers-reduced-motion` tokens.css'te ele alınır, bileşen ayrıca süre yazmaz.

## Metin, tarih, para
- Tüm kullanıcı metni Türkçe ve `docs/urun/urun-kurallari.md` ses-tonuna uygun. Metinler bileşen içinde sabit yazılabilir (v1 tek dil), ama **biçimlendirme** yalnız `@omvila/ui-web/format` ile: `formatMoney`, `formatDate`, `formatTime`, `formatDuration`, `formatPhone`.
- Zaman dönüşümü yalnız `@omvila/ui-web/time` ile ve her zaman şubenin `timeZone`'u ile. `new Date().getHours()` gibi cihaz saat dilimine bağlı kod yasak 🔒 (ESLint `no-restricted-syntax`).
- Türkçe büyük harf: CSS `text-transform: uppercase` + `lang="tr"`; JS'de `toLocaleUpperCase('tr-TR')`.

## Durum ve yönlendirme
- Panel: TanStack Router (dosya tabanlı, tip güvenli parametreler). URL durumun kaynağıdır: seçili şube, tarih, görünüm (`/calendar?branch=…&date=2026-11-25&view=day`).
- Global istemci durumu en aza indirilir; gerekiyorsa Zustand, tek mağaza, alt kümeler.
- Formlar React Hook Form + Zod. Zod şemaları sözleşme tiplerinden türetilir veya onlarla `satisfies` ile hizalanır.

## Rezervasyon (Next.js) özel
- Sunucu bileşenleri yalnız okuma yapar (salon kartı, katalog) ve `generateMetadata` ile Open Graph etiketleri üretir.
- Adım akışı istemci bileşeni; adım durumu URL arama parametresinde (`?step=time&services=…`). Sayfa yenilenince akış korunur.
- Next.js API route'larında veya server action'larda iş mantığı yazılmaz; doğrudan .NET API çağrılır.
- İlk yükleme bütçesi: JS < 120 KB gzip (salon sayfası), LCP < 2,5 sn (yavaş 4G). CI'da Lighthouse kontrolü.

## Yasaklar
- Kart bilgisi alan herhangi bir form veya ödeme SDK'sı (ürün kuralı 1).
- Salon arama/keşif ekranı (ürün kuralı 2).
- Ekranda sahte sayı veya rozet; boş durumda `0` veya `—`.
- `dangerouslySetInnerHTML` (salonun girdiği metinler dahil).
