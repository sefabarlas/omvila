import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import { calendarDemo, demoSunday, overviewDemo } from './calendar-demo';
import { cashDemo, checkoutDemo } from './cash-demo';
import { customersDemo, demoCustomerId } from './customers-demo';
import { newAppointmentDemo } from './new-appointment-demo';
import { queuesDemo } from './queues-demo';
import { depositSetup } from './support';

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
  /** Tablet kırılımları (web/salon-panel/AGENTS.md: 1199 → ikon şeridi, 899 → tek sütun); varsayılan 1440×900. */
  viewport?: { width: number; height: number };
}

const dayCalendar = (page: Page) =>
  expect(page.getByRole('region', { name: 'Günlük takvim' })).toBeVisible();
const customerTable = (page: Page) =>
  expect(page.getByRole('table', { name: 'Müşteriler' })).toBeVisible();

const screens: ScreenCase[] = [
  {
    path: '/',
    name: 'GenelBakis',
    heading: 'İyi günler, Elif',
    setup: overviewDemo,
    act: async (page) => {
      await expect(page.getByText('2 online talep onay bekliyor')).toBeVisible();
      await expect(page.getByText('Gizem Arslan kapora bekliyor')).toBeVisible();
    },
  },
  { path: '/hizmetler', name: 'Hizmetler', heading: 'Hizmetler' },
  { path: '/personel', name: 'Personel', heading: 'Personel' },
  { path: '/ayarlar/isletme', name: 'AyarlarIsletme', heading: 'Ayarlar' },
  { path: '/ayarlar/sube', name: 'AyarlarSube', heading: 'Ayarlar' },
  {
    path: '/ayarlar/randevu-kurallari',
    name: 'Ayarlar',
    heading: 'Ayarlar',
    setup: (page) => depositSetup(page, true),
    act: (page) => expect(page.getByText(/^Kapora IBAN’ı tanımlı · /)).toBeVisible(),
  },
  {
    path: '/ayarlar/randevu-kurallari',
    name: 'AyarlarKaporaYok',
    heading: 'Ayarlar',
    setup: (page) => depositSetup(page, false),
    act: (page) => expect(page.getByRole('button', { name: 'IBAN ekle' })).toBeVisible(),
  },
  {
    path: '/ayarlar/kapora',
    name: 'AyarlarOdeme',
    heading: 'Ayarlar',
    setup: (page) => depositSetup(page),
    act: (page) => expect(page.getByText('OMVILA ödeme almaz.')).toBeVisible(),
  },
  {
    path: '/ayarlar/kapora',
    name: 'AyarlarOdeme1199',
    heading: 'Ayarlar',
    setup: (page) => depositSetup(page),
    act: (page) => expect(page.getByText('OMVILA ödeme almaz.')).toBeVisible(),
    viewport: { width: 1199, height: 900 },
  },
  {
    path: '/ayarlar/kapora',
    name: 'AyarlarOdeme899',
    heading: 'Ayarlar',
    setup: (page) => depositSetup(page),
    act: (page) => expect(page.getByText('OMVILA ödeme almaz.')).toBeVisible(),
    viewport: { width: 899, height: 1200 },
  },
  { path: '/ayarlar/rezervasyon', name: 'AyarlarRezervasyon', heading: 'Ayarlar' },
  {
    path: '/ayarlar/bildirimler',
    name: 'AyarlarBildirim',
    heading: 'Ayarlar',
    act: (page) => expect(page.getByRole('heading', { name: 'Mesaj şablonları' })).toBeVisible(),
  },
  {
    path: '/ayarlar/bildirimler',
    name: 'AyarlarBildirim899',
    heading: 'Ayarlar',
    act: (page) => expect(page.getByRole('heading', { name: 'Mesaj şablonları' })).toBeVisible(),
    viewport: { width: 899, height: 1200 },
  },
  {
    path: '/takvim',
    name: 'Main',
    heading: 'Takvim',
    setup: calendarDemo,
    act: dayCalendar,
  },
  // Tablet tasarımları 1024×768 ve 768×1024; kırılım sınırları (1199, 899) ayrıca denenir.
  {
    path: '/takvim',
    name: 'TabletTakvim',
    heading: 'Takvim',
    setup: calendarDemo,
    act: dayCalendar,
    viewport: { width: 1024, height: 768 },
  },
  {
    path: '/takvim',
    name: 'TabletTakvim1199',
    heading: 'Takvim',
    setup: calendarDemo,
    act: dayCalendar,
    viewport: { width: 1199, height: 900 },
  },
  {
    path: '/takvim',
    name: 'TabletDikeyTakvim',
    heading: 'Takvim',
    setup: calendarDemo,
    act: dayCalendar,
    viewport: { width: 768, height: 1024 },
  },
  {
    path: '/takvim',
    name: 'TabletDikeyTakvim899',
    heading: 'Takvim',
    setup: calendarDemo,
    act: dayCalendar,
    viewport: { width: 899, height: 1100 },
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
    path: '/takvim',
    name: 'YeniRandevu',
    heading: 'Takvim',
    setup: newAppointmentDemo,
    act: async (page) => {
      await page.getByRole('button', { name: 'Randevu ekle', exact: true }).click();
      const dialog = page.getByRole('dialog', { name: 'Yeni randevu' });
      await dialog.getByRole('searchbox', { name: 'Müşteri ara' }).fill('Derya');
      await dialog.getByRole('button', { name: /Derya Şahin · 0532 760 21 18/ }).click();
      await dialog.getByRole('button', { name: 'Saç kesimi · 45 dk' }).click();
      await dialog.getByRole('radio', { name: '14:30' }).check();
      await expect(dialog.getByText('Çar 25 Kas · 14:30–15:15')).toBeVisible();
      await page.waitForFunction(() =>
        document.getAnimations().every((animation) => animation.playState !== 'running'),
      );
    },
  },
  {
    path: '/takvim',
    name: 'OdemeAl',
    heading: 'Takvim',
    setup: checkoutDemo,
    act: async (page) => {
      await page.getByRole('button', { name: /^Selin Öztürk/ }).click();
      await page.getByRole('button', { name: 'Tamamla ve ödeme al' }).click();
      const dialog = page.getByRole('dialog', { name: 'Tamamla ve ödeme al' });
      await dialog.getByText('Kart', { exact: true }).click();
      await expect(dialog.getByText('Kapora (havale, 11 Kas)')).toBeVisible();
      await page.waitForFunction(() =>
        document.getAnimations().every((animation) => animation.playState !== 'running'),
      );
    },
  },
  {
    path: '/kasa',
    name: 'Kasa',
    heading: 'Kasa',
    setup: cashDemo,
    act: async (page) => {
      await expect(page.getByRole('table', { name: 'Günün tahsilatları' })).toBeVisible();
      await page.getByLabel('Sayılan nakit').fill('3.400');
    },
  },
  {
    path: '/kasa',
    name: 'Kasa1199',
    heading: 'Kasa',
    setup: cashDemo,
    act: (page) => expect(page.getByRole('table', { name: 'Günün tahsilatları' })).toBeVisible(),
    viewport: { width: 1199, height: 900 },
  },
  {
    path: '/kasa',
    name: 'Kasa899',
    heading: 'Kasa',
    setup: cashDemo,
    act: (page) => expect(page.getByRole('table', { name: 'Günün tahsilatları' })).toBeVisible(),
    viewport: { width: 899, height: 1200 },
  },
  {
    path: '/kasa',
    name: 'KasaBos',
    heading: 'Kasa',
    setup: (page) => cashDemo(page, { empty: true }),
    act: (page) =>
      expect(page.getByRole('heading', { name: 'Bugün henüz tahsilat yok' })).toBeVisible(),
  },
  {
    path: '/musteriler',
    name: 'Musteriler',
    heading: 'Müşteriler',
    setup: customersDemo,
    act: customerTable,
  },
  {
    path: '/musteriler',
    name: 'TabletDikeyMusteriler',
    heading: 'Müşteriler',
    setup: customersDemo,
    act: customerTable,
    viewport: { width: 768, height: 1024 },
  },
  {
    path: '/musteriler',
    name: 'TabletDikeyMusteriler899',
    heading: 'Müşteriler',
    setup: customersDemo,
    act: customerTable,
    viewport: { width: 899, height: 1100 },
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
  {
    path: '/bekleme',
    name: 'BeklemeListesi',
    heading: 'Bekleme listesi',
    setup: queuesDemo,
    act: (page) => expect(page.getByText('Perşembe 11:00 boşaldı.')).toBeVisible(),
  },
  {
    path: '/bekleme',
    name: 'BeklemeListesiBos',
    heading: 'Bekleme listesi',
    setup: (page) => queuesDemo(page, { empty: true }),
    act: (page) => expect(page.getByRole('heading', { name: 'Bekleme listesi boş' })).toBeVisible(),
  },
  {
    path: '/takvim/kapida-sira',
    name: 'KapidaSira',
    heading: 'Kapıda sıra',
    setup: queuesDemo,
    act: (page) => expect(page.getByRole('table', { name: 'Sıradakiler' })).toBeVisible(),
  },
  {
    path: '/takvim/kapida-sira',
    name: 'KapidaSiraBos',
    heading: 'Kapıda sıra',
    setup: (page) => queuesDemo(page, { empty: true }),
    act: (page) => expect(page.getByRole('heading', { name: 'Sırada kimse yok' })).toBeVisible(),
  },
];

for (const theme of ['light', 'dark'] as const) {
  for (const screen of screens) {
    test(`${screen.name} (${theme === 'light' ? 'açık' : 'koyu'})`, async ({ page }) => {
      const errors: string[] = [];
      page.on('pageerror', (error) => errors.push(error.message));
      if (screen.viewport) await page.setViewportSize(screen.viewport);
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
