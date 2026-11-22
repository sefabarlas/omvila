import { createFileRoute } from '@tanstack/react-router';
import { SubscriptionPage } from '@/features/subscription/SubscriptionPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Abonelik ve fatura: yalnız subscription.manage (işletme sahibi); bölüm menüde de yalnız o izinle görünür.
export const Route = createFileRoute('/_panel/ayarlar/abonelik')({
  component: () => (
    <RequirePermission permission="subscription.manage">
      <SubscriptionPage />
    </RequirePermission>
  ),
});
