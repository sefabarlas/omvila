import { createFileRoute } from '@tanstack/react-router';
import { SubscriptionsPage, subscriptionFilters } from '@/features/subscriptions/SubscriptionsPage';
import type { SubscriptionFilter } from '@/features/subscriptions/SubscriptionsPage';
import { RequirePermission } from '@/shared/RequirePermission';

interface SubscriptionsSearch {
  filtre?: SubscriptionFilter;
  q?: string;
}

function Subscriptions() {
  const { filtre = 'All', q = '' } = Route.useSearch();
  return (
    <RequirePermission permission="platform.brands.view">
      <SubscriptionsPage filter={filtre} q={q} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/abonelikler')({
  validateSearch: (search: Record<string, unknown>): SubscriptionsSearch => ({
    ...(subscriptionFilters.some((f) => f.value === search.filtre)
      ? { filtre: search.filtre as SubscriptionFilter }
      : {}),
    ...(typeof search.q === 'string' && search.q.trim() !== '' ? { q: search.q.trim() } : {}),
  }),
  component: Subscriptions,
});
