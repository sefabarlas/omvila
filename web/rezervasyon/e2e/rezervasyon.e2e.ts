import { expect, test } from '@playwright/test';
import { bookingApi } from './support';

// Kritik akış (test-stratejisi.md): üyeliksiz online randevu, hizmetten başarı ekranına; yenileme ve geri tuşu
// seçimi korur.
test('salon sayfasından randevu alınır', async ({ page }) => {
  const errors: string[] = [];
  page.on('pageerror', (error) => errors.push(error.message));
  await bookingApi(page);
  await page.goto('/kuafor-elif');

  await expect(page.getByRole('heading', { level: 1, name: 'Kuaför Elif' })).toBeVisible();
  await page.getByText('Saç kesimi', { exact: true }).click();
  await page.getByText('Fön', { exact: true }).click();
  await expect(page.getByRole('main').getByText('2 hizmet · 1 sa 15 dk')).toBeVisible();
  await page.getByRole('button', { name: 'Devam et' }).click();

  await page.getByRole('radio', { name: /Elif Kaya/ }).click();
  await page.getByRole('button', { name: 'Devam et' }).click();
  await expect(page.getByRole('heading', { name: 'Gün ve saat' })).toBeVisible();

  // Yenileme adımı ve seçimi korur; geri tuşu personel adımına döner.
  await page.reload();
  await expect(page.getByRole('heading', { name: 'Gün ve saat' })).toBeVisible();
  await page.getByRole('button', { name: 'Geri' }).click();
  await expect(page.getByRole('radio', { name: /Elif Kaya/ })).toHaveAttribute(
    'aria-checked',
    'true',
  );
  await page.getByRole('button', { name: 'Devam et' }).click();

  await page.getByRole('radio', { name: '14:30' }).click();
  await expect(page.getByRole('main').getByText('Sal 24 Kas · 14:30–15:45')).toBeVisible();
  await page.getByRole('button', { name: 'Devam et' }).click();

  await page.getByLabel('Ad soyad').fill('Ayşe Yılmaz');
  await page.getByLabel('Telefon numaranız').fill('532 123 45 67');
  await page.getByText(/okudum; randevu için/).click();
  await page.getByRole('button', { name: 'Doğrulama kodu gönder' }).click();

  await page.getByRole('textbox', { name: 'Doğrulama kodu, hane 1' }).fill('482193');
  await expect(page.getByRole('heading', { name: 'Son kontrol' })).toBeVisible();
  await page.getByRole('button', { name: 'Randevuyu oluştur' }).click();

  await expect(page.getByRole('heading', { name: 'Randevunuz onaylandı' })).toBeVisible();
  await expect(page).toHaveURL(/\/kuafor-elif$/);
  expect(errors).toEqual([]);
});
