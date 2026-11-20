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
}

export function fakeFetch({
  signedIn = true,
  session = sessionFixture(),
  verify = 'SignedIn',
  calls = {},
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
    if (request.method === 'POST') {
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
