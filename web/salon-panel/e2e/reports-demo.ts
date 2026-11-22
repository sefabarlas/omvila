import type { Page } from '@playwright/test';
import { ownerMembership, ownerPermissions } from './calendar-demo';
import { sessionAs } from './support';

// Raporlar ve prim (salon-paneli/Raporlar, RaporlarBos) ve Primim ekran görüntüleri. Prism örneği 18–24 Kasım'ı döner;
// tasarımın haftası Pazartesi–Pazar olduğu için yanıtı test verir. Gün Pazar 29 Kasım 2026 akşamı (hafta bitmiş).
// Kurgusal kişiler.
const corsJson = (body: unknown) => ({
  status: 200,
  contentType: 'application/json',
  headers: {
    'Access-Control-Allow-Origin': 'http://localhost:5180',
    'Access-Control-Allow-Credentials': 'true',
  },
  body: JSON.stringify(body),
});
const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' });

/** Haftanın son günü, akşam (İstanbul). */
export const reportsNow = '2026-11-29T20:00:00+03:00';

// [gün, tahsilat (kuruş), kapalı]
const week: [string, number, boolean][] = [
  ['2026-11-23', 0, true],
  ['2026-11-24', 1120000, false],
  ['2026-11-25', 950000, false],
  ['2026-11-26', 1300000, false],
  ['2026-11-27', 1580000, false],
  ['2026-11-28', 1845000, false],
  ['2026-11-29', 0, true],
];

function report(url: URL, empty: boolean) {
  const from = url.searchParams.get('from') ?? '2026-11-23';
  const to = url.searchParams.get('to') ?? '2026-11-29';
  const days = week
    .filter(([day]) => day >= from && day <= to)
    .map(([date, amount, isClosed]) => ({ date, amountMinor: empty ? 0 : amount, isClosed }));
  return {
    branchId: url.searchParams.get('branchId'),
    timeZone: 'Europe/Istanbul',
    from,
    to,
    earnedRevenue: money(empty ? 0 : 6120000),
    completedAppointmentCount: empty ? 0 : 118,
    earnedRevenueChangePercent: empty ? null : 9,
    collections: money(empty ? 0 : 6795000),
    noShow: empty
      ? { count: 0, percent: null, lostRevenue: money(0) }
      : { count: 5, percent: 4, lostRevenue: money(215000) },
    online: empty ? { appointmentCount: 0, percent: null } : { appointmentCount: 54, percent: 46 },
    dailyCollections: days,
    currency: 'TRY',
  };
}

const commissionRows = [
  ['0193a8f0-0000-7000-8000-0000000000c1', 'Elif Kaya', 38, 2430000, 24, 583200],
  ['0193a8f0-0000-7000-8000-0000000000c2', 'Mert Demir', 31, 1595000, 22, 350900],
  ['0193a8f0-0000-7000-8000-0000000000c3', 'Can Yücel', 27, 1620000, 22, 356400],
  ['0193a8f0-0000-7000-8000-0000000000c4', 'Zeynep Aksoy', 22, 1150000, 24, 276000],
] as const;

function commissions(url: URL) {
  const items = commissionRows.map(([staffId, staffName, count, revenue, pct, total]) => ({
    staffId,
    staffName,
    appointmentCount: count,
    serviceRevenueMinor: revenue,
    serviceCommissionPercent: pct,
    serviceCommissionMinor: total,
    saleCommissionMinor: 0,
    totalCommissionMinor: total,
  }));
  return {
    branchId: url.searchParams.get('branchId'),
    from: url.searchParams.get('from'),
    to: url.searchParams.get('to'),
    currency: 'TRY',
    items,
    totalCommissionMinor: items.reduce((sum, row) => sum + row.totalCommissionMinor, 0),
  };
}

/** Tam yetkili sahip; rapor ve prim yanıtları tasarım verisiyle (empty: RaporlarBos). */
export async function reportsDemo(page: Page, empty = false) {
  await page.clock.setFixedTime(reportsNow);
  await sessionAs(page, (session) => ({
    permissions: ownerPermissions,
    membership: ownerMembership,
    brand: { ...(session.brand as Record<string, unknown>), status: 'Active' },
  }));
  await page.route(/\/api\/v1\/manage\/reports\/overview\?/, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(corsJson(report(new URL(route.request().url()), empty))),
  );
  await page.route(/\/api\/v1\/manage\/reports\/commissions\?/, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(corsJson(commissions(new URL(route.request().url())))),
  );
}

/**
 * Primim: personel oturumu. Prim özeti Prism örneğinden (1–25 Kasım, Ayşe Yılmaz ve Buse Kaya satırları); gün örnekteki
 * "bugün" (25 Kasım) olsun diye saat sabitlenir.
 */
export async function myCommissionDemo(page: Page) {
  await page.clock.setFixedTime('2026-11-25T12:10:00+03:00');
  await sessionAs(page, (session) => ({
    permissions: [
      'calendar.view',
      'customer.view',
      'walkin.manage',
      'commission.view',
      'appointment.create',
    ],
    membership: { role: 'Staff', branchIds: [], staffId: ownerMembership.staffId },
    brand: { ...(session.brand as Record<string, unknown>), status: 'Active' },
  }));
}
