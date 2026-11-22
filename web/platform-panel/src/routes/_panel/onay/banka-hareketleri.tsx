import { createFileRoute } from '@tanstack/react-router';
import { BankTransactionsPage } from '@/features/bank/BankTransactionsPage';
import { RequirePermission } from '@/shared/RequirePermission';

interface BankSearch {
  q?: string;
  eslesmemis?: boolean;
}

function BankTransactions() {
  const { q = '', eslesmemis = false } = Route.useSearch();
  return (
    <RequirePermission permission="platform.payments.match">
      <BankTransactionsPage q={q} unmatchedOnly={eslesmemis} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/onay/banka-hareketleri')({
  validateSearch: (search: Record<string, unknown>): BankSearch => ({
    ...(typeof search.q === 'string' && search.q.trim() !== '' ? { q: search.q.trim() } : {}),
    ...(search.eslesmemis === true || search.eslesmemis === 'true' ? { eslesmemis: true } : {}),
  }),
  component: BankTransactions,
});
