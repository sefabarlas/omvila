import { createFileRoute } from '@tanstack/react-router';
import { StaffPage } from '@/features/staff/StaffPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Personel yalnız staff.manage izniyle (roller-yetkiler.md); veri seçili şubeye göre sayfada yüklenir.
export const Route = createFileRoute('/_panel/personel')({
  component: () => (
    <RequirePermission permission="staff.manage">
      <StaffPage />
    </RequirePermission>
  ),
});
