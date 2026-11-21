import type { components } from '@omvila/api-client-ts';
import { localDateTimeToUtc } from '@omvila/ui-web/time';
import type { ManageSession } from '@/app/api';

// Sahte kasa ve paket uçları (manageGetCashDay, manageCloseCashDay, manageListCustomerPackages). Veriler
// salon-paneli/Kasa'dan, kurgusal kişiler; gün Çarşamba 25 Kasım 2026 (İstanbul).
type CashDay = components['schemas']['CashDay'];
type PaymentView = components['schemas']['PaymentView'];
type CustomerPackage = components['schemas']['CustomerPackage'];
type CloseRequest = components['schemas']['CashDayCloseRequest'];

const tz = 'Europe/Istanbul';
export const cashDate = '2026-11-25';
const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' as const });

export interface SalesFakeOptions {
  /** Günde tahsilat yok (KasaBos). */
  cashEmpty?: boolean;
  /** Gün kapatılmış (sayım özeti görünür). */
  cashClosed?: boolean;
  /** Kapatma yanıtı: gün başka oturumda kapatılmış (409). */
  closeError?: 'cash_day_already_closed';
  /** Müşterinin kullanılabilir paketi var ("Paketten" görünür): müşteri kimliği. */
  packageCustomerId?: string;
}

// [saat, müşteri, kalem türü, yöntem, personel, tutar (kuruş)]
type Row = [string, string, Partial<PaymentView>, PaymentView['method'], string | null, number];
const rows: Row[] = [
  [
    '12:05',
    'Selin Öztürk',
    { serviceNames: ['Boya', 'Fön'], isBalanceAfterDeposit: true },
    'Card',
    'Elif Kaya',
    124000,
  ],
  ['11:40', 'Okan Er', { serviceNames: ['Saç kesimi'] }, 'Cash', 'Mert Demir', 45000],
  [
    '11:35',
    'Melis Tunç',
    { serviceNames: ['Manikür ve pedikür'] },
    'PackageRedemption',
    'Zeynep Aksoy',
    0,
  ],
  [
    '11:10',
    'Gizem Arslan',
    { isDeposit: true, appointmentStartUtc: localDateTimeToUtc(cashDate, '14:30', tz) },
    'Transfer',
    null,
    15000,
  ],
  ['10:20', 'Ayşe Yılmaz', { serviceNames: ['Saç kesimi'] }, 'Cash', 'Elif Kaya', 45000],
  ['10:05', 'Can Demir', { serviceNames: ['Sakal ve kesim'] }, 'Card', 'Mert Demir', 55000],
  [
    '09:50',
    'Hande Er',
    { purpose: 'CustomerPackage', packageName: 'Fön paketi', packageSessionCount: 5 },
    'Transfer',
    'Can Yücel',
    50000,
  ],
];

function cashPayments(branchId: string): PaymentView[] {
  return rows.map(([time, name, extra, method, staffName, amount], i) => ({
    id: `0193a8f0-0000-7000-8000-0000000c0${String(i).padStart(3, '0')}`,
    branchId,
    method,
    amount: money(amount),
    isDeposit: false,
    status: 'Recorded',
    paidUtc: localDateTimeToUtc(cashDate, time, tz),
    purpose: 'Appointment',
    customer: {
      brandCustomerId: `0193a8f0-0000-7000-8000-0000000d0${String(i).padStart(3, '0')}`,
      fullName: name,
    },
    staffName,
    recordedAfterDayClose: false,
    ...extra,
  }));
}

const total = (list: PaymentView[], method?: PaymentView['method']) => {
  const picked = list.filter((p) => method === undefined || p.method === method);
  return {
    amount: money(picked.reduce((sum, p) => sum + p.amount.amountMinor, 0)),
    count: picked.length,
  };
};

export function cashDayFixture(branchId: string, date: string, empty: boolean): CashDay {
  const payments = empty ? [] : cashPayments(branchId);
  const transfers = payments.filter((p) => p.method === 'Transfer');
  const deposit = transfers
    .filter((p) => p.isDeposit)
    .reduce((s, p) => s + p.amount.amountMinor, 0);
  const transferTotal = total(payments, 'Transfer');
  const cash = total(payments, 'Cash');
  return {
    branchId,
    date,
    timeZone: tz,
    status: 'Open',
    total: { amount: total(payments).amount, count: payments.length },
    cash,
    card: total(payments, 'Card'),
    transfer: {
      ...transferTotal,
      depositAmount: money(deposit),
      otherAmount: money(transferTotal.amount.amountMinor - deposit),
    },
    cashRefunded: money(0),
    expectedCash: cash.amount,
    payments,
    closing: null,
  };
}

