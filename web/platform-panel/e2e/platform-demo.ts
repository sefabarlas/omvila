import type { Page } from '@playwright/test';
import { api, respond } from './support';

// Tasarımdaki veri (platform-paneli/Main, PMarkalar, PMarkaDetay): Prism örnekleri tek satırlık; ekran karşılaştırması
// tasarımla aynı satırlarla yapılır.
export const demoBrandId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5001';

const attention = (
  brandName: string,
  industryCode: string,
  city: string,
  planName: string,
  patch: Record<string, unknown>,
) => ({
  brandId: `0193a1b2-7c4d-7e8f-9a0b-${String(brandName.length).padStart(12, '0')}`,
  brandName,
  industryCode,
  city,
  planName,
  pastDueDays: null,
  trialDaysLeft: null,
  branchCount: 1,
  ...patch,
});

export async function overviewDemo(page: Page) {
  await respond(page, '/overview', {
    generatedUtc: '2026-11-20T07:00:00Z',
    activeBrands: { count: 412, newThisMonth: 18 },
    pendingApprovals: { total: 7, paymentNotices: 5, smsSenders: 2 },
    trialing: { count: 64, endingThisWeek: 9 },
    pastDue: { count: 11, readOnlySoon: 3 },
    funnel: {
      windowDays: 30,
      signedUp: 120,
      addedServices: 86,
      setWorkingHours: 71,
      firstOnlineAppointment: 44,
    },
    attentionBrands: [
      attention('Berber Kaan', 'barber', 'Ankara', 'Salon', {
        reason: 'PastDue',
        pastDueDays: 6,
        appointmentsThisMonth: 0,
      }),
      attention('Nail Studio Moda', 'nail', 'İstanbul', 'Salon', {
        reason: 'CreditsDepleted',
        appointmentsThisMonth: 38,
      }),
      attention('Dyt. Ece Arslan', 'dietitian', 'İstanbul', 'Tekil', {
        reason: 'TrialEnding',
        trialDaysLeft: 2,
        appointmentsThisMonth: 12,
      }),
    ],
  });
}

const brand = (
  n: number,
  name: string,
  industryCode: string,
  city: string,
  planName: string,
  subscriptionStatus: string,
  patch: Record<string, unknown> = {},
) => ({
  id: n === 1 ? demoBrandId : `0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f500${String(n)}`,
  name,
  industryCode,
  city,
  brandStatus: 'Active',
  planCode: planName.toLowerCase().replace(' ', '_'),
  planName,
  subscriptionStatus,
  trialDaysLeft: null,
  pastDueDays: null,
  branchCount: 1,
  appointmentsThisMonth: 0,
  creditBalance: 500,
  depositEnabled: false,
  ...patch,
});

export async function brandsDemo(page: Page) {
  await respond(page, '/brands*', {
    counts: { active: 412, trialing: 64 },
    items: [
      brand(1, 'Kuaför Elif', 'hair_salon', 'İstanbul', 'Salon', 'Trialing', {
        trialDaysLeft: 9,
        branchCount: 2,
        appointmentsThisMonth: 412,
      }),
      brand(2, 'Berber Kaan', 'barber', 'Ankara', 'Salon', 'PastDue', { pastDueDays: 6 }),
      brand(3, 'Nail Studio Moda', 'nail', 'İstanbul', 'Salon', 'Active', {
        appointmentsThisMonth: 38,
      }),
      brand(4, 'Spa Moda', 'spa', 'İstanbul', 'Salon Plus', 'Active', {
        branchCount: 3,
        appointmentsThisMonth: 291,
      }),
      brand(5, 'Dyt. Ece Arslan', 'dietitian', 'İstanbul', 'Tekil', 'Trialing', {
        trialDaysLeft: 2,
        appointmentsThisMonth: 12,
      }),
      brand(6, 'Saç Atölyesi', 'hair_salon', 'İzmir', 'Salon', 'ReadOnly'),
    ],
    nextCursor: null,
  });
}

/** Prism örneği tasarımla aynı; yalnız dördüncü kontör satırı ("Onay ve hatırlatma") eklenir. */
export async function brandDetailDemo(page: Page) {
  await page.route(`${api}/brands/${demoBrandId}`, async (route) => {
    if (route.request().method() !== 'GET') {
      await route.fallback();
      return;
    }

    const response = await route.fetch();
    const detail = (await response.json()) as { recentCreditTransactions: unknown[] };
    const [first, second, third] = detail.recentCreditTransactions;
    await route.fulfill({
      response,
      json: {
        ...detail,
        recentCreditTransactions: [
          first,
          second,
          {
            id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa305',
            occurredUtc: '2026-11-17T06:00:00Z',
            type: 'Usage',
            credits: -52,
            balanceAfter: 286,
            note: null,
          },
          third,
        ],
      },
    });
  });
}
