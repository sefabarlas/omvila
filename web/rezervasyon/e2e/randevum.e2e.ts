import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import { accessCode, etag } from '../src/test/appointment-fixtures';
import { appointmentApi } from './support';

// SMS bağlantısı (/r/{kod}): Randevum, saat değiştir ve iptal. Ekran görüntüleri e2e-results/ekranlar altına
// (design/ekranlar/rezervasyon-sayfasi/Randevum, Ertele, Iptal ile yan yana); kritik akış: müşteri iptali.

interface ScreenCase {
  name: string;
  path: string;
  ready: (page: Page) => Promise<void>;
}

const screens: ScreenCase[] = [
  {
    name: 'Randevum',
    path: `/r/${accessCode}`,
    ready: async (page) => {
      await expect(page.getByRole('heading', { name: 'Plan değişti mi?' })).toBeVisible();
    },
  },
  {
    name: 'Ertele',
    path: `/r/${accessCode}?islem=saat`,
    ready: async (page) => {
      await page.getByRole('radio', { name: '15:15' }).click();
      await expect(page.getByText("16:30'da biter")).toBeVisible();
    },
  },
  {
    name: 'Iptal',
    path: `/r/${accessCode}?islem=iptal`,
    ready: async (page) => {
      await page.getByText('Başka saat istiyorum').click();
      await expect(page.getByText('İptal yerine saati değiştirebilirsiniz')).toBeVisible();
    },
  },
];

for (const theme of ['light', 'dark'] as const) {
  for (const screen of screens) {
    test(`${screen.name} (${theme === 'light' ? 'açık' : 'koyu'})`, async ({ page }) => {
      const errors: string[] = [];
      page.on('pageerror', (error) => errors.push(error.message));
      await page.emulateMedia({ colorScheme: theme });
      await appointmentApi(page);
      await page.goto(screen.path);
      await screen.ready(page);
      await page.screenshot({
        path: `e2e-results/ekranlar/${screen.name}${theme === 'dark' ? 'Koyu' : ''}.png`,
        fullPage: true,
        animations: 'disabled',
      });
      expect(errors).toEqual([]);
    });
  }
}

test('SMS bağlantısından randevu iptal edilir', async ({ page }) => {
  const errors: string[] = [];
  page.on('pageerror', (error) => errors.push(error.message));
  const calls = await appointmentApi(page);
  await page.goto(`/r/${accessCode}`);

  await expect(page.getByRole('heading', { level: 1, name: 'Çar 25 Kas · 14:30' })).toBeVisible();
  await page.getByRole('button', { name: 'Randevuyu iptal et' }).click();
  const dialog = page.getByRole('dialog', { name: 'Randevu iptal edilsin mi?' });
  await expect(dialog).toBeVisible();
  await expect(page).toHaveURL(/\?islem=iptal$/);
  await dialog.getByText('Planım değişti').click();
  await dialog.getByRole('button', { name: 'İptal et' }).click();

  await expect(page.getByRole('heading', { name: 'Randevunuz iptal edildi' })).toBeVisible();
  await expect(page.getByText('Randevunuz · İptal')).toBeVisible();
  await expect(page).toHaveURL(new RegExp(`/r/${accessCode}$`));
  expect(calls.cancel).toEqual([{ ifMatch: etag, body: { reason: 'PlanChanged' } }]);

  // Yenilemede iptal durumu kalır (iptal edilen randevunun kodu geçerli, T-52 (1)).
  await page.reload();
  await expect(page.getByRole('heading', { name: 'Randevunuz iptal edildi' })).toBeVisible();
  expect(errors).toEqual([]);
});

test('bilinmeyen kod "Bu randevu bulunamadı" gösterir', async ({ page }) => {
  await appointmentApi(page);
  await page.goto('/r/Bilinmeyn1');
  await expect(
    page.getByRole('heading', { level: 1, name: 'Bu randevu bulunamadı' }),
  ).toBeVisible();
});