function packageFixture(brandCustomerId: string, branchId: string): CustomerPackage {
  return {
    id: '0193a8f0-0000-7000-8000-0000000f7a03',
    customer: { brandCustomerId, fullName: 'Selin Öztürk' },
    packageId: '0193a8f0-0000-7000-8000-0000000f7a01',
    packageName: 'Kesim paketi',
    branchId,
    isAvailableAllBranches: true,
    price: money(170000),
    purchasedUtc: '2026-10-01T09:00:00Z',
    expiresOn: '2026-12-31',
    status: 'Active',
    items: [
      {
        customerPackageItemId: '0193a8f0-0000-7000-8000-0000000f7a04',
        serviceId: '0193a8f0-0000-7000-8000-0000000000f1',
        serviceName: 'Saç kesimi',
        totalSessions: 5,
        remainingSessions: 3,
      },
    ],
    totalSessions: 5,
    remainingSessions: 3,
    version: '2',
  };
}

const json = (status: number, body: unknown) =>
  new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } });

/** Kasa ve paket uçlarını karşılar; ilgisiz istekte null döner (fakeFetch devam eder). */
export function createSalesFake(session: ManageSession, options: SalesFakeOptions = {}) {
  const closed = new Map<string, CashDay>();
  const can = (permission: string) => session.permissions.includes(permission);
  return async (request: Request): Promise<Response | null> => {
    const url = new URL(request.url);
    const path = url.pathname;
    const packages = /^\/api\/v1\/manage\/customers\/([^/]+)\/packages$/.exec(path);
    if (packages && request.method === 'GET') {
      const id = packages[1] ?? '';
      const branchId = session.branches[0]?.id ?? '';
      return json(200, {
        items: id === options.packageCustomerId ? [packageFixture(id, branchId)] : [],
        nextCursor: null,
      });
    }

    if (path === '/api/v1/manage/cash' && request.method === 'GET') {
      if (!can('payment.collect')) {
        return json(403, { code: 'forbidden' });
      }

      const branchId = url.searchParams.get('branchId') ?? '';
      const date = url.searchParams.get('date') ?? cashDate;
      const day = closed.get(`${branchId}|${date}`);
      if (day) {
        return json(200, day);
      }

      const fresh = cashDayFixture(branchId, date, options.cashEmpty ?? date !== cashDate);
      return json(
        200,
        options.cashClosed
          ? {
              ...fresh,
              status: 'Closed',
              closing: {
                expectedCash: fresh.expectedCash,
                countedCash: money(fresh.expectedCash.amountMinor - 5000),
                differenceMinor: -5000,
                difference: 'Shortage',
                differenceNote: 'Bozuk para verildi',
                closedByName: 'Elif Kaya',
                closedUtc: localDateTimeToUtc(date, '19:10', tz),
              },
            }
          : fresh,
      );
    }

    if (path === '/api/v1/manage/cash/close' && request.method === 'POST') {
      if (!can('cash.close')) {
        return json(403, { code: 'forbidden' });
      }

      if (!request.headers.get('Idempotency-Key')) {
        return json(400, { code: 'validation_failed' });
      }

      const body = (await request.clone().json()) as CloseRequest;
      if (options.closeError || closed.has(`${body.branchId}|${body.date}`)) {
        return json(409, { code: 'cash_day_already_closed' });
      }

      const day = cashDayFixture(
        body.branchId,
        body.date,
        options.cashEmpty ?? body.date !== cashDate,
      );
      const differenceMinor = body.countedCash.amountMinor - day.expectedCash.amountMinor;
      if (differenceMinor !== 0 && !body.differenceNote) {
        return json(400, { code: 'validation_failed', errors: { differenceNote: ['required'] } });
      }

      const saved: CashDay = {
        ...day,
        status: 'Closed',
        closing: {
          expectedCash: day.expectedCash,
          countedCash: body.countedCash,
          differenceMinor,
          difference: differenceMinor === 0 ? 'None' : differenceMinor < 0 ? 'Shortage' : 'Surplus',
          differenceNote: body.differenceNote ?? null,
          closedByName: session.user.fullName,
          closedUtc: localDateTimeToUtc(body.date, '19:10', tz),
        },
      };
      closed.set(`${body.branchId}|${body.date}`, saved);
      return json(200, saved);
    }

    return null;
  };
}
