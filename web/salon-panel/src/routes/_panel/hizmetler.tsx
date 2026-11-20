import { createFileRoute } from '@tanstack/react-router';
import { ServicesPage } from '@/features/services/ServicesPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Hizmetler yalnız service.manage izniyle (roller-yetkiler.md); veri seçili şubeye göre sayfada yüklenir.
export const Route = createFileRoute('/_panel/hizmetler')({
  component: () => (
    <RequirePermission permission="service.manage">
      <ServicesPage />
    </RequirePermission>
  ),
});
