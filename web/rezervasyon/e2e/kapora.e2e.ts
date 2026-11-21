import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import {
  accessCode,
  depositExpiredFixture,
  linkDepositFixture,
} from '../src/test/appointment-fixtures';
import { appointmentApi, bookingApi } from './support';

// Kapora havalesi (Dilim 4, KK3, KK4, KK6; S-03), müşteri tarafı: kaporalı hizmet → Onay'da kapora kutusu → oluştur →
// Havale (dört alan kopyalanır) → "Havaleyi yaptım, salona bildir" → "Salon onayı bekleniyor". Salonun "Kapora geldi"
// işareti panel tarafındadır; burada sunucunun sonraki yanıtı verilir. "Şimdi" 24 Kasım 2026 10:00; havale son zamanı
// 11:58 (1 sa 58 dk).
const api = 'http://localhost:4011/api/v1/public';

const next = (page: Page) => page.getByRole('button', { name: 'Devam et' }).click();

test('kaporalı randevu: Onay kapora kutusu → Havale → bildir → Salon onayı bekleniyor', async ({
  page,
  context,
}) => {
  const errors: string[] = [];
  page.on('pageerror', (error) => errors.push(error.message));
  await context.grantPermissions(['clipboard-read', 'clipboard-write']);
  const calls = await bookingApi(page, { deposit: true });
  await page.goto('/kuafor-elif');

  await expect(page.getByText('45 dk · Kapora 150 ₺')).toBeVisible();
  await page.getByText('Saç kesimi', { exact: true }).click();
  await page.getByText('Fön', { exact: true }).click();
  await next(page);
  await page.getByRole('radio', { name: /Elif Kaya/ }).click();
  await next(page);
  await page.getByRole('radio', { name: '14:30' }).click();
  await next(page);
  await page.getByLabel('Ad soyad').fill('Ayşe Yılmaz');
  await page.getByLabel('Telefon numaranız').fill('532 123 45 67');
  await page.getByText(/okudum; randevu için/).click();
  await page.getByRole('button', { name: 'Doğrulama kodu gönder' }).click();
  await page.getByRole('textbox', { name: 'Doğrulama kodu, hane 1' }).fill('4821');
  await page.getByRole('textbox', { name: 'Doğrulama kodu, hane 5' }).fill('93');

  const summary = page.getByRole('region', { name: 'Randevu özeti' });
  await expect(summary.getByText('Kapora (havale ile)')).toBeVisible();
  await expect(summary.getByText('−150 ₺')).toBeVisible();
  await expect(summary.getByText('600 ₺')).toBeVisible();
  await expect(page.getByRole('region', { name: 'Kapora' })).toContainText(
    'Bu randevu 150 ₺ kapora ile kesinleşir.',
  );
  await page.getByRole('button', { name: 'Randevuyu oluştur' }).click();

  await expect(page.getByRole('heading', { name: 'Kaporayı havale ile gönderin' })).toBeVisible();
  await expect(page.getByText('1 sa 58 dk kaldı')).toBeVisible();
  await page.getByRole('button', { name: 'Kopyala IBAN' }).click();
  await expect(page.getByRole('button', { name: 'Kopyalandı IBAN' })).toBeVisible();
  expect(await page.evaluate(() => navigator.clipboard.readText())).toBe(
    'TR120006400000112345678901',
  );
  await page.getByRole('button', { name: 'Kopyala Açıklama kodu' }).click();
  expect(await page.evaluate(() => navigator.clipboard.readText())).toBe('OMV-7K2X');

  await page.getByRole('button', { name: 'Havaleyi yaptım, salona bildir' }).click();
  await expect(page.getByRole('heading', { name: 'Salon onayı bekleniyor' })).toBeVisible();
  expect(calls.report).toEqual([`/api/v1/public/appointments/${accessCode}/deposit/report`]);
  await expect(page.getByRole('button', { name: 'Havaleyi yaptım, salona bildir' })).toHaveCount(0);
  await expect(page.getByText('Randevunuz onaylandı')).toHaveCount(0);
  expect(errors).toEqual([]);
});

test('SMS bağlantısı: bildir → Salon onayı bekleniyor; salon kaporayı alınca "Onaylandı" ve kapora satırı', async ({
  page,
}) => {
  const calls = await appointmentApi(page, linkDepositFixture('Awaiting'));
  await page.goto(`/r/${accessCode}`);

  await expect(
    page.getByRole('heading', { level: 1, name: 'Kaporayı havale ile gönderin' }),
  ).toBeVisible();
  await page.getByRole('button', { name: 'Havaleyi yaptım, salona bildir' }).click();
  await expect(page.getByRole('heading', { name: 'Salon onayı bekleniyor' })).toBeVisible();
  expect(calls.report).toBe(1);

  // Salon "Kapora geldi" dedi (A3): sonraki okuma onaylı.
  await page.route(`${api}/appointments/${accessCode}`, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      headers: {
        'Access-Control-Allow-Origin': 'http://localhost:3100',
        'Access-Control-Expose-Headers': 'ETag',
        ETag: 'W/"8821"',
      },
      body: JSON.stringify(linkDepositFixture('Received')),
    });
  });
  await page.reload();
  await expect(page.getByText('Randevunuz · Onaylandı')).toBeVisible();
  await expect(page.getByText('Kapora alındı (havale ile)')).toBeVisible();
  await expect(page.getByText('600 ₺')).toBeVisible();
});

test('süre sayfa açıkken dolunca durum yeniden okunur: "Kapora gelmedi" (KK6)', async ({
  page,
}) => {
  await appointmentApi(page, linkDepositFixture('Awaiting'));
  await page.goto(`/r/${accessCode}`);
  await expect(page.getByText('1 sa 58 dk kaldı')).toBeVisible();

  // Otomatik iptal işi çalıştı: sonraki okuma iptal; saat son zamanın ötesine alınır.
  await page.route(`${api}/appointments/${accessCode}`, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      headers: { 'Access-Control-Allow-Origin': 'http://localhost:3100' },
      body: JSON.stringify(depositExpiredFixture()),
    });
  });
  await page.clock.setFixedTime(new Date('2026-11-24T08:58:30Z'));
  await expect(page.getByText('Kapora süresinde gelmedi, randevu iptal edildi.')).toBeVisible({
    timeout: 20_000,
  });
  await expect(page.getByText('Randevunuz · İptal')).toBeVisible();
});
