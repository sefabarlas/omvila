import type { components } from '@omvila/api-client-ts';
import { customerIds } from './fake-customers';

// Sahte değerlendirme uçları (manageListReviews, manageReplyReview). Şekiller sözleşmeyle aynı; sıralama sözleşmedeki
// filtre kuralıyla (AwaitingReply: düşük puan önce, sonra eskiden yeniye; diğerleri yeniden eskiye). Kurgusal kişiler;
// bugün 25 Kasım 2026.
type ReviewView = components['schemas']['ReviewView'];
type ReviewOverview = components['schemas']['ReviewOverview'];

export interface ReviewsFakeOptions {
  /** Hiç değerlendirme yok (DegerlendirmelerBos). */
  empty?: boolean;
  /** Yanıt kaydında sunucu hatası. */
  replyError?: boolean;
}

const branch = '0193a8f0-0000-7000-8000-0000000000b1';

export const reviewIds = {
  ece: '0193a8f0-0000-7000-8000-00000000e001',
  buse: '0193a8f0-0000-7000-8000-00000000e002',
  ayse: '0193a8f0-0000-7000-8000-00000000e003',
} as const;

export const reviewsFixture: ReviewView[] = [
  {
    id: reviewIds.ece,
    appointmentId: '0193a8f0-0000-7000-8000-00000000a201',
    branchId: branch,
    rating: 3,
    comment: 'İşlem güzeldi ama 20 dakika bekledim.',
    customer: { brandCustomerId: customerIds.gizem, fullName: 'Gizem Arslan' },
    serviceNames: ['Kaş ve kirpik'],
    staffId: '0193a8f0-0000-7000-8000-0000000000c2',
    staffName: 'Mert Demir',
    createdUtc: '2026-11-22T13:20:00Z',
    isPublished: true,
    reply: null,
  },
  {
    id: reviewIds.buse,
    appointmentId: '0193a8f0-0000-7000-8000-00000000a202',
    branchId: branch,
    rating: 5,
    comment: null,
    customer: { brandCustomerId: customerIds.selin, fullName: 'Selin Öztürk' },
    serviceNames: ['Keratin'],
    staffId: '0193a8f0-0000-7000-8000-0000000000c1',
    staffName: 'Elif Kaya',
    createdUtc: '2026-11-24T15:00:00Z',
    isPublished: true,
    reply: null,
  },
  {
    id: reviewIds.ayse,
    appointmentId: '0193a8f0-0000-7000-8000-00000000a203',
    branchId: branch,
    rating: 5,
    comment: 'Elif Hanım’ın eline sağlık, tam istediğim gibi oldu.',
    customer: { brandCustomerId: customerIds.selin, fullName: 'Selin Öztürk' },
    serviceNames: ['Saç kesimi'],
    staffId: '0193a8f0-0000-7000-8000-0000000000c1',
    staffName: 'Elif Kaya',
    createdUtc: '2026-11-18T10:00:00Z',
    isPublished: true,
    reply: {
      text: 'Teşekkür ederiz Selin Hanım, tekrar bekleriz.',
      repliedUtc: '2026-11-18T12:00:00Z',
      repliedByName: 'Elif Kaya',
    },
  },
];

function overviewOf(store: ReviewView[]): ReviewOverview {
  if (store.length === 0) {
    return {
      averageRating: null,
      ratingCount: 0,
      totalCount: 0,
      awaitingReplyCount: 0,
      thisMonthCount: 0,
      thisMonthInviteResponsePercent: null,
      topStaff: null,
    };
  }

  return {
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
  };
}

/** Sözleşmedeki filtre ve sıralama. */
export function filterReviews(store: readonly ReviewView[], filter: string | null): ReviewView[] {
  const newestFirst = (a: ReviewView, b: ReviewView) => b.createdUtc.localeCompare(a.createdUtc);
  switch (filter) {
    case 'All':
      return [...store].sort(newestFirst);
    case 'LowRating':
      return store.filter((r) => r.rating <= 3).sort(newestFirst);
    default:
      return store
        .filter((r) => r.reply === null)
        .sort((a, b) => a.rating - b.rating || a.createdUtc.localeCompare(b.createdUtc));
  }
}

export function createReviewsFake(options: ReviewsFakeOptions) {
  const store: ReviewView[] = options.empty ? [] : reviewsFixture.map((r) => ({ ...r }));
  const json = (status: number, body: unknown) =>
    new Response(JSON.stringify(body), {
      status,
      headers: { 'Content-Type': 'application/json' },
    });

  return {
    store,
    handle: async (request: Request): Promise<Response | null> => {
      const url = new URL(request.url);
      if (url.pathname === '/api/v1/manage/reviews') {
        const found = filterReviews(store, url.searchParams.get('filter'));
        // Sayfa boyu 2: "Daha fazla göster" denensin.
        const start = Number(url.searchParams.get('cursor') ?? '0');
        const next = start + 2;
        return json(200, {
          items: found.slice(start, next),
          nextCursor: next < found.length ? String(next) : null,
          overview: overviewOf(store),
        });
      }

      const ofCustomer = /^\/api\/v1\/manage\/customers\/([^/]+)\/reviews$/.exec(url.pathname);
      if (ofCustomer) {
        return json(200, {
          items: filterReviews(store, 'All').filter(
            (r) => r.customer.brandCustomerId === ofCustomer[1],
          ),
          nextCursor: null,
        });
      }

      const reply = /^\/api\/v1\/manage\/reviews\/([^/]+)\/reply$/.exec(url.pathname);
      if (reply && request.method === 'POST') {
        const review = store.find((r) => r.id === reply[1]);
        if (!review) {
          return json(404, { code: 'not_found' });
        }

        if (options.replyError) {
          return json(500, { code: 'internal_error' });
        }

        const body = (await request.clone().json()) as { text: string };
        review.reply = {
          text: body.text,
          repliedUtc: '2026-11-25T09:00:00Z',
          repliedByName: 'Elif Kaya',
        };
        return json(200, review);
      }

      return null;
    },
  };
}
