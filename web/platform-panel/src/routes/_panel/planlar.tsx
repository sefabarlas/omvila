import { createFileRoute } from '@tanstack/react-router';
import { PlansPage } from '@/features/plans/PlansPage';
import { RequirePermission } from '@/shared/RequirePermission';

function Plans() {
  return (
    <RequirePermission permission="platform.plans.view">
      <PlansPage />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/planlar')({ component: Plans });
