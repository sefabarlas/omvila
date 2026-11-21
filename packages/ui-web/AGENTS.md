# packages/ui-web/: web bileşenleri ve yardımcılar

Salon paneli, platform paneli, rezervasyon sayfası ve tanıtım sitesinin ortak UI paketi.

## İçerik
| Yol | Ne |
|---|---|
| `src/panel/` | Panel tasarım sistemi bileşenleri: Button, IconButton, Input, Select, Checkbox, Switch, Tabs, Chip, StatusBadge, Badge, Banner, EmptyState, Modal, Drawer (yan panel), Table, Bento, Avatar, Sidebar, Topbar, Toast, Skeleton |
| `src/calendar/` | Takvim ızgarası (gün/hafta), blok, bekleme şeridi, şu anki saat çizgisi, sürükle-bırak (`@dnd-kit`) |
| `src/booking/` | Mobil tasarım sisteminin web karşılıkları (rezervasyon, `@omvila/ui-web/booking`): SalonHeader, BookingSteps, ServiceItem, CategoryChips, StaffPicker, SwitchCard, DayStrip, TimeSlotGrid, FullDayNotice, StickyFooter, AppointmentTicket, CopyField, BookingButton, BookingAvatar; `branchOpenStatus`. Yalnız **mobil** ölçeğin tokenlarını kullanır (test denetler); panel bileşenlerini içe aktarmaz |
| `src/format/` | `formatMoney`, `formatCount` ("1.240"), `formatPrice`, `formatDate`, `formatTime`, `formatDuration`, `formatPhone`, `maskPhone`, `formatIban`, `withTimeSuffix` ("20:00'ye") |
| `src/time/` | Şube saat dilimiyle dönüşüm (`toBranchTime`, `branchToday`, `localDateTimeToUtc`) |
| `src/errors/tr.ts` | `ErrorCode` → Türkçe kullanıcı mesajı (sözleşmedeki her kod için bir girdi; test eksik kodu yakalar) |
| `src/status/` | `displayStatus` → etiket ("Onay bekliyor") + token öneki (`durum-onay`) |

## Kurallar
- Referans: `design/tasarim-sistemleri/panel/components/<Bileşen>/README.md` ve `preview.html` (React 18 önizlemeleri). Ölçü, durum ve davranış oradaki gibi. Kod kopyalanmaz, TypeScript ile yeniden yazılır.
- Stil yalnız tokenlarla (`var(--…)`); sabit renk/aralık yok (stylelint).
- Her bileşen: açık ve koyu tema, klavye erişimi, odak halkası, `pointer: coarse` dokunma alanı, `prefers-reduced-motion`.
- Her bileşenin Vitest testi ve hikâye/önizleme sayfası (`/preview` rotası, Ladle) vardır.
- Biçimlendirme fonksiyonlarının birim testleri `docs/urun/urun-kurallari.md § Biçimler` tablosundaki örnekleri birebir doğrular ("1.250 ₺", "85,50 ₺", "0532 123 45 67", "21 Kasım Cuma").
- `errors/tr.ts` sözleşmedeki `ErrorCode` enum'unun her değerini içermelidir (tip düzeyinde `Record<ErrorCode, string>`).
- Uygulamaya özgü bileşen buraya taşınmaz; yalnız iki veya daha fazla uygulamanın kullandığı ya da tasarım sisteminde tanımlı olanlar.

## Komutlar
```bash
pnpm --filter @omvila/ui-web dev     # bileşen önizleme
pnpm --filter @omvila/ui-web test
pnpm --filter @omvila/ui-web lint
```
