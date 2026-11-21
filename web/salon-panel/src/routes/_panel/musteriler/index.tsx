import { createFileRoute } from '@tanstack/react-router';
import { CustomersPage } from '@/features/customers/CustomersPage';
import type { CustomersSearch } from '@/features/customers/CustomersPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Müşteriler (customer.view; personelde de açık, telefon maskeli): arama ve filtre adreste (?q=4418&filtre=duzenli).
function CustomersRoute() {
  const search = Route.useSearch();
  return (
    <RequirePermission permission="customer.view">
      <CustomersPage search={search} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/musteriler/')({
  validateSearch: (search: Record<string, unknown>): CustomersSearch => ({
    ...(typeof search.q === 'string' && search.q.trim() !== '' ? { q: search.q.slice(0, 80) } : {}),
    // Sayı gibi görünen arama (?q=4418) adresten sayı olarak gelir.
    ...(typeof search.q === 'number' ? { q: String(search.q) } : {}),
    ...(typeof search.filtre === 'string' ? { filtre: search.filtre } : {}),
  }),
  component: CustomersRoute,
});
