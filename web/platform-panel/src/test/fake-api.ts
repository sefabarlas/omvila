import type { components } from '@omvila/api-client-ts';
import type { AdminPermission, AdminSession } from '@/app/api';
import { platformRoute } from './fake-platform';
import type { PlatformFakeOptions } from './fake-platform';

// Test için sahte admin API: üretilmiş istemcinin fetch'i yerine geçer (gerçek ağ yok). Veriler sözleşme örnekleriyle
// ve platform-paneli tasarımlarıyla aynı.
type Schemas = components['schemas'];

export const supportPermissions: AdminPermission[] = [
  'platform.brands.view',
  'platform.payments.match',
  'platform.sms-senders.decide',
  'platform.plans.view',
  'platform.audit.view',
];
export const adminPermissions: AdminPermission[] = [
  ...supportPermissions,
  'platform.brands.manage',
  'platform.credits.grant',
  'platform.plans.manage',
  'platform.billing.manage',
  'platform.audit.export',
];

export const financePermissions: AdminPermission[] = [
  'platform.brands.view',
  'platform.payments.match',
  'platform.plans.view',
  'platform.plans.manage',
  'platform.billing.manage',
  'platform.audit.view',
];

const rolePermissions: Record<AdminSession['role'], AdminPermission[]> = {
  PlatformAdmin: adminPermissions,
  PlatformSupport: supportPermissions,
  PlatformFinance: financePermissions,
};

export function sessionFixture(role: AdminSession['role'] = 'PlatformSupport'): AdminSession {
  return {
    user: {
      id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa101',
      fullName: 'Ayşe Demir',
      phoneMasked: '0532 *** ** 20',
    },
    role,
    permissions: rolePermissions[role],
    pendingApprovalCount: 7,
  };
}

export const overviewFixture: Schemas['AdminOverview'] = {
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
    {
      brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa201',
      brandName: 'Berber Kaan',
      industryCode: 'barber',
      city: 'Ankara',
      planName: 'Salon',
      reason: 'PastDue',
      pastDueDays: 6,
      trialDaysLeft: null,
      branchCount: 1,
      appointmentsThisMonth: 0,
    },
    {
      brandId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa202',
      brandName: 'Nail Studio Moda',
      industryCode: 'nail',
      city: 'İstanbul',
      planName: 'Salon',
      reason: 'CreditsDepleted',
      pastDueDays: null,
      trialDaysLeft: null,
      branchCount: 1,
      appointmentsThisMonth: 38,
    },
  ],
};

export const brandId = '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5001';

const brandItem = (
  id: string,
  name: string,
  patch: Partial<Schemas['AdminBrandListItem']> = {},
): Schemas['AdminBrandListItem'] => ({
  id,
  name,
  industryCode: 'hair_salon',
  city: 'İstanbul',
  brandStatus: 'Active',
  planCode: 'salon',
  planName: 'Salon',
  subscriptionStatus: 'Active',
  trialDaysLeft: null,
  pastDueDays: null,
  branchCount: 1,
  appointmentsThisMonth: 38,
  creditBalance: 500,
  depositEnabled: false,
  ...patch,
});

export const brandsFixture: Schemas['AdminBrandListItem'][] = [
  brandItem(brandId, 'Kuaför Elif', {
    subscriptionStatus: 'Trialing',
    trialDaysLeft: 9,
    branchCount: 2,
    appointmentsThisMonth: 412,
    depositEnabled: true,
  }),
  brandItem('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa201', 'Berber Kaan', {
    industryCode: 'barber',
    city: 'Ankara',
    subscriptionStatus: 'PastDue',
    pastDueDays: 6,
    appointmentsThisMonth: 0,
  }),
  brandItem('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa204', 'Saç Atölyesi', {
    city: 'İzmir',
    subscriptionStatus: 'ReadOnly',
    appointmentsThisMonth: 0,
  }),
];

