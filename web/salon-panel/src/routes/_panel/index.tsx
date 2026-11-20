import { createFileRoute } from '@tanstack/react-router';
import { onboardingQuery } from '@/features/overview/api';
import { OverviewPage } from '@/features/overview/OverviewPage';

export const Route = createFileRoute('/_panel/')({
  loader: ({ context }) =>
    context.queryClient.query(onboardingQuery(context.api)).catch(() => undefined),
  component: OverviewPage,
});
