import type { ManageSession } from '@/app/api';

// Test için sahte API: üretilmiş istemcinin fetch'i yerine geçer (gerçek ağ yok). Veriler sözleşme örnekleriyle aynı.
export const ownerPermissions = [
  'settings.manage',
  'service.manage',
  'staff.manage',
  'calendar.view',
  'customer.view',
  'timeoff.request',
];
export const staffPermissions = [
  'calendar.view',
  'customer.view',
  'customer.edit',
  'timeoff.request',
  'walkin.manage',
  'commission.view',
];

export function sessionFixture(overrides: Partial<ManageSession> = {}): ManageSession {
  return {
    user: {
      id: '0193a8f0-0000-7000-8000-000000000001',
      fullName: 'Elif Kaya',
      phoneMasked: '0532 *** ** 05',
    },
    brand: {
      id: '0193a8f0-0000-7000-8000-00000000000a',
      name: 'Kuaför Elif',
      slug: 'kuafor-elif',
      status: 'Onboarding',
    },
    membership: { role: 'Owner', branchIds: [], staffId: '0193a8f0-0000-7000-8000-0000000000c1' },
    permissions: ownerPermissions,
    branches: [{ id: '0193a8f0-0000-7000-8000-0000000000b1', name: 'Merkez' }],
    subscription: { status: 'Trialing', trialEndsOn: '2026-12-09' },
    ...overrides,
  };
}

export interface FakeApiOptions {
  /** Yenileme çerezi var mı (sayfa yenilendiğinde oturum sürer mi). */
  signedIn?: boolean;
  session?: ManageSession;
  /** Doğrulama sonucu: kayıtlı numara (SignedIn), kayıtsız (SignupRequired) veya hata kodu. */
  verify?: 'SignedIn' | 'SignupRequired' | 'otp_invalid' | 'otp_attempts_exceeded';
  /** İstek gövdeleri (yol → gövde listesi). */
  calls?: Record<string, unknown[]>;
  /** Tamamlanmış kurulum adımları (BusinessCreated kayıtla tamam). */
  doneSteps?: string[];
  /** Kaydetme yanıtı: 412 kayıt çakışması veya sunucu hata kodu (slug_unavailable, validation_failed:phone). */
  saveError?: 'concurrency_conflict' | 'slug_unavailable' | 'validation_failed:phone';
  /** Aktif IBAN var mı (kapora ayarları). */
  depositAvailable?: boolean;
  /** Şube eklerken plan sınırı (403 + meta.planLimit). */
  branchLimitReached?: boolean;
  /** Hizmetler dolu mu (yoksa HizmetlerBos). */
  withServices?: boolean;
}

export const branchId = '0193a8f0-0000-7000-8000-0000000000b1';

export const branchFixture = {
  id: branchId,
  name: 'Merkez',
  slug: 'merkez',
  addressLine: 'Moda Cad. 42',
  district: 'Kadıköy',
  city: 'İstanbul',
  postalCode: null,
  latitude: null,
  longitude: null,
  phone: null,
  email: null,
  timeZone: 'Europe/Istanbul',
  slotIntervalMinutes: null,
  isActive: true,
  version: '5',
};

export const secondBranchId = '0193a8f0-0000-7000-8000-0000000000b2';
const categoryHair = '0193a8f0-0000-7000-8000-0000000000e1';
const categoryColor = '0193a8f0-0000-7000-8000-0000000000e2';
const staffElif = '0193a8f0-0000-7000-8000-0000000000c1';
const staffMert = '0193a8f0-0000-7000-8000-0000000000c2';
const staffCan = '0193a8f0-0000-7000-8000-0000000000c3';

export const categoriesFixture = [
  { id: categoryHair, name: 'Saç', sortOrder: 1, isActive: true, serviceCount: 2 },
  { id: categoryColor, name: 'Renk', sortOrder: 2, isActive: true, serviceCount: 0 },
];

const staffMember = (id: string, fullName: string, role: string) => ({
  id,
  fullName,
  displayName: null,
  title: null,
  photoUrl: null,
  calendarColor: '#0F6E5C',
  isBookableOnline: true,
  isActive: true,
  branchIds: [branchId],
  role,
  membershipStatus: 'Active',
});

export const staffFixture = [
  staffMember(staffElif, 'Elif Kaya', 'Owner'),
  staffMember(staffMert, 'Mert Demir', 'Staff'),
  staffMember(staffCan, 'Can Yücel', 'Staff'),
];

