import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import { calendarDemo, demoSunday } from './calendar-demo';
import { customersDemo, demoCustomerId } from './customers-demo';

// Görsel karşılaştırma (test-stratejisi.md): ekranların açık ve koyu ekran görüntüleri e2e-results/ekranlar altına
// yazılır; tasarım görüntüleriyle (design/ekranlar/salon-paneli) yan yana insan gözüyle karşılaştırılır. Otomatik piksel
// karşılaştırması yok (Prism örnek verisi tasarımdaki verilerden farklı). Takvim ekranları tasarımdaki veriyle
// (calendar-demo.ts), müşteri ekranları customers-demo.ts ile açılır.
interface ScreenCase {
  path: string;
  name: string;
  heading: string | RegExp;
  setup?: (page: Page) => Promise<void>;
  /** Ekran görüntüsünden önce (yan paneli açmak gibi). */
  act?: (page: Page) => Promise<void>;
}

const screens: ScreenCase[] = [
  { path: '/', name: 'GenelBakis', heading: /^(Hoş geldiniz|Günaydın|İyi günler|İyi akşamlar)/ },
  { path: '/hizmetler', name: 'Hizmetler', heading: 'Hizmetler' },
  { path: '/personel', name: 'Personel', heading: 'Personel' },
  { path: '/ayarlar/isletme', name: 'AyarlarIsletme', heading: 'Ayarlar' },
  { path: '/ayarlar/sube', name: 'AyarlarSube', heading: 'Ayarlar' },
  { path: '/ayarlar/randevu-kurallari', name: 'Ayarlar', heading: 'Ayarlar' },
  { path: '/ayarlar/rezervasyon', name: 'AyarlarRezervasyon', heading: 'Ayarlar' },
  {
    path: '/takvim',
    name: 'Main',
    heading: 'Takvim',
    setup: calendarDemo,
    act: (page) => expect(page.getByRole('region', { name: 'Günlük takvim' })).toBeVisible(),
  },
  {
    path: '/takvim?gorunum=hafta',
    name: 'HaftaTakvim',
    heading: 'Takvim',
    setup: calendarDemo,
    act: (page) =>
      expect(page.getByRole('region', { name: 'Elif Kaya haftalık takvim' })).toBeVisible(),
  },
  {
    path: `/takvim?tarih=${demoSunday}`,
    name: 'TakvimKapali',
    heading: 'Takvim',
    setup: calendarDemo,
    act: (page) => expect(page.getByRole('heading', { name: 'Kapalı gün' })).toBeVisible(),
  },
  {
    path: '/takvim',
    name: 'RandevuOnayli',
    heading: 'Takvim',
    setup: calendarDemo,
    act: async (page) => {
      await page.getByRole('button', { name: /^Derya Şahin/ }).click();
      await expect(
        page
          .getByRole('dialog', { name: 'Randevu · #A-20476 · Telefonla' })
          .getByText('13:00–13:45'),
      ).toBeVisible();
      // Yan panelin açılış hareketi bitsin.
      await page.waitForFunction(() =>
        document.getAnimations().every((animation) => animation.playState !== 'running'),
      );
    },
  },
  {
    path: '/musteriler',
    name: 'Musteriler',
    heading: 'Müşteriler',
    setup: customersDemo,
    act: (page) => expect(page.getByRole('table', { name: 'Müşteriler' })).toBeVisible(),
  },
  {
    path: '/musteriler',
    name: 'MusterilerBos',
    heading: 'Müşteriler',
    setup: (page) => customersDemo(page, { empty: true }),
    act: (page) =>
      expect(page.getByRole('heading', { name: 'Henüz müşteriniz yok' })).toBeVisible(),
  },
  {
    path: '/musteriler?q=Kaan%20Y%C4%B1l&filtre=duzenli',
    name: 'MusterilerAramaYok',
    heading: 'Müşteriler',
    setup: customersDemo,
    act: (page) => expect(page.getByRole('heading', { name: /için sonuç yok$/ })).toBeVisible(),
  },
  {
    path: '/musteriler',
    name: 'MusteriEkle',
    heading: 'Müşteriler',
    setup: customersDemo,
    act: async (page) => {
      await page.getByRole('button', { name: 'Müşteri ekle' }).click();
      const dialog = page.getByRole('dialog', { name: 'Müşteri ekle' });
      await dialog.getByLabel('Ad soyad').fill('Gizem Arslan');
      await dialog.getByLabel('Cep telefonu').fill('0535 881 40 09');
      await dialog.getByRole('button', { name: 'Müşteriyi ekle' }).click();
      await expect(dialog.getByRole('button', { name: 'Mevcut kartı aç' })).toBeVisible();
      await expect(dialog.getByText(/Gizem Arslan, 3 ziyaret/)).toBeVisible();
      await page.waitForFunction(() =>
        document.getAnimations().every((animation) => animation.playState !== 'running'),
      );
    },
  },
  {
    path: `/musteriler/${demoCustomerId}`,
    name: 'MusteriKarti',
    heading: 'Selin Öztürk',
    setup: customersDemo,
    act: (page) => expect(page.getByRole('table', { name: 'Randevular' })).toBeVisible(),
  },
];

for (const theme of ['light', 'dark'] as const) {
  for (const screen of screens) {
    test(`${screen.name} (${theme === 'light' ? 'açık' : 'koyu'})`, async ({ page }) => {
      const errors: string[] = [];
      page.on('pageerror', (error) => errors.push(error.message));
      await page.emulateMedia({ colorScheme: theme });
      await page.addInitScript((value) => {
        try {
          localStorage.setItem('omvila.tema', value);
        } catch {
          // Depolama kapalıysa sistem teması kullanılır.
        }
      }, theme);
      await screen.setup?.(page);
      await page.goto(screen.path);
      await expect(page.getByRole('heading', { level: 1, name: screen.heading })).toBeVisible();
      await screen.act?.(page);
      await page.screenshot({
        path: `e2e-results/ekranlar/${screen.name}${theme === 'dark' ? 'Koyu' : ''}.png`,
        fullPage: true,
      });
      expect(errors).toEqual([]);
    });
  }
}
