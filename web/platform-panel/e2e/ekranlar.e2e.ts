import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import {
  auditDemo,
  bankDemo,
  elifNoticeId,
  emptyQueueDemo,
  noticeDetailDemo,
  plansDemo,
  queueDemo,
  subscriptionsDemo,
} from './approvals-demo';
import { brandDetailDemo, brandsDemo, demoBrandId, overviewDemo } from './platform-demo';
import { asAdmin, signedIn } from './support';

// Görsel karşılaştırma (test-stratejisi.md): ekranların açık ve koyu görüntüleri e2e-results/ekranlar altına yazılır;
// tasarım görüntüleriyle (design/ekranlar/platform-paneli) yan yana insan gözüyle karşılaştırılır. Otomatik piksel
// karşılaştırması yok. Prism oturum örneği Destek rolüdür (tasarımdaki gibi kilitli butonlar); Yönetici ayrıca çekilir.
interface ScreenCase {
  path: string;
  name: string;
  heading: string;
  setup?: (page: Page) => Promise<void>;
  act?: (page: Page) => Promise<void>;
  viewport?: { width: number; height: number };
}

const screens: ScreenCase[] = [
  {
    path: '/',
    name: 'Main',
    heading: 'Platform',
    setup: overviewDemo,
    act: (page) => expect(page.getByText('Dyt. Ece Arslan')).toBeVisible(),
  },
  {
    path: '/',
    name: 'Main1199',
    heading: 'Platform',
    setup: overviewDemo,
    act: (page) => expect(page.getByText('Dyt. Ece Arslan')).toBeVisible(),
    viewport: { width: 1199, height: 900 },
  },
  {
    path: '/',
    name: 'Main899',
    heading: 'Platform',
    setup: overviewDemo,
    act: (page) => expect(page.getByText('Dyt. Ece Arslan')).toBeVisible(),
    viewport: { width: 899, height: 1200 },
  },
  {
    path: '/markalar',
    name: 'PMarkalar',
    heading: 'Markalar',
    setup: brandsDemo,
    act: (page) => expect(page.getByText('Saç Atölyesi')).toBeVisible(),
  },
  {
    path: `/markalar/${demoBrandId}`,
    name: 'PMarkaDetay',
    heading: 'Kuaför Elif',
    setup: brandDetailDemo,
    act: async (page) => {
      await expect(page.getByText('Bu görüntüleme denetim kaydına yazıldı.')).toBeVisible();
      await expect(page.getByRole('button', { name: 'Kontör ekle' })).toBeDisabled();
      await expect(page.getByRole('button', { name: 'Planı değiştir' })).toBeDisabled();
    },
  },
  {
    path: `/markalar/${demoBrandId}`,
    name: 'PMarkaDetayYonetici',
    heading: 'Kuaför Elif',
    setup: async (page) => {
      await asAdmin(page);
      await brandDetailDemo(page);
    },
    act: async (page) => {
      await expect(page.getByRole('button', { name: 'Kontör ekle' })).toBeEnabled();
      await expect(page.getByText(/yönetici yetkisi ister/)).toHaveCount(0);
    },
  },
  {
    path: `/markalar/${demoBrandId}`,
    name: 'PMarkaDetayKontorEkle',
    heading: 'Kuaför Elif',
    setup: async (page) => {
      await asAdmin(page);
      await brandDetailDemo(page);
    },
    act: async (page) => {
      await page.getByRole('button', { name: 'Kontör ekle' }).click();
      await expect(page.getByRole('dialog', { name: 'Kontör ekle' })).toBeVisible();
      await settle(page);
    },
  },
  {
    path: `/markalar/${demoBrandId}?sekme=kontor`,
    name: 'PMarkaDetayKontor',
    heading: 'Kuaför Elif',
    act: (page) => expect(page.getByRole('table', { name: 'Kontör hareketleri' })).toBeVisible(),
  },
  {
    path: '/onay',
    name: 'POnay',
    heading: 'Onay kuyrukları',
    setup: queueDemo,
    act: (page) => expect(page.getByText('Bankada eşleşen hareket var')).toHaveCount(2),
  },
  {
    path: '/onay?sekme=sms',
    name: 'POnaySms',
    heading: 'Onay kuyrukları',
    setup: queueDemo,
    act: (page) => expect(page.getByText(/NAILSTUDIO · şu an OMVILA/)).toBeVisible(),
  },
  {
    path: '/onay',
    name: 'POnayBos',
    heading: 'Onay kuyrukları',
    setup: emptyQueueDemo,
    act: (page) => expect(page.getByRole('heading', { name: 'Bekleyen onay yok' })).toBeVisible(),
  },
  {
    path: `/onay/havale/${elifNoticeId}`,
    name: 'POnayDetay',
    heading: 'Kuaför Elif · havale bildirimi',
    setup: noticeDetailDemo,
    act: (page) => expect(page.getByRole('radio', { name: /ELİF KAYA/ })).toBeChecked(),
  },
  {
    path: `/onay/havale/${elifNoticeId}`,
    name: 'POnayDetayKontrol',
    heading: 'Kuaför Elif · havale bildirimi',
    setup: noticeDetailDemo,
    act: async (page) => {
      await page.getByRole('radio', { name: /KAAN ÖZTÜRK/ }).check();
      await expect(page.getByText('Kontrol edin:')).toBeVisible();
    },
  },
  {
    path: '/onay/banka-hareketleri',
    name: 'PBankaHareketleri',
    heading: 'Banka hareketleri',
    setup: bankDemo,
    act: (page) => expect(page.getByText('Eşleşti · Spa Moda')).toBeVisible(),
  },
  {
    path: '/abonelikler',
    name: 'PAbonelikler',
    heading: 'Abonelikler',
    setup: subscriptionsDemo,
    act: (page) => expect(page.getByText('Aktif · kontör bitti')).toBeVisible(),
  },
  {
    path: '/denetim',
    name: 'PDenetim',
    heading: 'Denetim kaydı',
    setup: auditDemo,
    act: async (page) => {
      await expect(page.getByText('KVKK silme tamamlandı')).toBeVisible();
      await expect(page.getByRole('button', { name: 'Dışa aktar' })).toBeDisabled();
    },
  },
  {
    path: '/planlar',
    name: 'PPlanlar',
    heading: 'Planlar ve fiyat',
    setup: plansDemo,
    act: async (page) => {
      await expect(page.getByText('241 marka')).toBeVisible();
      await expect(page.getByRole('button', { name: 'Planı düzenle' })).toBeDisabled();
    },
  },
  // T-129: kenar menüdeki kullanıcı satırından açılan profil menüsü (kimlik, tema, "Oturumu kapat").
  {
    path: '/',
    name: 'ProfilMenu',
    heading: 'Platform',
    setup: overviewDemo,
    act: async (page) => {
      await page.getByRole('button', { name: 'Profil menüsü' }).click();
      const menu = page.getByRole('menu', { name: 'Profil menüsü' });
      await expect(menu.getByRole('menuitem', { name: 'Oturumu kapat' })).toBeVisible();
      await settle(page);
    },
  },
];

/** Pencerenin açılış hareketi bitsin. */
async function settle(page: Page) {
  await page.waitForFunction(() =>
    document.getAnimations().every((animation) => animation.playState !== 'running'),
  );
}

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
      await signedIn(page);
      await screen.setup?.(page);
      await page.goto(screen.path);
      await expect(
        page.getByRole('heading', { level: 1, name: screen.heading, exact: true }),
      ).toBeVisible();
      await screen.act?.(page);
      await page.screenshot({
        path: `e2e-results/ekranlar/${screen.name}${theme === 'dark' ? 'Koyu' : ''}.png`,
        fullPage: true,
      });
      expect(errors).toEqual([]);
    });
  }
}
