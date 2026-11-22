import { expect, test } from '@playwright/test';

// Menü, bölüme kaydırma ve salon paneli bağlantıları (web/tanitim/AGENTS.md § Kurallar).
test('masaüstü menüsü bölüme kaydırır ve panel adreslerine gider', async ({ page }) => {
  await page.goto('/');
  const menu = page.getByRole('navigation', { name: 'Ana menü' });
  await menu.getByRole('link', { name: 'Nasıl çalışır' }).click();
  await expect(page).toHaveURL(/#nasil-calisir$/);
  await expect(
    page.getByRole('heading', { name: 'Üç adımda ilk online randevu' }),
  ).toBeInViewport();

  await expect(page.getByRole('banner').getByRole('link', { name: 'Giriş yap' })).toHaveAttribute(
    'href',
    'https://salon.example.test/giris',
  );
  await expect(
    page.getByRole('banner').getByRole('link', { name: 'Ücretsiz dene' }),
  ).toHaveAttribute('href', 'https://salon.example.test/kayit');

  await menu.getByRole('link', { name: 'Sık sorulanlar' }).click();
  await expect(page).toHaveURL(/\/fiyatlar\/#sss$/);
  await expect(page.getByRole('heading', { name: 'Deneme bitince ne olur?' })).toBeVisible();
});

test('mobilde menü butonu açılır listeyi gösterir', async ({ page }) => {
  await page.setViewportSize({ width: 390, height: 844 });
  await page.goto('/');
  await expect(page.getByRole('navigation', { name: 'Ana menü' })).toBeHidden();
  await page.getByRole('button', { name: 'Menüyü aç' }).click();
  const menu = page.getByRole('navigation', { name: 'Menü', exact: true });
  await menu.getByRole('link', { name: 'Fiyatlar' }).click();
  await expect(
    page.getByRole('heading', { level: 1, name: 'İşletmenize göre plan' }),
  ).toBeVisible();
  await expect(menu).toBeHidden();
});

test('yasal bağlantılar yer tutucu sayfaya gider', async ({ page }) => {
  await page.goto('/');
  await page.getByRole('contentinfo').getByRole('link', { name: 'Kullanım koşulları' }).click();
  await expect(page.getByRole('heading', { level: 1, name: 'Kullanım koşulları' })).toBeVisible();
  await expect(page.getByText('Bu sayfa hazırlanıyor.')).toBeVisible();
});
