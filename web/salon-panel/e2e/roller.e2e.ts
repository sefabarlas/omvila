import { expect, test } from '@playwright/test';
import { demoUsers, kadikoy, rolesDemo } from './access-demo';
import { captureBody } from './support';

// Dilim 6 uçtan uca: Ayarlar → Roller ve yetkiler'de kullanıcının rolünü değiştirme (K6) ve son işletme sahibi kuralı
// (409 last_owner_required). Kayıt isteği Prism'e gider (sözleşme doğrulaması); 409'u test verir.
const deniz = demoUsers[3];

test('rol değiştirme: personel şube yöneticisi olur, istek If-Match ile gider', async ({
  page,
}) => {
  await rolesDemo(page);
  let ifMatch: string | null = null;
  await page.route(/\/api\/v1\/manage\/users\/[^/]+$/, async (route) => {
    if (route.request().method() === 'PATCH') {
      ifMatch = route.request().headers()['if-match'] ?? null;
    }

    await route.fallback();
  });
  const body = await captureBody(page, `/users/${deniz?.userId ?? ''}`, 'PATCH');
  await page.goto('/ayarlar/roller');
  await page.getByRole('button', { name: 'Deniz Kara: düzenle' }).click();
  const dialog = page.getByRole('dialog', { name: 'Deniz Kara' });
  await dialog.getByRole('combobox', { name: 'Rol' }).selectOption('BranchManager');
  await expect(dialog.getByRole('checkbox', { name: 'Kadıköy' })).toBeChecked();
  await dialog.getByRole('button', { name: 'Kaydet' }).click();
  expect(await body()).toEqual({ role: 'BranchManager', branchIds: [kadikoy] });
  expect(ifMatch).toBe(deniz?.version);
  await expect(page.getByText('Deniz Kara için rol kaydedildi')).toBeVisible();
  await expect(dialog).toBeHidden();
});

test('son işletme sahibi başka role alınamaz', async ({ page }) => {
  await rolesDemo(page);
  await page.route(/\/api\/v1\/manage\/users\/[^/]+$/, (route) =>
    route.request().method() === 'PATCH'
      ? route.fulfill({
          status: 409,
          contentType: 'application/problem+json',
          headers: {
            'Access-Control-Allow-Origin': 'http://localhost:5180',
            'Access-Control-Allow-Credentials': 'true',
          },
          body: JSON.stringify({ code: 'last_owner_required', status: 409 }),
        })
      : route.fallback(),
  );
  await page.goto('/ayarlar/roller');
  await page.getByRole('button', { name: 'Elif Kaya: düzenle' }).click();
  const dialog = page.getByRole('dialog', { name: 'Elif Kaya' });
  await dialog.getByRole('combobox', { name: 'Rol' }).selectOption('Staff');
  await dialog.getByRole('checkbox', { name: 'Kadıköy' }).check();
  await dialog.getByRole('button', { name: 'Kaydet' }).click();
  await expect(dialog.getByText('İşletmede en az bir işletme sahibi kalmalı.')).toBeVisible();
});
