import { createFileRoute } from '@tanstack/react-router';
import { CustomerCardPage } from '@/features/customers/CustomerCardPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Müşteri kartı (customer.view); kart marka geneldir (roller-yetkiler.md).
function CustomerCardRoute() {
  const { id } = Route.useParams();
  return (
    <RequirePermission permission="customer.view">
      <CustomerCardPage key={id} brandCustomerId={id} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/musteriler/$id')({
  component: CustomerCardRoute,
});
