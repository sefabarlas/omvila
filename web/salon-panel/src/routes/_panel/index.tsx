import { createFileRoute } from '@tanstack/react-router';
import { isPersonalView } from '@/app/menu';
import { useSession } from '@/app/session';
import { onboardingQuery } from '@/features/overview/api';
import { OverviewPage } from '@/features/overview/OverviewPage';
import { StaffOverview } from '@/features/overview/StaffOverview';

// Ana sayfa: raporları gören kullanıcıya genel bakış (kurulum veya ilk hafta), personele kendi özeti
// (roller-yetkiler.md § Menü görünürlüğü: "report.view yoksa personel özetine döner").
function HomePage() {
  const { can } = useSession();
  return isPersonalView(can) ? <StaffOverview /> : <OverviewPage />;
}

export const Route = createFileRoute('/_panel/')({
  loader: ({ context }) =>
    context.queryClient.query(onboardingQuery(context.api)).catch(() => undefined),
  component: HomePage,
});
