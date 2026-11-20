import { createFileRoute } from '@tanstack/react-router';
import { brandQuery } from '@/features/settings/api';
import { BusinessSettingsPage } from '@/features/settings/BusinessSettingsPage';

export const Route = createFileRoute('/_panel/ayarlar/isletme')({
  loader: ({ context }) => context.queryClient.query(brandQuery(context.api)),
  component: BusinessSettingsPage,
});
