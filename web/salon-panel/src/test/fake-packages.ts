import type { components } from '@omvila/api-client-ts';
import type { ManageSession } from '@/app/api';

// Sahte paket uçları (manageListPackages, manageCreatePackage, manageUpdatePackage, manageListAllCustomerPackages,
// manageSellPackage, manageRefundCustomerPackage). Veriler salon-paneli/Paketler'den, kurgusal kişiler. Satılan
// paketler müşterinin kartında (manageListCustomerPackages) görünür; satış yoksa istek fake-sales'e düşer.
type PackageDefinition = components['schemas']['PackageDefinition'];
type PackageWrite = components['schemas']['PackageWrite'];
type CustomerPackage = components['schemas']['CustomerPackage'];
type SaleRequest = components['schemas']['PackageSaleRequest'];
type RefundRequest = components['schemas']['PackageRefundRequest'];

export interface PackagesFakeOptions {
  /** Hiç paket tanımı yok (PaketlerBos). */
  empty?: boolean;
  /** Satış yanıtı: satan personel şubede çalışmıyor (400 errors.soldByStaffId). */
  sellError?: 'soldByStaffId';
}

const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' as const });
const id = (n: number) => `0193a8f0-0000-7000-8000-0000000e${String(n).padStart(4, '0')}`;
const saçKesimi = '0193a8f0-0000-7000-8000-0000000000f1';
const keratin = '0193a8f0-0000-7000-8000-0000000000f2';

// [ad, hizmet, hizmet adı, seans, fiyat ₺, gün, satılan, aktif, satışta]
type Row = [string, string, string, number, number, number, number, number, boolean];
const rows: Row[] = [
  ['Fön paketi', id(901), 'Fön', 5, 1000, 90, 64, 18, true],
  ['Fön paketi · 10', id(901), 'Fön', 10, 1800, 180, 22, 9, true],
  ['Cilt bakımı', id(902), 'Cilt bakımı', 4, 2900, 120, 15, 6, true],
  ['Lazer epilasyon', id(903), 'Lazer epilasyon', 8, 6400, 365, 11, 8, true],
  ['Saç kesimi', saçKesimi, 'Saç kesimi', 6, 1700, 90, 14, 5, true],
  ['Keratin bakım', keratin, 'Keratin', 3, 4800, 180, 3, 0, false],
];

function definitions(): PackageDefinition[] {
  return rows.map(
    ([name, serviceId, serviceName, sessions, lira, days, sold, active, isActive], i) => ({
      id: id(i + 1),
      name,
      price: money(lira * 100),
      items: [{ serviceId, serviceName, sessionCount: sessions }],
      totalSessions: sessions,
      validityDays: days,
      saleCommissionPercent: 10,
      isAvailableAllBranches: true,
      isActive,
      soldCount: sold,
      activeCount: active,
      version: '1',
    }),
  );
}

// [müşteri, paket, kalan, toplam, son gün]
const expiring: [string, string, number, number, string][] = [
  ['Selin Öztürk', 'Fön paketi', 2, 5, '2026-12-31'],
  ['Melis Tunç', 'Manikür', 3, 6, '2026-12-08'],
  ['Hande Er', 'Cilt bakımı', 1, 4, '2026-12-12'],
  ['Ayşe Yılmaz', 'Fön paketi · 10', 4, 10, '2026-12-20'],
  ['Buse Kaya', 'Fön paketi', 1, 5, '2026-12-23'],
];

function customerPackages(branchId: string): CustomerPackage[] {
  return expiring.map(([fullName, packageName, remaining, total, expiresOn], i) => ({
    id: id(100 + i),
    customer: { brandCustomerId: id(200 + i), fullName },
    packageId: id(1),
    packageName,
    branchId,
    isAvailableAllBranches: true,
    price: money(100000),
    purchasedUtc: '2026-09-01T09:00:00Z',
    expiresOn,
    soldByStaffId: null,
    soldByStaffName: null,
    status: 'Active',
    items: [],
    totalSessions: total,
    remainingSessions: remaining,
    version: '1',
  }));
}

const json = (status: number, body: unknown) =>
  new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } });

const serviceNames: Record<string, string> = {
  [saçKesimi]: 'Saç kesimi',
  [keratin]: 'Keratin',
};

