import { createFileRoute } from '@tanstack/react-router';
import { CashPage } from '@/features/cash/CashPage';
import type { CashSearch } from '@/features/cash/CashPage';
import { RequirePermission } from '@/shared/RequirePermission';

const isDate = (value: unknown): value is string =>
  typeof value === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(value);

// Kasa (payment.collect; roller-yetkiler.md § Menü): "Önceki günler" adreste (?tarih=2026-11-24); "Günü kapat"
// ayrıca cash.close ister.
function CashRoute() {
  const search = Route.useSearch();
  return (
    <RequirePermission permission="payment.collect">
      <CashPage search={search} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/kasa')({
  validateSearch: (search: Record<string, unknown>): CashSearch =>
    isDate(search.tarih) ? { tarih: search.tarih } : {},
  component: CashRoute,
});
