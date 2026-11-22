import type { Page } from '@playwright/test';
import { demoNow, ownerPermissions } from './calendar-demo';
import { sessionAs } from './support';

// Değerlendirme ekran görüntüleri için salon-paneli/Degerlendirmeler verisi (Prism örneği tek kayıt döner). Filtre
// sözleşmedeki gibi uygulanır; yanıt POST'u kartı günceller. Kişiler kurgusal.
const corsJson = (body: unknown, status = 200) => ({
  status,
  contentType: 'application/json',
  headers: {
    'Access-Control-Allow-Origin': 'http://localhost:5180',
    'Access-Control-Allow-Credentials': 'true',
  },
  body: JSON.stringify(body),
});

const branchId = '0193a8f0-0000-7000-8000-0000000000b1';
const review = (
  n: number,
  fullName: string,
  rating: number,
  comment: string | null,
  service: string,
  staffName: string,
  createdUtc: string,
  reply: string | null,
) => ({
  id: `0193a8f0-0000-7000-8000-0000000e${String(n).padStart(4, '0')}`,
  appointmentId: `0193a8f0-0000-7000-8000-0000000b${String(n).padStart(4, '0')}`,
  branchId,
  rating,
  comment,
  customer: {
    brandCustomerId: `0193a8f0-0000-7000-8000-0000000c${String(n).padStart(4, '0')}`,
    fullName,
  },
  serviceNames: [service],
  staffId: null,
  staffName,
  createdUtc,
  isPublished: true,
  reply: reply
    ? { text: reply, repliedUtc: '2026-11-18T12:00:00Z', repliedByName: 'Elif Kaya' }
    : null,
});

/** Değerlendirmeler; `empty`: hiç değerlendirme yok (DegerlendirmelerBos). */
export async function reviewsDemo(page: Page, { empty = false } = {}) {
  await page.clock.setFixedTime(demoNow);
  await sessionAs(page, {
    permissions: ownerPermissions,
    membership: { role: 'Owner', branchIds: [], staffId: '0193a8f0-0000-7000-8000-0000000000c1' },
  });
  const store = empty
    ? []
    : [
        review(
          1,
          'Ece Koç',
          3,
          'İşlem güzeldi ama 20 dakika bekledim.',
          'Kaş ve kirpik',
          'Zeynep Aksoy',
          '2026-11-22T13:20:00Z',
          null,
        ),
        review(2, 'Buse Kaya', 5, null, 'Keratin', 'Elif Kaya', '2026-11-24T15:00:00Z', null),
        review(
          3,
          'Ayşe Yılmaz',
          5,
          'Elif Hanım’ın eline sağlık, tam istediğim gibi oldu.',
          'Saç kesimi',
          'Elif Kaya',
          '2026-11-18T10:00:00Z',
          'Teşekkür ederiz Ayşe Hanım, tekrar bekleriz.',
        ),
      ];
  await page.route(/\/api\/v1\/manage\/reviews(\?.*)?$/, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    const filter = new URL(route.request().url()).searchParams.get('filter') ?? 'AwaitingReply';
    const items =
      filter === 'All'
        ? [...store].sort((a, b) => b.createdUtc.localeCompare(a.createdUtc))
        : filter === 'LowRating'
          ? store.filter((r) => r.rating <= 3)
          : store
              .filter((r) => r.reply === null)
              .sort((a, b) => a.rating - b.rating || a.createdUtc.localeCompare(b.createdUtc));
    await route.fulfill(
      corsJson({
        items,
        nextCursor: null,
        overview: empty
          ? {
              averageRating: null,
              ratingCount: 0,
              totalCount: 0,
              awaitingReplyCount: 0,
              thisMonthCount: 0,
              thisMonthInviteResponsePercent: null,
              topStaff: null,
            }
          : {
              averageRating: 4.8,
              ratingCount: 214,
              totalCount: 214,
              awaitingReplyCount: store.filter((r) => r.reply === null).length,
              thisMonthCount: 31,
              thisMonthInviteResponsePercent: 38,
              topStaff: {
                staffId: '0193a8f0-0000-7000-8000-0000000000c1',
                fullName: 'Elif Kaya',
                averageRating: 4.9,
                count: 96,
              },
            },
      }),
    );
  });
  await page.route(/\/api\/v1\/manage\/reviews\/[^/]+\/reply$/, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    const id = new URL(route.request().url()).pathname.split('/')[5];
    const target = store.find((r) => r.id === id);
    if (!target) {
      await route.fulfill(corsJson({ code: 'not_found', status: 404 }, 404));
      return;
    }

    const body = route.request().postDataJSON() as { text: string };
    target.reply = {
      text: body.text,
      repliedUtc: new Date(demoNow).toISOString(),
      repliedByName: 'Elif Kaya',
    };
    await route.fulfill(corsJson(target));
  });
}
