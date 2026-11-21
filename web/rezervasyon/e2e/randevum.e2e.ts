import { expect, test } from '@playwright/test';
import type { Page } from '@playwright/test';
import type { PublicAppointment } from '../src/appointment/appointment-api';
import { accessCode, etag, requestCancelledFixture } from '../src/test/appointment-fixtures';
import { appointmentApi } from './support';

// SMS bağlantısı (/r/{kod}): Randevum, saat değiştir ve iptal. Ekran görüntüleri e2e-results/ekranlar altına
// (design/ekranlar/rezervasyon-sayfasi/Randevum, Ertele, Iptal ile yan yana). Kritik akış 6: iptal (neden zorunlu) ve
// saat değiştirme (son süre). "Şimdi" 24 Kasım 10:00; randevu 25 Kasım 14:30 (iptal 24, saat değişikliği 12 saat öncesine
// kadar).

interface ScreenCase {
  name: string;
  path: string;
  /** İlk okumanın alanları (ör. onay bekleyen talep). */
  appointment?: Partial<PublicAppointment>;
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
    // Dilim 3: Randevum'da "Onay bekliyor" (tasarımda ayrı görüntü yok; Randevum.png düzeni).
    name: 'RandevumOnayBekliyor',
    path: `/r/${accessCode}`,
    appointment: {
      status: 'PendingApproval',
      displayStatus: 'PendingApproval',
      holdExpiresUtc: '2026-11-24T08:35:00Z',
    },
    ready: async (page) => {
      await expect(page.getByText('Randevunuz · Onay bekliyor')).toBeVisible();
      await expect(page.getByRole('heading', { name: 'Salon onayı bekleniyor' })).toBeVisible();
    },
  },
  {
    // Dilim 3: salonun onaylayamadığı talep müşteriye "İptal" (kriter 3; tasarımda ayrı görüntü yok).
    name: 'RandevumTalepIptal',
    path: `/r/${accessCode}`,
    appointment: requestCancelledFixture('DeclinedBySalon'),
    ready: async (page) => {
      await expect(page.getByText('Randevunuz · İptal')).toBeVisible();
      await expect(page.getByRole('heading', { name: 'Talebiniz iptal edildi' })).toBeVisible();
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
      await appointmentApi(page, screen.appointment);
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

test.describe('Kritik akış 6: SMS bağlantısından iptal ve saat değiştirme', () => {
  test('iptal: neden seçilmeden gönderilmez; nedenle iptal edilir', async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    const calls = await appointmentApi(page);
    await page.goto(`/r/${accessCode}`);

    await expect(page.getByRole('heading', { level: 1, name: 'Çar 25 Kas · 14:30' })).toBeVisible();
    await page.getByRole('button', { name: 'Randevuyu iptal et' }).click();
    const dialog = page.getByRole('dialog', { name: 'Randevu iptal edilsin mi?' });
    await expect(dialog).toBeVisible();
    await expect(page).toHaveURL(/\?islem=iptal$/);

    // Neden zorunlu: seçilmeden istek gitmez.
    await dialog.getByRole('button', { name: 'İptal et' }).click();
    await expect(dialog.getByText('İptal nedenini seçin.')).toBeVisible();
    expect(calls.cancel).toEqual([]);

    // "Diğer"de açıklama zorunlu.
    await dialog.getByText('Diğer', { exact: true }).click();
    await dialog.getByRole('button', { name: 'İptal et' }).click();
    await expect(dialog.getByText('Kısa bir açıklama yazın.')).toBeVisible();
    expect(calls.cancel).toEqual([]);

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

  test('saat değiştirme: son süreden önce yeni saat seçilir (If-Match)', async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', (error) => errors.push(error.message));
    const calls = await appointmentApi(page);
    await page.goto(`/r/${accessCode}`);

    await page.getByRole('button', { name: 'Saati değiştir' }).click();
    await expect(page.getByRole('heading', { name: 'Yeni saat seçin' })).toBeVisible();
    await expect(page).toHaveURL(/\?islem=saat$/);
    await expect(
      page.getByText('Saat değişikliği randevudan 12 saat öncesine kadar yapılabilir.'),
    ).toBeVisible();
    await expect(page.getByRole('radio', { name: '14:30, dolu' })).toBeVisible();
    await page.getByRole('radio', { name: '15:15' }).click();
    await page.getByRole('button', { name: 'Saati değiştir' }).click();

    await expect(page.getByText('Saatiniz değiştirildi.')).toBeVisible();
    await expect(page.getByRole('heading', { level: 1, name: 'Çar 25 Kas · 15:15' })).toBeVisible();
    expect(calls.reschedule).toEqual([
      { ifMatch: etag, body: { startUtc: '2026-11-25T12:15:00Z' } },
    ]);
    expect(errors).toEqual([]);
  });

  test('son süre geçtiyse saat değiştirme ve iptal yerine "Salonu ara"', async ({ page }) => {
    await appointmentApi(page, {
      canCancel: false,
      canReschedule: false,
      cancelDeadlineUtc: '2026-11-24T05:30:00Z',
      rescheduleDeadlineUtc: '2026-11-24T05:30:00Z',
    });
    // Adres çubuğundaki ?islem=saat de saat ekranını açmaz.
    await page.goto(`/r/${accessCode}?islem=saat`);

    await expect(page.getByRole('heading', { name: 'Plan değişti mi?' })).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Yeni saat seçin' })).toHaveCount(0);
    await expect(page.getByRole('button', { name: 'Saati değiştir' })).toHaveCount(0);
    await expect(page.getByRole('button', { name: 'Randevuyu iptal et' })).toHaveCount(0);
    await expect(
      page.getByText(
        'Online değişiklik süresi geçti. Saati değiştirmek veya iptal etmek için salonu arayın.',
      ),
    ).toBeVisible();
    await expect(page.getByRole('link', { name: 'Salonu ara' })).toHaveAttribute(
      'href',
      'tel:+905321234567',
    );
    await page.screenshot({ path: 'e2e-results/ekranlar/RandevumSureGecti.png', fullPage: true });
  });
});

test('bilinmeyen kod "Bu randevu bulunamadı" gösterir', async ({ page }) => {
  await appointmentApi(page);
  await page.goto('/r/Bilinmeyn1');
  await expect(
    page.getByRole('heading', { level: 1, name: 'Bu randevu bulunamadı' }),
  ).toBeVisible();
  await page.screenshot({ path: 'e2e-results/ekranlar/DurumBulunamadi.png', fullPage: true });
});
