import { createFileRoute } from '@tanstack/react-router';
import { CalendarPage } from '@/features/calendar/CalendarPage';
import type { CalendarSearch } from '@/features/calendar/CalendarPage';
import { RequirePermission } from '@/shared/RequirePermission';

const isDate = (value: unknown): value is string =>
  typeof value === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(value);

// Takvim (calendar.view): tarih, görünüm ve haftanın personeli adreste (?tarih=2026-11-25&gorunum=hafta&personel=…);
// ?randevu=… o randevunun yan panelini açar (bildirim paneli "Havale bildirildi").
function CalendarRoute() {
  const search = Route.useSearch();
  return (
    <RequirePermission permission="calendar.view">
      <CalendarPage search={search} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/takvim')({
  validateSearch: (search: Record<string, unknown>): CalendarSearch => ({
    ...(isDate(search.tarih) ? { tarih: search.tarih } : {}),
    ...(search.gorunum === 'hafta' ? { gorunum: 'hafta' as const } : {}),
    ...(typeof search.personel === 'string' ? { personel: search.personel } : {}),
    ...(typeof search.randevu === 'string' ? { randevu: search.randevu } : {}),
  }),
  component: CalendarRoute,
});
