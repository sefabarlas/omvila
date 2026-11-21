import { expect, test } from '@playwright/test';
import { ids } from '../src/test/booking-fixtures';
import { bookingApi } from './support';

// Kritik akış 2 (test-stratejisi.md): onaysız salonda üyeliksiz online randevu; hizmet → personel ("Fark etmez") →
// saat → telefon → kod → onay → başarı. Yenileme ve geri tuşu seçimi korur; "Fark etmez"de atanan kişi onayda
// görünür ve o kişi gönderilir (dilim 2 kriter 6).
test('Kritik akış 2: hizmet → personel → saat → telefon → kod → onay → başarı (onaysız salon)', async ({
  page,
}) => {
  const errors: string[] = [];
  page.on('pageerror', (error) => errors.push(error.message));
  const calls = await bookingApi(page);
  await page.goto('/kuafor-elif');

  // Hizmet
  await expect(page.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeVisible();
  await page.getByText('Saç kesimi', { exact: true }).click();
  await page.getByText('Fön', { exact: true }).click();
  await expect(page.getByRole('main').getByText('2 hizmet · 1 sa 15 dk')).toBeVisible();
  await page.getByRole('button', { name: 'Devam et' }).click();

  // Personel: ilk seçenek "Fark etmez"
  const anyStaff = page.getByRole('radio', { name: /Fark etmez/ });
  await expect(page.getByRole('radio').first()).toHaveAccessibleName(/Fark etmez/);
  await anyStaff.click();
  await page.getByRole('button', { name: 'Devam et' }).click();
  await expect(page.getByRole('heading', { name: 'Gün ve saat' })).toBeVisible();

  // Yenileme adımı ve seçimi korur; geri tuşu personel adımına döner.
  await page.reload();
  await expect(page.getByRole('heading', { name: 'Gün ve saat' })).toBeVisible();
  await page.getByRole('button', { name: 'Geri' }).click();
  await expect(anyStaff).toHaveAttribute('aria-checked', 'true');
  await page.getByRole('button', { name: 'Devam et' }).click();

  // Saat
  await page.getByRole('radio', { name: '14:30' }).click();
  await expect(page.getByRole('main').getByText('Sal 24 Kas · 14:30–15:45')).toBeVisible();
  await page.getByRole('button', { name: 'Devam et' }).click();

  // Telefon
  await page.getByLabel('Ad soyad').fill('Ayşe Yılmaz');
  await page.getByLabel('Telefon numaranız').fill('532 123 45 67');
  await page.getByText(/okudum; randevu için/).click();
  await page.getByRole('button', { name: 'Doğrulama kodu gönder' }).click();

  // Kod
  await page.getByRole('textbox', { name: 'Doğrulama kodu, hane 1' }).fill('482193');

  // Onay: "Fark etmez"de atanan kişinin adı
  await expect(page.getByRole('heading', { name: 'Son kontrol' })).toBeVisible();
  const summary = page.getByRole('region', { name: 'Randevu özeti' });
  await expect(summary.getByText('Saç kesimi · Zeynep Aksoy')).toBeVisible();
  await expect(summary.getByText('Fön · Zeynep Aksoy')).toBeVisible();
  await page.getByRole('button', { name: 'Randevuyu oluştur' }).click();

  // Başarı
  await expect(page.getByRole('heading', { name: 'Randevunuz onaylandı' })).toBeVisible();
  await expect(page).toHaveURL(/\/kuafor-elif$/);
  expect(calls.create).toHaveLength(1);
  expect(calls.create[0]).toMatchObject({
    items: [
      { serviceId: ids.sac, staffId: ids.zeynep },
      { serviceId: ids.fon, staffId: ids.zeynep },
    ],
  });
  expect(errors).toEqual([]);
});

test('çok şubeli salonda önce şube seçilir', async ({ page }) => {
  await bookingApi(page);
  await page.goto('/kuafor-elif-subeler');
  await expect(
    page.getByRole('heading', { name: 'Hangi şubeye gelmek istersiniz?' }),
  ).toBeVisible();
  await page.getByRole('button', { name: /^Kadıköy/ }).click();
  await expect(page.getByText('Saç kesimi', { exact: true })).toBeVisible();
  await expect(page).toHaveURL(/branch=/);
});
