import { createFileRoute } from '@tanstack/react-router';
import { BookingPageSettingsPage } from '@/features/settings/BookingPageSettingsPage';
import { bookingPageQuery } from '@/shared/booking-link';

export const Route = createFileRoute('/_panel/ayarlar/rezervasyon')({
  loader: ({ context }) => context.queryClient.query(bookingPageQuery(context.api)),
  component: BookingPageSettingsPage,
});
