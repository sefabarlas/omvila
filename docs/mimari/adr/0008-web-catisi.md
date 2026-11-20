# ADR-0008: Web çatısı

- Durum: Kabul edildi (kullanıcı onayı, 2026-11-20)
- Tarih: 2026-11-20

## Bağlam
- **Salon ve platform panelleri:** giriş arkasında, gün boyu açık, yoğun takvim ve sürükle-bırak. SEO gerekmez.
- **Rezervasyon sayfası:** SMS bağlantısıyla çoğunlukla telefonda, yavaş bağlantıda açılır. Hızlı ilk yükleme ve salon başına bağlantı önizlemesi (Open Graph) ister.
- **Tanıtım sitesi:** statik, SEO önemli.
- Panel tasarım sisteminin önizleme bileşenleri React 18 ile yazılmış. Ajanların en çok deneyimi React/TypeScript'te.

## Karar
| Proje | Çatı |
|---|---|
| `web/salon-panel`, `web/platform-panel` | React 19 + TypeScript (strict) + Vite, SPA. Yönlendirme TanStack Router (tip güvenli), sunucu durumu TanStack Query, formlar React Hook Form + Zod. Ortak UI `packages/ui-web` |
| `web/rezervasyon` | Next.js (App Router). Salon sayfası sunucuda render (`generateMetadata` ile önizleme); adım akışı istemci bileşenleri. API'ye doğrudan bağlanır, Next.js sunucusunda iş mantığı yoktur |
| `web/tanitim` | Next.js, statik üretim (`output: 'export'`) |

Ortak:
- Stil: tasarım tokenları CSS değişkenleri olarak (`packages/design-tokens/dist/tokens.css`), bileşen stilleri CSS Modules. Tailwind kullanılmaz; tokenlar tek kaynaktır, sınıf adlarıyla ikinci bir sistem kurulmaz.
- Takvim ızgarası kendi bileşenimizdir (tasarımdaki ölçüler: 1 saat = 64 px, 15 dk = 16 px). Sürükle-bırak için `@dnd-kit`. Hazır takvim kütüphanesi (FullCalendar vb.) kullanılmaz; tasarıma uymaz ve bekleme dilimi gösterimini desteklemez.
- İkon `lucide-react`. Tarih ve saat `@js-temporal/polyfill` ya da `date-fns-tz` ile tek yardımcıdan (`packages/ui-web/time`).
- Test: Vitest + Testing Library, uçtan uca Playwright.
- Paket yöneticisi pnpm (ADR-0010).

## Sonuçlar
- Panel tasarım sistemindeki `bundle.js` bileşenleri **referanstır**, kopyalanmaz. `packages/ui-web` içinde aynı ölçü ve davranışla TypeScript olarak yeniden yazılır.
- Rezervasyon sayfası panel UI paketini değil, mobil tasarım sisteminin web karşılığını kullanır (`packages/ui-web/booking`).

## Değerlendirilen alternatifler
- Hepsi Next.js: paneller için gereksiz sunucu katmanı; .NET'in önüne ikinci bir backend kurma eğilimi.
- Angular: tasarım sistemi React'te; rezervasyon için ayrı çözüm gerekirdi.
- Blazor: tek dil avantajı; mobilde ilk yükleme ağır, ekosistem ve ajan deneyimi zayıf.
