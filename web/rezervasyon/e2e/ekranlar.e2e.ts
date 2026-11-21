import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import { bookingApi } from './support';

// Görsel karşılaştırma (test-stratejisi.md): akış ekranlarının açık ve koyu görüntüleri e2e-results/ekranlar altına
// yazılır; design/ekranlar/rezervasyon-sayfasi ile yan yana insan gözüyle karşılaştırılır. Otomatik piksel
// karşılaştırması yok: salon kartı Prism örneğinden gelir (adres, açıklama "string"), akış verisi test verisidir.
interface ScreenCase {
  name: string;
  desktop?: boolean;
  /** Ekrana kadar akışı yürütür. */
  reach: (page: Page) => Promise<void>;
}

const next = (page: Page) => page.getByRole('button', { name: 'Devam et' }).click();

async function services(page: Page) {
  await page.getByText('Saç kesimi', { exact: true }).click();
  await page.getByText('Fön', { exact: true }).click();
}

async function time(page: Page) {
  await services(page);
  await next(page);
  await page.getByRole('radio', { name: /Elif Kaya/ }).click();
  await next(page);
  await page.getByRole('radio', { name: '14:30' }).click();
}

async function details(page: Page) {
  await time(page);
  await next(page);
  await page.getByLabel('Ad soyad').fill('Ayşe Yılmaz');
  await page.getByLabel('Telefon numaranız').fill('532 123 45 67');
  await page.getByText(/okudum; randevu için/).click();
}

async function code(page: Page) {
  await details(page);
  await page.getByRole('button', { name: 'Doğrulama kodu gönder' }).click();
  await page.getByRole('textbox', { name: 'Doğrulama kodu, hane 1' }).fill('4821');
}

async function confirm(page: Page) {
  await code(page);
  await page.getByRole('textbox', { name: 'Doğrulama kodu, hane 5' }).fill('93');
  await expect(page.getByRole('button', { name: 'Randevuyu oluştur' })).toBeVisible();
}

const screens: ScreenCase[] = [
  { name: 'Main', reach: services },
  {
    name: 'Personel',
    reach: async (page) => {
      await services(page);
      await next(page);
      await page.getByRole('radio', { name: /Elif Kaya/ }).click();
    },
  },
  { name: 'Saat', reach: time },
  {
    name: 'DoluGun',
    reach: async (page) => {
      await time(page);
      await page.getByRole('radio', { name: '26 Kasım Perşembe, boş saat yok' }).click();
      await expect(page.getByRole('heading', { name: 'Perşembe dolu' })).toBeVisible();
    },
  },
  { name: 'Telefon', reach: details },
  { name: 'Kod', reach: code },
  { name: 'Onay', reach: confirm },
  {
    name: 'Basari',
    reach: async (page) => {
      await confirm(page);
      await page.getByRole('button', { name: 'Randevuyu oluştur' }).click();
      await expect(page.getByRole('heading', { name: 'Randevunuz onaylandı' })).toBeVisible();
    },
  },
  { name: 'Masaustu', desktop: true, reach: services },
  { name: 'MasaustuSaat', desktop: true, reach: time },
];

for (const theme of ['light', 'dark'] as const) {
  for (const screen of screens) {
    test(`${screen.name} (${theme === 'light' ? 'açık' : 'koyu'})`, async ({ page }) => {
      const errors: string[] = [];
      page.on('pageerror', (error) => errors.push(error.message));
      if (screen.desktop) await page.setViewportSize({ width: 1280, height: 1000 });
      await page.emulateMedia({ colorScheme: theme });
      await bookingApi(page);
      await page.goto('/kuafor-elif');
      await expect(page.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeAttached();
      await screen.reach(page);
      await page.screenshot({
        path: `e2e-results/ekranlar/${screen.name}${theme === 'dark' ? 'Koyu' : ''}.png`,
        fullPage: true,
        animations: 'disabled',
      });
      expect(errors).toEqual([]);
    });
  }
}
