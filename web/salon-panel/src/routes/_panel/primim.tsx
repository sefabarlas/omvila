import { createFileRoute } from '@tanstack/react-router';
import { MyCommissionPage } from '@/features/reports/MyCommissionPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Primim (commission.view; personel menüsü): yalnız oturumdaki kullanıcının kendi primi.
export const Route = createFileRoute('/_panel/primim')({
  component: () => (
    <RequirePermission permission="commission.view">
      <MyCommissionPage />
    </RequirePermission>
  ),
});