export const creditTransactions: Schemas['AdminCreditTransaction'][] = [
  {
    id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa301',
    occurredUtc: '2026-11-18T12:00:00Z',
    type: 'Purchase',
    credits: 1000,
    balanceAfter: 1240,
    note: null,
  },
  {
    id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa302',
    occurredUtc: '2026-11-18T06:00:00Z',
    type: 'Usage',
    credits: -46,
    balanceAfter: 240,
    note: null,
  },
  {
    id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa303',
    occurredUtc: '2026-11-11T09:00:00Z',
    type: 'TrialGift',
    credits: 500,
    balanceAfter: 500,
    note: null,
  },
];

export const brandDetailFixture: Schemas['AdminBrandDetail'] = {
  id: brandId,
  name: 'Kuaför Elif',
  slug: 'kuafor-elif',
  industryCode: 'hair_salon',
  city: 'İstanbul',
  brandStatus: 'Active',
  createdUtc: '2026-11-11T09:00:00Z',
  bookingUrl: 'https://omvila.com/kuafor-elif',
  owner: { fullName: 'Elif Kaya', phoneMasked: '0532 *** ** 05' },
  branches: [
    { id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a01', name: 'Kadıköy' },
    { id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a02', name: 'Moda' },
  ],
  activeStaffCount: 6,
  customerCount: 1284,
  googleCalendarConnectedStaffCount: 3,
  smsSender: { name: 'KUAFORELIF', status: 'None', effectiveName: 'OMVILA' },
  subscription: {
    status: 'Trialing',
    planCode: 'salon',
    planName: 'Salon',
    billingCycle: 'Monthly',
    paymentMethod: 'BankTransfer',
    trialEndsOn: '2026-11-29',
    trialDaysLeft: 9,
    pastDueSince: null,
    readOnlyOn: null,
    transferCode: 'ABN-KE-1125',
    pendingNoticeId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b01',
    pendingNoticeNotifiedUtc: '2026-11-18T11:20:00Z',
  },
  credits: { balance: 1240, estimatedDaysLeft: 26 },
  appointmentsThisMonth: { count: 412, onlinePercent: 61 },
  recentCreditTransactions: creditTransactions,
  version: 'W/"5120"',
};

export interface FakeApiOptions {
  /** Sekmede yenileme jetonu var mı (sayfa yenilendiğinde oturum sürer mi). */
  signedIn?: boolean;
  session?: AdminSession;
  verify?: 'ok' | 'otp_invalid';
  calls?: Record<string, unknown[]>;
  overview?: Schemas['AdminOverview'];
  /** Uç → hata durumu (ör. '/api/v1/admin/overview' → 403). */
  failures?: Record<string, number>;
  brands?: Schemas['AdminBrandListItem'][];
  /** Marka listesi iki sayfa mı (imleç). */
  paged?: boolean;
  /** Plan değişikliği yanıtı. */
  planError?: 'plan_limit_exceeded' | 'concurrency_conflict';
  /** Onay kuyruğu, bildirim, banka hareketleri, abonelik, denetim ve plan uçları (fake-platform.ts). */
  platform?: PlatformFakeOptions;
}

function json(status: number, body: unknown): Response {
  return new Response(JSON.stringify(body), {
    status,
    headers: { 'Content-Type': 'application/json' },
  });
}

const problem = (status: number, code: string, meta: Record<string, unknown> = {}) =>
  json(status, { type: 'about:blank', title: code, status, code, meta });

export function fakeFetch({
  session = sessionFixture(),
  verify = 'ok',
  calls = {},
  overview = overviewFixture,
  failures = {},
  brands = brandsFixture,
  paged = false,
  planError,
  platform = {},
}: FakeApiOptions = {}) {
  return async (request: Request): Promise<Response> => {
    const url = new URL(request.url);
    const path = url.pathname;
    const key = `${request.method} ${path}`;
    if (request.method !== 'GET') {
      const ifMatch = request.headers.get('If-Match');
      if (ifMatch !== null) {
        (calls[`${key} If-Match`] ??= []).push(ifMatch);
      }

      (calls[path] ??= []).push(
        await request
          .clone()
          .json()
          .catch(() => null),
      );
    } else {
      (calls[path] ??= []).push(Object.fromEntries(url.searchParams));
    }

    const failure = failures[path];
    if (failure !== undefined) {
      return problem(failure, failure === 403 ? 'forbidden' : 'try_again');
    }

    switch (key) {
      case 'POST /api/v1/admin/auth/otp/request':
        return json(202, {
          otpId: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa001',
          expiresInSeconds: 180,
          resendAfterSeconds: 60,
          phoneMasked: '0532 *** ** 20',
        });
      case 'POST /api/v1/admin/auth/otp/verify':
        return verify === 'ok'
          ? json(200, {
              accessToken: 'erisim-1',
              expiresInSeconds: 900,
              refreshToken: 'yenileme-1',
            })
          : problem(422, 'otp_invalid', { attemptsLeft: 4 });
      case 'POST /api/v1/admin/auth/refresh': {
        const body = (await request.clone().json()) as { refreshToken?: string };
        return body.refreshToken
          ? json(200, {
              accessToken: 'erisim-2',
              expiresInSeconds: 900,
              refreshToken: 'yenileme-2',
            })
          : problem(401, 'unauthorized');
      }
      case 'GET /api/v1/admin/session':
        return json(200, session);
      case 'GET /api/v1/admin/overview':
        return json(200, overview);
      case 'GET /api/v1/admin/brands': {
        const q = url.searchParams.get('q')?.toLocaleLowerCase('tr');
        const filter = url.searchParams.get('filter') ?? 'All';
        const items = brands.filter(
          (b) =>
            (!q || b.name.toLocaleLowerCase('tr').includes(q)) &&
            (filter === 'All' ||
              (filter === 'DepositEnabled' ? b.depositEnabled : b.subscriptionStatus === filter)),
        );
        const second = url.searchParams.get('cursor') === 'sayfa-2';
        const page = paged ? (second ? items.slice(2) : items.slice(0, 2)) : items;
        return json(200, {
          counts: { active: 412, trialing: 64 },
          items: page,
          nextCursor: paged && !second ? 'sayfa-2' : null,
        });
      }
      case `GET /api/v1/admin/brands/${brandId}`:
        return json(200, brandDetailFixture);
      case `GET /api/v1/admin/brands/${brandId}/credit-transactions`:
        return json(200, { items: creditTransactions, nextCursor: null });
      case `GET /api/v1/admin/brands/${brandId}/invoices`:
      case `GET /api/v1/admin/brands/${brandId}/payment-notices`:
        return json(200, { items: [], nextCursor: null });
      case `POST /api/v1/admin/brands/${brandId}/credits`:
        return json(201, {
          transaction: {
            id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa304',
            occurredUtc: '2026-11-20T21:31:00Z',
            type: 'Grant',
            credits: 500,
            balanceAfter: 1740,
            note: 'Destek talebi #4821',
          },
          balance: 1740,
        });
      case `POST /api/v1/admin/brands/${brandId}/plan`:
        if (planError === 'plan_limit_exceeded') {
          return problem(409, 'plan_limit_exceeded', { staffCount: 6, staffLimit: 1 });
        }

        if (planError === 'concurrency_conflict') {
          return problem(412, 'concurrency_conflict');
        }

        return json(200, {
          ...brandDetailFixture,
          subscription: {
            ...brandDetailFixture.subscription,
            planCode: 'salon_plus',
            planName: 'Salon Plus',
          },
          version: 'W/"5121"',
        });
      default: {
        const handled = await platformRoute(key, url, request, platform);
        return handled ?? problem(404, 'not_found');
      }
    }
  };
}
