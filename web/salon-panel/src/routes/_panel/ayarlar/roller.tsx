import { createFileRoute } from '@tanstack/react-router';
import { RolesPage } from '@/features/settings/RolesPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Roller ve yetkiler yalnız settings.roles izniyle (roller-yetkiler.md: yalnız Owner); doğrudan adreste 403.
export const Route = createFileRoute('/_panel/ayarlar/roller')({
  component: () => (
    <RequirePermission permission="settings.roles">
      <RolesPage />
    </RequirePermission>
  ),
});
