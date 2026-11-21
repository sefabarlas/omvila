import { createFileRoute } from '@tanstack/react-router';
import { notificationSettingsQuery, smsOverviewQuery } from '@/features/settings/notification-api';
import { NotificationSettingsPage } from '@/features/settings/NotificationSettingsPage';

export const Route = createFileRoute('/_panel/ayarlar/bildirimler')({
  loader: ({ context }) =>
    Promise.all([
      context.queryClient.query(notificationSettingsQuery(context.api)),
      context.queryClient.query(smsOverviewQuery(context.api)),
    ]),
  component: NotificationSettingsPage,
});
