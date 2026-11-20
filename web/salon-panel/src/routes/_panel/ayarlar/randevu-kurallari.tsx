import { createFileRoute } from '@tanstack/react-router';
import { brandSettingsQuery } from '@/features/settings/api';
import { BookingRulesPage } from '@/features/settings/BookingRulesPage';

export const Route = createFileRoute('/_panel/ayarlar/randevu-kurallari')({
  loader: ({ context }) => context.queryClient.query(brandSettingsQuery(context.api)),
  component: BookingRulesPage,
});
