import { expect, test } from '@playwright/test';
import { signedIn } from './support';

// Kritik akış: platform girişi (K2: telefon + SMS kodu) → Genel bakış → Markalar → marka detayı (denetim notu, maskeli
// telefon). Prism örnekleriyle; yenileme jetonu sekmede (sessionStorage) tutulur.
test('telefon ve kodla giriş, marka detayına gidiş', async ({ page }) => {
  await page.goto('/markalar');
  await expect(
    page.getByRole('heading', { level: 1, name: 'Platform paneline giriş' }),
  ).toBeVisible();
  await page.getByLabel('Cep telefonu').fill('0532 700 10 20');
  await page.getByRole('button', { name: 'SMS kodu gönder' }).click();
  await expect(page.getByText('numarasına 6 haneli kod gönderdik.')).toBeVisible();
  await page.getByLabel('Doğrulama kodu, hane 1').click();
  await page.keyboard.type('482193');
  await page.getByRole('button', { name: 'Giriş yap' }).click();

  await expect(page.getByRole('heading', { level: 1, name: 'Markalar' })).toBeVisible();
  expect(await page.evaluate(() => sessionStorage.getItem('omvila.platform.yenileme'))).toBe(
    'ornek-yenileme-jetonu',
  );
  await page.getByRole('table', { name: 'Markalar' }).getByText('Kuaför Elif').click();
  await expect(page.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeVisible();
  await expect(page.getByText('Bu görüntüleme denetim kaydına yazıldı.')).toBeVisible();
  await expect(page.getByText('Elif Kaya · 0532 *** ** 05')).toBeVisible();
});

test('üst bar araması (⌘K) Markalar listesini açar', async ({ page }) => {
  await signedIn(page);
  await page.goto('/');
  await expect(page.getByRole('heading', { level: 1, name: 'Platform' })).toBeVisible();
  await page.keyboard.press('ControlOrMeta+k');
  await page.keyboard.type('elif');
  await page.keyboard.press('Enter');
  await expect(page.getByRole('heading', { level: 1, name: 'Markalar' })).toBeVisible();
  await expect(page).toHaveURL(/\/markalar\?q=elif$/);
});