const serviceBase = {
  description: null,
  bufferBeforeMinutes: 0,
  bufferAfterMinutes: 0,
  requiresApproval: null,
  minNoticeMinutes: null,
  maxAdvanceDays: null,
  resourceTypeId: null,
  isActive: true,
  stages: [],
};

export const servicesFixture = [
  {
    ...serviceBase,
    id: '0193a8f0-0000-7000-8000-0000000000f1',
    categoryId: categoryHair,
    name: 'Saç kesimi',
    durationMinutes: 45,
    price: { amountMinor: 45000, currency: 'TRY' },
    priceDisplay: 'Fixed',
    isBranchPrice: false,
    deposit: { mode: 'Never' },
    isOnlineBookable: true,
    sortOrder: 1,
    staffIds: [staffElif, staffMert, staffCan],
    version: '4',
  },
  {
    ...serviceBase,
    id: '0193a8f0-0000-7000-8000-0000000000f2',
    categoryId: categoryHair,
    name: 'Keratin',
    description: 'Uzunluğa göre',
    durationMinutes: 90,
    price: { amountMinor: 180000, currency: 'TRY' },
    priceDisplay: 'StartingFrom',
    isBranchPrice: false,
    deposit: { mode: 'Always', amount: { amountMinor: 30000, currency: 'TRY' } },
    isOnlineBookable: false,
    sortOrder: 2,
    staffIds: [staffElif],
    version: '2',
  },
];

export const templatesFixture = [
  {
    id: '0193a8f0-0000-7000-8000-000000000a01',
    categoryName: 'Saç',
    name: 'Saç kesimi',
    defaultDurationMinutes: 45,
  },
  {
    id: '0193a8f0-0000-7000-8000-000000000a02',
    categoryName: 'Saç',
    name: 'Fön',
    defaultDurationMinutes: 30,
  },
  {
    id: '0193a8f0-0000-7000-8000-000000000a03',
    categoryName: 'Renk',
    name: 'Boya',
    defaultDurationMinutes: 120,
  },
];

export const workingHoursFixture = {
  days: [
    { dayOfWeek: 2, intervals: [{ start: '10:00', end: '20:00' }] },
    { dayOfWeek: 6, intervals: [{ start: '09:00', end: '19:00' }] },
  ],
  version: '9',
};

const closedDay = (id: string, date: string, note: string) => ({
  id,
  date,
  kind: 'Closure',
  isClosed: true,
  intervals: [],
  note,
});

export const overridesFixture = [
  {
    id: '0193a8f0-0000-7000-8000-0000000000d1',
    date: '2026-12-31',
    kind: 'ShortDay',
    isClosed: false,
    intervals: [{ start: '10:00', end: '16:00' }],
    note: 'Erken kapanış',
  },
  closedDay('0193a8f0-0000-7000-8000-0000000000d2', '2027-01-01', 'Yılbaşı'),
  closedDay('0193a8f0-0000-7000-8000-0000000000d3', '2027-03-20', 'Ramazan Bayramı'),
  closedDay('0193a8f0-0000-7000-8000-0000000000d4', '2027-03-21', 'Ramazan Bayramı'),
  closedDay('0193a8f0-0000-7000-8000-0000000000d5', '2027-03-22', 'Ramazan Bayramı'),
];

export const brandFixture = {
  id: '0193a8f0-0000-7000-8000-00000000000a',
  name: 'Kuaför Elif',
  slug: 'kuafor-elif',
  industryCode: 'hair_salon',
  status: 'Onboarding',
  shortDescription: null,
  logoUrl: null,
  phone: '+902163451212',
  email: null,
  instagram: '@kuaforelif',
  website: null,
  legalName: null,
  taxOffice: null,
  taxNumber: null,
  version: '7',
};

export function brandSettingsFixture(depositAvailable: boolean) {
  return {
    requiresApproval: false,
    approvalTimeoutMinutes: 120,
    allowStaffSelection: true,
    allowAnyStaffOption: true,
    cancellationCutoffHours: 24,
    rescheduleCutoffHours: 12,
    noShowAfterMinutes: 15,
    minNoticeMinutes: 60,
    maxAdvanceDays: 60,
    slotIntervalMinutes: 15,
    defaultBranchTransferMinutes: 0,
    depositPolicy: 'RiskyCustomersOnly',
    riskyCustomerNoShowThreshold: 2,
    maskCustomerPhoneFromStaff: true,
    reminderOffsetsMinutes: [1440, 120],
    reviewInviteEnabled: true,
    depositAvailable,
    version: '3',
  };
}

