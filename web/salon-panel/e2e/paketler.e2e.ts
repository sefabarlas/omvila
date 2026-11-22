import { expect, test } from '@playwright/test';
import { demoCustomerId, packagesDemo } from './packages-demo';

// Dilim 5 KK6, panel tarafı: paket oluştur (çok hizmetli) → müşteri kartından sat (Nakit) → kartın Paketler
// sekmesinde ve paket kartında görünür. Uçlar packagesDemo'da durumlu (Prism durumsuz).
test('paket oluştur → müşteriye sat → müşteri kartında görünür (KK6)', async ({ page }) => {
  await packagesDemo(page);
  await page.goto('/paketler');
  await page.getByRole('button', { name: 'Paket oluştur' }).click();
  const create = page.getByRole('dialog', { name: 'Paket oluştur' });
  await create.getByLabel('Paket adı').fill('Manikür · 6 seans');
  await create.getByLabel('Fiyat').fill('1.700');
  await create.getByLabel('1. hizmet', { exact: true }).selectOption({ label: 'Manikür' });
  await create.getByLabel('Manikür seans sayısı').fill('6');
  await create.getByRole('button', { name: 'Hizmet ekle' }).click();
  await create.getByLabel('2. hizmet', { exact: true }).selectOption({ label: 'Kalıcı oje' });
  await create.getByLabel('Kalıcı oje seans sayısı').fill('2');
  await create.getByLabel('Satış primi').fill('10');
  await create.getByRole('button', { name: 'Paketi oluştur' }).click();
  await expect(create).toBeHidden();
  const table = page.getByRole('table', { name: 'Paket tanımları' });
  await expect(table.getByText('Manikür · 6 seans')).toBeVisible();

  await page.goto(`/musteriler/${demoCustomerId}`);
  await page.getByRole('button', { name: 'Paket sat' }).click();
  const sell = page.getByRole('dialog', { name: 'Müşteriye paket sat' });
  await expect(sell.getByText('Selin Öztürk')).toBeVisible();
  await sell.getByLabel('Paket').selectOption({ label: 'Manikür · 6 seans' });
  await expect(sell.getByText('6 × Manikür, 2 × Kalıcı oje')).toBeVisible();
  await expect(sell.getByText("23 Şubat 2027'ye kadar")).toBeVisible();
  await sell.getByText('Nakit', { exact: true }).click();
  await sell.getByRole('button', { name: '1.700 ₺ al ve paketi tanımla' }).click();
  await expect(page.getByText('Paket tanımlandı')).toBeVisible();
  await page.getByRole('tab', { name: 'Paketler' }).click();
  const owned = page.getByRole('table', { name: 'Paketler' });
  await expect(owned.getByText('Manikür · 6 seans')).toBeVisible();
  await expect(owned.getByText('8/8 seans')).toBeVisible();
});
