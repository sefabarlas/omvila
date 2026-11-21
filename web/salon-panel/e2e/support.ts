import type { Page } from '@playwright/test';

const api = 'http://localhost:4010/api/v1/manage';

const json = (body: unknown, status = 200) => ({
  status,
  contentType: 'application/json',
  // Tarayıcıdan Prism'e çapraz köken istekleri; yanıtı test verdiğinde CORS başlıkları da gerekir.
  headers: {
    'Access-Control-Allow-Origin': 'http://localhost:5180',
    'Access-Control-Allow-Credentials': 'true',
  },
  body: JSON.stringify(body),
});

/** Yenileme çerezi yok: giriş ve kayıt ekranları açılır. */
export async function signedOut(page: Page) {
  await page.route(`${api}/auth/refresh`, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(json({ code: 'unauthorized' }, 401)),
  );
}

/** Kod doğrulamada kayıtsız numara (SignupRequired). */
export async function unregisteredPhone(page: Page) {
  await page.route(`${api}/auth/otp/verify`, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(json({ result: 'SignupRequired', signupToken: 'kayit-jetonu' })),
  );
}

/**
 * Oturum özetini değiştirir (personel rolü); geri kalan alanlar Prism örneğinden. İç içe alan (ör. brand.status)
 * için yama, örnek oturumu alan bir işlev olabilir.
 */
export async function sessionAs(
  page: Page,
  patch: Record<string, unknown> | ((session: Record<string, unknown>) => Record<string, unknown>),
) {
  await page.route(`${api}/session`, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    const response = await route.fetch();
    const session = (await response.json()) as Record<string, unknown>;
    await route.fulfill(
      json({ ...session, ...(typeof patch === 'function' ? patch(session) : patch) }),
    );
  });
}

/**
 * Belirli yol ve yönteme giden ilk isteğin gövdesi. İstek Prism'e iletilir. (Akış gövdeli isteklerde
 * `request.postData()` boş dönebildiği için gövde yönlendirmede okunur.)
 */
export async function captureBody(
  page: Page,
  path: string,
  method: string,
): Promise<() => Promise<unknown>> {
  let resolve: (body: unknown) => void = () => undefined;
  const body = new Promise<unknown>((r) => {
    resolve = r;
  });
  await page.route(`${api}${path}`, async (route) => {
    if (route.request().method() === method) {
      resolve(route.request().postDataJSON() as unknown);
    }

    await route.fallback();
  });
  return () => body;
}

/**
 * Kapora: oturuma settings.payment izni eklenir (Prism oturum örneğinde yok) ve isteğe bağlı olarak BrandSettings
 * yanıtında depositAvailable sabitlenir (IBAN tanımlı / tanımsız desenleri).
 */
export async function depositSetup(page: Page, depositAvailable?: boolean) {
  await sessionAs(page, (session) => ({
    permissions: [...(session.permissions as string[]), 'settings.payment'],
  }));
  if (depositAvailable === undefined) {
    return;
  }

  await page.route(`${api}/brand/settings`, async (route) => {
    if (route.request().method() !== 'GET') {
      await route.fallback();
      return;
    }

    const response = await route.fetch();
    const settings = (await response.json()) as Record<string, unknown>;
    await route.fulfill(json({ ...settings, depositAvailable }));
  });
}
