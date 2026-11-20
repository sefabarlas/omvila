import { expect, test } from '@playwright/test';
import { captureBody, sessionAs } from './support';

// Dilim 1 uçtan uca: personel daveti → personel girişi sonrası kısıtlı menü (kabul kriteri 10).
test('personel ekle: telefonla davet gönderilir', async ({ page }) => {
  await page.goto('/personel');
  await expect(page.getByRole('heading', { level: 1, name: 'Personel' })).toBeVisible();
  await page.getByRole('button', { name: 'Personel ekle' }).first().click();
  const drawer = page.getByRole('dialog', { name: 'Personel ekle' });
  await drawer.getByRole('textbox', { name: 'Ad soyad' }).fill('Deniz Kara');
  await drawer.getByRole('textbox', { name: 'Cep telefonu' }).fill('0555 000 00 02');
  await drawer.getByRole('textbox', { name: 'Unvan' }).fill('Renk uzmanı');
  const create = await captureBody(page, '/staff', 'POST');
  await drawer.getByRole('button', { name: 'Personeli ekle' }).click();
  expect(await create()).toMatchObject({
    fullName: 'Deniz Kara',
    phone: '0555 000 00 02',
    title: 'Renk uzmanı',
    role: 'Staff',
    sendInvite: true,
    isBookableOnline: true,
  });
  await expect(page.getByText("Davet SMS'i gönderildi.")).toBeVisible();
});

test('personel girişi: menüde yalnız kişisel bölümler, yönetim sayfaları kapalı', async ({
  page,
}) => {
  await sessionAs(page, {
    user: {
      id: '0193a8f0-0000-7000-8000-000000000002',
      fullName: 'Deniz Kara',
      phoneMasked: '0555 *** ** 02',
    },
    membership: { role: 'Staff', branchIds: [], staffId: '0193a8f0-0000-7000-8000-0000000000c9' },
    permissions: [
      'calendar.view',
      'customer.view',
      'customer.edit',
      'timeoff.request',
      'walkin.manage',
      'commission.view',
    ],
  });
  await page.goto('/');
  await expect(page.getByRole('heading', { level: 1, name: 'Merhaba Deniz' })).toBeVisible();
  const nav = page.getByRole('navigation', { name: 'Ana menü' });
  await expect(nav.getByRole('link')).toHaveText(['Takvimim']);
  await expect(nav.getByText('Yönetim')).toHaveCount(0);
  await page.screenshot({ path: 'e2e-results/ekranlar/PersonelRolu.png', fullPage: true });

  await page.goto('/ayarlar/isletme');
  await expect(
    page.getByRole('heading', { level: 1, name: 'Bu sayfayı görme yetkiniz yok' }),
  ).toBeVisible();
});
