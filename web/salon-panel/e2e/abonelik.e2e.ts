import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import { calendarDemo } from './calendar-demo';
import { subscriptionDemo, subscriptionSession } from './subscription-demo';

// Dilim 6 KK1, KK2 (salon tarafı): havale bildirimi (multipart), kontör al (KNT-, purpose Credits), abonelik şeritleri.
// Durumlar'daki "Abonelik · ödeme gecikti" ve "Abonelik · salt okunur" şeritlerinin görüntüleri (DurumOdemeGecikti,
// DurumSaltOkunur; açık ve koyu) e2e-results/ekranlar altına yazılır.
const cors = {
  'Access-Control-Allow-Origin': 'http://localhost:5180',
  'Access-Control-Allow-Credentials': 'true',
};

/** Havale bildirimi ucuna giden multipart gövdeler; yanıt sözleşme örneği gibi 201. */
async function captureNotices(page: Page): Promise<string[]> {
  const bodies: string[] = [];
  await page.route(/\/api\/v1\/manage\/subscription\/payment-notices$/, async (route) => {
    if (route.request().method() !== 'POST') {
      await route.fallback();
      return;
    }

    bodies.push(route.request().postDataBuffer()?.toString('utf8') ?? '');
    await route.fulfill({
      status: 201,
      contentType: 'application/json',
      headers: cors,
      body: JSON.stringify({
        id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b01',
        purpose: 'Subscription',
        transferCode: 'ABN-KE-1125',
        expectedAmount: null,
        credits: null,
        senderName: null,
        hasReceipt: true,
        status: 'Pending',
        notifiedUtc: '2026-11-25T09:20:00Z',
      }),
    });
  });
  return bodies;
}

test('abonelik havalesini bildir: gönderen adı ve dekont multipart gider', async ({ page }) => {
  await subscriptionDemo(page);
  const bodies = await captureNotices(page);
  await page.goto('/ayarlar/abonelik');
  await page.getByRole('button', { name: 'Havaleyi bildir (dekont isteğe bağlı)' }).click();
  const dialog = page.getByRole('dialog', { name: 'Havaleyi bildir' });
  await dialog.getByLabel('Gönderen adı (isteğe bağlı)').fill('Elif Kaya');
  await dialog.getByLabel('Dekont dosyası').setInputFiles({
    name: 'dekont.pdf',
    mimeType: 'application/pdf',
    buffer: Buffer.from('%PDF-1.4\n'),
  });
  await dialog.getByRole('button', { name: 'Bildir' }).click();
  await expect(page.getByText('Havale bildiriminiz alındı')).toBeVisible();
  expect(bodies).toHaveLength(1);
  expect(bodies[0]).toMatch(/name="purpose"\r\n\r\nSubscription/);
  expect(bodies[0]).toMatch(/name="senderName"\r\n\r\nElif Kaya/);
  expect(bodies[0]).toMatch(/name="receipt"; filename="dekont.pdf"/);
});

test('kontör al: paket → KNT- havale bilgisi → bildir (purpose Credits)', async ({ page }) => {
  await subscriptionDemo(page);
  const bodies = await captureNotices(page);
  await page.goto('/ayarlar/bildirimler');
  await page.getByRole('button', { name: 'Kontör al' }).first().click();
  const dialog = page.getByRole('dialog', { name: 'Kontör al' });
  await dialog.getByRole('radio', { name: /Büyük/ }).check();
  await dialog.getByRole('button', { name: 'Havale bilgisini göster' }).click();
  await expect(dialog.getByText('KNT-KE-1125')).toBeVisible();
  await dialog.getByRole('button', { name: 'Havaleyi bildir' }).click();
  await dialog.getByRole('button', { name: 'Bildir' }).click();
  await expect(page.getByText('Havale bildiriminiz alındı')).toBeVisible();
  expect(bodies[0]).toMatch(/name="purpose"\r\n\r\nCredits/);
  expect(bodies[0]).toMatch(/name="creditPackageId"\r\n\r\n[0-9a-f-]{36}/);
});

for (const theme of ['light', 'dark'] as const) {
  test(`abonelik şeritleri (${theme === 'light' ? 'açık' : 'koyu'})`, async ({ page }) => {
    const suffix = theme === 'dark' ? 'Koyu' : '';
    await page.emulateMedia({ colorScheme: theme });
    await page.addInitScript((value) => {
      try {
        localStorage.setItem('omvila.tema', value);
      } catch {
        // Depolama kapalıysa sistem teması kullanılır.
      }
    }, theme);
    await calendarDemo(page);
    await subscriptionSession(page, {
      status: 'PastDue',
      trialEndsOn: null,
      pastDueSince: '2026-11-20',
      readOnlyOn: '2026-11-27',
    });
    await page.goto('/hizmetler');
    const pastDue = page.getByRole('status').filter({ hasText: 'Abonelik ödemesi' });
    await expect(pastDue).toContainText('6. gün');
    await expect(pastDue).toContainText('2 gün içinde havale ile ödemezseniz');
    await pastDue.screenshot({ path: `e2e-results/ekranlar/DurumOdemeGecikti${suffix}.png` });
    await pastDue.getByRole('button', { name: 'Havale ile öde' }).click();
    await expect(page).toHaveURL(/\/ayarlar\/abonelik$/);

    await page.unrouteAll({ behavior: 'ignoreErrors' });
    await calendarDemo(page);
    await subscriptionSession(page, {
      status: 'ReadOnly',
      trialEndsOn: null,
      pastDueSince: '2026-11-18',
      readOnlyOn: null,
    });
    await page.goto('/hizmetler');
    const readOnly = page.getByRole('alert').filter({ hasText: 'Panel salt okunur' });
    await expect(readOnly).toContainText('7 gündür alınamadı');
    await readOnly.screenshot({ path: `e2e-results/ekranlar/DurumSaltOkunur${suffix}.png` });
    await expect(readOnly.getByRole('button', { name: 'Ödemeyi tamamla' })).toBeVisible();
  });
}