export const bookingPageFixture = {
  slug: 'kuafor-elif',
  url: 'https://omvila.com/kuafor-elif',
  coverColor: 'petrol',
  showReviews: true,
  showPrices: true,
  preAppointmentNote: null,
  version: '1',
};

const stepKeys = ['BusinessCreated', 'WorkingHours', 'Services', 'Staff', 'ShareBookingLink'];

export function fakeFetch({
  signedIn = true,
  session = sessionFixture(),
  verify = 'SignedIn',
  calls = {},
  doneSteps = ['BusinessCreated'],
  saveError,
  depositAvailable = false,
  branchLimitReached = false,
  withServices = false,
}: FakeApiOptions = {}) {
  const json = (status: number, body: unknown) =>
    Promise.resolve(
      new Response(JSON.stringify(body), {
        status,
        headers: { 'Content-Type': 'application/json' },
      }),
    );
  return async (request: Request): Promise<Response> => {
    const path = new URL(request.url).pathname;
    if (request.method !== 'GET') {
      const ifMatch = request.headers.get('If-Match');
      if (ifMatch !== null) {
        (calls[`${request.method} ${path} If-Match`] ??= []).push(ifMatch);
      }

      (calls[path] ??= []).push(
        await request
          .clone()
          .json()
          .catch(() => null),
      );
    }

    if (path === '/api/v1/manage/auth/otp/request') {
      return json(202, {
        otpId: '0193a8f0-0000-7000-8000-0000000000f1',
        expiresInSeconds: 180,
        resendAfterSeconds: 60,
        phoneMasked: '0532 *** ** 05',
      });
    }

    if (path === '/api/v1/manage/auth/otp/verify') {
      switch (verify) {
        case 'SignedIn':
          return json(200, { result: 'SignedIn', accessToken: 'jeton', expiresInSeconds: 900 });
        case 'SignupRequired':
          return json(200, { result: 'SignupRequired', signupToken: 'kayit-jetonu' });
        case 'otp_invalid':
          return json(422, { code: 'otp_invalid', meta: { attemptsLeft: 4 } });
        case 'otp_attempts_exceeded':
          return json(429, { code: 'otp_attempts_exceeded', meta: { retryAfterSeconds: 300 } });
      }
    }

    if (path === '/api/v1/manage/onboarding') {
      const steps = stepKeys.map((key) => ({ key, done: doneSteps.includes(key) }));
      return json(200, {
        steps,
        completedCount: steps.filter((st) => st.done).length,
        totalCount: steps.length,
      });
    }

    if (path === '/api/v1/manage/onboarding/events') {
      return Promise.resolve(new Response(null, { status: 204 }));
    }

    const saveFailure = (): Promise<Response> | null => {
      switch (saveError) {
        case 'concurrency_conflict':
          return json(412, { code: 'concurrency_conflict' });
        case 'slug_unavailable':
          return json(409, { code: 'slug_unavailable' });
        case 'validation_failed:phone':
          return json(400, { code: 'validation_failed', errors: { phone: ['invalid_format'] } });
        default:
          return null;
      }
    };
    const body = async () => (await request.clone().json()) as Record<string, unknown>;

    const services = withServices ? servicesFixture : [];
    if (path === '/api/v1/manage/services') {
      if (request.method === 'GET') {
        // Nişantaşı'nda saç kesimi şube fiyatıyla (kabul kriteri 9).
        const atSecond = new URL(request.url).searchParams.get('branchId') === secondBranchId;
        return json(200, {
          items: services.map((service) =>
            atSecond && service.name === 'Saç kesimi'
              ? { ...service, price: { amountMinor: 55000, currency: 'TRY' }, isBranchPrice: true }
              : service,
          ),
        });
      }

      const created = await body();
      if (
        created.deposit &&
        (created.deposit as { mode: string }).mode === 'Always' &&
        !depositAvailable
      ) {
        return json(409, { code: 'deposit_unavailable_no_iban' });
      }

      return json(201, {
        ...serviceBase,
        ...created,
        id: '0193a8f0-0000-7000-8000-0000000000f9',
        version: '1',
      });
    }

    const serviceMatch = /^\/api\/v1\/manage\/services\/([^/]+)$/.exec(path);
    if (serviceMatch && serviceMatch[1] !== 'from-templates') {
      const service = servicesFixture.find((item) => item.id === serviceMatch[1]);
      if (!service) {
        return json(404, { code: 'not_found' });
      }

      return request.method === 'GET'
        ? json(200, service)
        : (saveFailure() ?? json(200, { ...service, ...(await body()), version: '9' }));
    }

    if (/^\/api\/v1\/manage\/services\/[^/]+\/branch-prices\/[^/]+$/.test(path)) {
      return request.method === 'DELETE'
        ? Promise.resolve(new Response(null, { status: 204 }))
        : json(200, await body());
    }

    if (path === '/api/v1/manage/services/from-templates') {
      const { items } = (await body()) as { items: unknown[] };
      return json(201, { items: items.map(() => servicesFixture[0]) });
    }

    if (path === '/api/v1/manage/service-categories') {
      if (request.method === 'GET') {
        return json(200, { items: withServices ? categoriesFixture : [] });
      }

      const { name } = (await body()) as { name: string };
      return categoriesFixture.some((category) => category.name === name) && withServices
        ? json(409, { code: 'conflict' })
        : json(201, {
            id: '0193a8f0-0000-7000-8000-0000000000e9',
            name,
            sortOrder: 3,
            isActive: true,
            serviceCount: 0,
          });
    }

    if (path.startsWith('/api/v1/manage/industries/')) {
      return json(200, { items: templatesFixture });
    }

    if (path === '/api/v1/manage/staff' && request.method === 'GET') {
      return json(200, { items: staffFixture });
    }

    const branchBase = `/api/v1/manage/branches/${branchId}`;
    if (path === '/api/v1/manage/branches' && request.method === 'POST') {
      if (branchLimitReached) {
        return json(403, { code: 'forbidden', meta: { planLimit: 1 } });
      }

      const created = await body();
      return json(201, {
        ...branchFixture,
        ...created,
        id: '0193a8f0-0000-7000-8000-0000000000b2',
        version: '1',
      });
    }

    if (path === branchBase) {
      return request.method === 'GET'
        ? json(200, branchFixture)
        : (saveFailure() ?? json(200, { ...branchFixture, ...(await body()), version: '6' }));
    }

    if (path === `${branchBase}/working-hours`) {
      return request.method === 'GET'
        ? json(200, workingHoursFixture)
        : (saveFailure() ?? json(200, { ...(await body()), version: '10' }));
    }

    if (path === `${branchBase}/overrides`) {
      if (request.method === 'GET') {
        return json(200, { items: overridesFixture });
      }

      const created = await body();
      return json(201, {
        ...closedDay('0193a8f0-0000-7000-8000-0000000000d9', String(created.fromDate), ''),
        ...created,
      });
    }

    if (path.startsWith(`${branchBase}/overrides/`) && request.method === 'DELETE') {
      return Promise.resolve(new Response(null, { status: 204 }));
    }

    if (path === '/api/v1/manage/brand') {
      if (request.method === 'GET') {
        return json(200, brandFixture);
      }

      return saveFailure() ?? json(200, { ...brandFixture, ...(await body()), version: '8' });
    }

    if (path === '/api/v1/manage/brand/settings') {
      const settings = brandSettingsFixture(depositAvailable);
      if (request.method === 'GET') {
        return json(200, settings);
      }

      return saveFailure() ?? json(200, { ...settings, ...(await body()), version: '4' });
    }

    if (path === '/api/v1/manage/brand/booking-page') {
      if (request.method === 'GET') {
        return json(200, bookingPageFixture);
      }

      const saved = await body();
      return (
        saveFailure() ??
        json(200, {
          ...bookingPageFixture,
          ...saved,
          url: `https://omvila.com/${String(saved.slug)}`,
          version: '2',
        })
      );
    }

    if (path === '/api/v1/manage/auth/signup') {
      return json(201, { result: 'SignedIn', accessToken: 'jeton', expiresInSeconds: 900 });
    }

    if (path === '/api/v1/manage/auth/refresh') {
      return signedIn
        ? json(200, { accessToken: 'jeton', expiresInSeconds: 900 })
        : json(401, { code: 'unauthorized' });
    }

    if (path === '/api/v1/manage/session') {
      return request.headers.get('Authorization') === 'Bearer jeton'
        ? json(200, session)
        : json(401, { code: 'unauthorized' });
    }

    return json(404, { code: 'not_found' });
  };
}
