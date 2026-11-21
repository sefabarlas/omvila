import { createFileRoute } from '@tanstack/react-router';
import { KvkkSettingsPage } from '@/features/settings/KvkkSettingsPage';
import { RequirePermission } from '@/shared/RequirePermission';

// KVKK ve veri: silme talepleri yalnız customer.kvkk (sahip) ile; bölüm menüde de yalnız o izinle görünür.
export const Route = createFileRoute('/_panel/ayarlar/kvkk')({
  component: () => (
    <RequirePermission permission="customer.kvkk">
      <KvkkSettingsPage />
    </RequirePermission>
  ),
});
