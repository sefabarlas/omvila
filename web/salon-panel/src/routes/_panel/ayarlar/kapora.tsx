import { createFileRoute } from '@tanstack/react-router';
import { bankAccountQuery } from '@/features/settings/bank-account-api';
import { BankAccountPage } from '@/features/settings/BankAccountPage';
import { sessionQuery } from '@/app/session';
import { RequirePermission } from '@/shared/RequirePermission';

// Kapora ve IBAN yalnız settings.payment izniyle (roller-yetkiler.md: yalnız Owner); doğrudan adreste 403.
export const Route = createFileRoute('/_panel/ayarlar/kapora')({
  loader: async ({ context }) => {
    const session = await context.queryClient.query(sessionQuery(context.api));
    if (session.permissions.includes('settings.payment')) {
      await context.queryClient.query(bankAccountQuery(context.api));
    }
  },
  component: () => (
    <RequirePermission permission="settings.payment">
      <BankAccountPage />
    </RequirePermission>
  ),
});
