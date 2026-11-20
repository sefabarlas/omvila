import { createFileRoute, Outlet } from '@tanstack/react-router';
import { RequirePermission } from '@/shared/RequirePermission';

// Ayarlar yalnız settings.manage izniyle (roller-yetkiler.md); menüde de yalnız o izinle görünür.
export const Route = createFileRoute('/_panel/ayarlar')({
  component: () => (
    <RequirePermission permission="settings.manage">
      <Outlet />
    </RequirePermission>
  ),
});