/** Paket uçlarını karşılar; ilgisiz istekte null döner. Durumlu: oluşturma, düzenleme, satış ve iade saklanır. */
export function createPackagesFake(session: ManageSession, options: PackagesFakeOptions = {}) {
  const branchId = session.branches[0]?.id ?? '';
  let packages = options.empty ? [] : definitions();
  let owned = options.empty ? [] : customerPackages(branchId);
  const sold: CustomerPackage[] = [];
  const can = (permission: string) => session.permissions.includes(permission);
  const toDefinition = (
    body: PackageWrite,
    base: Partial<PackageDefinition>,
  ): PackageDefinition => ({
    soldCount: 0,
    activeCount: 0,
    id: id(50 + packages.length),
    ...base,
    name: body.name,
    price: body.price,
    items: body.items.map((i) => ({
      ...i,
      serviceName: serviceNames[i.serviceId] ?? 'Hizmet',
    })),
    totalSessions: body.items.reduce((sum, i) => sum + i.sessionCount, 0),
    validityDays: body.validityDays,
    saleCommissionPercent: body.saleCommissionPercent ?? null,
    isAvailableAllBranches: body.isAvailableAllBranches,
    isActive: body.isActive,
    version: String(Number(base.version ?? '0') + 1),
  });

  return async (request: Request): Promise<Response | null> => {
    const url = new URL(request.url);
    const path = url.pathname;
    if (path === '/api/v1/manage/packages') {
      if (!can('package.manage') && !can('package.sell')) {
        return json(403, { code: 'forbidden' });
      }

      if (request.method === 'GET') {
        const active = [...owned, ...sold].filter((p) => p.status === 'Active');
        return json(200, {
          items: [...packages].sort((a, b) => Number(b.isActive) - Number(a.isActive)),
          overview: {
            activeCustomerPackages: options.empty ? 0 : 46 + sold.length,
            remainingSessions: options.empty ? 0 : 182,
            soldThisMonth: options.empty ? 0 : 12 + sold.length,
            soldThisMonthAmount: money(options.empty ? 0 : 1860000),
            expiringSoon: active.filter((p) => p.expiresOn <= '2026-12-25').length,
            usageRatePercent: options.empty ? null : 81,
          },
        });
      }

      if (!can('package.manage')) {
        return json(403, { code: 'forbidden' });
      }

      const created = toDefinition((await request.clone().json()) as PackageWrite, {});
      packages = [...packages, created];
      return json(201, created);
    }

    const single = /^\/api\/v1\/manage\/packages\/([^/]+)$/.exec(path);
    if (single && request.method === 'PUT') {
      const current = packages.find((p) => p.id === single[1]);
      if (!current) {
        return json(404, { code: 'not_found' });
      }

      if (request.headers.get('If-Match') !== current.version) {
        return json(412, { code: 'concurrency_conflict' });
      }

      const saved = toDefinition((await request.clone().json()) as PackageWrite, current);
      packages = packages.map((p) => (p.id === saved.id ? saved : p));
      return json(200, saved);
    }

    if (path === '/api/v1/manage/customer-packages' && request.method === 'GET') {
      const filter = url.searchParams.get('filter') ?? 'Active';
      const q = (url.searchParams.get('q') ?? '').toLocaleLowerCase('tr-TR');
      const limit = Number(url.searchParams.get('limit') ?? '50');
      const items = [...sold, ...owned]
        .filter((p) =>
          filter === 'All'
            ? true
            : filter === 'Expired'
              ? p.status === 'Expired'
              : p.status === 'Active' && (filter === 'Active' || p.expiresOn <= '2026-12-25'),
        )
        .filter((p) => p.packageName.toLocaleLowerCase('tr-TR').includes(q))
        .sort((a, b) => (filter === 'ExpiringSoon' ? a.expiresOn.localeCompare(b.expiresOn) : 0));
      return json(200, { items: items.slice(0, limit), nextCursor: null });
    }

    const refund = /^\/api\/v1\/manage\/customer-packages\/([^/]+)\/refund$/.exec(path);
    if (refund && request.method === 'POST') {
      const current = [...sold, ...owned].find((p) => p.id === refund[1]);
      if (!current) {
        return json(404, { code: 'not_found' });
      }

      const body = (await request.clone().json()) as RefundRequest;
      if (body.amount.amountMinor <= 0 || body.amount.amountMinor > current.price.amountMinor) {
        return json(400, { code: 'validation_failed', errors: { amount: ['out_of_range'] } });
      }

      const saved: CustomerPackage = {
        ...current,
        status: 'Refunded',
        refundedAmount: body.amount,
        refundedUtc: '2026-11-25T09:00:00Z',
        version: String(Number(current.version) + 1),
      };
      owned = owned.map((p) => (p.id === saved.id ? saved : p));
      const index = sold.findIndex((p) => p.id === saved.id);
      if (index >= 0) {
        sold[index] = saved;
      }

      return json(200, saved);
    }

    const customer = /^\/api\/v1\/manage\/customers\/([^/]+)\/packages$/.exec(path);
    if (customer && request.method === 'POST') {
      if (!can('package.sell')) {
        return json(403, { code: 'forbidden' });
      }

      if (!request.headers.get('Idempotency-Key')) {
        return json(400, { code: 'validation_failed' });
      }

      if (options.sellError) {
        return json(400, { code: 'validation_failed', errors: { soldByStaffId: ['invalid'] } });
      }

      const body = (await request.clone().json()) as SaleRequest;
      const pkg = packages.find((p) => p.id === body.packageId);
      if (!pkg?.isActive) {
        return json(409, { code: 'invalid_state_transition' });
      }

      const created: CustomerPackage = {
        id: id(300 + sold.length),
        customer: { brandCustomerId: customer[1] ?? '', fullName: 'Selin Öztürk' },
        packageId: pkg.id,
        packageName: pkg.name,
        branchId: body.branchId,
        isAvailableAllBranches: pkg.isAvailableAllBranches,
        price: pkg.price,
        purchasedUtc: '2026-11-25T09:00:00Z',
        expiresOn: '2027-02-23',
        soldByStaffId: body.soldByStaffId,
        soldByStaffName: 'Elif Kaya',
        status: 'Active',
        items: pkg.items.map((i, n) => ({
          customerPackageItemId: id(400 + n),
          serviceId: i.serviceId,
          serviceName: i.serviceName,
          totalSessions: i.sessionCount,
          remainingSessions: i.sessionCount,
        })),
        totalSessions: pkg.totalSessions,
        remainingSessions: pkg.totalSessions,
        usages: [],
        version: '1',
      };
      sold.unshift(created);
      return json(201, created);
    }

    if (customer && request.method === 'GET') {
      const mine = sold.filter((p) => p.customer.brandCustomerId === customer[1]);
      return mine.length > 0 ? json(200, { items: mine, nextCursor: null }) : null;
    }

    return null;
  };
}
