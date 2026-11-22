import type { Page, Route } from '@playwright/test';
import { demoNow, demoStaff } from './calendar-demo';
import { customersDemo, demoCustomerId } from './customers-demo';
import { sessionAs } from './support';

// Paketler (salon-paneli/Paketler, PaketlerBos, PaketOlustur, PaketSat) için tasarım verisi ve durumlu sahte uçlar:
// Prism durumsuzdur; oluşturulan paket listede, satılan paket müşteri kartında görünsün diye yanıtı test verir.
// Kurgusal kişiler; saat Çarşamba 25 Kasım 2026, 12:10 İstanbul.
const cors = {
  'Access-Control-Allow-Origin': 'http://localhost:5180',
  'Access-Control-Allow-Credentials': 'true',
};
const json = (body: unknown, status = 200) => ({
  status,
  contentType: 'application/json',
  headers: cors,
  body: JSON.stringify(body),
});
const money = (lira: number) => ({ amountMinor: lira * 100, currency: 'TRY' });
const id = (n: number) => `0193a8f0-0000-7000-8000-0000000e${String(n).padStart(4, '0')}`;
const branchId = '0193a8f0-0000-7000-8000-0000000000b1';

const serviceRows: [string, string, number, number][] = [
  [id(901), 'Fön', 30, 350],
  [id(902), 'Cilt bakımı', 60, 900],
  [id(903), 'Lazer epilasyon', 45, 1000],
  [id(904), 'Manikür', 45, 350],
  [id(905), 'Kalıcı oje', 30, 300],
  [id(906), 'Keratin', 120, 1800],
];
const demoServices = serviceRows.map(([serviceId, name, durationMinutes, lira]) => ({
  id: serviceId,
  categoryId: null,
  name,
  description: null,
  durationMinutes,
  bufferBeforeMinutes: 0,
  bufferAfterMinutes: 0,
  price: money(lira),
  priceDisplay: 'Fixed',
  isBranchPrice: false,
  deposit: { mode: 'Never' },
  requiresApproval: null,
  minNoticeMinutes: null,
  maxAdvanceDays: null,
  resourceTypeId: null,
  isOnlineBookable: true,
  isActive: true,
  staffIds: [],
  stages: [],
}));
const serviceName = new Map(serviceRows.map(([serviceId, name]) => [serviceId, name]));

// [ad, [hizmet, seans][], fiyat ₺, gün, satılan, aktif, satışta]
type Row = [string, [string, number][], number, number, number, number, boolean];
const rows: Row[] = [
  ['Fön paketi', [[id(901), 5]], 1000, 90, 64, 18, true],
  ['Fön paketi · 10', [[id(901), 10]], 1800, 180, 22, 9, true],
  ['Cilt bakımı', [[id(902), 4]], 2900, 120, 15, 6, true],
  ['Lazer epilasyon', [[id(903), 8]], 6400, 365, 11, 8, true],
  ['Manikür', [[id(904), 6]], 1700, 90, 14, 5, true],
  ['Keratin bakım', [[id(906), 3]], 4800, 180, 3, 0, false],
];

interface Write {
  name: string;
  price: { amountMinor: number; currency: string };
  items: { serviceId: string; sessionCount: number }[];
  validityDays: number;
  saleCommissionPercent: number | null;
  isAvailableAllBranches: boolean;
  isActive: boolean;
}

function definition(n: number, body: Write, soldCount = 0, activeCount = 0) {
  return {
    id: id(n),
    ...body,
    items: body.items.map((i) => ({ ...i, serviceName: serviceName.get(i.serviceId) ?? '' })),
    totalSessions: body.items.reduce((sum, i) => sum + i.sessionCount, 0),
    soldCount,
    activeCount,
    version: '1',
  };
}

// [müşteri, paket, kalan, toplam, son gün]
const expiringRows: [string, string, number, number, string][] = [
  ['Selin Öztürk', 'Fön paketi', 2, 5, '2026-12-31'],
  ['Melis Tunç', 'Manikür', 3, 6, '2026-12-08'],
  ['Hande Er', 'Cilt bakımı', 1, 4, '2026-12-12'],
  ['Ayşe Yılmaz', 'Fön paketi · 10', 4, 10, '2026-12-20'],
  ['Buse Kaya', 'Fön paketi', 1, 5, '2026-12-23'],
];
const owned = expiringRows
  .map(([fullName, packageName, remaining, total, expiresOn], i) => ({
    id: id(100 + i),
    customer: { brandCustomerId: id(200 + i), fullName },
    packageId: id(1),
    packageName,
    branchId,
    isAvailableAllBranches: true,
    price: money(1000),
    purchasedUtc: '2026-09-01T09:00:00Z',
    expiresOn,
    soldByStaffId: null,
    soldByStaffName: null,
    status: 'Active',
    items: [],
    totalSessions: total,
    remainingSessions: remaining,
    version: '1',
  }))
  .sort((a, b) => a.expiresOn.localeCompare(b.expiresOn));

