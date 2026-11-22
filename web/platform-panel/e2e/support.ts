import type { Page } from '@playwright/test';

export const api = `http://localhost:${process.env.E2E_API_PORT ?? '4010'}/api/v1/admin`;

export const json = (body: unknown, status = 200) => ({
  status,
  contentType: 'application/json',
  // Tarayıcıdan Prism'e çapraz köken istekleri; yanıtı test verdiğinde CORS başlığı da gerekir (admin yüzeyi çerezsiz).
  headers: { 'Access-Control-Allow-Origin': '*' },
  body: JSON.stringify(body),
});

/** Sekmede yenileme jetonu var (sayfa yenilenmiş gibi): Prism yenileme örneğiyle oturum açılır. */
export async function signedIn(page: Page) {
  await page.addInitScript(() => {
    try {
      sessionStorage.setItem('omvila.platform.yenileme', 'ornek-yenileme-jetonu');
    } catch {
      // Depolama kapalıysa giriş ekranı açılır.
    }
  });
}

/** Belirli GET yanıtını verir (geri kalan istekler Prism'e). */
export async function respond(page: Page, path: string, body: unknown) {
  await page.route(`${api}${path}`, (route) =>
    route.request().method() === 'GET' ? route.fulfill(json(body)) : route.fallback(),
  );
}

/** Oturum rolünü değiştirir (Prism örneği Destek). */
export async function asAdmin(page: Page) {
  await respond(page, '/session', {
    user: {
      id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4fa102',
      fullName: 'Deniz Yılmaz',
      phoneMasked: '0555 *** ** 01',
    },
    role: 'PlatformAdmin',
    permissions: [
      'platform.brands.view',
      'platform.brands.manage',
      'platform.payments.match',
      'platform.sms-senders.decide',
      'platform.credits.grant',
      'platform.plans.view',
      'platform.plans.manage',
      'platform.billing.manage',
      'platform.audit.view',
      'platform.audit.export',
    ],
    pendingApprovalCount: 7,
  });
}
