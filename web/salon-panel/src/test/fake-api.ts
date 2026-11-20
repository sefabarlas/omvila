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
}

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
