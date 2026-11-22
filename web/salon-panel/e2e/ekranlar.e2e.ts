import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import { rolesDemo, staffDemo } from './access-demo';
import { calendarDemo, demoSunday, overviewDemo } from './calendar-demo';
import { cashDemo, checkoutDemo } from './cash-demo';
import { customersDemo, demoCustomerId } from './customers-demo';
import { newAppointmentDemo } from './new-appointment-demo';
import { packagesDemo } from './packages-demo';
import { queuesDemo } from './queues-demo';
import { myCommissionDemo, reportsDemo } from './reports-demo';
import { reviewsDemo } from './reviews-demo';
import { subscriptionDemo, subscriptionSession } from './subscription-demo';
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
const packageScreen = (page: Page) =>
  expect(
    page.getByRole('region', { name: 'Süresi yaklaşanlar' }).getByText('Melis Tunç'),
  ).toBeVisible();
const kvkkScreen = async (page: Page) => {
  await expect(page.getByText('OMVILA standart aydınlatma metni')).toBeVisible();
  await expect(page.getByRole('table', { name: 'Silme talepleri' })).toBeVisible();
};
const rolesScreen = async (page: Page) => {
  await expect(
    page.getByRole('table', { name: 'Kullanıcılar' }).getByText('Deniz Kara'),
  ).toBeVisible();
  await expect(page.getByRole('table', { name: 'Rol izinleri' })).toBeVisible();
};
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
      await expect(
        page.getByRole('region', { name: 'Bu hafta doluluk' }).getByText('Zeynep Aksoy'),
      ).toBeVisible();
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
    path: '/paketler',
    name: 'Paketler',
    heading: 'Paketler',
    setup: packagesDemo,
    act: packageScreen,
  },
  {
    path: '/paketler',
    name: 'Paketler1199',
    heading: 'Paketler',
    setup: packagesDemo,
    act: packageScreen,
    viewport: { width: 1199, height: 900 },
  },
  {
    path: '/paketler',
    name: 'Paketler899',
    heading: 'Paketler',
    setup: packagesDemo,
    act: packageScreen,
    viewport: { width: 899, height: 1400 },
  },
  {
    path: '/paketler',
    name: 'PaketlerBos',
    heading: 'Paketler',
    setup: (page) => packagesDemo(page, { empty: true }),
    act: (page) =>
      expect(page.getByRole('heading', { name: 'Henüz paket tanımı yok' })).toBeVisible(),
  },
  {
    path: '/paketler',
    name: 'PaketOlustur',
    heading: 'Paketler',
    setup: packagesDemo,
    act: async (page) => {
      await packageScreen(page);
      await page.getByRole('button', { name: 'Paket oluştur' }).click();
      const dialog = page.getByRole('dialog', { name: 'Paket oluştur' });
      await dialog.getByLabel('Paket adı').fill('Manikür · 6 seans');
      await dialog.getByLabel('Fiyat').fill('1.700');
      await dialog.getByLabel('1. hizmet', { exact: true }).selectOption({ label: 'Manikür' });
      await dialog.getByLabel('Manikür seans sayısı').fill('6');
      await dialog.getByRole('button', { name: 'Hizmet ekle' }).click();
      await dialog.getByLabel('2. hizmet', { exact: true }).selectOption({ label: 'Kalıcı oje' });
      await dialog.getByLabel('Kalıcı oje seans sayısı').fill('2');
      await dialog.getByLabel('Satış primi').fill('10');
      await settle(page);
    },
  },
  {
    path: '/paketler',
    name: 'PaketSat',
    heading: 'Paketler',
    setup: packagesDemo,
    act: async (page) => {
      await packageScreen(page);
      await page.getByRole('button', { name: 'Müşteriye paket sat' }).click();
      const dialog = page.getByRole('dialog', { name: 'Müşteriye paket sat' });
      await dialog.getByRole('searchbox', { name: 'Müşteri ara' }).fill('Melis');
      await dialog.getByRole('button', { name: /Melis Tunç/ }).click();
      await dialog.getByLabel('Paket').selectOption({ label: 'Manikür' });
      await expect(
        dialog.getByRole('button', { name: '1.700 ₺ al ve paketi tanımla' }),
      ).toBeVisible();
      await settle(page);
    },
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
  // Dilim 5 (T-99): değerlendirmeler, müşteri kartı sekmeleri ve kayıt işlemleri, CSV içe aktarma, KVKK.
  {
    path: '/degerlendirmeler',
    name: 'Degerlendirmeler',
    heading: 'Değerlendirmeler',
    setup: reviewsDemo,
    act: (page) =>
      expect(page.getByRole('article', { name: 'Ece Koç değerlendirmesi' })).toBeVisible(),
  },
  {
    path: '/degerlendirmeler?filtre=tumu',
    name: 'DegerlendirmelerTumu',
    heading: 'Değerlendirmeler',
    setup: reviewsDemo,
    act: (page) => expect(page.getByText('Yanıtınız · Elif Kaya')).toBeVisible(),
  },
  {
    path: '/degerlendirmeler',
    name: 'Degerlendirmeler899',
    heading: 'Değerlendirmeler',
    setup: reviewsDemo,
    act: (page) =>
      expect(page.getByRole('article', { name: 'Ece Koç değerlendirmesi' })).toBeVisible(),
    viewport: { width: 899, height: 1200 },
  },
  {
    path: '/degerlendirmeler',
    name: 'DegerlendirmelerBos',
    heading: 'Değerlendirmeler',
    setup: (page) => reviewsDemo(page, { empty: true }),
    act: (page) =>
      expect(page.getByRole('heading', { name: 'Henüz değerlendirme yok' })).toBeVisible(),
  },
  {
    path: `/musteriler/${demoCustomerId}`,
    name: 'MusteriKartiPaketler',
    heading: 'Selin Öztürk',
    setup: customersDemo,
    act: async (page) => {
      await expect(page.getByText('seans kaldı · 31 Ara')).toBeVisible();
      await page.getByRole('tab', { name: 'Paketler' }).click();
      await expect(page.getByRole('table', { name: 'Paketler' })).toBeVisible();
    },
  },
  {
    path: `/musteriler/${demoCustomerId}`,
    name: 'MusteriKartiOdemeler',
    heading: 'Selin Öztürk',
    setup: customersDemo,
    act: async (page) => {
      await page.getByRole('tab', { name: 'Ödemeler' }).click();
      await expect(page.getByRole('table', { name: 'Ödemeler' })).toBeVisible();
    },
  },
  {
    path: `/musteriler/${demoCustomerId}`,
    name: 'MusteriKartiDegerlendirmeler',
    heading: 'Selin Öztürk',
    setup: customersDemo,
    act: async (page) => {
      await page.getByRole('tab', { name: 'Değerlendirmeler' }).click();
      await expect(
        page.getByRole('article', { name: 'Selin Öztürk değerlendirmesi' }),
      ).toBeVisible();
    },
  },
  {
    path: `/musteriler/${demoCustomerId}`,
    name: 'MusteriBirlestir',
    heading: 'Selin Öztürk',
    setup: customersDemo,
    act: async (page) => {
      await page.getByRole('button', { name: 'Başka kayıtla birleştir' }).click();
      const dialog = page.getByRole('dialog', { name: 'Kayıtları birleştir' });
      await dialog.getByRole('searchbox', { name: 'Birleşecek kaydı ara' }).fill('Derya');
      await dialog.getByRole('button', { name: /Derya Şahin/ }).click();
      await expect(dialog.getByRole('region', { name: 'Birleşecek kayıt' })).toContainText('6');
      await settle(page);
    },
  },
  {
    path: `/musteriler/${demoCustomerId}`,
    name: 'MusteriSil',
    heading: 'Selin Öztürk',
    setup: customersDemo,
    act: async (page) => {
      await page.getByRole('button', { name: 'KVKK silme talebi' }).click();
      const dialog = page.getByRole('dialog', { name: 'KVKK silme talebi' });
      await expect(dialog.getByText('12 randevu, 9 ödeme')).toBeVisible();
      await settle(page);
    },
  },
  {
    path: '/musteriler',
    name: 'MusteriIceAktar',
    heading: 'Müşteriler',
    setup: customersDemo,
    act: async (page) => {
      await page.getByRole('button', { name: 'İçe aktar' }).click();
      const dialog = page.getByRole('dialog', { name: 'Müşterileri içe aktar' });
      await dialog.getByLabel('CSV dosyası').setInputFiles({
        name: 'musteriler-eski-program.csv',
        mimeType: 'text/csv',
        buffer: Buffer.from('Ad;Telefon\n'),
      });
      await expect(dialog.getByRole('button', { name: '1.242 kaydı içe aktar' })).toBeVisible();
      await settle(page);
    },
  },
  {
    path: '/raporlar',
    name: 'Raporlar',
    heading: 'Raporlar ve prim',
    setup: (page) => reportsDemo(page),
    act: (page) =>
      expect(
        page.getByRole('region', { name: 'Personel ve prim' }).getByText('15.665 ₺'),
      ).toBeVisible(),
  },
  {
    path: '/raporlar',
    name: 'Raporlar1199',
    heading: 'Raporlar ve prim',
    setup: (page) => reportsDemo(page),
    act: (page) =>
      expect(
        page.getByRole('region', { name: 'Personel ve prim' }).getByText('15.665 ₺'),
      ).toBeVisible(),
    viewport: { width: 1199, height: 900 },
  },
  {
    path: '/raporlar',
    name: 'Raporlar899',
    heading: 'Raporlar ve prim',
    setup: (page) => reportsDemo(page),
    act: (page) =>
      expect(
        page.getByRole('region', { name: 'Personel ve prim' }).getByText('15.665 ₺'),
      ).toBeVisible(),
    viewport: { width: 899, height: 1100 },
  },
  {
    path: '/raporlar',
    name: 'RaporlarBos',
    heading: 'Raporlar ve prim',
    setup: (page) => reportsDemo(page, true),
    act: (page) =>
      expect(page.getByRole('heading', { name: 'Rapor için henüz veri yok' })).toBeVisible(),
  },
  {
    path: '/primim',
    name: 'Primim',
    heading: 'Primim',
    setup: myCommissionDemo,
    act: (page) =>
      expect(
        page.getByRole('region', { name: 'Son primler' }).getByText('Buse Kaya'),
      ).toBeVisible(),
  },
  {
    path: '/ayarlar/kvkk',
    name: 'AyarlarKvkk',
    heading: 'Ayarlar',
    setup: customersDemo,
    act: kvkkScreen,
  },
  {
    path: '/ayarlar/kvkk',
    name: 'AyarlarKvkk899',
    heading: 'Ayarlar',
    setup: customersDemo,
    act: kvkkScreen,
    viewport: { width: 899, height: 1100 },
  },
  {
    path: '/ayarlar/roller',
    name: 'AyarlarRoller',
    heading: 'Ayarlar',
    setup: rolesDemo,
    act: rolesScreen,
  },
  {
    path: '/ayarlar/roller',
    name: 'AyarlarRoller899',
    heading: 'Ayarlar',
    setup: rolesDemo,
    act: rolesScreen,
    viewport: { width: 899, height: 1100 },
  },
  {
    path: '/personel',
    name: 'PersonelGoogle',
    heading: 'Personel',
    setup: staffDemo,
    act: (page) => expect(page.getByText('Google Takvim · bağlı')).toBeVisible(),
  },
  {
    path: '/ayarlar/abonelik',
    name: 'AyarlarAbonelik',
    heading: 'Ayarlar',
    setup: (page) => subscriptionDemo(page),
    act: (page) =>
      expect(
        page.getByRole('region', { name: 'Faturalar' }).getByText('OMV-2026-000121'),
      ).toBeVisible(),
  },
  {
    path: '/ayarlar/abonelik',
    name: 'AyarlarAbonelikTanimsiz',
    heading: 'Ayarlar',
    setup: (page) => subscriptionDemo(page, false),
    act: (page) =>
      expect(
        page
          .getByRole('region', { name: 'Havale / EFT ile öde' })
          .getByText(/OMVILA hesap bilgisi henüz tanımlı değil/),
      ).toBeVisible(),
  },
  {
    path: '/ayarlar/abonelik',
    name: 'AyarlarAbonelik899',
    heading: 'Ayarlar',
    setup: (page) => subscriptionDemo(page),
    act: (page) =>
      expect(
        page.getByRole('region', { name: 'Faturalar' }).getByText('OMV-2026-000121'),
      ).toBeVisible(),
    viewport: { width: 899, height: 1300 },
  },
  {
    path: '/ayarlar/abonelik',
    name: 'HavaleBildir',
    heading: 'Ayarlar',
    setup: (page) => subscriptionDemo(page),
    act: async (page) => {
      await page.getByRole('button', { name: 'Havaleyi bildir (dekont isteğe bağlı)' }).click();
      await expect(page.getByRole('dialog', { name: 'Havaleyi bildir' })).toBeVisible();
      await settle(page);
    },
  },
  {
    path: '/ayarlar/bildirimler',
    name: 'KontorAl',
    heading: 'Ayarlar',
    setup: (page) => subscriptionDemo(page),
    act: async (page) => {
      await page.getByRole('button', { name: 'Kontör al' }).first().click();
      const dialog = page.getByRole('dialog', { name: 'Kontör al' });
      await expect(dialog.getByText('20.000 SMS')).toBeVisible();
      await settle(page);
    },
  },
  {
    path: '/ayarlar/bildirimler',
    name: 'KontorAlHavale',
    heading: 'Ayarlar',
    setup: (page) => subscriptionDemo(page),
    act: async (page) => {
      await page.getByRole('button', { name: 'Kontör al' }).first().click();
      const dialog = page.getByRole('dialog', { name: 'Kontör al' });
      await dialog.getByRole('radio', { name: /Orta/ }).check();
      await dialog.getByRole('button', { name: 'Havale bilgisini göster' }).click();
      await expect(dialog.getByText('KNT-KE-1125')).toBeVisible();
      await settle(page);
    },
  },
  {
    path: '/',
    name: 'GenelBakisGecikti',
    heading: 'İyi günler, Elif',
    setup: async (page) => {
      await overviewDemo(page);
      await subscriptionSession(page, {
        status: 'PastDue',
        trialEndsOn: null,
        pastDueSince: '2026-11-23',
        readOnlyOn: '2026-11-30',
      });
    },
    act: (page) =>
      expect(page.getByRole('status').filter({ hasText: 'Abonelik ödemesi' })).toContainText(
        '3. gün',
      ),
  },
  {
    path: '/takvim',
    name: 'TakvimSaltOkunur',
    heading: 'Takvim',
    setup: async (page) => {
      await calendarDemo(page);
      await subscriptionSession(page, {
        status: 'ReadOnly',
        trialEndsOn: null,
        pastDueSince: '2026-11-18',
        readOnlyOn: null,
      });
    },
    act: async (page) => {
      await dayCalendar(page);
      await expect(page.getByRole('button', { name: 'Randevu ekle', exact: true })).toBeDisabled();
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
