import type { components } from '@omvila/api-client-ts';

// Sahte bildirim uçları (manageListInAppNotifications, manageMarkNotificationsRead). Şekiller ve metinler sözleşme
// örneğiyle aynı (salon-paneli/BildirimPaneli); bugün 25 Kasım 2026, saat 12:00 İstanbul.
type InAppNotification = components['schemas']['InAppNotification'];

export interface NotificationsFakeOptions {
  /** Hiç bildirim yok. */
  empty?: boolean;
  /** Liste ucu 500 döner. */
  error?: boolean;
}

export const notificationIds = {
  approval: '0193a8f0-0000-7000-8000-00000000b001',
  cancelled: '0193a8f0-0000-7000-8000-00000000b002',
  review: '0193a8f0-0000-7000-8000-00000000b003',
  credits: '0193a8f0-0000-7000-8000-00000000b004',
} as const;

const branchId = '0193a8f0-0000-7000-8000-0000000000b1';

export function notificationsFixture(): InAppNotification[] {
  return [
    {
      id: notificationIds.approval,
      event: 'staff.approval_needed',
      title: 'Yeni online talep · Pınar Yalçın',
      body: 'Cilt bakımı, bugün 16:00',
      createdUtc: '2026-11-25T09:02:00Z',
      readUtc: null,
      branchId,
      target: {
        type: 'Appointment',
        id: '0193a8f0-0000-7000-8000-00000000a061',
        holdExpiresUtc: '2026-11-25T10:35:00Z',
      },
    },
    {
      id: notificationIds.cancelled,
      event: 'staff.appointment_changed',
      title: 'Müşteri iptal etti · Hande Er',
      body: 'Saç kesimi 15:00 · Mert · sebep: planım değişti',
      createdUtc: '2026-11-25T07:15:00Z',
      readUtc: null,
      branchId,
      target: { type: 'Calendar', id: '0193a8f0-0000-7000-8000-00000000a070' },
    },
    {
      id: notificationIds.review,
      event: 'staff.review_received',
      title: 'Yeni değerlendirme · 3 yıldız',
      body: 'Ece Koç · Zeynep',
      createdUtc: '2026-11-25T06:40:00Z',
      readUtc: '2026-11-25T06:50:00Z',
      branchId,
      target: { type: 'Reviews', id: null },
    },
    {
      id: notificationIds.credits,
      event: 'owner.credits_low',
      title: 'SMS kontörü azalıyor',
      body: '1.240 kontör · ~18 gün',
      createdUtc: '2026-11-24T08:00:00Z',
      readUtc: null,
      branchId: null,
      target: { type: 'SmsCredits', id: null },
    },
  ];
}

export function createNotificationsFake(options: NotificationsFakeOptions) {
  const items = options.empty ? [] : notificationsFixture();
  const unread = () => items.filter((item) => item.readUtc === null).length;
  const json = (status: number, body: unknown) =>
    new Response(JSON.stringify(body), {
      status,
      headers: { 'Content-Type': 'application/json' },
    });
  return async (request: Request): Promise<Response | null> => {
    const path = new URL(request.url).pathname;
    if (path === '/api/v1/manage/notifications' && request.method === 'GET') {
      return options.error
        ? json(500, { code: 'internal_error' })
        : json(200, { items, nextCursor: null, unreadCount: unread() });
    }

    if (path === '/api/v1/manage/notifications/read' && request.method === 'POST') {
      const body = (await request.clone().json()) as { notificationIds?: string[]; all?: boolean };
      for (const item of items) {
        if (item.readUtc === null && (body.all || body.notificationIds?.includes(item.id))) {
          item.readUtc = '2026-11-25T09:00:00Z';
        }
      }

      return json(200, { unreadCount: unread() });
    }

    return null;
  };
}
