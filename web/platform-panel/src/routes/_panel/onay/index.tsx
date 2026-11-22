import { createFileRoute } from '@tanstack/react-router';
import { ApprovalsPage } from '@/features/approvals/ApprovalsPage';
import type { ApprovalTab } from '@/features/approvals/ApprovalsPage';
import { RequirePermission } from '@/shared/RequirePermission';

interface ApprovalsSearch {
  sekme?: ApprovalTab;
}

function Approvals() {
  const { sekme } = Route.useSearch();
  return (
    <RequirePermission permission="platform.payments.match">
      <ApprovalsPage tab={sekme} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/onay/')({
  validateSearch: (search: Record<string, unknown>): ApprovalsSearch =>
    search.sekme === 'sms' || search.sekme === 'havale' ? { sekme: search.sekme } : {},
  component: Approvals,
});
