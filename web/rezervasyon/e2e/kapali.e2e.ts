import { expect, test } from '@playwright/test';
import { ids } from '../src/test/booking-fixtures';
import { bookingApi } from './support';

// Dilim 6 KK1 (rezervasyon sayfası): salt okunur abonelikte online randevu kapalıdır. Salon kartı `bookingEnabled:
// false` dönerse akış hiç açılmaz ("şu an online randevu almıyor", "Salonu ara"); akış açıkken abonelik salt okunura
// geçerse oluşturma 403 booking_disabled döner ve aynı desen gösterilir. Panel tarafı:
// web/salon-panel/e2e/salt-okunur.e2e.ts; sunucu tarafı api entegrasyon testleri.
test('salt okunur salon: online randevu kapalı, akış açılmaz, "Salonu ara" (KK1)', async ({
  page,
}) => {
  const errors: string[] = [];
  page.on('pageerror', (error) => errors.push(error.message));
  const calls = await bookingApi(page);
  await page.goto('/kuafor-elif-kapali');

  await expect(page.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeAttached();
  const notice = page.getByRole('status').filter({ hasText: 'online randevu almıyor' });
  await expect(notice).toContainText('Kuaför Elif şu an online randevu almıyor.');
  await expect(notice).toContainText('Randevu için salonu arayabilirsiniz.');
  await expect(page.getByRole('link', { name: 'Salonu ara' })).toHaveAttribute(
    'href',
    'tel:+905321234567',
  );

  // Hizmet listesi, adımlar ve devam butonu yok.
  await expect(page.getByText('Saç kesimi', { exact: true })).toHaveCount(0);
  await expect(page.getByRole('button', { name: 'Devam et' })).toHaveCount(0);

  // Adım adresi elle yazılsa da akış açılmaz.
  await page.goto(
    `/kuafor-elif-kapali?step=details&services=${ids.sac}&staff=${ids.elif}&date=2026-11-24&time=14:30`,
  );
  await expect(page.getByRole('link', { name: 'Salonu ara' })).toBeVisible();
  await expect(page.getByLabel('Telefon numaranız')).toHaveCount(0);
  expect(calls.create).toHaveLength(0);
  expect(errors).toEqual([]);
});

test('akış sırasında salt okunura geçiş: oluşturma 403 booking_disabled → kapalı deseni (KK1)', async ({
  page,
}) => {
  await bookingApi(page);
  // Sonradan eklenen yönlendirme önce çalışır: yalnız oluşturma isteği 403 döner.
  await page.route('http://localhost:4011/api/v1/public/**/appointments', async (route) => {
    if (route.request().method() !== 'POST') {
      await route.fallback();
      return;
    }

    await route.fulfill({
      status: 403,
      contentType: 'application/problem+json',
      headers: {
        'Access-Control-Allow-Origin': 'http://localhost:3100',
        'Access-Control-Allow-Credentials': 'true',
      },
      body: JSON.stringify({
        type: 'about:blank',
        title: 'booking_disabled',
        status: 403,
        code: 'booking_disabled',
      }),
    });
  });
  await page.goto(
    `/kuafor-elif?step=details&services=${ids.sac},${ids.fon}&staff=${ids.elif}&date=2026-11-24&time=14:30`,
  );
  await page.getByLabel('Ad soyad').fill('Ayşe Yılmaz');
  await page.getByLabel('Telefon numaranız').fill('532 123 45 67');
  await page.getByText(/okudum; randevu için/).click();
  await page.getByRole('button', { name: 'Doğrulama kodu gönder' }).click();
  await page.getByRole('textbox', { name: 'Doğrulama kodu, hane 1' }).fill('482193');
  await page.getByRole('button', { name: 'Randevuyu oluştur' }).click();

  await expect(page.getByText('Kuaför Elif şu an online randevu almıyor.')).toBeVisible();
  await expect(page.getByRole('link', { name: 'Salonu ara' })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'Randevunuz onaylandı' })).toHaveCount(0);
  await expect(page.getByRole('button', { name: 'Randevuyu oluştur' })).toHaveCount(0);
});