const permissions = [
  'settings.manage',
  'service.manage',
  'staff.manage',
  'calendar.view',
  'calendar.view.all-staff',
  'customer.view',
  'customer.phone.view',
  'customer.edit',
  'report.view',
  'payment.collect',
  'review.view',
  'package.sell',
  'package.manage',
];

const handle = (route: Route, fn: () => Promise<void>) =>
  route.request().method() === 'OPTIONS' ? route.fallback() : fn();

/** Paketler ekranı ve akışı; `empty`: hiç paket tanımı yok (PaketlerBos). Müşteri kartı uçları customersDemo'dan. */
export async function packagesDemo(page: Page, { empty = false }: { empty?: boolean } = {}) {
  await customersDemo(page);
  await page.clock.setFixedTime(demoNow);
  await sessionAs(page, {
    permissions,
    membership: { role: 'Owner', branchIds: [], staffId: demoStaff[0]?.staffId },
    branches: [{ id: branchId, name: 'Kadıköy' }],
  });
  let packages = empty
    ? []
    : rows.map(([name, items, lira, validityDays, sold, active, isActive], i) =>
        definition(
          i + 1,
          {
            name,
            price: money(lira),
            items: items.map(([serviceId, sessionCount]) => ({ serviceId, sessionCount })),
            validityDays,
            saleCommissionPercent: 10,
            isAvailableAllBranches: true,
            isActive,
          },
          sold,
          active,
        ),
      );
  const sold: Record<string, unknown>[] = [];

  await page.route(/\/api\/v1\/manage\/services(\?.*)?$/, (route) =>
    handle(route, () => route.fulfill(json({ items: demoServices }))),
  );
  await page.route(/\/api\/v1\/manage\/staff(\?.*)?$/, (route) =>
    handle(route, () =>
      route.fulfill(
        json({
          items: demoStaff.map((s) => ({
            id: s.staffId,
            fullName: s.fullName,
            displayName: null,
            title: null,
            photoUrl: null,
            calendarColor: s.calendarColor,
            isBookableOnline: true,
            isActive: true,
            branchIds: [branchId],
            role: null,
            membershipStatus: 'Active',
          })),
        }),
      ),
    ),
  );
  await page.route(/\/api\/v1\/manage\/packages(\?.*)?$/, (route) =>
    handle(route, async () => {
      if (route.request().method() === 'POST') {
        const created = definition(50 + packages.length, route.request().postDataJSON() as Write);
        packages = [...packages, created];
        await route.fulfill(json(created, 201));
        return;
      }

      await route.fulfill(
        json({
          items: [...packages].sort((a, b) => Number(b.isActive) - Number(a.isActive)),
          overview: {
            activeCustomerPackages: empty ? 0 : 46 + sold.length,
            remainingSessions: empty ? 0 : 182,
            soldThisMonth: empty ? 0 : 12 + sold.length,
            soldThisMonthAmount: money(empty ? 0 : 18600),
            expiringSoon: empty ? 0 : 5,
            usageRatePercent: empty ? null : 81,
          },
        }),
      );
    }),
  );
  await page.route(/\/api\/v1\/manage\/customer-packages(\?.*)?$/, (route) =>
    handle(route, () => route.fulfill(json({ items: empty ? [] : owned, nextCursor: null }))),
  );
  const customerPackages = /\/api\/v1\/manage\/customers\/([^/?]+)\/packages(\?.*)?$/;
  await page.route(customerPackages, (route) =>
    handle(route, async () => {
      const request = route.request();
      const brandCustomerId = customerPackages.exec(request.url())?.[1] ?? '';
      if (request.method() === 'POST') {
        const body = request.postDataJSON() as { packageId: string; soldByStaffId: string };
        const pkg = packages.find((p) => p.id === body.packageId);
        const created = {
          id: id(300 + sold.length),
          customer: { brandCustomerId, fullName: 'Selin Öztürk' },
          packageId: body.packageId,
          packageName: pkg?.name ?? '',
          branchId,
          isAvailableAllBranches: true,
          price: pkg?.price ?? money(0),
          purchasedUtc: new Date(demoNow).toISOString(),
          expiresOn: '2027-05-25',
          soldByStaffId: body.soldByStaffId,
          soldByStaffName: 'Elif Kaya',
          status: 'Active',
          items: (pkg?.items ?? []).map((i, n) => ({
            customerPackageItemId: id(400 + n),
            serviceId: i.serviceId,
            serviceName: i.serviceName,
            totalSessions: i.sessionCount,
            remainingSessions: i.sessionCount,
          })),
          totalSessions: pkg?.totalSessions ?? 0,
          remainingSessions: pkg?.totalSessions ?? 0,
          usages: [],
          version: '1',
        };
        sold.unshift(created);
        await route.fulfill(json(created, 201));
        return;
      }

      const mine = sold.filter(
        (p) => (p.customer as { brandCustomerId: string }).brandCustomerId === brandCustomerId,
      );
      if (mine.length === 0) {
        await route.fallback();
        return;
      }

      await route.fulfill(json({ items: mine, nextCursor: null }));
    }),
  );
}

export { demoCustomerId };
