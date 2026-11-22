import { createFileRoute } from '@tanstack/react-router';
import { PaymentNoticePage } from '@/features/approvals/PaymentNoticePage';
import { RequirePermission } from '@/shared/RequirePermission';

function PaymentNotice() {
  const { noticeId } = Route.useParams();
  return (
    <RequirePermission permission="platform.payments.match">
      <PaymentNoticePage noticeId={noticeId} />
    </RequirePermission>
  );
}

export const Route = createFileRoute('/_panel/onay/havale/$noticeId')({
  component: PaymentNotice,
});
