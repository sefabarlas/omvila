import { createFileRoute } from '@tanstack/react-router';
import { WaitlistPage } from '@/features/waitlist/WaitlistPage';
import { RequirePermission } from '@/shared/RequirePermission';

// Bekleme listesi (waitlist.manage: sahip ve şube müdürü; personel 403).
function WaitlistRoute() {
  return (
    <RequirePermission permission="waitlist.manage">
      <WaitlistPage />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/bekleme')({ component: WaitlistRoute });
